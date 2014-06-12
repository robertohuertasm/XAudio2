using System;
using System.Collections.Generic;
using System.Linq;

#if WINDOWS_PHONE
namespace Codecoding.XAudio2.Phone
#elif NETFX_CORE
namespace Codecoding.XAudio2.Store
#endif
{
    public class XSoundGroup : IDisposable
    {
        private readonly SharpDX.XAudio2.XAudio2 _device;
        private readonly int _groupId;
        private readonly IList<XSound> _sounds;

        public XSoundGroup(IList<XSound> sounds, int groupId)
        {
            _device = sounds.First().Device;
            _groupId = groupId;
            _sounds = sounds;
        }

        public XSoundGroup(SharpDX.XAudio2.XAudio2 device, IEnumerable<string> fileNames, int groupId)
        {
            _device = device;
            _groupId = groupId;
            _sounds = new List<XSound>();
            foreach (var fileName in fileNames)
            {
                var snd = new XSound(device, fileName, groupId);
                _sounds.Add(snd);
            }
        }

        public void Play(float volume = 1f)
        {
            Stop();
            foreach (var xSound in _sounds)
            {
                xSound.Play(volume);
            }
            _device.CommitChanges(_groupId);
        }

        public void Stop()
        {
            foreach (var xSound in _sounds)
            {
                xSound.Stop();
            }
            _device.CommitChanges(_groupId);
        }

        ~XSoundGroup()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;
            //disposed managed items:
            foreach (var xSound in _sounds)
            {
                xSound.Dispose();
            }
        }
    }
}
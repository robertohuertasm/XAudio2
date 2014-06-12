using System;
using System.IO;
#if NETFX_CORE
using Windows.ApplicationModel;
#endif
using SharpDX.Multimedia;
using SharpDX.XAudio2;

#if WINDOWS_PHONE
 namespace Codecoding.XAudio2.Phone
#elif NETFX_CORE
namespace Codecoding.XAudio2.Store
#endif
{
    public class XSound : IDisposable
    {
        
        private WaveFormat _format;
        private AudioBuffer _buffer;
        private uint[] _packetsInfo;

        public SharpDX.XAudio2.XAudio2 Device { get; private set; }
        public string FileName { get; private set; }
        public int? GroupId { get; set; }

        public SourceVoice Voice { get; set; }

#if WINDOWS_PHONE
        public XSound(SharpDX.XAudio2.XAudio2 device, string fileName, int? group = null)
        {
            Device = device;
            FileName = fileName;
            GroupId = @group;
            Voice = CreateVoice(device, fileName);
        }

        protected SourceVoice CreateVoice(SharpDX.XAudio2.XAudio2 device, string fileName)
        {
            using (var stream = new SoundStream(File.OpenRead(fileName)))
            {
                _format = stream.Format;
                _buffer = new AudioBuffer
                {
                    Stream = stream.ToDataStream(),
                    AudioBytes = (int) stream.Length,
                    Flags = BufferFlags.EndOfStream
                };
                _packetsInfo = stream.DecodedPacketsInfo;
            }

            var sourceVoice = new SourceVoice(device, _format, true);
            return sourceVoice;
        }
#endif
#if NETFX_CORE

        //public static async Task<XSound> CreateAsync(SharpDX.XAudio2.XAudio2 device, string fileName, int? group = null)
        //{
        //    var sound = new XSound(device, fileName, group);
        //    sound.Voice = await 
        //}

        public XSound(SharpDX.XAudio2.XAudio2 device, string fileName, int? group = null)
        {
            Device = device;
            FileName = fileName;
            GroupId = @group;
            Voice = CreateVoice(device, fileName);
        }

        protected  SourceVoice CreateVoice(SharpDX.XAudio2.XAudio2 device, string fileName)
        {
            fileName = fileName.Replace("/", "\\");
            var file = Package.Current.InstalledLocation.GetFileAsync(fileName).Await();
            var streamWithContentType = file.OpenReadAsync().Await();
            var st = streamWithContentType.AsStreamForRead();

            using (var stream = new SoundStream(st))
            {
                _format = stream.Format;
                _buffer = new AudioBuffer
                {
                    Stream = stream.ToDataStream(),
                    AudioBytes = (int)stream.Length,
                    Flags = BufferFlags.EndOfStream
                };
                _packetsInfo = stream.DecodedPacketsInfo;
            }

            var sourceVoice = new SourceVoice(device, _format, true);
            return sourceVoice;
        }
#endif
        protected void Reload()
        {
            Voice.FlushSourceBuffers();
            Voice.SubmitSourceBuffer(_buffer, _packetsInfo);
        }

        public void Play(float volume = 1f)
        {
            if (Voice == null)
            {
                return;
            }
            try
            {
                Voice.SetVolume(volume);
            }
            catch (Exception){
               //swallow
            }
           
            if (GroupId.HasValue)
            {
                Reload();
                Voice.Start(GroupId.Value);
            }
            else
            {
                Stop();
                Reload();
                Voice.Start();
            }
        }

        public void Stop()
        {
            if (Voice == null)
                return;
            if (GroupId.HasValue)
            {
                Voice.Stop(GroupId.Value);
            }
            else
            {
                Voice.Stop();
            }
        }

        ~XSound()
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
            Voice.DestroyVoice();
            Voice.Dispose();
            _buffer.Stream.Dispose();
        }

       
    }
}
using System;
using SharpDX.XAudio2;

#if WINDOWS_PHONE
namespace Codecoding.XAudio2.Phone
#elif NETFX_CORE
namespace Codecoding.XAudio2.Store
#endif
{
    public class XSoundManager : IDisposable
    {
        public SharpDX.XAudio2.XAudio2 XAudio2 { get; set; }
        public MasteringVoice MasteringVoice { get; set; }

        public XSoundManager()
        {
            XAudio2 = new SharpDX.XAudio2.XAudio2();
            MasteringVoice = new MasteringVoice(XAudio2);
        }

        ~XSoundManager()
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
            XAudio2.Dispose();
            MasteringVoice.Dispose();
            
        }
    }
}
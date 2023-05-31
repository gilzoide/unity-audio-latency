#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
using System;
using System.Runtime.InteropServices;

namespace Gilzoide.AudioLatency.Platforms
{
    using Id = IntPtr;
    using Selector = IntPtr;

    public static class AppleAudioLatency
    {
        /// <summary>Find the device's audio output latency as reported by AVAudioSession</summary>
        /// <remarks>Available in iOS/iPadOS 6.0+, Mac Catalyst 13.1+ and tvOS 9.0+</remarks>
        /// <returns>Audio output latency, if available. Returns <see langword="null"/> otherwise.</returns>
        /// <seealso href="https://developer.apple.com/documentation/avfaudio/avaudioengine"/>
        public static double? GetOutputLatency()
        {
            return TryMessageAVAudioSession("outputLatency");
        }

        /// <summary>Find the device's audio input latency as reported by AVAudioSession</summary>
        /// <remarks>Available in iOS/iPadOS 6.0+, Mac Catalyst 13.1+ and tvOS 9.0+</remarks>
        /// <returns>Audio input latency, if available. Returns <see langword="null"/> otherwise.</returns>
        /// <seealso href="https://developer.apple.com/documentation/avfaudio/avaudioengine"/>
        public static double? GetInputLatency()
        {
            return TryMessageAVAudioSession("inputLatency");
        }

        private static double? TryMessageAVAudioSession(string selectorName)
        {
            Selector selector = sel_registerName(selectorName);
            Id AVAudioSession = objc_getClass("AVAudioSession");
            if (AVAudioSession == IntPtr.Zero || !class_respondsToSelector(AVAudioSession, selector))
            {
                return null;
            }

            Id audioSession = objc_msgSend_Id(AVAudioSession, sel_registerName("sharedInstance"));
            if (audioSession == IntPtr.Zero)
            {
                return null;
            }

            return objc_msgSend_double(audioSession, selector);
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private extern static Id objc_getClass(string name);

        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        public extern static Selector sel_registerName(string name);

        [DllImport("__Internal")]
        public extern static bool class_respondsToSelector(Id cls, Selector sel);

        [DllImport("__Internal", EntryPoint = "objc_msgSend")]
        private static extern Id objc_msgSend_Id(Id id, Selector selector);

        [DllImport("__Internal", EntryPoint = "objc_msgSend")]
        private static extern double objc_msgSend_double(Id id, Selector selector);
    }
}
#endif
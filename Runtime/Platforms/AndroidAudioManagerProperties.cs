#if UNITY_ANDROID
using UnityEngine;

namespace Gilzoide.AudioLatency.Platforms
{
    public static class AndroidAudioManagerProperties
    {
        public const string JavaClassName = "com.gilzoide.audiolatency.AudioManagerProperties";

#if UNITY_EDITOR
        public static bool IsSupported => false;
#else
        public static bool IsSupported
        {
            get
            {
                using(var cls = new AndroidJavaClass(JavaClassName))
                {
                    return cls.CallStatic<bool>("isSupported");
                }
            }
        }
#endif

        public static int? GetOptimalOutputBufferSize()
        {
            if (!IsSupported)
            {
                return null;
            }

            using(var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using(var context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
            using(var androidAudioManagerClass = new AndroidJavaClass(JavaClassName))
            {
                int bufferSize = androidAudioManagerClass.CallStatic<int>("getOutputFramesPerBuffer", context);
                return bufferSize > 0 ? bufferSize : null;
            }
        }
    }
}
#endif
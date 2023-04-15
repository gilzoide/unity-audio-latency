namespace Gilzoide.AudioLatency
{
    public static class AudioLatency
    {
        public static double? GetOutputLatency()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
            return Platforms.AppleAudioLatency.GetOutputLatency();
#else
            return null;
#endif
        }

        public static double? GetInputLatency()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
            return Platforms.AppleAudioLatency.GetInputLatency();
#else
            return null;
#endif
        }
    }
}

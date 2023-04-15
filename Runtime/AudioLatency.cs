using System.Threading;
using System.Threading.Tasks;

namespace Gilzoide.AudioLatency
{
    public static class AudioLatency
    {
        public static Task<double?> GetOutputLatencyAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
            return Task.FromResult(Platforms.AppleAudioLatency.GetOutputLatency());
#elif UNITY_ANDROID
            return Platforms.AndroidAudioOutputLatency.GetOutputLatencyAsync(cancellationToken);
#else
            return Task.FromResult<double?>(null);
#endif
        }

        public static Task<double?> GetInputLatencyAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
            return Task.FromResult(Platforms.AppleAudioLatency.GetInputLatency());
#else
            return Task.FromResult<double?>(null);
#endif
        }
    }
}

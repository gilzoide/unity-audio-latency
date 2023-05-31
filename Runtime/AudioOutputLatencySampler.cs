using UnityEngine;

namespace Gilzoide.AudioLatency
{
    public class AudioOutputLatencySampler : MonoBehaviour
    {
        [Tooltip("Minimum time before the system is sampled again for audio latency, in seconds. Sampling audio latency is not performant in some systems, use this to avoid doing it all the time")]
        public float TimeBetweenSamples = 5;

        public double? AudioOutputLatency
        {
            get
            {
                if (Time.realtimeSinceStartup - _lastSampleTime >= TimeBetweenSamples)
                {
                    Refresh();
                }
                return _audioOutputLatency;
            }
        }

        private double? _audioOutputLatency;
        private float _lastSampleTime;

        private void OnEnable()
        {
            Refresh();
            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        }

        private void OnDisable()
        {
            AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
            PlatformDisable();
        }

        public void Refresh()
        {
            PlatformRefresh();
            _lastSampleTime = Time.realtimeSinceStartup;
        }

        public void OnAudioConfigurationChanged(bool deviceChanged)
        {
            PlatformDisable();
            Refresh();
        }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_TVOS
        private void PlatformRefresh()
        {
            _audioOutputLatency = Platforms.AppleAudioLatency.GetOutputLatency();
        }

        private void PlatformDisable()
        {
        }
#elif UNITY_ANDROID
        private Platforms.AndroidAudioOutputLatency _androidNativeLatencySampler;

        private void PlatformRefresh()
        {
            if (_androidNativeLatencySampler == null)
            {
                _androidNativeLatencySampler = new Platforms.AndroidAudioOutputLatency();
            }
            StartCoroutine(PlatformRefreshRoutine());
        }

        private System.Collections.IEnumerator PlatformRefreshRoutine()
        {
            while (!_androidNativeLatencySampler.HasOutputLatency)
            {
                yield return null;
            }
            _audioOutputLatency = _androidNativeLatencySampler.OutputLatency;
        }

        private void PlatformDisable()
        {
            _androidNativeLatencySampler?.Dispose();
            _androidNativeLatencySampler = null;
        }
#else
        private void PlatformRefresh()
        {
        }

        private void PlatformDisable()
        {
        }
#endif
    }
}

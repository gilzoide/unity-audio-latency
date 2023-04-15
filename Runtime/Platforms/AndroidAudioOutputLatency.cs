#if UNITY_ANDROID
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Gilzoide.AudioLatency.Platforms
{
    public class AndroidAudioOutputLatency : IDisposable
    {
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
        public const string JavaClassName = "com.gilzoide.audiolatency.AudioOutputLatency";

        public static async Task<double?> GetOutputLatencyAsync(CancellationToken cancellationToken = default)
        {
            if (!AndroidAudioOutputLatency.IsSupported)
            {
                return null;
            }

            using (var latencyMeasure = new AndroidAudioOutputLatency())
            {
                while (!latencyMeasure.HasOutputLatency)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Yield();
                }
                return latencyMeasure.OutputLatency;
            }
        }

        private AndroidJavaObject _javaObject;

        public bool HasOutputLatency => OutputLatency > 0;
        public double OutputLatency => _javaObject?.Call<double>("getOutputLatency") ?? -1;

        public AndroidAudioOutputLatency(bool startMeasuring = true)
        {
            if (!IsSupported)
            {
                return;
            }

            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            _javaObject = new AndroidJavaObject(JavaClassName, audioConfig.sampleRate, audioConfig.dspBufferSize);

            if (startMeasuring)
            {
                StartMeasuring();
            }
        }

        ~AndroidAudioOutputLatency()
        {
            Dispose();
        }

        public void StartMeasuring()
        {
            _javaObject?.Call("start");
        }

        public void StopMeasuring()
        {
            _javaObject?.Call("stop");
        }

        public void Dispose()
        {
            StopMeasuring();
            _javaObject?.Dispose();
            _javaObject = null;
        }
    }
}
#endif
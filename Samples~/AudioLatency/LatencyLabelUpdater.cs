using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.AudioLatency.Sample
{
    public class LatencyLabelUpdater : MonoBehaviour
    {
        [SerializeField] private Text _outputLatencyText;
        [SerializeField] private Text _inputLatencyText;
        [SerializeField] private float _updateSeconds;

        public void Start()
        {
            StartCoroutine(UpdateRoutine());
        }

        private IEnumerator UpdateRoutine()
        {
            while (true)
            {
                UpdateAsync();
                yield return new WaitForSecondsRealtime(_updateSeconds);
            }
        }

        private async void UpdateAsync()
        {
            if (_outputLatencyText)
            {
                _outputLatencyText.text = await AudioLatency.GetOutputLatencyAsync() is double latency
                    ? $"{(float) (latency * 1000):0.##} ms"
                    : "?";
            }

            if (_inputLatencyText)
            {
                _inputLatencyText.text = await AudioLatency.GetInputLatencyAsync() is double latency
                    ? $"{(float) (latency * 1000):0.##} ms"
                    : "?";
            }
        }
    }
}

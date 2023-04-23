using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.AudioLatency.Sample
{
    public class BufferSizeChooser : MonoBehaviour
    {
        [SerializeField] private Dropdown _dropdown;
        
        private readonly List<int> _options = new List<int>();

        private void OnEnable()
        {
            RefreshOptions();
            _dropdown.onValueChanged.AddListener(OnBufferSizeChosen);
        }

        private void OnDisable()
        {
            _dropdown.onValueChanged.RemoveListener(OnBufferSizeChosen);
        }

        public void OnBufferSizeChosen(int index)
        {
            if (index < 0 || index >= _options.Count)
            {
                return;
            }

            int bufferSize = _options[index];
            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            if (audioConfiguration.dspBufferSize != bufferSize)
            {
                Debug.Log($"[{nameof(BufferSizeChooser)}] Setting DSP buffer size to {bufferSize}", this);
                audioConfiguration.dspBufferSize = bufferSize;
                AudioSettings.Reset(audioConfiguration);
            }
        }

        private void RefreshOptions()
        {
            _options.Clear();
            _dropdown.ClearOptions();

            var dropdownOptions = new List<Dropdown.OptionData>();

            _options.Add(256);
            dropdownOptions.Add(new Dropdown.OptionData("Best Latency (256)"));

            _options.Add(512);
            dropdownOptions.Add(new Dropdown.OptionData("Good Latency (512)"));

            _options.Add(1024);
            dropdownOptions.Add(new Dropdown.OptionData("Best Performance (1024)"));

            if (AudioLatency.GetOptimalOutputBufferSize() is int optimalBufferSize)
            {
                _options.Add(optimalBufferSize);
                dropdownOptions.Add(new Dropdown.OptionData($"Optimal buffer size ({optimalBufferSize})"));
            }

            int currentBufferSize = AudioSettings.GetConfiguration().dspBufferSize;
            int currentIndex = _options.IndexOf(currentBufferSize);
            if (currentIndex < 0)
            {
                currentIndex = _options.Count;
                
                _options.Add(currentBufferSize);
                dropdownOptions.Add(new Dropdown.OptionData(currentBufferSize.ToString()));
            }

            _dropdown.AddOptions(dropdownOptions);
            _dropdown.value = currentIndex;
        }
    }
}

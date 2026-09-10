using UnityEngine;
using UnityEngine.Audio;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class AudioMixerExtension
    {

        public static float GetLinearFloat(this AudioMixerGroup audioMixerGroup)
        {
            return audioMixerGroup.audioMixer.GetLinearFloat(audioMixerGroup.name);
        }

        public static float GetLinearFloat(this AudioMixer mixer, string name)
        {
            if (mixer.GetFloat(name, out float value))
            {
                return DecibelToLinear(value);
            }
            return 1.0f;
        }

        private static float DecibelToLinear(float dB)
        {
            float linear = Mathf.Pow(10.0f, dB / 20.0f);

            return linear;
        }
    }
}

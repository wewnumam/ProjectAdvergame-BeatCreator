using UnityEngine;
using UnityEngine.UI;

namespace ProjectAdvergame.Utility
{
    public class AudioSpectrumScaler : MonoBehaviour
    {
        public AudioSource audioSource; // The AudioSource component
        public Image targetImage; // The UI Image to scale
        public float scaleMultiplier = 10f; // Multiplier for scaling
        public float smoothSpeed = 2f; // Speed of smoothing

        private float[] spectrumData = new float[256]; // Array to hold spectrum data
        private Vector3 targetScale = Vector3.one; // Target scale for the image

        void Update()
        {
            // Get the spectrum data from the audio source
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            // Calculate the average spectrum value
            float averageSpectrumValue = 0f;
            foreach (float value in spectrumData)
            {
                averageSpectrumValue += value;
            }
            averageSpectrumValue /= spectrumData.Length;

            // Calculate the target scale based on the average spectrum value
            float scaleFactor = 1 + averageSpectrumValue * scaleMultiplier;
            targetScale = new Vector3(scaleFactor, scaleFactor, 1);

            // Smoothly interpolate the current scale towards the target scale
            targetImage.rectTransform.localScale = Vector3.Lerp(targetImage.rectTransform.localScale, targetScale, Time.deltaTime * smoothSpeed);
        }
    }
}
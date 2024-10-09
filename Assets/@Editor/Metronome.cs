using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public AudioSource tickSound; // Assign the tick sound in the Inspector
    public Slider bpmSlider; // Assign a UI slider to adjust BPM
    public TMP_Text bpmText; // Optional: Display the current BPM value
    public float bpm = 120f; // Default BPM value

    private float timer;
    private float interval; // Time between ticks

    void Start()
    {
        if (PlayerPrefs.HasKey("Metronome"))
        {
            bpm = PlayerPrefs.GetFloat("Metronome");
            bpmSlider.value = bpm;
            bpmText.SetText($"BPM: {bpm:F2}");
        }
        else
            PlayerPrefs.SetFloat("Metronome", bpm);
        UpdateInterval(); // Initialize interval based on BPM
        bpmSlider.onValueChanged.AddListener(delegate { OnBPMChanged(); });
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            PlayTick();
            timer = 0f; // Reset the timer after each tick
        }
    }

    void PlayTick()
    {
        tickSound.Play(); // Play the tick sound
    }

    public void OnBPMChanged()
    {
        bpm = bpmSlider.value; // Adjust BPM based on slider value
        bpmText.SetText($"BPM: {bpm:F2}"); // Update BPM display (if needed)
        PlayerPrefs.SetFloat("Metronome", bpm);
        UpdateInterval(); // Update interval after changing BPM
    }

    void UpdateInterval()
    {
        interval = 60f / bpm; // Interval between ticks based on BPM
    }

    public void ResetMetronome()
    {
        UpdateInterval(); // Recalculate the interval
        timer = 0f; // Reset the timer
    }
}

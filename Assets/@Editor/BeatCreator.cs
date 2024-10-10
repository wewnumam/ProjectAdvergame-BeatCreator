using DG.Tweening;
using NaughtyAttributes;
using ProjectAdvergame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeatCreator : MonoBehaviour
{
    [ReadOnly] public bool isPlay;
    [ReadOnly] public float currentTime;
    [ReadOnly] public int currentSwitchDirectionIndex;
    [ReadOnly] public EnumManager.Direction currentDirection;
    [ReadOnly] public List<GameObject> beatObjs;

    [SerializeField] TMP_Text currentTimeText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] SO_BeatCreatorData beatCreatorData;
    [SerializeField] Transform beatsParent;
    [SerializeField] GameObject beatPrefab;
    [SerializeField] TMP_InputField clipboardText;
    [SerializeField] SpectrumMover spectrumMover;
    [SerializeField] TMP_Text debugText;

    private void Update()
    {
        if (!isPlay)
            return;

        currentTime += Time.deltaTime;
        currentTimeText?.SetText($"{currentTime:F2}");

        if (currentSwitchDirectionIndex < beatCreatorData.switchDirections.Count)
        {
            if (currentTime > beatCreatorData.switchDirections[currentSwitchDirectionIndex])
            {
                if (currentDirection == EnumManager.Direction.FromEast)
                    currentDirection = EnumManager.Direction.FromWest;
                else
                    currentDirection = EnumManager.Direction.FromEast;

                currentSwitchDirectionIndex++;
            }
        }
    }

    public void SetBeatCreatorData(SO_BeatCreatorData beatCreatorData)
    {
        this.beatCreatorData = beatCreatorData;
        currentDirection = EnumManager.Direction.FromEast;
        audioSource.clip = beatCreatorData.clip;

        if (beatCreatorData.beatListWrapper.beats.Count > 0)
        {
            if (beatObjs.Count > 0)
                foreach (var beatObj in beatObjs)
                    Destroy(beatObj);
            
            beatObjs = new List<GameObject>();

            for (int i = 0; i < beatCreatorData.beatListWrapper.beats.Count; i++)
            {
                Beat beat = beatCreatorData.beatListWrapper.beats[i];
                GameObject obj = Instantiate(beatPrefab, new Vector3(beat.direction == EnumManager.Direction.FromEast ? beat.interval : -beat.interval, 0, 0), Quaternion.identity, beatsParent);

                if (beat.type == EnumManager.StoneType.LongBeat)
                {
                    if (i < beatCreatorData.beatListWrapper.beats.Count - 1)
                    {
                        Vector3 modScale = obj.transform.localScale;
                        modScale.x = beat.interval - beatCreatorData.beatListWrapper.beats[i + 1].interval;
                        obj.transform.localScale = modScale;
                    }
                }
                
                beatObjs.Add(obj);
            }
        }

        clipboardText.text = JsonUtility.ToJson(beatCreatorData.beatListWrapper, true);
    }


    public void AddNormalBeat() => AddBeat(EnumManager.StoneType.Normal);
    public void AddLongBeat() => AddBeat(EnumManager.StoneType.LongBeat);

    private void AddBeat(EnumManager.StoneType stoneType)
    {
        Beat beat = new Beat();
        beat.type = stoneType;
        beat.interval = currentTime;
        beat.direction = currentDirection;
        beatCreatorData.beatListWrapper.beats.Add(beat);
        debugText?.SetText(currentTime.ToString());
    }

    public void Record()
    {
        Time.timeScale = 1;
        beatCreatorData.beatListWrapper.beats = new List<Beat>();
        isPlay = true;
        spectrumMover.Play(audioSource.clip.length);
        audioSource.Play();
    }

    public void StopRecord()
    {
        Time.timeScale = 1;
        spectrumMover.Stop();
        audioSource.Stop();
        isPlay = false;
        currentTime = 0;
        currentSwitchDirectionIndex = 0;
        SetBeatCreatorData(beatCreatorData);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void Playback()
    {
        Time.timeScale = 1;
        spectrumMover.Play(audioSource.clip.length);
        isPlay = true;
        for (int i = 0; i < beatObjs.Count; i++)
        {
            int index = i;  // Capture the current value of i
            beatObjs[index].transform.DOMoveX(beatObjs[index].transform.position.x > 0 ? -beatObjs[index].transform.localScale.x / 2 : beatObjs[index].transform.localScale.x / 2, beatCreatorData.beatListWrapper.beats[index].interval)
                .SetEase(Ease.Linear)
                .OnComplete(() => beatObjs[index].SetActive(false));
        }
        audioSource.Play();
    }

    public void CopyClipBoard()
    {
        string yamlContent = JsonUtility.ToJson(beatCreatorData.beatListWrapper, true);
        GUIUtility.systemCopyBuffer = yamlContent;
        Debug.Log(yamlContent);
    }

    public void OverwriteBeatsFromInput()
    {
        if (string.IsNullOrEmpty(clipboardText.text))
        {
            debugText?.SetText("Input field is empty.");
            return;
        }

        try
        {
            // Deserialize the JSON into a temporary class for the beats field
            BeatListWrapper newBeatsData = JsonUtility.FromJson<BeatListWrapper>(clipboardText.text);

            if (newBeatsData != null && newBeatsData.beats != null)
            {
                // Overwrite the beats field in beatCreatorData
                beatCreatorData.beatListWrapper = newBeatsData;

                // Re-apply the updated beatCreatorData
                SetBeatCreatorData(beatCreatorData);

                debugText?.SetText("Beats have been successfully overwritten from input field.");
            }
            else
            {
                debugText?.SetText("Failed to parse input field JSON for beats.");
            }
        }
        catch (Exception ex)
        {
            debugText?.SetText($"Error parsing input field JSON: {ex.Message}");
        }
    }

    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.Play();

        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
    }
}

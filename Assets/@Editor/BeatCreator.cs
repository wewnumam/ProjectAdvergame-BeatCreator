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
    [SerializeField] TMP_Text clipboardText;

    private StringBuilder clipboard;

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
        if (clipboard != null)
            clipboard.Clear();
        clipboard = new StringBuilder();

        if (beatCreatorData.beats.Count > 0)
        {
            if (beatObjs.Count > 0)
                foreach (var beatObj in beatObjs)
                    Destroy(beatObj);
            
            beatObjs = new List<GameObject>();

            for (int i = 0; i < beatCreatorData.beats.Count; i++)
            {
                Beat beat = beatCreatorData.beats[i];
                GameObject obj = Instantiate(beatPrefab, new Vector3(beat.direction == EnumManager.Direction.FromEast ? beat.interval : -beat.interval, 0, 0), Quaternion.identity, beatsParent);

                if (beat.type == EnumManager.StoneType.LongBeat)
                {
                    if (i < beatCreatorData.beats.Count - 1)
                    {
                        Vector3 modScale = obj.transform.localScale;
                        modScale.x = beat.interval - beatCreatorData.beats[i + 1].interval;
                        obj.transform.localScale = modScale;
                    }
                }
                
                beatObjs.Add(obj);
                clipboard.Insert(0, $"{beat.interval:F2}\n");
            }
        }

        clipboardText?.SetText(clipboard.ToString());
    }


    public void AddNormalBeat() => AddBeat(EnumManager.StoneType.Normal);
    public void AddLongBeat() => AddBeat(EnumManager.StoneType.LongBeat);

    private void AddBeat(EnumManager.StoneType stoneType)
    {
        Beat beat = new Beat();
        beat.type = stoneType;
        beat.interval = currentTime;
        beat.direction = currentDirection;
        beatCreatorData.beats.Add(beat);
        clipboard.Insert(0, $"{currentTime:F2}\n");
        clipboardText?.SetText(clipboard.ToString());
    }

    public void Record()
    {
        beatCreatorData.beats = new List<Beat>();
        isPlay = true;
        audioSource.Play();
    }

    public void StopRecord()
    {
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
        isPlay = true;
        for (int i = 0; i < beatObjs.Count; i++)
        {
            int index = i;  // Capture the current value of i
            beatObjs[index].transform.DOMoveX(beatObjs[index].transform.position.x > 0 ? -beatObjs[index].transform.localScale.x / 2 : beatObjs[index].transform.localScale.x / 2, beatCreatorData.beats[index].interval)
                .SetEase(Ease.Linear)
                .OnComplete(() => beatObjs[index].SetActive(false));
        }
        audioSource.Play();
    }

    public void CopyClipBoard()
    {
        string yamlContent = JsonUtility.ToJson(beatCreatorData, true);
        GUIUtility.systemCopyBuffer = yamlContent;
        Debug.Log(yamlContent);
    }
}

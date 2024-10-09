using NaughtyAttributes;
using ProjectAdvergame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeatCreatorData_", menuName = "ProjectAdvergame/BeatCreatorData", order = 5)]
public class SO_BeatCreatorData : ScriptableObject
{
    public AudioClip clip;
    public List<float> switchDirections;
    public BeatListWrapper beatListWrapper;
}

[System.Serializable]
public class Beat
{
    public float interval;
    [Foldout("Details")]
    public EnumManager.StoneType type;
    [Foldout("Details")]
    public EnumManager.Direction direction;
}

[Serializable]
public class BeatListWrapper
{
    public List<Beat> beats;
}
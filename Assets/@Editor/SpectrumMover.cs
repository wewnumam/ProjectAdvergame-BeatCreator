using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectrumMover : MonoBehaviour
{
    [SerializeField] Image spectrum;

    private RectTransform rectTransform;

    public void Play(float duration)
    {
        spectrum.SetNativeSize();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, .5f);
        rectTransform.DOAnchorPosX(-rectTransform.rect.width, duration).SetEase(Ease.Linear);
    }

    public void SetSprite(Sprite sprite)
    {
        spectrum.sprite = sprite;
        spectrum.SetNativeSize();
    }

    public void Stop()
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = Vector2.zero;
    }
}

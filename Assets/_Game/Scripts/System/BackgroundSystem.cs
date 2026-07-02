using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SMT3.Data;
using SMT3.Game;

public class BackgroundSystem : MonoBehaviour
{
    [System.Serializable]
    private class BackgroundData
    {
        public string Key;
        public Sprite BackgroundSprite;
    }
    [SerializeField] private List<BackgroundData> _backgroundSprites = new();
    [SerializeField] private SpriteRenderer _backgroundVisual;
    private Transform _backgroundTransform => _backgroundVisual.transform;
    private Sequence _backgroundVisualSequence;
    private Vector3 _defaultBackgroundScale;
    private float _scaleMultiplier = 1.2f;
    private float _scaleDuration = 0.3f;

    private void Start()
    {
        _defaultBackgroundScale = _backgroundVisual.transform.localScale;
    }

    private void OnEnable()
    {
        GameEvents.OnMoodChanged += MoodHandle;
    }

    private void OnDisable()
    {
        GameEvents.OnMoodChanged -= MoodHandle;
    }


    private void MoodHandle(MetaData data)
    {
        Debug.Log("Mood Handle");
        if (data == null || _backgroundVisual == null)
        {
            Debug.Log("background is null or data is null");
            return;
        }
        BackgroundChange(data.Value);
    }

    private void BackgroundChange(string key)
    {
        Sprite background = GetBackgroundSprite(key);
        if (background == null)
        {
            Debug.Log($"background with {key} is null");
            return;
        }
        _backgroundVisual.sprite = background;
        BouncingBackground();
    }

    private Sprite GetBackgroundSprite(string key)
    {
        BackgroundData data = _backgroundSprites.Find(x => x.Key.Equals(key));
        return data?.BackgroundSprite;
    }

    private void BouncingBackground()
    {
        _backgroundVisualSequence?.Kill();
        _backgroundVisualSequence = DOTween.Sequence();
        _backgroundVisualSequence.Append(_backgroundTransform.DOScale(_defaultBackgroundScale * _scaleMultiplier, _scaleDuration));
        _backgroundVisualSequence.Append(_backgroundTransform.DOScale(_defaultBackgroundScale, _scaleDuration));
    }
}

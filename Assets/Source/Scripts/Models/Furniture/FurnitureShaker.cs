using Core;
using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public sealed class FurnitureShaker
{

    private Tween _shakeTween = null;
    private Vector3 _originalScale = Vector3.zero;
    private Transform _furtnitureTransfrom = null;
    [Header("Shake Options")]
    [SerializeField] private float _shakeStrength = .5f;
    [SerializeField] private float _shakeDuration = .25f;
    [SerializeField] private float _randomless = 90f;
    [SerializeField] private int _vibrato = 10;
    public FurnitureShaker(Transform transform, Vector3 originalScale)
    {
        _furtnitureTransfrom = transform;
        _originalScale = originalScale;
    }

    public void ClearDependencis()
    {
    }

    private void HandleBuild(bool condition)
    {
        if (condition) StartShake();
        else StopShaking();
    }

    public void StartShake()
    {
        if (_shakeTween != null && _shakeTween.IsActive()) return;

        _shakeTween = _furtnitureTransfrom.DOPunchScale(
            Vector3.one * _shakeStrength, // одинаковый punch по всем осям
            _shakeDuration,
            _vibrato,
            0f // elasticity = 0 — без упругости, чистое дрожание
        ).SetLoops(-1);
    }

    public void StopShaking()
    {
        _shakeTween?.Kill();
        _shakeTween = null;
        _furtnitureTransfrom.localScale = _originalScale; // Возвращаем обычный масштаб
    }
}

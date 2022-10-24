using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DoTweenGo : MonoBehaviour
{
    public ScorePrefabMB TextPrefab { get; set; }
    GameObject Particleeffect { get; set; }

    private float _effectRadius = 250;

    public void Effect(Transform transform, GameObject particleeffect, Camera camera)
    {
        Vector3 cam = camera.ScreenToWorldPoint(new Vector3( Screen.width / 2, Screen.height / 2, 0));
        float randomDirection = Random.Range(0, -180);

        Particleeffect = particleeffect;

        float x = Mathf.Cos(randomDirection) * _effectRadius;
        float y = Mathf.Sin(randomDirection) * _effectRadius;

        Vector3 randomPoint = cam + new Vector3(x, y);

        var seq = DOTween.Sequence();
        seq
            .Append(transform.DOLocalMove(randomPoint, TextPrefab.TimeDuration * 0.1f))
            .Join(transform.DOScale(8f, TextPrefab.TimeDuration * 0.1f))
            .SetEase(Ease.InCubic)
            .InsertCallback(TextPrefab.TimeDuration * 0.1f, ()=> PlayParticle())
            .Append(transform.DOScale(10f, TextPrefab.TimeDuration * 0.9f))
            .Join(transform.DOPunchScale(new Vector3(12f, 12f, 1), TextPrefab.TimeDuration * 0.9f, 3, 2))
            .Join(TextPrefab.Text.DOFade(0, TextPrefab.TimeDuration * 0.9f))
            .SetEase(Ease.InCubic)
        .OnKill(Destr);
    }

    public void Effect(ref Camera camera)
    {
        Vector3 cam = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        float randomDirection = Random.Range(0, -180);

        Particleeffect = TextPrefab.ParticleEffect;

        float x = Mathf.Cos(randomDirection) * _effectRadius;
        float y = Mathf.Sin(randomDirection) * _effectRadius;

        Vector3 randomPoint = cam + new Vector3(x, y);

        var seq = DOTween.Sequence();
        seq
            .Append(TextPrefab.transform.DOLocalMove(randomPoint, TextPrefab.TimeDuration * 0.1f))
            .Join(TextPrefab.transform.DOScale(8f, TextPrefab.TimeDuration * 0.1f))
            .SetEase(Ease.InCubic)
            .InsertCallback(TextPrefab.TimeDuration * 0.1f, () => PlayParticle())
            .Append(TextPrefab.transform.DOScale(10f, TextPrefab.TimeDuration * 0.9f))
            .Join(TextPrefab.transform.DOPunchScale(new Vector3(12f, 12f, 1), TextPrefab.TimeDuration * 0.9f, 3, 2))
            .Join(TextPrefab.Text.DOFade(0, TextPrefab.TimeDuration * 0.9f))
            .SetEase(Ease.InCubic)
        .OnKill(Destr);
    }

    private void Destr()
    {
        TextPrefab.gameObject.SetActive(false);
    }

    private void PlayParticle()
    {
        Particleeffect.gameObject.SetActive(true);
    }
}

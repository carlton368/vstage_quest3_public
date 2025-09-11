using UnityEngine;
using System.Collections;

public class FlowerShineEndEffectManager : MonoBehaviour
{
    [Header("순차 활성화할 오브젝트들")]
    [SerializeField] private GameObject flowerPetalEffect;
    [SerializeField] private GameObject lightStickWaveEffect;

    [Header("타이밍")]
    [SerializeField] private float delayAfterBigFlower = 0.25f; // (현재 미사용)
    [SerializeField] private float delayAfterPetal    = 0.25f;

    [Header("파도타기 VFX 몇번 반복될지 시간 조정 (한번 재생될 때 2초 * 반복횟수)")]
    [SerializeField] private float waveEffectTime = 10.0f; // ← 활성화 후 이 시간 지나면 끔

    [Header("디버그")]
    [SerializeField] private bool logDebug = false;

    bool _running;
    Coroutine _waveOffRoutine;

    // ★ RecordEndEffectComponent.OnAutoSequenceCompleted에 연결
    public void PlaySequence()
    {
        if (_running) return;
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        _running = true;

        if (flowerPetalEffect) flowerPetalEffect.SetActive(true);
        if (delayAfterPetal > 0f) yield return new WaitForSeconds(delayAfterPetal);

        if (lightStickWaveEffect)
        {
            lightStickWaveEffect.SetActive(true);

            // 이전 스케줄이 남아있다면 취소
            if (_waveOffRoutine != null) StopCoroutine(_waveOffRoutine);
            _waveOffRoutine = StartCoroutine(DisableAfter(lightStickWaveEffect, waveEffectTime));
        }

        _running = false;
    }

    IEnumerator DisableAfter(GameObject go, float t)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, t));
        if (go) go.SetActive(false);
        if (logDebug) Debug.Log("[FlowerShineEndEffectManager] lightStickWaveEffect OFF");
    }
}
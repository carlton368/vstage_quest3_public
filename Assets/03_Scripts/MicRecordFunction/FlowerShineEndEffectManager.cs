using UnityEngine;
using System.Collections;

public class FlowerShineEndEffectManager : MonoBehaviour
{
    [Header("원래(상시) VFX")]
    [SerializeField] private GameObject baseLightStickVFX;   // 평소 켜둘 기본 VFX

    [Header("순차 활성화할 오브젝트들")]
    [SerializeField] private GameObject flowerPetalEffect;
    [SerializeField] private GameObject lightStickWaveEffect;

    [Header("타이밍")]
    [SerializeField] private float delayAfterBigFlower = 0.25f; // (현재 미사용)
    [SerializeField] private float delayAfterPetal    = 0.25f;

    [Header("지속 시간 (효과별로 개별 설정)")]
    [SerializeField] private float petalEffectTime = 10.0f;   // 꽃잎 이펙트 꺼질 때까지 유지
    [SerializeField] private float waveEffectTime  = 10.0f;   // 웨이브 이펙트 꺼질 때까지 유지

    [Header("디버그")]
    [SerializeField] private bool logDebug = false;

    bool _running;
    bool _waveActive;                     // 웨이브가 켜져있는지
    bool _baseWasActiveBeforeWave;        // 웨이브 시작 전 원래 VFX의 상태
    Coroutine _waveOffRoutine;

    // ★ RecordEndEffectComponent.OnAutoSequenceCompleted 에 연결
    public void PlaySequence()
    {
        if (_running) return;
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        _running = true;

        if (flowerPetalEffect)
        {
            flowerPetalEffect.SetActive(true);
            StartCoroutine(DisableAfter(flowerPetalEffect, petalEffectTime)); // 🌸 꽃잎은 petalEffectTime 뒤 꺼짐
        }

        if (delayAfterPetal > 0f) yield return new WaitForSeconds(delayAfterPetal);

        if (lightStickWaveEffect)
        {
            if (!_waveActive && baseLightStickVFX)
            {
                _baseWasActiveBeforeWave = baseLightStickVFX.activeSelf;
                baseLightStickVFX.SetActive(false);
            }

            lightStickWaveEffect.SetActive(true);
            _waveActive = true;

            if (_waveOffRoutine != null) StopCoroutine(_waveOffRoutine);
            _waveOffRoutine = StartCoroutine(DisableAfter(lightStickWaveEffect, waveEffectTime)); // 🌊 웨이브는 waveEffectTime 뒤 꺼짐
        }

        _running = false;
    }

    IEnumerator DisableAfter(GameObject go, float t)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, t));

        if (go) go.SetActive(false);

        // 웨이브 종료 처리
        if (go == lightStickWaveEffect)
        {
            _waveActive = false;
            if (baseLightStickVFX) baseLightStickVFX.SetActive(_baseWasActiveBeforeWave);
            if (logDebug) Debug.Log("[FlowerShineEndEffectManager] Wave OFF → Base VFX restored");
            _waveOffRoutine = null;
        }
    }

    // (선택) 외부에서 즉시 웨이브 중단하고 복구하고 싶을 때 호출
    public void StopWaveNow()
    {
        if (_waveOffRoutine != null) { StopCoroutine(_waveOffRoutine); _waveOffRoutine = null; }
        if (lightStickWaveEffect) lightStickWaveEffect.SetActive(false);
        if (_waveActive)
        {
            _waveActive = false;
            if (baseLightStickVFX) baseLightStickVFX.SetActive(_baseWasActiveBeforeWave);
        }
    }
}

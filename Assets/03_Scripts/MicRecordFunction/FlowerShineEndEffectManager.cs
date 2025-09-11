using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerShineEndEffectManager : MonoBehaviour
{
    [Header("순차 활성화할 오브젝트들")]
    [SerializeField] private GameObject flowerPetalEffect;
    [SerializeField] private GameObject lightStickWaveEffect;

    [Header("타이밍")]
    [SerializeField] private float delayAfterBigFlower = 0.25f;
    [SerializeField] private float delayAfterPetal    = 0.25f;
    
    [Header("디버그")]
    [SerializeField] private bool logDebug = false;

    bool _running;

    // ★ 이 메서드를 RecordEndEffectComponent.OnAutoSequenceComplete에 연결!
    public void PlaySequence()
    {
        if (_running) return;
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        _running = true;
        
        if (flowerPetalEffect)  flowerPetalEffect.SetActive(true);
        if (delayAfterPetal > 0) yield return new WaitForSeconds(delayAfterPetal);

        if (lightStickWaveEffect) lightStickWaveEffect.SetActive(true);

        _running = false;
    }

    
}

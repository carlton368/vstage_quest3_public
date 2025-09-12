using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DarkFilter : MonoBehaviour
{
    [SerializeField] private Image darkPanel; // 검은 UI 이미지

    public void FadeInDark()
    {
        darkPanel.DOFade(1f, 2f); // 2초 동안 어두워짐
    }

    public void FadeOutDark()
    {
        darkPanel.DOFade(0f, 5f); // 다시 밝게
    }

    // 새 메서드: 페이드인 실행 후 지정한 초만큼 기다렸다가 페이드아웃 실행
    public void FadeInThenFadeOutAfterDelay(float delaySeconds = 10f)
    {
        StartCoroutine(FadeInThenOutRoutine(delaySeconds));
    }

    private System.Collections.IEnumerator FadeInThenOutRoutine(float delaySeconds)
    {
        FadeInDark();
        yield return new WaitForSeconds(delaySeconds);
        FadeOutDark();
    }
}
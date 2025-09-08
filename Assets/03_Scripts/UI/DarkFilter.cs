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
        darkPanel.DOFade(0f, 2f); // 다시 밝게
    }
}
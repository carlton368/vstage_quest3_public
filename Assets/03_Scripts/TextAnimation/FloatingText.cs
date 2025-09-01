using TMPro;
using DG.Tweening;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    
    private void Start()
    {
        // 위아래로 둥실둥실 움직임
        transform.DOMoveY(transform.position.y + 0.40f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 크기가 살짝 커졌다 줄어드는 효과
        // transform.DOScale(1.05f, 2f)
        //     .SetLoops(-1, LoopType.Yoyo)
        //     .SetEase(Ease.InOutSine);
    }
}

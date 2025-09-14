// BubbleItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BubbleItem : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text text;
    public Image background;
    public CanvasGroup canvasGroup;

    [Header("Anim")]
    public float fadeDuration = 0.2f;
    public float slideDistance = 24f;
    public float slideDuration = 0.2f;

    RectTransform _rt;

    void Awake()
    {
        _rt = transform as RectTransform;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void Setup(string message, Color bgColor)
    {
        if (text != null) text.text = message;
        if (background != null) background.color = bgColor;
        StartCoroutine(PlayIn());
    }

    IEnumerator PlayIn()
    {
        var startPos = _rt.anchoredPosition;
        var from = startPos - new Vector2(0, slideDistance);
        var to = startPos;
        float t = 0f;

        _rt.anchoredPosition = from;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, slideDuration);
            _rt.anchoredPosition = Vector2.Lerp(from, to, t);
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Clamp01(t * (slideDuration / fadeDuration));
            yield return null;
        }
        _rt.anchoredPosition = to;
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }
}
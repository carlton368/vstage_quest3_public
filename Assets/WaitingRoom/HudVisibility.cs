// HudVisibility.cs
using UnityEngine;
using System.Collections;

public class HudVisibility : MonoBehaviour
{
    public CanvasGroup group;
    public float idleHideDelay = 3f;
    public float fade = 0.2f;
    float _lastShown;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        ShowNow();
    }

    public void ShowNow()
    {
        _lastShown = Time.time;
        StopAllCoroutines();
        StartCoroutine(CoFadeTo(1f));
    }

    public void OnNewMessageArrived() => ShowNow();

    void Update()
    {
        if (Time.time - _lastShown > idleHideDelay && group.alpha > 0.01f)
            StartCoroutine(CoFadeTo(0f));
    }

    IEnumerator CoFadeTo(float target)
    {
        float start = group.alpha, t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fade;
            group.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
        group.alpha = target;
    }
}
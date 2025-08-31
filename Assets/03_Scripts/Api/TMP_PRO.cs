using TMPro;
using UnityEngine;
using System.Text; 
using System.Collections;
using System.Collections.Generic;
using EasyTextEffects;

public class TMP_PRO : MonoBehaviour
{
    [Header("각 키워드를 표시할 20개의 TMP_Text")] [Tooltip("씬에 배치한 20개의 키워드 텍스트 오브젝트를 순서대로 넣으세요.")]
    public List<TMP_Text> keywordTexts = new List<TMP_Text>(20);

    // [Header("감정 텍스트 (원본 리스트)")]
    // public TMP_Text emotionText;

    [Header("Emotion→Color 매핑 컴포넌트")] public EmotionColorMapper colorMapper;
    
    public float revealDelay = 0.5f;
    
    private Coroutine revealCoroutine;
    
    public List<string>  keywordSamples = new List<string>() { "루나 너무 멋져!!" };
    public List<string> emotionSamples = new List<string>() { "행복" };

    private void Start()
    {
        StartCoroutine(WaitForStore());
    }

    private IEnumerator WaitForStore()
    {
        // AIResponseStore.Instance가 생성될 때까지 대기
        while (AIResponseStore.Instance == null)
        {
            yield return null;
        }

        // 이벤트 구독
        AIResponseStore.Instance.OnDataUpdated += StartReveal;

        // 기존 데이터로 한 번 즉시 갱신
        StartReveal();
    }
    
    // 기존 UpdateText 대신 "순차 표시 시작"
    public void StartReveal()
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);

        revealCoroutine = StartCoroutine(ShowKeywords());
    }
    
    private IEnumerator ShowKeywords()
    {
        var keywords = AIResponseStore.Instance.LatestKeywords;
        var emotions = AIResponseStore.Instance.LatestEmotions;

        // 우선 다 비워두고 시작
        foreach (var txt in keywordTexts)
            txt.text = "";

        int count = Mathf.Min(keywordTexts.Count, keywords.Count);

        for (int i = 0; i < count; i++)
        {
            var txt = keywordTexts[i];
            var col = colorMapper.GetColor(emotions[i]);
            txt.color = col;
            txt.text = keywords[i];

            var vfx = txt.GetComponent<TextEffect>();
            if (vfx) vfx.Refresh();

            Debug.Log($"{i} : {txt.text}, {txt.color}");

            yield return new WaitForSeconds(revealDelay);
        }
    }
    
    [ContextMenu("Test Sequential Reveal")]
    public void TestSequentialReveal()
    {
        if (revealCoroutine != null)
            StopCoroutine(revealCoroutine);
        revealCoroutine = StartCoroutine(TestShowKeywords());
    }
    
    private IEnumerator TestShowKeywords()
    {
        // 기존 keywords 대신 샘플 사용
        var keywords = keywordSamples;
        var emotions = emotionSamples;

        // 초기화
        foreach (var txt in keywordTexts)
            txt.text = "";

        int count = Mathf.Min(keywordTexts.Count, keywords.Count);

        for (int i = 0; i < count; i++)
        {
            var txt = keywordTexts[i];
            var col = colorMapper.GetColor(emotions[i]);
            txt.color = col;
            txt.text = keywords[i];

            var vfx = txt.GetComponent<TextEffect>();
            if (vfx) vfx.Refresh();

            Debug.Log($"[Test] {i} : {txt.text}, {txt.color}");

            yield return new WaitForSeconds(revealDelay);
        }
    }

    // public void UpdateText()
    // {
    //     var keywords = AIResponseStore.Instance.LatestKeywords;
    //     var emotions = AIResponseStore.Instance.LatestEmotions;
    //
    //     Debug.Log("TMP_PRO UpdateText 호출됨");
    //     Debug.Log("키워드: " + string.Join(", ", keywords));
    //     Debug.Log("감정: " + string.Join(", ", emotions));
    //     // 1) 각 키워드 텍스트에 인덱스별 색상과 키워드 할당
    //     var count = Mathf.Min(keywordTexts.Count, keywords.Count);
    //     for (var i = 0; i < count; i++)
    //     {
    //         var txt = keywordTexts[i];
    //         var col = colorMapper.GetColor(emotions[i]);
    //         txt.color = col;
    //         txt.text  = keywords[i];
    //
    //         var vfx = txt.GetComponent<TextEffect>();
    //         vfx.Refresh();
    //         
    //         Debug.Log($"{i} : {txt.text}, {txt.color}");
    //     }
    //     // 남은 텍스트는 빈 문자열 처리
    //     for (var i = count; i < keywordTexts.Count; i++)
    //     {
    //         keywordTexts[i].text = "";
    //     }
    // }
    //
    //
    // [ContextMenu("Test")]
    // public void UpdateTextTest()
    // {
    //     var keywords = keywordSamples;
    //     var emotions = emotionSamples;
    //
    //     Debug.Log("TMP_PRO UpdateText 호출됨");
    //     Debug.Log("키워드: " + string.Join(", ", keywords));
    //     Debug.Log("감정: " + string.Join(", ", emotions));
    //     // 1) 각 키워드 텍스트에 인덱스별 색상과 키워드 할당
    //     var count = Mathf.Min(keywordTexts.Count, keywords.Count);
    //     for (var i = 0; i < count; i++)
    //     {
    //         var txt = keywordTexts[i];
    //         var col = colorMapper.GetColor(emotions[i]);
    //         txt.color = col;
    //         txt.text  = keywords[i];
    //
    //         var vfx = txt.GetComponent<TextEffect>();
    //         vfx.Refresh();
    //
    //         Debug.Log($"{i} : {txt.text}, {txt.color}");
    //     }
    //     // 남은 텍스트는 빈 문자열 처리
    //     for (var i = count; i < keywordTexts.Count; i++)
    //     {
    //         keywordTexts[i].text = "";
    //     }
    // }

    private void OnDestroy()
    {
        if (AIResponseStore.Instance != null)
        {
            AIResponseStore.Instance.OnDataUpdated -= StartReveal;
        }
    }
}
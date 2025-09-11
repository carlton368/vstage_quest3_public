// ChatUIController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChatUIController : MonoBehaviour
{
    [Header("Refs")]
    public FanApiClient_Http apiClient;      
    public MicCaptureSenderPTT micSender;    // 음성 전송시 사용
    public ScrollRect scroll;
    public Transform contentParent;          
    public GameObject bubblePrefab;
    public HudVisibility hudVisibility;      

    [Header("Colors")]
    public Color cheerColor   = new Color(0.90f, 0.98f, 1f);
    public Color infoColor    = new Color(0.92f, 1f, 0.92f);
    public Color requestColor = new Color(1f, 0.97f, 0.90f);
    public Color trollColor   = new Color(1f, 0.90f, 0.90f);

    void Awake()
    {
        if (apiClient) apiClient.OnEvent += HandleEvent;
        if (micSender) micSender.BindApiClient(apiClient); // ★ HTTP API 바인딩
    }

    void OnDestroy()
    {
        if (apiClient) apiClient.OnEvent -= HandleEvent;
    }

    public void HandleEvent(FanEventDto e)
    {
        string prefix = e.Role switch {
            "cheer" => "응원", "info" => "정보",
            "request" => "요청", "troll" => "트롤", _ => "팬"
        };
        string message = $"[{prefix}] {e.FanText}";
        Color bg = e.Role switch {
            "cheer" => cheerColor, "info" => infoColor,
            "request" => requestColor, "troll" => trollColor, _ => Color.white
        };

        var go = Instantiate(bubblePrefab, contentParent);
        var item = go.GetComponent<BubbleItem>();
        if (item != null) item.Setup(message, bg);
        else
        {
            var tmp = go.GetComponentInChildren<TMP_Text>();
            if (tmp) tmp.text = message;
            var img = go.GetComponent<Image>();
            if (img) img.color = bg;
        }

        if (hudVisibility) hudVisibility.OnNewMessageArrived();
        StartCoroutine(ScrollToBottomNextFrame());
    }

    IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
        yield return null;
        if (scroll) scroll.verticalNormalizedPosition = 0f; // 0=bottom
    }

    // UI 버튼: 텍스트 발화 보내기
    public void OnClick_SendSingerText(TMP_InputField input)
    {
        if (!apiClient || !input) return;
        var text = input.text.Trim();
        if (string.IsNullOrEmpty(text)) return;
        StartCoroutine(apiClient.CoSendSingerText(text));
        input.text = "";
    }
}

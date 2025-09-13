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
    
    public Color bubbleColor = new Color(0.95f, 0.97f, 1f);

    void Awake()
    {
        if (apiClient) apiClient.OnEvent += HandleEvent;
        if (micSender) micSender.BindApiClient(apiClient); 
    }

    void OnDestroy()
    {
        if (apiClient) apiClient.OnEvent -= HandleEvent;
    }

    public void HandleEvent(FanEventDto e)
    {
        
        string message = e.FanText;
        
        Color bg = bubbleColor;

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

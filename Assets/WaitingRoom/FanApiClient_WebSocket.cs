// FanApiClient_WebSocket_Newtonsoft.cs
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NativeWebSocket;

public class FanApiClient_WebSocket : MonoBehaviour
{
    [Header("Server")]
    public string wsUrl = "ws://221.163.19.142:58030/ws";  // 포트번호 수정필요
    public float pingIntervalSec = 20f;

    [Header("Events")]
    public Action<FanEventDto> OnEvent;

    private WebSocket _socket;
    private Coroutine _pingLoop;

    [Serializable]
    private class WsEnvelope
    {
        public string type;
        public string op;
        public string status;
        public JToken payload;
        public string msg;
    }

    private async void Start()
    {
        _socket = new WebSocket(wsUrl);

        _socket.OnOpen += () =>
        {
            Debug.Log("[FanWS] open");
            _ = _socket.SendText("{\"type\":\"start\"}");
            if (_pingLoop == null) _pingLoop = StartCoroutine(CoPingLoop());
        };

        _socket.OnError += e => Debug.LogError("[FanWS] error: " + e);

        _socket.OnClose += code =>
        {
            Debug.LogWarning("[FanWS] closed: " + code);
            if (_pingLoop != null) { StopCoroutine(_pingLoop); _pingLoop = null; }
        };

        _socket.OnMessage += bytes =>
        {
            var json = Encoding.UTF8.GetString(bytes);
            try
            {
                var env = JsonConvert.DeserializeObject<WsEnvelope>(json);
                if (env == null || string.IsNullOrEmpty(env.type)) return;

                switch (env.type)
                {
                    case "event":
                        var ev = env.payload?.ToObject<FanEventDto>();
                        if (ev != null) OnEvent?.Invoke(ev);
                        break;
                    case "ack":
                    case "welcome":
                    case "pong":
                        break;
                    case "error":
                        Debug.LogError("[FanWS] server error: " + env.msg);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[FanWS] parse error: " + ex.Message + "\n" + json);
            }
        };

        await _socket.Connect();
    }

    private IEnumerator CoPingLoop()
    {
        var wait = new WaitForSeconds(pingIntervalSec);
        while (true)
        {
            if (_socket != null && _socket.State == WebSocketState.Open)
                _ = _socket.SendText("{\"type\":\"ping\"}");
            yield return wait;
        }
    }

    // 텍스트 전송
    public IEnumerator CoSendSingerText(string singerText, Action onSent = null)
    {
        if (_socket == null || _socket.State != WebSocketState.Open)
        {
            Debug.LogWarning("[FanWS] send failed: socket not open");
            yield break;
        }

        var payload = new { type = "singer_text", text = singerText ?? "" };
        var json = JsonConvert.SerializeObject(payload);

        var task = _socket.SendText(json);
        while (!task.IsCompleted) yield return null;

        if (task.Exception != null) Debug.LogError("[FanWS] send error: " + task.Exception.Message);
        onSent?.Invoke(); // 응답은 서버가 event로 push
    }

    // 오디오(WAV bytes) base64 전송
    public IEnumerator CoSendSingerAudioB64(byte[] wavBytes, Action onSent = null)
    {
        if (_socket == null || _socket.State != WebSocketState.Open)
        {
            Debug.LogWarning("[FanWS] send failed: socket not open");
            yield break;
        }

        string b64 = Convert.ToBase64String(wavBytes ?? Array.Empty<byte>());
        var payload = new
        {
            type = "singer_audio_b64",
            mime = "audio/wav",
            data = b64
        };
        var json = JsonConvert.SerializeObject(payload);

        var task = _socket.SendText(json);
        while (!task.IsCompleted) yield return null;

        if (task.Exception != null) Debug.LogError("[FanWS] audio send error: " + task.Exception.Message);
        onSent?.Invoke(); // 응답은 서버가 event로 push
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _socket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (_pingLoop != null) { StopCoroutine(_pingLoop); _pingLoop = null; }
        if (_socket != null && _socket.State == WebSocketState.Open)
            await _socket.Close();
    }

    private async void OnDestroy()
    {
        if (_pingLoop != null) { StopCoroutine(_pingLoop); _pingLoop = null; }
        if (_socket != null && _socket.State == WebSocketState.Open)
            await _socket.Close();
    }
}

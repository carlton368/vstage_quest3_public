// FanApiClient_Http_Newtonsoft.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FanApiClient_Http : MonoBehaviour
{
    [Header("Server")]
    public string baseUrl = "http://221.163.19.142:58029";
    public float pollIntervalSec = 1.0f;

    [Header("Events")]
    public Action<FanEventDto> OnEvent;

    private Coroutine _pollLoop;

    private IEnumerator Start()
    {
        // 1) 스트림 시작 (POST /stream/start)
        using (var req = UnityWebRequest.PostWwwForm($"{baseUrl}/stream/start", ""))
        {
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[FanApi] /stream/start failed: {req.error}");
            else
                Debug.Log($"[FanApi] started: {req.downloadHandler.text}");
        }

        // 2) 폴링 시작
        _pollLoop = StartCoroutine(CoPollLoop());
    }

    private void OnDestroy()
    {
        if (_pollLoop != null) StopCoroutine(_pollLoop);
    }

    private IEnumerator CoPollLoop()
    {
        var wait = new WaitForSeconds(pollIntervalSec);

        while (true)
        {
            using (var req = UnityWebRequest.Get($"{baseUrl}/events/poll"))
            {
                req.SetRequestHeader("Accept", "application/json");
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    var json = req.downloadHandler.text;
                    try
                    {
                        var resp = JsonConvert.DeserializeObject<PollRespDto>(json);
                        if (resp?.Events != null)
                        {
                            Debug.Log("[FanApi] poll success: " + json);
                            
                            foreach (var ev in resp.Events)
                                OnEvent?.Invoke(ev);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[FanApi] poll parse error: {ex.Message}\n{json}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[FanApi] poll error: {req.error}");
                }
            }

            yield return wait;
        }
    }

    // ---- 가수 텍스트 전송 (POST /singer/respond_text) ----
    public IEnumerator CoSendSingerText(string singerText, Action<FanEventDto> onDone = null)
    {
        var url = $"{baseUrl}/singer/respond_text";
        var payload = new { text = singerText ?? "" };
        var body = JsonConvert.SerializeObject(payload);
        var bytes = Encoding.UTF8.GetBytes(body);

        using (var req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bytes);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var json = req.downloadHandler.text;
                try
                {
                    var ev = JsonConvert.DeserializeObject<FanEventDto>(json);
                    onDone?.Invoke(ev);
                    OnEvent?.Invoke(ev); // 즉시 반영
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FanApi] respond_text parse error: {ex.Message}\n{json}");
                }
            }
            else
            {
                Debug.LogError($"[FanApi] respond_text failed: {req.error}");
            }
        }
    }

    // ---- 가수 오디오 전송 (POST /singer/respond_audio, multipart/form-data) ----
    public IEnumerator CoSendSingerAudioWav(byte[] wavBytes, Action<FanEventDto> onDone = null)
    {
        if (wavBytes == null || wavBytes.Length == 0)
        {
            Debug.LogWarning("[FanApi] audio empty");
            yield break;
        }

        var form = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", wavBytes, "singer.wav", "audio/wav")
        };

        using (var req = UnityWebRequest.Post($"{baseUrl}/singer/respond_audio", form))
        {
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var json = req.downloadHandler.text;
                try
                {
                    var ev = JsonConvert.DeserializeObject<FanEventDto>(json);
                    onDone?.Invoke(ev);
                    OnEvent?.Invoke(ev); // 즉시 반영
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FanApi] respond_audio parse error: {ex.Message}\n{json}");
                }
            }
            else
            {
                Debug.LogError($"[FanApi] respond_audio failed: {req.error}");
            }
        }
    }
}

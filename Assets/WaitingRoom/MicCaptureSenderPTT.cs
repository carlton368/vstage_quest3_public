// MicCaptureSenderPTT.cs
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

public class MicCaptureSenderPTT : MonoBehaviour
{
    [Header("Mic Settings")]
    public int sampleRate = 16000;   // STT 권장
    public string deviceName = null; // null = 기본 장치
    public bool debugLog = true;

    [Header("UI (optional)")]
    public TMP_Text statusLabel;

    private FanApiClient_WebSocket _api; // ★ WebSocket API 참조
    AudioClip _clip;
    bool _recording;
    int _clipStartPos;

    public void BindApiClient(FanApiClient_WebSocket api) => _api = api;

    // UI/Button → OnPointerDown
    public void OnPressDown()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
            Log("마이크 권한 요청 중…");
            return;
        }
#endif
        if (_recording) return;

        _clip = Microphone.Start(deviceName, true, 30, sampleRate); // 최대 30초 버퍼
        _clipStartPos = 0;
        _recording = true;
        Log("녹음 시작 (누르는 동안)");
    }

    // UI/Button → OnPointerUp
    public void OnPressUp()
    {
        if (!_recording) return;

        int endPos = Microphone.GetPosition(deviceName);
        Microphone.End(deviceName);
        _recording = false;

        if (_clip == null || endPos <= 0)
        {
            Log("녹음 길이 0");
            return;
        }

        // 실제 길이 만큼 샘플 추출
        int sampleCount = endPos - _clipStartPos;
        if (sampleCount <= 0) sampleCount = _clip.samples * _clip.channels;

        float[] samples = new float[sampleCount * _clip.channels];
        _clip.GetData(samples, _clipStartPos);

        // WAV bytes 만들기(메모리로 작성)
        byte[] wavBytes = MakeWavBytes(samples, _clip.channels, sampleRate);

        // WS로 전송
        if (_api == null)
        {
            Log("API 참조 없음");
            return;
        }
        StartCoroutine(_api.CoSendSingerAudioB64(wavBytes, () => Log("오디오 전송 완료")));
    }

    void Log(string msg)
    {
        if (debugLog) Debug.Log($"[MicCapture] {msg}");
        if (statusLabel) statusLabel.text = msg;
    }

    // ==== float PCM → WAV bytes (PCM16) ====
    byte[] MakeWavBytes(float[] samples, int channels, int hz)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        short bitsPerSample = 16;
        int sampleCount = samples.Length;
        int byteRate = hz * channels * (bitsPerSample / 8);

        // 헤더
        WriteString(bw, "RIFF");
        bw.Write(36 + sampleCount * 2);
        WriteString(bw, "WAVE");
        WriteString(bw, "fmt ");
        bw.Write(16);
        bw.Write((short)1);            // PCM
        bw.Write((short)channels);
        bw.Write(hz);
        bw.Write(byteRate);
        bw.Write((short)(channels * 2)); // BlockAlign
        bw.Write(bitsPerSample);
        WriteString(bw, "data");
        bw.Write(sampleCount * 2);

        // 데이터 (float -> int16)
        for (int i = 0; i < samples.Length; i++)
        {
            short val = (short)Mathf.Clamp(samples[i] * 32767f, short.MinValue, short.MaxValue);
            bw.Write(val);
        }

        bw.Flush();
        return ms.ToArray();
    }

    static void WriteString(BinaryWriter bw, string s) => bw.Write(System.Text.Encoding.UTF8.GetBytes(s));
}

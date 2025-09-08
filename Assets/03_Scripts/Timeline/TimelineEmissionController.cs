using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// ===== Timeline Signal Receiver =====
public class TimelineEmissionController : MonoBehaviour
{
    [Header("Emission Controller 연결")]
    public GlobalEmissionController emissionController;
    
    [Header("Timeline 연결")]
    public PlayableDirector playableDirector;
    
    [Header("디버그")]
    public bool showDebugLogs = true;
    
    void Start()
    {
        // PlayableDirector가 연결되어 있으면 이벤트 등록
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;
            playableDirector.played += OnTimelinePlayed;
            playableDirector.paused += OnTimelinePaused;
        }
    }
    
    void OnDestroy()
    {
        // 이벤트 해제
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped;
            playableDirector.played -= OnTimelinePlayed;
            playableDirector.paused -= OnTimelinePaused;
        }
    }
    
    // ===== Timeline 이벤트 =====
    
    void OnTimelinePlayed(PlayableDirector director)
    {
        if (showDebugLogs)
            Debug.Log("📽️ Timeline 시작");
    }
    
    void OnTimelinePaused(PlayableDirector director)
    {
        if (showDebugLogs)
            Debug.Log("⏸️ Timeline 일시정지");
    }
    
    void OnTimelineStopped(PlayableDirector director)
    {
        if (showDebugLogs)
            Debug.Log("⏹️ Timeline 종료 - Emission을 끕니다!");
        
        // Timeline이 끝났을 때 Emission 끄기
        if (emissionController != null)
        {
            emissionController.TurnOffEmission();
        }
    }
    
    // ===== Signal Receiver 메서드 (Timeline Signal용) =====
    
    public void OnSignalTurnOffEmission()
    {
        if (showDebugLogs)
            Debug.Log("🔴 Signal: Emission OFF");
        
        if (emissionController != null)
            emissionController.TurnOffEmission();
    }
    
    public void OnSignalTurnOnEmission()
    {
        if (showDebugLogs)
            Debug.Log("🟢 Signal: Emission ON");
        
        if (emissionController != null)
            emissionController.TurnOnEmission();
    }
    
    public void OnSignalRestoreEmission()
    {
        if (showDebugLogs)
            Debug.Log("🔄 Signal: Emission 복구");
        
        if (emissionController != null)
            emissionController.RestoreEmission();
    }
    
    public void OnSignalTurnOffSpecific(int index)
    {
        if (showDebugLogs)
            Debug.Log($"🔴 Signal: Material {index}번 OFF");
        
        if (emissionController != null)
            emissionController.TurnOffSpecificMaterial(index);
    }
    
    // ===== 수동 제어 메서드 =====
    
    public void PlayTimeline()
    {
        if (playableDirector != null)
            playableDirector.Play();
    }
    
    public void StopTimeline()
    {
        if (playableDirector != null)
            playableDirector.Stop();
    }
    
    public void PauseTimeline()
    {
        if (playableDirector != null)
            playableDirector.Pause();
    }
    
    public void RestartTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.time = 0;
            playableDirector.Play();
        }
    }
}
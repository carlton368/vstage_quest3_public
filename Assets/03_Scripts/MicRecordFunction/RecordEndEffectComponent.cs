using UnityEngine;

public class RecordEndEffectComponent : MonoBehaviour
{
    
    [SerializeField] private GameObject flyingEffectPrefab;
    
    private void OnEnable()
    {
        //녹음 종료 이벤트 구독
        //MicComponent.onRecordingFinished += HandleRecordingFinished;
    }

    private void OnDisable()
    {
        //MicComponent.onRecordingFinished -= HandleRecordingFinished;
    }
}

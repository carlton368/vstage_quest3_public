using MicRecordFunction;
using UnityEngine;
using UnityEngine.Events;

public class RecordEndEffectComponent : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform stageImpactPoint;// 무대 도착 지점
    [SerializeField] private float impactRandomRadius = 0.5f; // 착지 지점 랜덤 분산(자연스러움)
    
    [Header("Speed/Arrive")]
    [SerializeField] private float maxSpeed = 2.2f;
    [SerializeField] private float arriveRadius = 2.5f;
    [SerializeField] private float stopDistance = 0.12f;

    [Header("Drift/Hover")]
    [SerializeField] private float wanderStrength = 0.45f;
    [SerializeField] private float wanderFreq = 0.55f;
    [SerializeField] private float hoverAmp = 0.25f;
    [SerializeField] private float hoverFreq = 1.2f;

    [Header("Rotate")]
    [SerializeField] private float turnSpeed = 4f;

    [Header("도착 시 이벤트(꽃 피우기/게이지 등)")]
    public UnityEvent OnArrived;
   
    // 내부 상태
    Vector3 _vel;
    Vector3 _targetPos;
    float _seed;
    bool _moving;
    
    public void SetStageImpactPoint(Transform t) => stageImpactPoint = t; // 필요시 외부에서 세팅
    
    void Awake() => _seed = Random.value * 1000f;
    
    private void OnEnable()
    {
        if (!stageImpactPoint)
        {
            Debug.LogWarning("[RecordEndEffectComponent] stageImpactPoint가 비어있음");
            gameObject.SetActive(false);
            return;
        }

        // 활성화 시점에 타겟 좌표 확정(살짝 랜덤)
        Vector2 r = Random.insideUnitCircle * impactRandomRadius;
        _targetPos = stageImpactPoint.position + new Vector3(r.x, 0f, r.y);

        _vel = Vector3.zero;
        _moving = true;
    }

    private void Update()
    {
        if (!_moving) return;

        Vector3 to = _targetPos - transform.position;
        float dist = to.magnitude;
        Vector3 dir = dist > 0.0001f ? to / dist : Vector3.zero;

        // Arrive(가까울수록 감속)
        float speed = dist < arriveRadius ? Mathf.Lerp(0.1f, maxSpeed, dist / arriveRadius) : maxSpeed;
        Vector3 desired = dir * speed;

        // 드리프트 + 부유
        float t = Time.time + _seed;
        Vector3 wander = new Vector3(
            Mathf.PerlinNoise(t * wanderFreq, 0f) - 0.5f,
            0f,
            Mathf.PerlinNoise(0f, t * wanderFreq) - 0.5f) * 2f;
        Vector3 hover = Vector3.up * Mathf.Sin(t * hoverFreq) * hoverAmp;

        desired += wander * wanderStrength + hover;

        // 부드러운 가감속
        _vel = Vector3.Lerp(_vel, desired, 0.1f);
        transform.position += _vel * Time.deltaTime;

        // 수평 회전
        Vector3 look = _vel; look.y = 0f;
        if (look.sqrMagnitude > 0.0001f)
        {
            Quaternion q = Quaternion.LookRotation(look);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        }

        // 도착 처리
        if (dist <= stopDistance && _vel.magnitude < 0.05f)
        {
            _moving = false;
            OnArrived?.Invoke();  // 꽃/게이지 트리거
            gameObject.SetActive(false); // 스스로 꺼짐(풀링 친화)
        }
    }
    
}

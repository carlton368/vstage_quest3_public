using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [비활성 대기] → SetActive(true) 되는 순간 현재 위치에서 stageImpactPoint 쪽으로
/// 둥둥 떠다니며(드리프트+부유) 접근, 도착하면 OnArrived 호출 후 스스로 비활성화.
/// </summary>
public class RecordEndEffectComponent : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform stageImpactPoint;   // 무대 도착 지점(씬에서 할당)
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
    
    [SerializeField] private float fadeOutGrace = 0.1f; // 추가로 더 기다리고 싶으면

    [Header("도착 시 이벤트(꽃 피우기/게이지 등)")]
    public UnityEvent OnArrived;

    [Header("Flower Spawn")]
    [SerializeField] private GameObject[] flowerPrefabs; // 여러 개 프리팹 넣기
    [SerializeField] private int flowerCount = 5;        // 몇 송이?
    [SerializeField] private float flowerRadius = 1.2f;  // 도착점 주변 반경
    [SerializeField] private float yOffset = 0.02f;      // 바닥 위 살짝 띄우기
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool spawnAsRing = false;
    
    // 내부 상태
    Vector3 _vel;
    Vector3 _targetPos;
    float _seed;
    bool _moving;
    private bool _isFading;
    
    [SerializeField] private bool snapOnArrive = true;   // 도착 시 살짝 스냅
    [SerializeField] private float maxFlightTime = 4.0f; // 안전 타임아웃(초)
    float _flightTimer;

    public void SetStageImpactPoint(Transform t) => stageImpactPoint = t; // 필요시 외부에서 세팅

    // 캐시
    private ParticleSystem[] _particleSystems;
    
    void Awake() {
        _seed = Random.value * 1000f;
        _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    void OnEnable()
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
        _flightTimer = 0f;

        Debug.Log($"[RecordEndEffect] START from {transform.position} -> target {_targetPos} (stop={stopDistance}, arriveR={arriveRadius})");
    }

    void Update()
    {
        if (!_moving) return;

        // 수평 거리만으로 도착을 판정 (y축 호버 영향 제거)
        Vector3 to = _targetPos - transform.position;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float distXZ = toXZ.magnitude;

        // 가까워질수록 드리프트/호버 강도 줄이기 (근접 안정화)
        float arriveT = Mathf.Clamp01(distXZ / arriveRadius); // 멀리=1 -> 가까이=0
        float driftScale = Mathf.Lerp(0.0f, 1.0f, arriveT);   // 가까울수록 0
        float hoverScale = Mathf.Lerp(0.2f, 1.0f, arriveT);   // 완전 0은 너무 딱딱 → 최소 0.2 남김

        // Arrive(가까울수록 감속) - 수평 방향만 사용
        Vector3 dirXZ = (distXZ > 0.0001f) ? (toXZ / distXZ) : Vector3.zero;
        float speed = (distXZ < arriveRadius) ? Mathf.Lerp(0.1f, maxSpeed, distXZ / arriveRadius) : maxSpeed;
        Vector3 desired = dirXZ * speed;

        // 드리프트 + 부유 (축소 적용)
        float tt = Time.time + _seed;
        Vector3 wander = new Vector3(
            Mathf.PerlinNoise(tt * wanderFreq, 0f) - 0.5f,
            0f,
            Mathf.PerlinNoise(0f, tt * wanderFreq) - 0.5f) * (2f * wanderStrength * driftScale);

        Vector3 hover = Vector3.up * (Mathf.Sin(tt * hoverFreq) * hoverAmp * hoverScale);

        desired += wander + hover;

        // 부드러운 가감속
        _vel = Vector3.Lerp(_vel, desired, turnSpeed * Time.deltaTime);
        transform.position += _vel * Time.deltaTime;

        // 수평 회전
        Vector3 look = new Vector3(_vel.x, 0f, _vel.z);
        if (look.sqrMagnitude > 0.0001f)
        {
            Quaternion q = Quaternion.LookRotation(look);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        }

        // --- 도착 / 타임아웃 판정 ---
        _flightTimer += Time.deltaTime;
        bool arrived = distXZ <= stopDistance;          // ✅ 수평 거리로 판정
        bool timeout = _flightTimer >= maxFlightTime;   // 안전 장치

        if ((arrived || timeout) && !_isFading)
        {
            // 근접 떨림 마무리: 수평 스냅(선택)
            if (snapOnArrive)
            {
                transform.position = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);
            }

            _moving = false;
            _isFading = true;

            // 꽃 생성 + 로그
            SpawnFlowersAt(_targetPos);
            Debug.Log("[RecordEndEffect] Arrived -> spawn flowers");

            OnArrived?.Invoke();

            // 파티클 방출 중단 → 남은 수명대로 자연 페이드
            foreach (var ps in _particleSystems)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            StartCoroutine(WaitParticlesAndDisable());
        }
    }
    private System.Collections.IEnumerator WaitParticlesAndDisable()
    {
        // 자식 포함해서 전부 사라질 때까지 대기
        bool AnyAlive()
        {
            foreach (var ps in _particleSystems)
                if (ps.IsAlive(true)) return true;
            return false;
        }

        while (AnyAlive()) yield return null;

        if (fadeOutGrace > 0f) yield return new WaitForSeconds(fadeOutGrace);

        gameObject.SetActive(false); // 풀링/재사용 용이
    }
    
    private void SpawnFlowersAt(Vector3 center)
    {
        if (flowerPrefabs == null || flowerPrefabs.Length == 0) return;

        for (int i = 0; i < flowerCount; i++)
        {
            // 랜덤 위치 계산
            Vector2 p;
            if (spawnAsRing)
            {
                float angle = (Mathf.PI * 2f) * (i / (float)flowerCount);
                p = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * flowerRadius;
            }
            else
            {
                p = Random.insideUnitCircle * flowerRadius;
            }

            Vector3 pos = center + new Vector3(p.x, 2f, p.y);

            // 랜덤 프리팹 선택
            GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];

            // 바닥 붙이기
            if (Physics.Raycast(pos, Vector3.down, out var hit, 5f, groundMask, QueryTriggerInteraction.Ignore))
            {
                pos = hit.point + Vector3.up * yOffset;

                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                rot *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                Instantiate(prefab, pos, rot);
            }
            else
            {
                Instantiate(prefab,
                    center + new Vector3(p.x, yOffset, p.y),
                    Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            }
        }
    }
}

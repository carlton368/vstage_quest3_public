using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [비활성 대기] → SetActive(true) 되는 순간 현재 위치에서 stageImpactPoint 쪽으로
/// 둥둥 떠다니며(드리프트+부유) 접근, 도착하면 꽃 스폰 후 자연 페이드 → 스스로 비활성화.
/// </summary>
public class RecordEndEffectComponent : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform stageImpactPoint;   // 무대 도착 지점(씬에서 할당)
    [SerializeField] private float impactRandomRadius = 0.5f; // 착지 지점 랜덤 분산(자연스러움)

    [Header("Speed/Arrive")]
    [SerializeField] private float maxSpeed = 2.2f;
    [SerializeField] private float arriveRadius = 2.5f;
    [SerializeField] private float stopDistance = 0.25f;   // 살짝 여유 (기존 0.12 → 0.25 권장)

    [Header("Drift/Hover")]
    [SerializeField] private float wanderStrength = 0.45f;
    [SerializeField] private float wanderFreq = 0.55f;
    [SerializeField] private float hoverAmp = 0.25f;
    [SerializeField] private float hoverFreq = 1.2f;

    [Header("Rotate")]
    [SerializeField] private float turnSpeed = 4f;

    [SerializeField] private float fadeOutGrace = 0.1f;    // 파티클 모두 사라진 뒤 추가 대기

    [Header("도착 시 이벤트(꽃 피우기/게이지 등)")]
    public UnityEvent OnArrived;

    [Header("Flower Spawn")]
    [SerializeField] private GameObject[] flowerPrefabs; // 여러 개 프리팹 넣기
    [SerializeField] private int   flowerCount  = 5;     // 몇 송이?
    [SerializeField] private float flowerRadius = 0;  // 도착점 주변 반경
    [SerializeField] private float yOffset      = 0f; // 무대 위 살짝 띄우기
    [SerializeField] private bool  spawnAsRing  = false; // 원형 균등 배치 여부
    // groundMask는 더 이상 사용하지 않지만, 인스펙터 오류 피하려면 남겨둬도 무방
    [SerializeField] private LayerMask groundMask;

    // 내부 상태
    Vector3 _vel;
    Vector3 _targetPos;
    float   _seed;
    bool    _moving;
    bool    _isFading;

    [SerializeField] private bool  snapOnArrive   = true;   // 도착 시 수평 스냅
    [SerializeField] private float maxFlightTime  = 4.0f;   // 안전 타임아웃(초)
    float _flightTimer;

    public void SetStageImpactPoint(Transform t) => stageImpactPoint = t;

    // 캐시
    private ParticleSystem[] _particleSystems;

    void Awake()
    {
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

        // 수평 거리만으로 도착 판정 (y축 호버 영향 제거)
        Vector3 to   = _targetPos - transform.position;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float   distXZ = toXZ.magnitude;

        // 가까울수록 드리프트/호버 축소
        float arriveT    = Mathf.Clamp01(distXZ / arriveRadius); // 멀리=1 → 가까이=0
        float driftScale = Mathf.Lerp(0.0f, 1.0f, arriveT);
        float hoverScale = Mathf.Lerp(0.2f, 1.0f, arriveT);

        // Arrive (수평 방향)
        Vector3 dirXZ = (distXZ > 0.0001f) ? (toXZ / distXZ) : Vector3.zero;
        float   speed = (distXZ < arriveRadius) ? Mathf.Lerp(0.1f, maxSpeed, distXZ / arriveRadius) : maxSpeed;
        Vector3 desired = dirXZ * speed;

        // 드리프트 + 부유 (효율적으로 계산)
        float  tt          = Time.time + _seed;
        float  wf          = tt * wanderFreq;
        float  wanderGain  = (wanderStrength * driftScale) * 2f; // [-1,1] 스케일
        float  hoverGain   =  hoverAmp * hoverScale;
        float  nx          = Mathf.PerlinNoise(wf, 0f) * 2f - 1f;
        float  nz          = Mathf.PerlinNoise(0f, wf) * 2f - 1f;
        Vector3 wander     = new Vector3(nx * wanderGain, 0f, nz * wanderGain);
        Vector3 hover      = new Vector3(0f, Mathf.Sin(tt * hoverFreq) * hoverGain, 0f);

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

        // --- 도착 / 타임아웃 ---
        _flightTimer += Time.deltaTime;
        bool arrived = distXZ <= stopDistance;
        bool timeout = _flightTimer >= maxFlightTime;

        if ((arrived || timeout) && !_isFading)
        {
            if (snapOnArrive)
            {
                transform.position = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);
            }

            _moving   = false;
            _isFading = true;
            
            float stageY = stageImpactPoint != null && stageImpactPoint.TryGetComponent(out Collider col)
                ? col.bounds.min.y + yOffset
                : transform.position.y;
            Debug.Log($"[RecordEndEffect] Arrived at {transform.position}, stageY={stageY}");

            // ✅ 이펙트가 사라진 "현재 위치"에서 꽃 스폰
            SpawnFlowersAt(transform.position);
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
        bool AnyAlive()
        {
            foreach (var ps in _particleSystems)
                if (ps.IsAlive(true)) return true;
            return false;
        }

        while (AnyAlive()) yield return null;

        if (fadeOutGrace > 0f) yield return new WaitForSeconds(fadeOutGrace);

        gameObject.SetActive(false); // 풀링/재사용
    }

    // ── 꽃 스폰: 현재 지점 중심으로 분산, 바닥 레이캐스트 없이 간단히 ──
    private void SpawnFlowersAt(Vector3 center)
    {
        if (flowerPrefabs == null || flowerPrefabs.Length == 0) return;

        // 무대 콜라이더 바닥 높이 -> 이미 stageImpactPoint에 콜라이더 있다고 했으니 이 값이 정확한 바닥
        float stageY = stageImpactPoint != null && stageImpactPoint.TryGetComponent(out Collider col)
            ? col.bounds.min.y + yOffset
            : center.y + yOffset;

        for (int i = 0; i < flowerCount; i++)
        {
            Vector2 p = spawnAsRing
                ? new Vector2(Mathf.Cos((Mathf.PI * 2f) * (i / (float)flowerCount)),
                    Mathf.Sin((Mathf.PI * 2f) * (i / (float)flowerCount))) * flowerRadius
                : Random.insideUnitCircle * flowerRadius;

            // 일단 바닥 높이에 맞춰 스폰
            Vector3 pos = new Vector3(center.x + p.x, stageY, center.z + p.y);
            GameObject prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            var go = Instantiate(prefab, pos, rot);

            // ✅ 스폰 후, 렌더러 바닥을 무대 y에 정확히 맞추기 (피벗이 중앙이어도 OK)
            SnapRendererBottomToY(go.transform, stageY);
            Debug.Log($"[FlowerSpawn] Flower {i} at {go.transform.position}, stageY={stageY}");
        }
    }

    // 렌더러(자식 포함)의 bounds.min.y를 기준으로 아래쪽을 무대 y에 스냅
    private void SnapRendererBottomToY(Transform t, float targetY)
    {
        // 여러 렌더러가 있을 수 있으니 모두 검사
        var renderers = t.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0) return;

        // 가장 낮은 minY를 바닥으로 간주
        float minY = float.PositiveInfinity;
        foreach (var r in renderers)
        {
            // 비활성 렌더러는 무시하고 싶으면 아래 줄로 필터링 가능
            // if (!r.enabled) continue;
            if (r.bounds.min.y < minY) minY = r.bounds.min.y;
        }

        if (float.IsInfinity(minY)) return;

        float delta = minY - targetY;        // 현재 바닥과 목표 바닥의 차이
        if (Mathf.Abs(delta) > 0.0001f)
            t.position += Vector3.down * delta;  // 그대로 내려(또는 올려) 맞춤
    }
}

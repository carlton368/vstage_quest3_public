using UnityEngine;
using UnityEngine.Events;

public class RecordEndEffectComponent : MonoBehaviour
{
    [Header("Flower Sequence (씬에 배치된 꽃들 순서대로 넣기)")]
    [SerializeField] private FlowerTarget[] flowerOrder;
    [SerializeField] private bool loopSequence = false;
    [SerializeField] private int startIndex = 0;

    private static int s_nextIndex = -1;

    [Header("Move Target (자동 세팅됨)")]
    [SerializeField] private Transform stageImpactPoint;
    [SerializeField] private float impactRandomRadius = 0.0f;

    [Header("Speed/Arrive")]
    [SerializeField] private float maxSpeed = 2.2f;
    [SerializeField] private float arriveRadius = 2.5f;
    [SerializeField] private float stopDistance = 0.25f;

    [Header("Drift/Hover")]
    [SerializeField] private float wanderStrength = 0.45f;
    [SerializeField] private float wanderFreq = 0.55f;
    [SerializeField] private float hoverAmp = 0.25f;
    [SerializeField] private float hoverFreq = 1.2f;

    [Header("Rotate")]
    [SerializeField] private float turnSpeed = 4f;

    [SerializeField] private float fadeOutGrace = 0.1f;
    public UnityEvent OnArrived;

    // 내부 상태
    Vector3 _vel, _targetPos;
    float _seed;
    bool _moving, _isFading;
    float _flightTimer;
    [SerializeField] private bool snapOnArrive = true;
    [SerializeField] private float maxFlightTime = 4.0f;

    private ParticleSystem[] _particleSystems;
    private FlowerTarget _currentFlower;

    void Awake()
    {
        _seed = Random.value * 1000f;
        _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        if (s_nextIndex < 0) s_nextIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, (flowerOrder?.Length ?? 1) - 1));
    }

    void OnEnable()
    {
        _currentFlower = PickNextFlower();
        if (_currentFlower == null)
        {
            Debug.LogWarning("[RecordEndEffect] 사용할 꽃이 없습니다.");
            gameObject.SetActive(false);
            return;
        }

        stageImpactPoint = _currentFlower.transform;

        Vector2 r = (impactRandomRadius > 0f) ? Random.insideUnitCircle * impactRandomRadius : Vector2.zero;
        _targetPos = stageImpactPoint.position + new Vector3(r.x, 0f, r.y);

        _isFading = false; _moving = true; _flightTimer = 0f; _vel = Vector3.zero;
        if (_particleSystems != null)
        {
            foreach (var ps in _particleSystems) { ps.Clear(true); ps.Play(true); }
        }

        Debug.Log($"[RecordEndEffect] Start -> flower[{s_nextIndex}] {_currentFlower.name}, target={_targetPos}");
    }

    void OnDisable()
    {
        _moving = false; _isFading = false; _vel = Vector3.zero; _flightTimer = 0f;
        StopAllCoroutines();
        if (_particleSystems != null) foreach (var ps in _particleSystems) ps.Clear(true);
    }

    void Update()
    {
        if (!_moving) return;

        // 수평 이동 + 드리프트/부유
        Vector3 to   = _targetPos - transform.position;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float distXZ = toXZ.magnitude;

        // ★ 근접 구간에서 드리프트/호버를 빠르게 줄여 떨림 제거
        float nearRadius = Mathf.Max(stopDistance * 2f, 0.05f);   // 도착 반경의 2배 정도
        float arriveT    = Mathf.Clamp01(distXZ / arriveRadius); // 멀수록 1, 가까울수록 0
        float nearT      = Mathf.Clamp01(distXZ / nearRadius);   // 아주 가까울수록 0

        float driftScale = (distXZ < nearRadius) ? nearT : Mathf.Lerp(0f, 1f, arriveT);
        float hoverScale = (distXZ < nearRadius) ? nearT * 0.2f : Mathf.Lerp(0.2f, 1f, arriveT);

        Vector3 dirXZ = (distXZ > 0.0001f) ? (toXZ / distXZ) : Vector3.zero;
        float   speed = (distXZ < arriveRadius) ? Mathf.Lerp(0.1f, maxSpeed, distXZ / arriveRadius) : maxSpeed;
        Vector3 desired = dirXZ * speed;

        float  tt         = Time.time + _seed;
        float  wf         = tt * wanderFreq;
        float  wanderGain = (wanderStrength * driftScale) * 2f;
        float  hoverGain  =  hoverAmp * hoverScale;
        float  nx         = Mathf.PerlinNoise(wf, 0f) * 2f - 1f;
        float  nz         = Mathf.PerlinNoise(0f, wf) * 2f - 1f;

        Vector3 wander = new Vector3(nx * wanderGain, 0f, nz * wanderGain);
        Vector3 hover  = new Vector3(0f, Mathf.Sin(tt * hoverFreq) * hoverGain, 0f);
        desired += wander + hover;

        _vel = Vector3.Lerp(_vel, desired, turnSpeed * Time.deltaTime);
        transform.position += _vel * Time.deltaTime;

        Vector3 look = new Vector3(_vel.x, 0f, _vel.z);
        if (look.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), turnSpeed * Time.deltaTime);

        // 도착/타임아웃
        _flightTimer += Time.deltaTime;
        bool arrived = distXZ <= stopDistance;
        bool timeout = _flightTimer >= maxFlightTime;

        if ((arrived || timeout) && !_isFading)
        {
            // ★ XYZ 전부 정확히 목표로 스냅
            if (snapOnArrive) transform.position = _targetPos;

            _moving = false; _isFading = true;

            // 이번 타겟 꽃 켜기 (머티리얼 Stars Emission Power 트윈)
            if (_currentFlower) _currentFlower.Activate();

            OnArrived?.Invoke();

            foreach (var ps in _particleSystems)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            StartCoroutine(WaitParticlesAndDisable());

            AdvanceIndex();
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
        gameObject.SetActive(false);
    }

    // ---- 순서 관리 ----
    private FlowerTarget PickNextFlower()
    {
        if (flowerOrder == null || flowerOrder.Length == 0) return null;

        int n = flowerOrder.Length;
        if (s_nextIndex < 0 || s_nextIndex >= n) s_nextIndex = 0;

        // 꺼진 꽃 우선 탐색
        for (int k = 0; k < n; k++)
        {
            int idx = (s_nextIndex + k) % n;
            var f = flowerOrder[idx];
            if (f != null && !f.IsOn) { s_nextIndex = idx; return f; }
        }

        // 전부 켜져 있으면 순서 유지(루프 시 계속 사용)
        return loopSequence ? flowerOrder[s_nextIndex % n] : null;
    }

    private void AdvanceIndex()
    {
        if (flowerOrder == null || flowerOrder.Length == 0) return;
        s_nextIndex++;
        if (s_nextIndex >= flowerOrder.Length)
            s_nextIndex = loopSequence ? 0 : flowerOrder.Length - 1;
    }
}

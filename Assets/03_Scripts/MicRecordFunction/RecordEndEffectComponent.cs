using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class RecordEndEffectComponent : MonoBehaviour
{
    [Header("Flower Sequence (씬에 배치된 꽃들 순서대로 넣기)")]
    [SerializeField] private FlowerTarget[] flowerOrder;
    [SerializeField] private bool loopSequence = false;
    [SerializeField] private int startIndex = 0;

    // ---- 전역 상태 ----
    private static int  s_nextIndex = -1;
    private static int  s_activatedCount = 0;          // 지금까지 켜진 꽃 개수
    private static bool s_autoSequenceRunning = false;  // 자동 시퀀스 중복 방지

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
    public UnityEvent OnAutoSequenceComplete;

    [Header("Auto sequence after first two")]
    [SerializeField] private bool  autoLightRestAfterTwo = true; // 두 송이 후 자동 진행
    [SerializeField] private float autoDelay = 0.25f;            // 각 꽃 사이 간격(초)

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

        // 처음 진입 시 이미 켜진 것이 있으면 카운트 보정
        if (s_activatedCount == 0 && flowerOrder != null)
        {
            int countOn = 0;
            foreach (var f in flowerOrder) if (f != null && f.IsOn) countOn++;
            s_activatedCount = countOn;
        }
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
            foreach (var ps in _particleSystems) { ps.Clear(true); ps.Play(true); }

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

        // 목표는 매 프레임 꽃의 현재 위치
        Vector3 target = stageImpactPoint ? stageImpactPoint.position : _targetPos;

        // 수평 이동 계산
        Vector3 to   = target - transform.position;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float   distXZ = toXZ.magnitude;

        float nearRadius = Mathf.Max(stopDistance * 2f, 0.05f);
        float arriveT    = Mathf.Clamp01(distXZ / arriveRadius);
        float nearT      = Mathf.Clamp01(distXZ / nearRadius);

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

        // 근접 구간은 SmoothDamp로 오버슈트 방지
        if (distXZ < nearRadius)
        {
            float smooth = Mathf.Lerp(0.06f, 0.18f, nearT);
            transform.position = Vector3.SmoothDamp(transform.position,
                                                    new Vector3(target.x, transform.position.y, target.z),
                                                    ref _vel, smooth, maxSpeed);
        }
        else
        {
            _vel = Vector3.Lerp(_vel, desired, turnSpeed * Time.deltaTime);
            transform.position += _vel * Time.deltaTime;
        }

        // 회전
        Vector3 look = new Vector3(_vel.x, 0f, _vel.z);
        if (look.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), turnSpeed * Time.deltaTime);

        // 도착/타임아웃
        _flightTimer += Time.deltaTime;
        bool arrived = distXZ <= stopDistance;
        bool timeout = _flightTimer >= maxFlightTime;

        if ((arrived || timeout) && !_isFading)
        {
            transform.position = target;

            _moving = false;
            _isFading = true;

            // 이번 꽃 켜기
            if (_currentFlower && !_currentFlower.IsOn)
            {
                _currentFlower.Activate();
                s_activatedCount++;
            }

            OnArrived?.Invoke();

            foreach (var ps in _particleSystems)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            StartCoroutine(WaitParticlesAndDisable());
            AdvanceIndex();

            // 두 송이 켜졌고, 아직 자동 시퀀스가 안 돌고 있다면 전역에서 시작
            if (autoLightRestAfterTwo && s_activatedCount >= 2 && !s_autoSequenceRunning)
            {
                s_autoSequenceRunning = true;

                // 필요한 값 캡처해서 전역 러너로 실행
                var listCopy = flowerOrder;                 // 필요하면 .ToArray()로 복사
                float delay  = Mathf.Max(0f, autoDelay);

                GlobalCoroutineRunner.Run(AutoLightRestCoroutineStatic(
                    listCopy,
                    delay,
                    () =>
                    {
                        s_autoSequenceRunning = false;
                        // ★ 모든 꽃 자동 점등 완료 시 이벤트 발동
                        try { OnAutoSequenceComplete?.Invoke(); } catch {}
                    } // 완료 콜백
                 ));
            }
        }
    }

    private IEnumerator WaitParticlesAndDisable()
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

    // ---- 자동 시퀀스(전역에서 실행): 나머지 모두 켜기 ----
    private static IEnumerator AutoLightRestCoroutineStatic(
        FlowerTarget[] flowerOrder,
        float autoDelay,
        System.Action onComplete
    )
    {
        if (flowerOrder != null)
        {
            for (int i = 0; i < flowerOrder.Length; i++)
            {
                var f = flowerOrder[i];
                if (f == null || f.IsOn) continue;

                f.Activate();       // 즉시 OnValue로 점등
                s_activatedCount++; // 통계용
                if (autoDelay > 0f)
                    yield return new WaitForSeconds(autoDelay);
            }
        }
        onComplete?.Invoke();
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

        // 전부 켜져 있으면 자동 진행만 필요 → null 반환해도 OK
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

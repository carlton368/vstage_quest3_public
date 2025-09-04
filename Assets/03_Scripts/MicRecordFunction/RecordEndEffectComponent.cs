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
    }

    void Update()
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
        if (dist <= stopDistance && _vel.magnitude < 0.05f && !_isFading)
        {
            _moving = false;
            _isFading = true;
            
            SpawnFlowersAt(_targetPos);
            Debug.Log("녹음 이펙트 도착 완료 및 꽃 생성");

            OnArrived?.Invoke(); // 꽃/게이지 등

            // 1) 더 이상 방출하지 않도록 정지 (기존 입자들은 수명에 따라 서서히 사라짐)
            foreach (var ps in _particleSystems)
                ps.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);

            // 2) 모든 파티클이 사라질 때까지 대기 후 오브젝트 비활성화
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

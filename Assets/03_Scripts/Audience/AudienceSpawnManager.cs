using UnityEngine;

[System.Serializable]
public class AudienceSpawnPoint {
    public Transform playerAnchor;   // XR Origin 둘 자리
    public Transform guideAnchor;    // User Guide Canvas 자리(없으면 null)
}

public class AudienceSpawnManager : MonoBehaviour
{
    [Header("Two spawn points")]
    public AudienceSpawnPoint pointA;
    public AudienceSpawnPoint pointB;

    [Header("Refs")]
    public Transform xrOrigin;           // XR Origin 루트
    public Transform userGuideCanvas;    // World Space Canvas
    public Transform faceTarget;         // 무대 중앙 등, 바라볼 목표(선택)

    [Header("Guide fallback offset (if no guideAnchor)")]
    public float guideForward = 1.2f;
    public float guideUp = 0.2f;

    [ContextMenu("Place Audience Random")]
    public void PlaceAudienceRandom()
    {
        var chosen = (Random.value < 0.5f) ? pointA : pointB;

        // XR Origin 배치
        xrOrigin.SetPositionAndRotation(chosen.playerAnchor.position, chosen.playerAnchor.rotation);

        // (선택) XR Origin이 무대를 보게 정렬하고 싶다면
        if (faceTarget != null)
        {
            var dir = (faceTarget.position - xrOrigin.position);
            dir.y = 0f; // 수평만
            if (dir.sqrMagnitude > 0.0001f)
                xrOrigin.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }

        // 가이드 캔버스 배치
        if (userGuideCanvas != null)
        {
            if (chosen.guideAnchor != null)
            {
                userGuideCanvas.SetPositionAndRotation(
                    chosen.guideAnchor.position, chosen.guideAnchor.rotation);
            }
            else
            {
                // 앵커 없으면 XR Origin 앞/위에 간단히
                var forward = xrOrigin.forward;
                var pos = xrOrigin.position + forward * guideForward + Vector3.up * guideUp;
                userGuideCanvas.position = pos;
                userGuideCanvas.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }
    }

    void Start()
    {
        // 앱 시작/입장 시 한 번 호출
        PlaceAudienceRandom();
    }

    // 씬에서 포인트를 보기 쉽게 기즈모 표시
    void OnDrawGizmosSelected()
    {
        Draw(pointA, Color.cyan);
        Draw(pointB, Color.magenta);

        void Draw(AudienceSpawnPoint p, Color c)
        {
            if (p == null || p.playerAnchor == null) return;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(p.playerAnchor.position, 0.2f);
            Gizmos.DrawRay(p.playerAnchor.position, p.playerAnchor.forward * 0.6f);
            if (p.guideAnchor != null)
            {
                Gizmos.color = new Color(c.r, c.g, c.b, 0.6f);
                Gizmos.DrawCube(p.guideAnchor.position, Vector3.one * 0.12f);
            }
        }
    }
}

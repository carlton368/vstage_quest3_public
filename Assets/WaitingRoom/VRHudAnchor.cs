// VRHudAnchor.cs
using UnityEngine;

public class VRHudAnchor : MonoBehaviour
{
    public Transform hmd;          // XR Origin의 Main Camera Transform
    public float distance = 1.2f;  // 카메라 앞 거리
    public float lateral = 0.25f;  // 오른쪽 오프셋
    public float height = -0.05f;  // 약간 아래
    public float posSmooth = 10f;  // 위치 보간
    public float rotSmooth = 10f;  // 회전 보간
    public Vector2 sizeMeters = new Vector2(0.6f, 0.8f);

    RectTransform _rt;

    void Awake()
    {
        _rt = transform as RectTransform;
        if (_rt != null) _rt.sizeDelta = sizeMeters * 1000f; // 1m ≈ 1000 units (World Space Canvas)
    }

    void LateUpdate()
    {
        if (!hmd) return;

        Vector3 fwd = hmd.forward; fwd.y = 0f; fwd.Normalize();
        if (fwd.sqrMagnitude < 0.0001f) fwd = hmd.forward;

        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;
        Vector3 targetPos = hmd.position + fwd * distance + right * lateral + Vector3.up * height;
        Quaternion targetRot = Quaternion.LookRotation(targetPos - hmd.position, Vector3.up);

        transform.position = Vector3.Lerp(transform.position, targetPos, 1 - Mathf.Exp(-posSmooth * Time.deltaTime));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1 - Mathf.Exp(-rotSmooth * Time.deltaTime));
    }
}
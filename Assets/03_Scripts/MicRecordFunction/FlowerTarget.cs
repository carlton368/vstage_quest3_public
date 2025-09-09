using UnityEngine;

public class FlowerTarget : MonoBehaviour
{
    [Header("Renderer(s) - 비우면 자동 수집")]
    [SerializeField] private Renderer[] renderers;

    [Header("Shader Property")]
    [SerializeField] private string starsEmissionProp = "_StarsEmissionPower"; // Shader Graph Reference
    [SerializeField] private float offValue = 0f;   // 꺼짐 값
    [SerializeField] private float onValue  = 6f;   // 켜짐 값(원하는 수치로 지정)
    [SerializeField] private bool applyImmediate = true; // 즉시 적용(트윈 안함)

    [Header("Fallback (선택)")]
    [SerializeField] private bool alsoSetSharedMaterialIfNoMPB = true;

    public bool IsOn { get; private set; }

    MaterialPropertyBlock _mpb;
    Renderer[] _targets; // 유효 렌더러(프로퍼티 있는 것만)

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        // starsEmissionProp를 가진 렌더러만 골라냄
        var list = new System.Collections.Generic.List<Renderer>();
        foreach (var r in renderers)
        {
            if (r && r.sharedMaterial && r.sharedMaterial.HasProperty(starsEmissionProp))
                list.Add(r);
        }
        _targets = list.ToArray();

        _mpb = new MaterialPropertyBlock();
        SetEmission(offValue); // 시작은 꺼짐
        IsOn = false;
    }

    public void Activate()
    {
        if (IsOn) return;
        IsOn = true;

        if (applyImmediate)
        {
            SetEmission(onValue);     // ✅ 즉시 지정값으로 세팅
        }
        else
        {
            // 필요하면 나중에 트윈 추가할 자리 (현재 요청은 즉시 적용이므로 생략)
            SetEmission(onValue);
        }
    }

    public void ResetOff(bool instant = true)
    {
        IsOn = false;
        SetEmission(offValue);
    }

    private void SetEmission(float v)
    {
        if (_targets.Length == 0)
        {
            // 최후의 수단: 지정된 renderers의 sharedMaterial에 직접 세팅
            if (alsoSetSharedMaterialIfNoMPB && renderers != null)
            {
                foreach (var r in renderers)
                    if (r && r.sharedMaterial && r.sharedMaterial.HasProperty(starsEmissionProp))
                        r.sharedMaterial.SetFloat(starsEmissionProp, v);
            }
            return;
        }

        foreach (var r in _targets)
        {
            r.GetPropertyBlock(_mpb);
            _mpb.SetFloat(starsEmissionProp, v);
            r.SetPropertyBlock(_mpb);
        }
    }
}

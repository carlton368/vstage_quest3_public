using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VFXColorController : MonoBehaviour
{
    public VisualEffect vfx;
    [ColorUsage(true, true)] public Color colorA = Color.red;
    [ColorUsage(true, true)] public Color colorB = Color.yellow;

    [Range(0.5f, 10f)] public float intensityA = 10f;

    [Range(0.5f, 10f)] public float intensityB = 10f;

    [Header("순환할 컬러(10개)")]
    [ColorUsage(true, true)] public Color[] cycleColors = new Color[10];
    private int indexA = 0;
    private int indexB = 5;

    void Start()
    {
        if (vfx != null)
        {
            // cycleColors가 설정되어 있으면 그 값을 우선 사용
            if (cycleColors != null && cycleColors.Length > 0)
            {
                colorA = cycleColors.Length > 0 ? cycleColors[indexA % cycleColors.Length] : colorA;
                colorB = cycleColors.Length > 1 ? cycleColors[indexB % cycleColors.Length] : colorB;
            }
            vfx.SetVector4("Color A", colorA * intensityA);
            vfx.SetVector4("Color B", colorB * intensityB);
        }

        StartCoroutine(ChangeColorsRoutine());
    }

    IEnumerator ChangeColorsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (vfx != null && cycleColors != null && cycleColors.Length > 0)
            {
                colorA = cycleColors[indexA % cycleColors.Length];
                vfx.SetVector4("Color A", colorA * intensityA);
                indexA = (indexA + 1) % cycleColors.Length;
            }

            yield return new WaitForSeconds(1f);

            if (vfx != null && cycleColors != null && cycleColors.Length > 0)
            {
                colorB = cycleColors[indexB % cycleColors.Length];
                vfx.SetVector4("Color B", colorB * intensityB);
                indexB = (indexB + 1) % cycleColors.Length;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.VFX;

public class VFXWaveTrigger : MonoBehaviour
{
    public VisualEffect vfx;
    public float speed = 3f;

    void Update()
    {
        // 스페이스 누를 때만 시간 흘리기
        if (Input.GetKey(KeyCode.Space))
            vfx.SetFloat("WaveTime", Time.time * speed);
        // 떼면 멈춤 (원하면 주석 해제)
        else
            vfx.SetFloat("WaveTime", 0f);
    }
}

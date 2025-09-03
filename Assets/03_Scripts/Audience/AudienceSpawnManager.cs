using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class AudienceRigSet {
    public GameObject xrOrigin;     // XR Origin 루트(카메라 포함)
    public GameObject guideCanvas;  // Concert User Guide Canvas
}

public class AudienceSpawnManager : MonoBehaviour
{
    [Header("미리 배치해 둔 2세트(또는 N세트)")]
    public AudienceRigSet[] sets;

    [Header("옵션")]
    public bool deactivateOthers = true;  // 선택된 것 외 나머지 전부 꺼두기
    public bool setCanvasCameraIfNeeded = true; // Canvas가 Screen Space - Camera일 때 카메라 연결

    int _currentIndex = -1;

    [ContextMenu("Activate Random Set")]
    public void ActivateRandom()
    {
        if (sets == null || sets.Length == 0) return;
        int idx = Random.Range(0, sets.Length);
        Activate(idx);
    }

    public void Activate(int index)
    {
        index = Mathf.Clamp(index, 0, sets.Length - 1);

        if (deactivateOthers)
        {
            for (int i = 0; i < sets.Length; i++)
            {
                if (i == index) continue;
                if (sets[i].xrOrigin)    sets[i].xrOrigin.SetActive(false);
                if (sets[i].guideCanvas) sets[i].guideCanvas.SetActive(false);
            }
        }

        // 순서: XR Origin → Canvas (카메라 레퍼런스 문제 방지)
        if (sets[index].xrOrigin)    sets[index].xrOrigin.SetActive(true);
        if (sets[index].guideCanvas) sets[index].guideCanvas.SetActive(true);

        // Canvas가 Screen Space - Camera 방식이면, 활성 XR Origin의 Camera를 연결
        if (setCanvasCameraIfNeeded && sets[index].guideCanvas)
        {
            var canvas = sets[index].guideCanvas.GetComponent<Canvas>();
            if (canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                var cam = sets[index].xrOrigin.GetComponentInChildren<Camera>(true);
                canvas.worldCamera = cam;
            }
        }

        _currentIndex = index;
    }

}
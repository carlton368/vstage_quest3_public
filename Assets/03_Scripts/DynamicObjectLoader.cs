using System.Collections;
using UnityEngine;

public class DynamicObjectLoader : MonoBehaviour
{
    // 인스펙터 창에서 로딩할 프리팹들을 순서대로 할당합니다.
    public GameObject[] objectsToLoad;

    // 프리팹들이 생성될 부모 오브젝트 (정리용)
    public Transform objectContainer;

    void Start()
    {
        // 코루틴을 시작하여 오브젝트 로딩을 시작합니다.
        StartCoroutine(LoadObjectsSequentially());
    }

    IEnumerator LoadObjectsSequentially()
    {
        Debug.Log("Starting dynamic object loading...");

        for (int i = 0; i < objectsToLoad.Length; i++)
        {
            if (objectsToLoad[i] != null)
            {
                // 프리팹을 인스턴스화하여 씬에 생성합니다.
                // objectContainer가 할당되었다면 그 자식으로 생성합니다.
                Instantiate(objectsToLoad[i], objectContainer);

                Debug.Log($"Loaded object {i + 1}/{objectsToLoad.Length}: {objectsToLoad[i].name}");

                // 다음 프레임까지 대기하여 로딩 부하를 분산시킵니다.
                yield return null;
            }
        }

        Debug.Log("Dynamic object loading complete.");
    }
}

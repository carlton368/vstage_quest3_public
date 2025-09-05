using UnityEngine;

[DefaultExecutionOrder(-100000)]
public class DebugDisable : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = false;
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEmissionController : MonoBehaviour
{
    [Header("전체 제어 설정")]
    [SerializeField] private List<Material> targetMaterials = new List<Material>(); // 여러 Material 관리
    [SerializeField] private bool turnOffOnStart = true;
    
    // 각 Material의 원본 Emission 상태 저장
    private Dictionary<Material, bool> originalEmissionStates = new Dictionary<Material, bool>();
    
    void Start()
    {
        // 원본 상태 저장
        SaveOriginalStates();
        
    }
    
    void SaveOriginalStates()
    {
        foreach (Material mat in targetMaterials)
        {
            if (mat != null)
            {
                // 각 Material의 Emission 활성화 상태 저장
                bool wasEnabled = mat.IsKeywordEnabled("_EMISSION");
                originalEmissionStates[mat] = wasEnabled;
                
                Debug.Log($"Material '{mat.name}' - 원본 Emission 상태: {wasEnabled}");
            }
        }
    }
    
    // 모든 Material의 Emission 끄기
    public void TurnOffEmission()
    {
        int count = 0;
        foreach (Material mat in targetMaterials)
        {
            if (mat != null)
            {
                mat.DisableKeyword("_EMISSION");
                count++;
            }
        }
        
        Debug.Log($"{count}개 Material의 Emission이 꺼졌습니다.");
    }
    
    // 모든 Material의 Emission 켜기
    public void TurnOnEmission()
    {
        int count = 0;
        foreach (Material mat in targetMaterials)
        {
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                count++;
            }
        }
        
        Debug.Log($"{count}개 Material의 Emission이 켜졌습니다.");
    }
    
    // 원래 상태로 복구
    public void RestoreEmission()
    {
        foreach (Material mat in targetMaterials)
        {
            if (mat != null && originalEmissionStates.ContainsKey(mat))
            {
                if (originalEmissionStates[mat])
                {
                    mat.EnableKeyword("_EMISSION");
                }
                else
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
        
        Debug.Log("모든 Material이 원래 상태로 복구되었습니다.");
    }
    
    // 특정 Material만 제어
    public void TurnOffSpecificMaterial(int index)
    {
        if (index >= 0 && index < targetMaterials.Count && targetMaterials[index] != null)
        {
            targetMaterials[index].DisableKeyword("_EMISSION");
            Debug.Log($"Material '{targetMaterials[index].name}'의 Emission이 꺼졌습니다.");
        }
    }
    
    public void TurnOnSpecificMaterial(int index)
    {
        if (index >= 0 && index < targetMaterials.Count && targetMaterials[index] != null)
        {
            targetMaterials[index].EnableKeyword("_EMISSION");
            Debug.Log($"Material '{targetMaterials[index].name}'의 Emission이 켜졌습니다.");
        }
    }
    
    
    // Material 추가/제거 메서드
    public void AddMaterial(Material newMaterial)
    {
        if (newMaterial != null && !targetMaterials.Contains(newMaterial))
        {
            targetMaterials.Add(newMaterial);
            originalEmissionStates[newMaterial] = newMaterial.IsKeywordEnabled("_EMISSION");
            Debug.Log($"Material '{newMaterial.name}'이 추가되었습니다.");
        }
    }
    
    public void RemoveMaterial(Material material)
    {
        if (targetMaterials.Contains(material))
        {
            targetMaterials.Remove(material);
            originalEmissionStates.Remove(material);
            Debug.Log($"Material '{material.name}'이 제거되었습니다.");
        }
    }
    
    // 에디터 종료 시 원본 상태 복구
    void OnApplicationQuit()
    {
        #if UNITY_EDITOR
        RestoreEmission();
        #endif
    }
    
    // 에디터에서 플레이 모드 종료 시 복구
    #if UNITY_EDITOR
    void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            RestoreEmission();
        }
    }
    #endif
}
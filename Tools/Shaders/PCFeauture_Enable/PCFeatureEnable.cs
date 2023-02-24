using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PCFeatureEnable : UdonSharpBehaviour
{
    [SerializeField] private Material[] _materials;

    private void Start()
    {
        foreach (Material mat in _materials)
        {
            if (!mat.HasProperty("_IsWindows")) continue;
#if UNITY_64
            mat.SetFloat("_IsWindows", 1);
#else
            mat.SetFloat("_IsWindows", 0);
#endif
        }
        gameObject.SetActive(false);
    }
}
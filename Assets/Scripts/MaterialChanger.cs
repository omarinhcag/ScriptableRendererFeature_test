using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] bool enabledState;
    [SerializeField] string axisName;
    [SerializeField] string propertyName;

    Renderer[] renderers;
    MaterialPropertyBlock mpb;
    int _id;
    float currentValue;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        _id = Shader.PropertyToID(propertyName);
    }

    void Update()
    {
        if (enabledState == false) return;
        if (axisName.Length == 0) return;

        float input = Input.GetAxis(axisName);
        if (input != 0)
        {
            foreach (var rend in renderers)
            {
                rend.GetPropertyBlock(mpb);
                currentValue = Mathf.Clamp01(currentValue + input * Time.deltaTime);
                mpb.SetFloat(_id, currentValue);
                rend.SetPropertyBlock(mpb);
            }
        }
    }
}

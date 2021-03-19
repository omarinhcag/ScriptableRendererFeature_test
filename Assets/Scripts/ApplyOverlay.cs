using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyOverlay : MonoBehaviour
{
    [SerializeField] string overlayName;
    private OverlayObject overlayObject;

    private void Awake()
    {
        overlayObject = GetComponent<OverlayObject>();
    }
    void Update()
    {
        if (overlayObject == null) return;

        if (Input.GetButtonDown("Jump"))
        {
            int id = Shader.PropertyToID(overlayName);

            if (overlayObject.HasEffect(id)) overlayObject.RemoveEffect(id); 
            else overlayObject.AddEffect(id);
        }
    }
}

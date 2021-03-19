using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "New Overlay Effect", menuName = "Create new overlay effect")]
public class OverlaySettings : ScriptableObject
{
    [Tooltip("Identifier for this overlay effect.")] public string overlayId;
    [Tooltip("Material that will be applied as overlay.")] public Material material;
    [Tooltip("Material's shader pass. Set to -1 to apply all passes.")] public int materialPassIndex = -1;
    [Tooltip("Layer ID for the effect.")] public int Layer = -1;
    
    [HideInInspector] [SerializeField] private int overlayIdHash;
    public int OverlayId { get { return overlayIdHash; } }

    private void OnValidate()
    {
        overlayIdHash = Shader.PropertyToID(overlayId);
    }
}

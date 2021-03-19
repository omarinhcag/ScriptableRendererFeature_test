using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class OverlayDrawer : MonoBehaviour
{
    #region Static referencing
    private static Dictionary<Camera, OverlayDrawer> _instances = new Dictionary<Camera, OverlayDrawer>();
    
    public static OverlayDrawer FindDrawer(Camera src)
    {
        if (_instances.TryGetValue(src, out var i))
        {
            return i;
        }
        return null;
    }

    private void OnEnable()
    {
        if (!_instances.ContainsKey(_camera)) _instances.Add(_camera, this);
    }

    private void OnDisable()
    {
        if (_instances.ContainsKey(_camera)) _instances.Remove(_camera);
    }

    #endregion

    #region Instance references
    [HideInInspector] [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask effectMask = -1;
    public LayerMask EffectMask { get { return effectMask; } }

    public Camera Cam { get { return _camera; } }

    private void OnValidate()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    #endregion
}

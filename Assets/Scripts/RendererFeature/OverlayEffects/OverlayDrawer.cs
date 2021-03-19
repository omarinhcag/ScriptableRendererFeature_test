using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayDrawer : MonoBehaviour
{
    #region Static referencing
    private static List<OverlayDrawer> _instances = new List<OverlayDrawer>();
    
    public static OverlayDrawer FindDrawer(Camera src)
    {
        foreach (var i in _instances)
        {
            if (i._camera == src) return i;
        }
        return null;
    }

    private void OnEnable()
    {
        if (!_instances.Contains(this)) _instances.Add(this);
    }

    private void OnDisable()
    {
        if (_instances.Contains(this)) _instances.Remove(this);
    }

    #endregion

    #region Instance references
    [SerializeField] private Camera _camera;
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

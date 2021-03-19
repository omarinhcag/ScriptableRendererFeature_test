using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayObject : MonoBehaviour
{
    #region Static references
    private static Dictionary<int, List<OverlayObject>> FilteredObjects = new Dictionary<int, List<OverlayObject>>();

    public static List<OverlayObject> GetList(int effectHash)
    {
        if (FilteredObjects.TryGetValue(effectHash, out var list))
        {
            return list;
        }
        // Not found, add empty and return.
        var newList = new List<OverlayObject>();
        FilteredObjects.Add(effectHash, newList);
        return newList;
    }

    private void OnEnable()
    {
        // Reapply all effects
        foreach (var hash in appliedEffects)
        {
            var list = GetList(hash);
            if (!list.Contains(this)) list.Add(this);
        }
    }

    private void OnDisable()
    {
        // Remove all effects
        foreach (var hash in appliedEffects)
        {
            var list = GetList(hash);
            if (list.Contains(this)) list.Remove(this);
        }
    }
    #endregion

    #region Instance references
    [SerializeField] private List<int> appliedEffects = new List<int>();

    [SerializeField] Renderer[] rendererList;
    public Renderer[] Renderers => rendererList;
    #endregion

    #region Effect control

    public void AddEffect(string name)
    {
        int nameHash = Shader.PropertyToID(name);
        AddEffect(nameHash);
    }
    public void AddEffect(int nameHash)
    {
        // Add to applied effects cache.
        appliedEffects.Add(nameHash);
        // Add to list of objects with that effect.
        var list = GetList(nameHash);
        if (!list.Contains(this)) list.Add(this);
    }

    public bool HasEffect(string name)
    {
        return HasEffect(Shader.PropertyToID(name));
    }
    public bool HasEffect(int nameHash)
    {
        return (appliedEffects.Contains(nameHash));
    }

    public void RemoveEffect(string name)
    {
        int nameHash = Shader.PropertyToID(name);
        RemoveEffect(nameHash);
    }
    public void RemoveEffect(int nameHash)
    {
        // Remove from applied effects cache.
        appliedEffects.Remove(nameHash);
        // Remove from list of objects with that effect.
        var list = GetList(nameHash);
        if (list.Contains(this)) list.Remove(this);
    }

    #endregion

    #region Utils and helpers
    [ContextMenu("Find all renderers")]
    private void FindAllRenderers()
    {
        rendererList = GetComponentsInChildren<Renderer>();
    }

    public bool IsVisible(Plane[] planes)
    {
        //var visibleCount = 0;
        foreach (var target in Renderers)
        {
            if (target != null && GeometryUtility.TestPlanesAABB(planes, target.bounds))
                //visibleCount++;
                return true;
        }

        return false; // visibleCount > 0;
    }
    #endregion
}

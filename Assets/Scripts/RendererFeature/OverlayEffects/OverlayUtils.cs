using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverlayUtils
{
    static List<OverlayObject> filter = new List<OverlayObject>();

    public static void MaskByLayer(Camera camera, LayerMask mask, List<OverlayObject> objList)
    {
        // Clear filter.
        filter.Clear();
        // Cull mask.
        var filteredMask = mask.value & camera.cullingMask;
        foreach(var obj in objList)
        {
            // Filter objects based on mask and if active
            var go = obj.gameObject;
            if (!go.activeInHierarchy) continue;
            if (((1 << go.layer) & filteredMask) == 0) continue;
#if UNITY_EDITOR
            // Check if inside a prefab being edited.
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null && !stage.IsPartOfPrefabContents(go)) continue;
#endif
            // Add object to result if everything is okay.
            filter.Add(obj);
        }
        // Submit changes to enter list.
        objList.Clear();
        objList.AddRange(filter);
    }

    internal static bool CheckLayer(int layer, LayerMask effectMask)
    {
        return (((1 << layer) & effectMask) != 0);
    }
}

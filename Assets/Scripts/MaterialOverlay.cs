using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOverlay : MonoBehaviour
{
    private static Dictionary<Renderer, MeshFilter> meshRendererReference = new Dictionary<Renderer, MeshFilter>();

    [SerializeField] Renderer[] rendererList;

    //
    private void OnRenderObject()
    {
        foreach (var r in rendererList)
        {
            var mesh = ResolveMesh(r);
        }
    }

    private static Mesh ResolveMesh(Renderer r)
    {
        if (r is MeshRenderer)
        {
            MeshFilter filter;
            if (!meshRendererReference.TryGetValue(r, out filter))
            {
                filter = r.GetComponent<MeshFilter>();
                meshRendererReference.Add(r, filter);
            }
            return filter.mesh;
        }
        else if (r is SkinnedMeshRenderer)
        {
            SkinnedMeshRenderer sr = (SkinnedMeshRenderer)r;
            return sr.sharedMesh;
        }
        return null;
    }

    /*private void OnWillRenderObject()
    {
        Blerg(4); //
    }*/
}

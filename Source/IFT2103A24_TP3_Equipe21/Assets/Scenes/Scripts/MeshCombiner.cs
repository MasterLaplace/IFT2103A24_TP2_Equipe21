using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void Start()
    {
        CombineMeshes();
        AdjustBoxCollider();
    }

    private void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length < 2)
        {
            Debug.LogWarning("Pas assez de MeshFilters à combiner. Ajoute des maillages enfants à ce GameObject.");
            return;
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject == gameObject)
                continue;

            combine[i].mesh = meshFilter.sharedMesh;
            combine[i].transform = meshFilter.transform.localToWorldMatrix;
            i++;
        }

        MeshFilter meshFilterParent = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();

        // MeshRenderer meshRendererParent = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new();
        meshFilterParent.mesh = combinedMesh;
        combinedMesh.CombineMeshes(combine);

        // Material[] materials = new Material[meshFilters.Length];

        // for (int j = 0; j < meshFilters.Length; j++)
        // {
        //     materials[j] = meshFilters[j].GetComponent<Renderer>().material;
        // }

        // meshRendererParent.materials = materials;

        // foreach (Transform child in transform)
        // {
        //     if (child.gameObject != gameObject)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
    }

    private void AdjustBoxCollider()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

        Mesh combinedMesh = GetComponent<MeshFilter>().mesh;

        Bounds bounds = combinedMesh.bounds;

        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}

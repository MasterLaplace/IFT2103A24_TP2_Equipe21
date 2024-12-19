using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void Start()
    {
        CombineMeshes();
        AdjustBoxCollider();
    }

    public void AddChildrenMesh(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            Debug.LogWarning("Le GameObject n'a pas de MeshFilter.");
            return;
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            Debug.LogWarning("Le GameObject n'a pas de MeshRenderer.");
            return;
        }

        meshFilter.mesh = Instantiate(meshFilter.sharedMesh);
        meshRenderer.material = Instantiate(meshRenderer.sharedMaterial);

        gameObject.transform.SetParent(transform);
    }

    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length < 2)
        {
            Debug.LogWarning("Pas assez de MeshFilters à combiner. Ajoute des maillages enfants à ce GameObject.");
            return;
        }

        List<CombineInstance> combineInstances = new(meshFilters.Length);
        List<Material> materials = new(meshFilters.Length);
        List<int> materialIndices = new();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject == gameObject)
            continue;

            Material material = meshFilter.GetComponent<Renderer>().material;

            int materialIndex = materials.IndexOf(material);
            if (materialIndex == -1)
            {
                materials.Add(material);
                materialIndex = materials.Count - 1;
            }

            CombineInstance combineInstance = new()
            {
                mesh = meshFilter.sharedMesh,
                transform = meshFilter.transform.localToWorldMatrix
            };

            combineInstances.Add(combineInstance);
            materialIndices.Add(materialIndex);

            Debug.Log($"Maillage combiné: {meshFilter.name}");
        }

        // Vérifie et ajoute les composants nécessaires
        MeshFilter meshFilterParent = GetComponent<MeshFilter>();
        if (!meshFilterParent)
        {
            meshFilterParent = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererParent = GetComponent<MeshRenderer>();
        if (!meshRendererParent)
        {
            meshRendererParent = gameObject.AddComponent<MeshRenderer>();
        }

        // Applique le maillage combiné au parent
        Mesh combinedMesh = new();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), false);
        meshFilterParent.mesh = combinedMesh;

        // Applique les matériaux combinés au parent
        meshRendererParent.materials = materials.ToArray();

        // Assigner les sous-maillages aux matériaux correspondants
        combinedMesh.subMeshCount = materials.Count;
        for (int i = 0; i < materialIndices.Count; i++)
        {
            combinedMesh.SetTriangles(combinedMesh.GetTriangles(i), materialIndices[i]);
        }

        // Supprime les maillages enfants
        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject != gameObject)
            {
                Destroy(meshFilter.gameObject);
            }
        }

        Debug.Log("Maillages combinés avec succès.");
    }

    public void AdjustBoxCollider()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

        Mesh combinedMesh = GetComponent<MeshFilter>().mesh;

        Bounds bounds = combinedMesh.bounds;

        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
    }
}

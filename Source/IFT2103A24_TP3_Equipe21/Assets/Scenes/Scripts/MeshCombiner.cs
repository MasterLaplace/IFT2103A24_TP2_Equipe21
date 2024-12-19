using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        CombineMeshes();
        AdjustBoxCollider();
    }

    void CombineMeshes()
    {
        // Récupère tous les MeshFilter des enfants
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
            if (meshFilter.gameObject == gameObject) continue; // Ignore le parent lui-même
            combine[i].mesh = meshFilter.sharedMesh;
            combine[i].transform = meshFilter.transform.localToWorldMatrix;
            meshFilter.gameObject.SetActive(false); // Désactive les objets sources
            i++;
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
            _ = gameObject.AddComponent<MeshRenderer>();
        }

        // Applique le maillage combiné au parent
        Mesh combinedMesh = new();
        meshFilterParent.mesh = combinedMesh;
        combinedMesh.CombineMeshes(combine);

        // Assure que le parent est actif pour afficher le résultat
        gameObject.SetActive(true);
    }

    void AdjustBoxCollider()
    {
        // Ajoute un BoxCollider s'il n'existe pas
        BoxCollider boxCollider = GetComponent<BoxCollider>() ?? gameObject.AddComponent<BoxCollider>();

        // Récupère le maillage combiné
        Mesh combinedMesh = GetComponent<MeshFilter>().mesh;

        // Calculer les dimensions du maillage combiné
        Bounds bounds = combinedMesh.bounds;

        // Appliquer les dimensions et la position au BoxCollider
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}

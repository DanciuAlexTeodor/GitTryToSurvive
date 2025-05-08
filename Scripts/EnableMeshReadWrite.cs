using UnityEngine;

public class EnableMeshReadWrite : MonoBehaviour
{
    void Start()
    {
        // Get the MeshFilter or SkinnedMeshRenderer component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        Mesh mesh = null;

        if (meshFilter != null)
        {
            mesh = meshFilter.mesh; // For static meshes
        }
        else if (skinnedMeshRenderer != null)
        {
            mesh = skinnedMeshRenderer.sharedMesh; // For skinned meshes
        }

        if (mesh != null && !mesh.isReadable)
        {
            Debug.Log("Mesh is not readable. Duplicating mesh to enable Read/Write.");

            // Create a new readable copy of the mesh
            Mesh newMesh = Instantiate(mesh);
            newMesh.name = mesh.name + "_Readable";

            // Replace the old mesh with the new readable mesh
            if (meshFilter != null)
            {
                meshFilter.mesh = newMesh;
            }
            else if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.sharedMesh = newMesh;
            }
        }
    }
}

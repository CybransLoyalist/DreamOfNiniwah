using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshNormalDisplayer : MonoBehaviour
    {
        [SerializeField] private bool ShallDisplay;
        [SerializeField] private float NormalLength = 0.5f;

        void OnDrawGizmosSelected()
        {
            if (ShallDisplay)
            {
                var mesh = GetComponent<MeshFilter>().sharedMesh;

                if (mesh != null)
                {
                    for (var i = 0; i < mesh.vertexCount; i++)
                    {
                        //potrzeba je przetransformowac do world space
                        var meshVertex = transform.TransformPoint(mesh.vertices[i]);
                        Vector3 normal = transform.TransformDirection(mesh.normals[i]);

                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(meshVertex, meshVertex + NormalLength * normal);
                    }
                }
            }
        }
    }
}
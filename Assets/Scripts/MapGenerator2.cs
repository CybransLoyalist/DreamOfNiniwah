using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class MapGenerator2 : MonoBehaviour
    {
        [SerializeField] protected Material Material;
        [SerializeField] private Vector3 Location;
        [SerializeField] [Range(1, 255)] private int XResolution;
        [SerializeField] [Range(1, 255)] private int ZResolution;
        [SerializeField] [Range(0, 1000)] private float Scale = 1f;

        private MeshFilter _meshFilter;
        protected MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private Mesh mesh;

        void Update()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();

            var mesh = new MapGenerator(
                Location,
                XResolution,
                ZResolution,
                Scale,
                Material,
                _meshFilter,
                _meshRenderer,
                _meshCollider).GenerateMesh();
        }

    }
}
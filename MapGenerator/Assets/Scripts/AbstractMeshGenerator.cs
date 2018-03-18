using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class AbstractMeshGenerator 
    {
        protected Material Material;
        protected abstract string Name { get; }

       public List<Vector3> _vertices { get; set; }
       public List<int> _triangles;

        protected int _numberOfVertices;
        protected int _numberOfTrianglesCorners;

        protected abstract void SetMeshNumbers();

        protected abstract void SetVertices();
        protected abstract void SetTriangles();

        protected List<Vector3> _normals;
        protected List<Vector4> _tangents;
        protected List<Vector2> _uvs;
        public List<Color32> _vertexColors { get; set; }

        protected MeshFilter _meshFilter;
        protected MeshRenderer _meshRenderer;
        protected MeshCollider _meshCollider;
        private Mesh mesh;

       protected Mesh BuildMesh()
        {

            _meshRenderer.material = Material;
            
            InitMesh();
            SetMeshNumbers();
            
            CreateMesh();

            return mesh;
        }
        

        private bool ValidateMesh()
        {
            //build a string containing errors
            string errorStr = "";

            //check there are the correct number of triangles and vertices
            errorStr += _vertices.Count == _numberOfVertices ? "" : "Should be " + _numberOfVertices + " vertices, but there are " + _vertices.Count + ". ";
            errorStr += _triangles.Count == _numberOfTrianglesCorners ? "" : "Should be " + _numberOfTrianglesCorners + " triangles, but there are " + _triangles.Count + ". ";

            //Check there are the correct number of normals - there should be the same number of normals as there are vertices. If we're not manually calculating normals, there will be 0.
            //Similarly for tangents, uvs, vertexColours
            errorStr += (_normals.Count == _numberOfVertices || _normals.Count == 0) ? "" : "Should be " + _numberOfVertices + " normals, but there are " + _normals.Count + ". ";
            errorStr += (_tangents.Count == _numberOfVertices || _tangents.Count == 0) ? "" : "Should be " + _numberOfVertices + " tangents, but there are " + _tangents.Count + ". ";
            errorStr += (_uvs.Count == _numberOfVertices || _uvs.Count == 0) ? "" : "Should be " + _numberOfVertices + " uvs, but there are " + _uvs.Count + ". ";
            errorStr += (_vertexColors.Count == _numberOfVertices || _vertexColors.Count == 0) ? "" : "Should be " + _numberOfVertices + " vertexColours, but there are " + _vertexColors.Count + ". ";

            bool isValid = string.IsNullOrEmpty(errorStr);
            if (!isValid)
            {
                Debug.LogError("Not drawing mesh. " + errorStr);
            }

            return isValid;
        }

        private void InitMesh()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            //optional
            _normals = new List<Vector3>();
            _tangents = new List<Vector4>();
            _uvs = new List<Vector2>();
            _vertexColors = new List<Color32>();
        }

        private void CreateMesh()
        {
            mesh = new Mesh();

            SetVertices();
            SetTriangles();

            SetNormals();
            SetUVs();
            SetTangents();
            SetVertexColours();

            if (ValidateMesh())
            {
                UpadteMesh();
            }
        }

        public void UpadteMesh()
        {
            //This should always be done vertices first, triangles second - Unity requires this.
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_triangles, 0);

            if (_normals.Count == 0)
            {
                mesh.RecalculateNormals();
                _normals.AddRange(mesh.normals);
            }


            mesh.SetNormals(_normals);

            //                mesh.RecalculateTangents();
            mesh.SetUVs(0, _uvs);
            mesh.SetTangents(_tangents);
            mesh.SetColors(_vertexColors);

            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        protected virtual void SetNormals() { }
        protected virtual void SetTangents() { }
        protected virtual void SetUVs() { }
        protected virtual void SetVertexColours() { }
    }
}
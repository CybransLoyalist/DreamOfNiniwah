using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapFrameBuilder : AbstractMeshGenerator
    {
        protected override string Name
        {
            get { return "Map"; }
        }

        private Vector3 Location;
        private int XResolution;
        private int ZResolution;
        private float Scale = 1f;

        private int TilesCount;
        private int VerticesXCount;
        private int VerticesZCount;
        private List<int>[,] VerticesLocations;

        private const int VerticesPerTile = 5;
        private const int TrianglesPerTile = 4;
        private const int TriangleCornersInTriangle = 3;
        private readonly Color _baseColor = Color.yellow;

        public MapFrameBuilder(
            Vector3 location,
            int xResolution,
            int zResolution,
            float scale,
            Material material,
            MeshFilter meshFilter,
            MeshRenderer meshRenderer,
            MeshCollider meshCollider)
        {
            Location = location;
            XResolution = xResolution;
            ZResolution = zResolution;
            Scale = scale;
            Material = material;
            _meshRenderer = meshRenderer;
            _meshCollider = meshCollider;
            _meshFilter = meshFilter;
        }


        protected override void SetMeshNumbers()
        {
            TilesCount = XResolution * ZResolution;
            VerticesXCount = XResolution * 2 + 1;
            VerticesZCount = ZResolution * 2 + 1;

            _numberOfVertices = TilesCount * VerticesPerTile;
            _numberOfTrianglesCorners = TilesCount * TrianglesPerTile * TriangleCornersInTriangle;
            InitializeVerticesLocations();
        }

        private void InitializeVerticesLocations()
        {
            VerticesLocations = new List<int>[VerticesXCount, VerticesZCount];
            for (var z = 0; z < VerticesZCount; z++)
            {
                for (var x = 0; x < VerticesXCount; x++)
                {
                    VerticesLocations[x, z] = new List<int>();
                }
            }
        }

        protected override void SetVertices()
        {
            var counter = 0;
            for (var z = 0; z < ZResolution; z++)
            {
                for (var x = 0; x < XResolution; x++)

                {
                    var vertex00 = new Vector3(Location.x + x * Scale, Location.y, Location.z + z * Scale);
                    var vertex01 = new Vector3(Location.x + x * Scale, Location.y, Location.z + (z + 1) * Scale);
                    var vertex11 = new Vector3(Location.x + (x + 1) * Scale, Location.y, Location.z + (z + 1) * Scale);
                    var vertex10 = new Vector3(Location.x + (x + 1) * Scale, Location.y, Location.z + z * Scale);
                    var vertexMiddle = new Vector3(Location.x + (x + 0.5f) * Scale, Location.y,
                        Location.z + (z + 0.5f) * Scale);


                    VerticesLocations[x * 2, z * 2].Add(counter);
                    VerticesLocations[x * 2, z * 2 + 2].Add(counter + 1);
                    VerticesLocations[x * 2 + 2, z * 2 + 2].Add(counter + 2);
                    VerticesLocations[x * 2 + 2, z * 2].Add(counter + 3);
                    VerticesLocations[x * 2 + 1, z * 2 + 1].Add(counter + 4);
                    counter += 5;


                    _vertices.Add(vertex00);
                    _vertices.Add(vertex01);
                    _vertices.Add(vertex11);
                    _vertices.Add(vertex10);
                    _vertices.Add(vertexMiddle);
                }
            }

            if (_vertexColors.Count == 0)
            {
                for (var i = 0; i < _numberOfVertices; i++)
                {
                    _vertexColors.Add(_baseColor);
                }
            }
        }

        protected override void SetTriangles()
        {
            var trianglesCount = 0;
            for (var z = 0; z < ZResolution; z++)
            {
                for (var x = 0; x < XResolution; x++)
                {
                    _triangles.Add(trianglesCount);
                    _triangles.Add(trianglesCount + 1);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 1);
                    _triangles.Add(trianglesCount + 2);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 2);
                    _triangles.Add(trianglesCount + 3);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 0);
                    _triangles.Add(trianglesCount + 4);
                    _triangles.Add(trianglesCount + 3);
                    trianglesCount += 5;
                }
            }
        }


        protected override void SetVertexColours()
        {
        }

        public IMap Build()
        {
            BuildMesh();
            var callback = new Action(() => UpadteMesh());

            return new Map(
                XResolution,
                ZResolution,
                Scale,
                VerticesLocations,
                _vertices,
                _vertexColors,
                _numberOfVertices,
                callback);
        }
    }
}
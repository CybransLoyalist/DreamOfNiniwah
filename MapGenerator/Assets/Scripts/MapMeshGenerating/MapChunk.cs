﻿using System.Collections.Generic;
using Assets.Scripts.MapChanging;
using UnityEngine;

namespace Assets.Scripts.MapMeshGenerating
{
    public class MapChunk : AbstractMeshGenerator, IMapChunk
    {
        protected override string Name
        {
            get { return "MapChanger"; }
        }

        private Vector3 Location;
        private int XResolution;
        private int ZResolution;
        private float Scale = 1f;

        private int TilesCount;
        private int VerticesXCount;
        private int VerticesZCount;
        public List<int>[,] VerticesLocations;

        private const int VerticesPerTile = 5;
        private const int TrianglesPerTile = 4;
        private const int TriangleCornersInTriangle = 3;
        private readonly Color _baseColor = Color.yellow;
        private MapChanger _mapChanger;
        private int _i;
        private int _j;
        private readonly MapChunksAccessor _mapChunksAccessor;

        public MapChunk(
            Vector3 location,
            int xResolution,
            int zResolution,
            float scale,
            Material material,
            MeshFilter meshFilter,
            MeshRenderer meshRenderer,
            MeshCollider meshCollider,
            int i, 
            int j,
            MapChanger mapChanger,
            MapChunksAccessor mapChunksAccessor)
        {
            Location = location;
            XResolution = xResolution;
            ZResolution = zResolution;
            Scale = scale;
            Material = material;
            _meshRenderer = meshRenderer;
            _meshCollider = meshCollider;
            _meshFilter = meshFilter;
            _i = i;
            _j = j;
            XLocationAmongstChunks = _i;
            ZLocationAmongstChunks = _j;
            _mapChanger = mapChanger;
            _mapChunksAccessor = mapChunksAccessor;
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


                    var tileGlobalLocation = new Vector2Int(_i * XResolution + x, _j * ZResolution + z);

                    var mapTile = _mapChanger.Tiles[tileGlobalLocation];

                    _mapChunksAccessor.OwnTileVertices[mapTile].Add(new Vertex() { Chunk = this, Index = counter });
                    _mapChunksAccessor.OwnTileVertices[mapTile].Add(new Vertex() { Chunk = this, Index = counter + 1 });
                    _mapChunksAccessor.OwnTileVertices[mapTile].Add(new Vertex() { Chunk = this, Index = counter + 2 });
                    _mapChunksAccessor.OwnTileVertices[mapTile].Add(new Vertex() { Chunk = this, Index = counter + 3 });
                    _mapChunksAccessor.OwnTileVertices[mapTile].Add(new Vertex() { IsMiddle = true, Chunk = this, Index = counter + 4 });

                    _vertices.Add(vertex00);
                    _vertices.Add(vertex01);
                    _vertices.Add(vertex11);
                    _vertices.Add(vertex10);
                    _vertices.Add(vertexMiddle);

                   
                    //left
                    if (tileGlobalLocation.x > 0)
                    {
                        var leftTileLocation = new Vector2Int(tileGlobalLocation.x - 1, tileGlobalLocation.y);
                        var leftTile = _mapChanger.Tiles[leftTileLocation];

                        _mapChunksAccessor.OwnTileVertices[leftTile].Add(new Vertex() { Chunk = this, Index = counter });
                        _mapChunksAccessor.OwnTileVertices[leftTile].Add(new Vertex() { Chunk = this, Index = counter + 1 });
                    }

                    //top
                    if (tileGlobalLocation.y < _mapChanger.ZResolution - 1)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x , tileGlobalLocation.y + 1);
                        var topTile = _mapChanger.Tiles[toptileocation];

                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 1 });
                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 2 });
                    }

                    //right
                    if (tileGlobalLocation.x < _mapChanger.XResolution - 1)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x + 1 , tileGlobalLocation.y );
                        var topTile = _mapChanger.Tiles[toptileocation];

                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 2 });
                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 3 });
                    }

                    //bottom
                    if (tileGlobalLocation.y > 0)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x  , tileGlobalLocation.y -1 );
                        var topTile = _mapChanger.Tiles[toptileocation];

                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter  });
                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 3 });
                    }

                    //topleft
                    if (tileGlobalLocation.x > 0 && tileGlobalLocation.y < _mapChanger.ZResolution - 1)
                    {
                        var leftTileLocation = new Vector2Int(tileGlobalLocation.x - 1, tileGlobalLocation.y + 1);
                        var leftTile = _mapChanger.Tiles[leftTileLocation];
                        
                        _mapChunksAccessor.OwnTileVertices[leftTile].Add(new Vertex() { Chunk = this, Index = counter + 1 });
                    }

                    //top right
                    if (tileGlobalLocation.x < _mapChanger.XResolution - 1 && tileGlobalLocation.y < _mapChanger.ZResolution - 1)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x + 1, tileGlobalLocation.y + 1);
                        var topTile = _mapChanger.Tiles[toptileocation];
                        
                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 2 });
                    }

                    //bottom right
                    if (tileGlobalLocation.x < _mapChanger.XResolution - 1 && tileGlobalLocation.y > 0)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x + 1, tileGlobalLocation.y - 1);
                        var topTile = _mapChanger.Tiles[toptileocation];
                        
                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 3 });
                    }

                    //bottom left
                    if (tileGlobalLocation.x > 0 && tileGlobalLocation.y > 0)
                    {
                        var toptileocation = new Vector2Int(tileGlobalLocation.x - 1, tileGlobalLocation.y - 1);
                        var topTile = _mapChanger.Tiles[toptileocation];

                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter });
                    }

                    //                    if (tileGlobalLocation.y > 0)
                    //                    {
                    //                        var topTileLocation = new Vector2Int(tileGlobalLocation.x , tileGlobalLocation.y - 1);
                    //                        var topTile = _mapChanger.Tiles[topTileLocation];
                    //
                    //                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 1});
                    //                        _mapChunksAccessor.OwnTileVertices[topTile].Add(new Vertex() { Chunk = this, Index = counter + 2});
                    //                    }


                    counter += 5;
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

        public void Build()
        {
            BuildMesh();
        }

        public List<Color32> VerticesColors {get { return _vertexColors; }}
        public List<Vector3> Vertices { get { return _vertices; } }

        public void CommitChanges()
        {
            SetYPositionOfMiddleVertices();
            UpadteMesh();
        }

        public int XLocationAmongstChunks { get; set; }
        public int ZLocationAmongstChunks { get; set; }

        public void SetYPositionOfMiddleVertices()
        {
            for (var i = 0; i < _numberOfVertices; i += 5)
            {
                var middleVertice = i + 4;

                var totalHeight = 0f;
                for (var j = i; j < i + 4; j++)
                {
                    totalHeight += _vertices[j].y;
                }

                _vertices[middleVertice] = new Vector3(_vertices[middleVertice].x, totalHeight / 4f,
                    _vertices[middleVertice].z);
            }
        }
    }
}
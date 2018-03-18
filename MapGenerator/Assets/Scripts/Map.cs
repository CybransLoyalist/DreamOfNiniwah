using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IMapChunk
    {
        List<Color32> _vertexColors { get; set; }
        List<Vector3> _vertices { get; set; }
        void CommitChanges();
        int X { get; set; }
        int Z { get; set; }
    }


    public class Vertex
    {
        public Vector3 Vector3
        {
            get { return Chunk._vertices[Index]; }
        }

        public IMapChunk Chunk { get; set; }
        public int Index { get; set; }
        public bool IsMiddle { get; set; }
    }

    public class Tile
    {
        public List<Vertex> Vertices { get; set; }
    }

    public class Map : IMap
    {
        public IMapChunk[,] _chunks;
        private readonly int _zResolution;
        private readonly int _xResolution;
        private int _chunksCountX;
        private int _chunksCountZ;
        public Dictionary<Vector2Int, Tile> Tiles;

        public Map(
            int xResolution,
            int zResolution,
            IMapChunk[,] chunks,
            int chunksCountX,
            int chunksCountZ)
        {
            _xResolution = xResolution;
            _zResolution = zResolution;
            _chunksCountX = chunksCountX;
            _chunksCountZ = chunksCountZ;
            _chunks = chunks;

            Tiles = new Dictionary<Vector2Int, Tile>();
            for (int x = 0; x < xResolution; x++)
            {
                for (int z = 0; z < _zResolution; z++)
                {
                    Tiles[new Vector2Int(x, z)] = new Tile() {Vertices = new List<Vertex>()};
                }
            }
        }

        public void CommitChanges()
        {
            for (int i = 0; i < _chunksCountX; i++)
            {
                for (int j = 0; j < _chunksCountZ; j++)
                {
                    _chunks[i, j].CommitChanges();
                }
            }
        }

        public void ColorTileExact(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                tileVertex.Chunk._vertexColors[tileVertex.Index] = color;
            }
        }

        public void RaiseTile(int x, int y, float height)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                RaiseVertex(tileVertex, height);
            }
            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Any(v => v.x == a.Vector3.x && v.z == a.Vector3.z)).ToList();
                foreach (var matchingVertex in matchingVertices)
                {
                    RaiseVertex(matchingVertex, height);
                }
            }
        }

        private void RaiseVertex(Vertex vertex, float height)
        {
            vertex.Chunk._vertices[vertex.Index] = new Vector3(vertex.Chunk._vertices[vertex.Index].x, height,
                vertex.Chunk._vertices[vertex.Index].z);
        }

        public void LowerTile(int x, int y, float height)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                LowerVertex(tileVertex, height);
            }
            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Any(v => v.x == a.Vector3.x && v.z == a.Vector3.z)).ToList();
                foreach (var matchingVertex in matchingVertices)
                {
                    LowerVertex(matchingVertex, height);
                }
            }
        }

        private void LowerVertex(Vertex vertex, float height)
        {
            vertex.Chunk._vertices[vertex.Index] = new Vector3(vertex.Chunk._vertices[vertex.Index].x, height,
                vertex.Chunk._vertices[vertex.Index].z);
        }


        public void ColorTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                tileVertex.Chunk._vertexColors[tileVertex.Index] = color;
            }

            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Contains(a.Vector3));
                foreach (var matchingVertex in matchingVertices)
                {
                    matchingVertex.Chunk._vertexColors[matchingVertex.Index] = color;
                }
                if (neighbour.x == x || neighbour.y == y)
                {
                    ColorMiddleVertexOfTile(neighbour.x, neighbour.y, color);
                }
            }
        }


        public void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
        {
            if (peakHeigh <= 0)
            {
                Debug.LogWarning("Building mountain with 0 or lower peak height");
                return;
            }
            var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth, _xResolution, _zResolution);
            foreach (var tileHeight in tileHeights)
            {
                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
            }
        }

        public void BuildHollow(int x, int y, int bottomDepth, int ringWidth)
        {
            if (bottomDepth >= 0)
            {
                Debug.LogWarning("Building hollow with 0 or higher depth");
                return;
            }
            var tileHeights = MountainBuilder.BuildHollow(x, y, bottomDepth, ringWidth, _xResolution, _zResolution);
            foreach (var tileHeight in tileHeights)
            {
                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
            }
        }

        public void ColorMiddleVertexOfTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];

            var middleVertes = tile.Vertices.First(a => a.IsMiddle);
            middleVertes.Chunk._vertexColors[middleVertes.Index] = color;
        }

        private List<Vector2Int> GetNeighbours(int x, int y)
        {
            var result = new List<Vector2Int>();
            if (x - 1 >= 0)
            {
                result.Add(new Vector2Int(x - 1, y));
            }
            if (x + 1 < _xResolution)
            {
                result.Add(new Vector2Int(x + 1, y));
            }
            if (y - 1 >= 0)
            {
                result.Add(new Vector2Int(x, y - 1));
            }
            if (y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x, y + 1));
            }

            if (x - 1 >= 0 && y - 1 >= 0)
            {
                result.Add(new Vector2Int(x - 1, y - 1));
            }
            if (x + 1 < _xResolution && y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x + 1, y + 1));
            }
            if (x + 1 < _xResolution && y - 1 >= 0)
            {
                result.Add(new Vector2Int(x + 1, y - 1));
            }
            if (x - 1 >= 0 && y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x - 1, y + 1));
            }
            return result;
        }

        public List<IMapChunk> GetChunksOfTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        private bool IsValidPointOnMap(int x, int y)
        {
            return MapOperationValidator.IsValidPointOnMap(x, y, _xResolution, _zResolution);
        }
    }
}
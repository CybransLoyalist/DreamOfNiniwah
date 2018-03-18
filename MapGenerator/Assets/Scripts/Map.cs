using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapChunksAccessor
    {
        public Dictionary<Tile, List<Vertex>> TileVertices;
        public Dictionary<Tile, List<Vertex>> TileVertices2;

        public MapChunksAccessor()
        {
            TileVertices = new Dictionary<Tile, List<Vertex>>();
            TileVertices2 = new Dictionary<Tile, List<Vertex>>();
        }

        public void Recalculate()
        {
            foreach (var tileVertex in TileVertices)
            {
                TileVertices2[tileVertex.Key] = tileVertex.Value.ToList();
            }

            foreach (var tileVertex1 in TileVertices2)
            {
                var tile = tileVertex1.Key;
                var neighbours = TileVertices.Where(a =>
                a.Key.X == tile.X - 1 && a.Key.Z == tile.Z ||
                a.Key.X == tile.X + 1 && a.Key.Z == tile.Z ||
                a.Key.X == tile.X  && a.Key.Z == tile.Z + 1 ||
                a.Key.X == tile.X && a.Key.Z == tile.Z - 1 ||

                a.Key.X == tile.X - 1 && a.Key.Z == tile.Z - 1 ||
                a.Key.X == tile.X - 1 && a.Key.Z == tile.Z + 1 ||
                a.Key.X == tile.X + 1 && a.Key.Z == tile.Z - 1 ||
                a.Key.X == tile.X + 1 && a.Key.Z == tile.Z + 1 );

                foreach (var keyValuePair in neighbours)
                {
                    foreach (var vertex in keyValuePair.Value)
                    {
                        if (tileVertex1.Value.Any(a => a.Vector3 == vertex.Vector3))
                        {
                            if (!tileVertex1.Value.Contains(vertex))
                            {
                                tileVertex1.Value.Add(vertex);
                            }
                        }
                    }
                }

            }

        }
    }
    public class Map : IMap
    {
        public IMapChunk[,] _chunks;
        private readonly int _zResolution;
        private readonly int _xResolution;
        private int _chunksCountX;
        private int _chunksCountZ;
        public Dictionary<Vector2Int, Tile> Tiles;
        public MapChunksAccessor MapChunksAccessor;

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
            MapChunksAccessor = new MapChunksAccessor();

            Tiles = new Dictionary<Vector2Int, Tile>();
            for (int x = 0; x < xResolution; x++)
            {
                for (int z = 0; z < _zResolution; z++)
                {
                    var tile = new Tile() {X = x, Z = z};
                    Tiles[new Vector2Int(x, z)] = tile;
                    MapChunksAccessor.TileVertices[tile] = new List<Vertex>();
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
            foreach (var tileVertex in MapChunksAccessor.TileVertices[tile])
            {
                tileVertex.Chunk.VerticesColors[tileVertex.Index] = color;
            }
        }

        public void RaiseTile(int x, int y, float height)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in MapChunksAccessor.TileVertices2[tile])
            {
                RaiseVertex(tileVertex, height);
            }

        }

        private void RaiseVertex(Vertex vertex, float height)
        {
            vertex.Chunk.Vertices[vertex.Index] = new Vector3(vertex.Chunk.Vertices[vertex.Index].x, height,
                vertex.Chunk.Vertices[vertex.Index].z);
        }

        public void LowerTile(int x, int y, float height)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in MapChunksAccessor.TileVertices2[tile])
            {
                LowerVertex(tileVertex, height);
            }
        }

        private void LowerVertex(Vertex vertex, float height)
        {
            vertex.Chunk.Vertices[vertex.Index] = new Vector3(vertex.Chunk.Vertices[vertex.Index].x, height,
                vertex.Chunk.Vertices[vertex.Index].z);
        }


        public void ColorTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in MapChunksAccessor.TileVertices2[tile])
            {
                tileVertex.Chunk.VerticesColors[tileVertex.Index] = color;
            }

            foreach (var neighbour in GetNeighbours(tile.X, tile.Z))
            {
                ColorMiddleVertexOfTile(neighbour.X, neighbour.Z, color);
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

            var middleVertes = MapChunksAccessor.TileVertices2[tile].First(a => a.IsMiddle);
            middleVertes.Chunk.VerticesColors[middleVertes.Index] = color;
        }

        private List<Tile> GetNeighbours(int x, int y)
        {
            var result = new List<Tile>();
            if (x - 1 >= 0)
            {
                result.Add(Tiles[new Vector2Int(x - 1, y)]);
            }
            if (x + 1 < _xResolution)
            {
                result.Add(Tiles[new Vector2Int(x + 1, y)]);
            }
            if (y - 1 >= 0)
            {
                result.Add(Tiles[new Vector2Int(x, y - 1)]);
            }
            if (y + 1 < _zResolution)
            {
                result.Add(Tiles[new Vector2Int(x, y + 1)]);
            }

//            if (x - 1 >= 0 && y - 1 >= 0)
//            {
//                result.Add(Tiles[new Vector2Int(x - 1, y - 1)]);
//            }
//            if (x + 1 < _xResolution && y + 1 < _zResolution)
//            {
//                result.Add(Tiles[new Vector2Int(x + 1, y + 1)]);
//            }
//            if (x + 1 < _xResolution && y - 1 >= 0)
//            {
//                result.Add(Tiles[new Vector2Int(x + 1, y - 1)]);
//            }
//            if (x - 1 >= 0 && y + 1 < _zResolution)
//            {
//                result.Add(Tiles[new Vector2Int(x - 1, y + 1)]);
//            }
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
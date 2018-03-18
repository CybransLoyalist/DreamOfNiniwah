using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Map : IMap
    {
        private readonly int _zResolution;
        private readonly int _xResolution;
        public Dictionary<Vector2Int, Tile> Tiles;
        public MapChunksAccessor MapChunksAccessor;

        public Map(
            int xResolution,
            int zResolution,
            MapChunksAccessor MapChunksAccessor)
        {
            _xResolution = xResolution;
            _zResolution = zResolution;
            this.MapChunksAccessor = MapChunksAccessor;

            Tiles = new Dictionary<Vector2Int, Tile>();
            for (int x = 0; x < xResolution; x++)
            {
                for (int z = 0; z < _zResolution; z++)
                {
                    var tile = new Tile() {X = x, Z = z};
                    Tiles[new Vector2Int(x, z)] = tile;
                    MapChunksAccessor.OwnTileVertices[tile] = new List<Vertex>();
                }
            }
        }

        public void ColorTileExact(int x, int y, Color color)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in MapChunksAccessor.OwnTileVertices[tile])
                {
                    tileVertex.SetColor(color);
                }
            }
        }

        public void RaiseTile(int x, int y, float height)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in MapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.Raise(height);
                }
            }
        }

        public void LowerTile(int x, int y, float height)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in MapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.Lower(height);
                }
            }
        }

        public void ColorTile(int x, int y, Color color)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in MapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.SetColor(color);
                }

                foreach (var neighbour in GetNeighbours(tile.X, tile.Z))
                {
                    ColorMiddleVertexOfTile(neighbour.X, neighbour.Z, color);
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

        private void ColorMiddleVertexOfTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];

            var middleVertex = MapChunksAccessor.AllTileVertices[tile].First(a => a.IsMiddle);
            middleVertex.SetColor(color);
        }

        private List<Tile> GetNeighbours(int x, int y, NeighbourMode neighbourMode = NeighbourMode.Orthogonal)
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
            if (neighbourMode == NeighbourMode.All)
            {
                if (x - 1 >= 0 && y - 1 >= 0)
                {
                    result.Add(Tiles[new Vector2Int(x - 1, y - 1)]);
                }
                if (x + 1 < _xResolution && y + 1 < _zResolution)
                {
                    result.Add(Tiles[new Vector2Int(x + 1, y + 1)]);
                }
                if (x + 1 < _xResolution && y - 1 >= 0)
                {
                    result.Add(Tiles[new Vector2Int(x + 1, y - 1)]);
                }
                if (x - 1 >= 0 && y + 1 < _zResolution)
                {
                    result.Add(Tiles[new Vector2Int(x - 1, y + 1)]);
                }
            }

            return result;
        }

        private bool IsValidPointOnMap(int x, int y)
        {
            return MapOperationValidator.IsValidPointOnMap(x, y, _xResolution, _zResolution);
        }
    }
}
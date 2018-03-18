using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapChunksAccessor
    {
        public Dictionary<Tile, List<Vertex>> OwnTileVertices;
        public Dictionary<Tile, List<Vertex>> AllTileVertices;

        public MapChunksAccessor()
        {
            OwnTileVertices = new Dictionary<Tile, List<Vertex>>();
            AllTileVertices = new Dictionary<Tile, List<Vertex>>();
        }

        public void Recalculate(IMap map)
        {
            foreach (var tileVertex in OwnTileVertices)
            {
                AllTileVertices[tileVertex.Key] = tileVertex.Value.ToList();
            }

//            foreach (var tileVertex1 in AllTileVertices)
//            {
//                var tile = tileVertex1.Key;
//                var neighbours = map.GetNeighbours(tile, NeighbourMode.All);
//
//                foreach (var neighbour in neighbours)
//                {
//                    foreach (var vertex in OwnTileVertices[neighbour])
//                    {
//                        if (tileVertex1.Value.Any(a => a.Location == vertex.Location))
//                        {
//                            if (!tileVertex1.Value.Contains(vertex))
//                            {
//                                tileVertex1.Value.Add(vertex);
//                            }
//                        }
//                    }
//                }
//
//            }

        }
    }
}
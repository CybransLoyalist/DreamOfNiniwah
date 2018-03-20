using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapChanging;
using Assets.Scripts.MapUtilities;

namespace Assets.Scripts.MapMeshGenerating
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

        public void Recalculate(IMapChanger mapChanger)
        {
            foreach (var tileVertex in OwnTileVertices)
            {
                AllTileVertices[tileVertex.Key] = tileVertex.Value.ToList();
            }

//            foreach (var tileVertex1 in AllTileVertices)
//            {
//                var tile = tileVertex1.Key;
//                var neighbours = mapChanger.GetNeighbours(tile, NeighbourMode.All);
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
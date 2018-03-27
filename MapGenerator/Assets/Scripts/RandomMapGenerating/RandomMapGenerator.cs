using Assets.Scripts.MapChanging;
using Assets.Scripts.MapUtilities;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.RandomMapGenerating
{
    public static class SideExtensions
    {
        public static Side GetOpposite(this Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return Side.Bottom;
                case Side.Right:
                    return Side.Left;
                case Side.Bottom:
                    return Side.Top;
                case Side.Left:
                    return Side.Right;
                default:
                    throw new ArgumentOutOfRangeException("side", side, null);
            }
        }
    }

    public enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public static class EnumRangdomValueGetter
    {
        public static T Get<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            return (T) values.GetValue(random.Next(values.Length));
        }
    }

    public class RandomMapGenerator
    {
        private readonly IMapChanger _mapChanger;
        private Random _random;

        public RandomMapGenerator(IMapChanger mapChanger, int? seed = null)
        {
            _mapChanger = mapChanger;
            if (seed == null)
            {
                seed = new Random().Next(Int32.MaxValue);
            }
            _random = new Random(seed.Value);
        }

        public void GenerateFor()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            //  BuildInitialSlope(peak);
//            BuildHills();
            BuildRiver();

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
        }

        private void BuildInitialSlope(Vector2Int peak)
        {
            int levelsCount = _random.Next(5, 9);
            int ringWidth = _random.Next(15, 25);
            float ringHeight = 0.1f + (float) (_random.NextDouble());
            _mapChanger.BuildMountain(peak.x, peak.y, levelsCount, ringWidth, ringHeight);
        }

        private void BuildRiver()
        {
            var river = new List<Vector2Int>();
            var sourceEdge =  EnumRangdomValueGetter.Get<Side>();
            var source = GetRandomPointOnEdge(sourceEdge);
            Vector2Int preSourceNode = GetExternalNodeOf(sourceEdge, source);

            var riverNodesCount = _random.Next(4, 6);
            var nodes = new List<Vector2Int>() { preSourceNode, source };


            var riverMouths = new Dictionary<Vector2Int, Side>();
            for (int i = 1; i < riverNodesCount + 2; i++)
            {
                AddNode(sourceEdge, i, riverNodesCount, nodes, riverMouths);
                
            }

            //            nodes.Add(new Vector2Int(20,6));
            //            nodes.Add(new Vector2Int(10,30));
            //            nodes.Add(new Vector2Int(5,5));

            switch (sourceEdge)
            {
                case Side.Top:
                    nodes = nodes.OrderByDescending(a => a.y).ToList();
                    break;
                case Side.Right:
                    nodes = nodes.OrderByDescending(a => a.x).ToList();
                    break;
                case Side.Bottom:
                    nodes = nodes.OrderBy(a => a.y).ToList();
                    break;
                case Side.Left:
                    nodes = nodes.OrderBy(a => a.x).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //            var previousNode = source;
            //            for (int i = 1; i < nodes.Count; i++)
            //            {
            //                var currentNode = nodes[i];
            //                currentNode = DrawLineBetweenNodes(river, previousNode, currentNode);
            //
            //                previousNode = currentNode;
            //            }


            var riverMouthsCount = 2;// _random.Next(1, 4);
            var riverMouthChunks = new List<Vector2Int>();


            var mainRiverMouth = riverMouths.First().Key;
            nodes.Add(mainRiverMouth);
            nodes.Add(GetExternalNodeOf(riverMouths[mainRiverMouth], mainRiverMouth));

            for (int i = 0; i < riverMouthsCount; i++)
            {
                if (i == 0)
                {
//                    var edge = sourceEdge.GetOpposite();
//                    var mouth = GetRandomPointOnEdge(edge);
//                    riverMouths[new Vector2Int(mouth.x, mouth.y)] = edge;
//                   // CreateMouthRiverChunk(nodes, mouth, riverMouthChunks, edge);
                }
                else
                {
                    Side edge = Side.Left;
                    while (edge == sourceEdge)
                    {
                        edge = EnumRangdomValueGetter.Get<Side>();
                    }

                    var mouth = GetRandomPointOnEdge(edge);
                    riverMouths[new Vector2Int(mouth.x, mouth.y)] = edge;
                   // CreateMouthRiverChunk(nodes, mouth, riverMouthChunks, edge);
                }
            }

            //  CreateMouthRiverChunkForNode(nodes, riverMouthClosestToLastNode, riverMouthChunks, riverMouths[riverMouthClosestToLastNode], nodes.Last());

            var rivewrMouthsToBeProcessed = riverMouths.Select(a => a.Key).Where(n => n != mainRiverMouth).ToList();
                        foreach (var riverMouth in rivewrMouthsToBeProcessed)
                        {
                
                            CreateMouthRiverChunk(riverNodesCount, nodes, riverMouth, riverMouthChunks, riverMouths);
                        }



            //  river = river.Where((x, i) => i % 10 == 0).ToList();

            river.AddRange(nodes);
            river = MapChanging.Smoother.Smooth(river);
            river.AddRange(riverMouthChunks);

            foreach (var vector2Int in nodes)
            {
                _mapChanger.ColorTile(vector2Int.x, vector2Int.y, Color.red);
            }

            foreach (var vector2Int in riverMouths.Select(a => a.Key))
            {
                _mapChanger.ColorTile(vector2Int.x, vector2Int.y, Color.green);
            }

            foreach (var tile in river)
            {
                _mapChanger.ColorTileExact(tile.x, tile.y, Color.blue);
            }
        }

        private void AddNode(Side sourceEdge, int i, int riverNodesCount, List<Vector2Int> nodes, Dictionary<Vector2Int, Side> riverMouths)
        {
            const int borderOffset = 15;
            const int stripDivider = 2;

            if (sourceEdge == Side.Top || sourceEdge == Side.Bottom)
            {

                int maxSideOffset = _mapChanger.XResolution / 2;
                var stripWidth = _mapChanger.ZResolution / ((riverNodesCount + 1) * stripDivider);

                var previousNode = nodes[i - 1];
                var minX = previousNode.x - maxSideOffset < borderOffset
                    ? borderOffset
                    : previousNode.x - maxSideOffset;
                var maxX = previousNode.x + maxSideOffset < _mapChanger.XResolution - borderOffset
                    ? previousNode.x + maxSideOffset
                    : _mapChanger.XResolution - borderOffset;

                var x = _random.Next(minX, maxX);

                int minY, maxY;
                if (sourceEdge == Side.Top)
                {
                    maxY = _mapChanger.ZResolution - (stripWidth * (stripDivider * i - 1));
                    minY = _mapChanger.ZResolution - (stripWidth * stripDivider * i);
                }
                else
                {
                     minY = stripWidth * (stripDivider * i - 1);
                    maxY = (stripWidth * stripDivider * i);
                }
                var y = _random.Next(minY, maxY);


                if (i == riverNodesCount + 1)
                {
                    if (sourceEdge == Side.Top)
                    {
                        riverMouths[new Vector2Int(x, 0)] = sourceEdge.GetOpposite();
                    }
                    else
                    {
                        riverMouths[new Vector2Int(x, _mapChanger.ZResolution - 1)] = sourceEdge.GetOpposite();
                    }
                }
                else
                {
                    _mainNodes.Add(new Vector2Int(x, y));
                    nodes.Add(new Vector2Int(x, y));
                }
            }
            if (sourceEdge == Side.Left || sourceEdge == Side.Right)
            {
                int maxSideOffset = _mapChanger.ZResolution / 2;
                var stripWidth = _mapChanger.XResolution / ((riverNodesCount + 1) * stripDivider);

                var previousNode = nodes[i - 1];
                var minY = previousNode.y - maxSideOffset < borderOffset
                    ? borderOffset
                    : previousNode.y - maxSideOffset;
                var maxY = previousNode.y + maxSideOffset < _mapChanger.ZResolution - borderOffset
                    ? previousNode.y + maxSideOffset
                    : _mapChanger.ZResolution - borderOffset;

                var y = _random.Next(minY, maxY);

                int minX, maxX;
                if (sourceEdge == Side.Right)
                {

                    maxX = _mapChanger.ZResolution - (stripWidth * (stripDivider * i - 1));
                    minX = _mapChanger.ZResolution - (stripWidth * (stripDivider * i));
                }
                else
                {
                    minX = (stripWidth * (stripDivider * i - 1));
                    maxX = (stripWidth * (stripDivider * i )); ;
                }
                var x = _random.Next(minX, maxX);


                if (i == riverNodesCount + 1)
                {
                    if (sourceEdge == Side.Left)
                    {
                        riverMouths[new Vector2Int(_mapChanger.XResolution - 1, y)] = sourceEdge.GetOpposite();
                    }
                    else
                    {
                        riverMouths[new Vector2Int( 0, y)] = sourceEdge.GetOpposite();
                    }
                }
                else
                {
                    _mainNodes.Add(new Vector2Int(x, y));
                    nodes.Add(new Vector2Int(x, y));
                }
            }
        }

        private void CreateMouthRiverChunk(int riverNodesCount, List<Vector2Int> nodes, Vector2Int riverMouth, List<Vector2Int> riverMouthChunks, Dictionary<Vector2Int, Side> riverMouths)
        {
            //var nodesToBeChecked = nodes.Where(n => !_mainNodes.Contains(n)).ToList();
            var closestNode = GetClosestNode(riverMouth, _mainNodes);
            CreateMouthRiverChunkForNode(nodes, riverMouth, riverMouthChunks, riverMouths[riverMouth], closestNode);
        }

        private static void CreateMouthRiverChunkForNode(List<Vector2Int> nodes, Vector2Int riverMouth, List<Vector2Int> riverMouthChunks, Side edge, Vector2Int closestNode)
        {
            if (!nodes.Contains(closestNode))
            {
                
            }
            var previousNodeOfClosestNode = nodes[nodes.IndexOf(closestNode) - 1];
            var externalNodeOf = GetExternalNodeOf(edge, riverMouth);
            var riverChunk = new List<Vector2Int> { previousNodeOfClosestNode, closestNode, riverMouth, externalNodeOf };
            var smoothedChunk = Smoother.Smooth(riverChunk);
            riverMouthChunks.AddRange(smoothedChunk);
        }

        private static Vector2Int GetExternalNodeOf(Side sourceEdge, Vector2Int source)
        {
            var preSourceNode = source;
            const int Offset = 16;
            switch (sourceEdge)
            {
                case Side.Top:
                    preSourceNode = new Vector2Int(preSourceNode.x, preSourceNode.y + Offset);
                    break;
                case Side.Right:
                    preSourceNode = new Vector2Int(preSourceNode.x + Offset, preSourceNode.y);
                    break;
                case Side.Bottom:
                    preSourceNode = new Vector2Int(preSourceNode.x, preSourceNode.y - Offset);
                    break;
                case Side.Left:
                    preSourceNode = new Vector2Int(preSourceNode.x - Offset, preSourceNode.y);
                    break;
            }

            return preSourceNode;
        }

        private Vector2Int DrawLineBetweenNodes(List<Vector2Int> river, Vector2Int previousNode, Vector2Int currentNode)
        {
            var lenght = Vector2Int.Distance(previousNode, currentNode);

            var aMultiplier = GetLinearFunction(previousNode, currentNode).x;
            if (Mathf.Abs(aMultiplier) <= 1) //szybciej rosnie X
            {
                for (float j = 0; j < lenght; j += 5)
                {
                    bool isXGrowing = previousNode.x < currentNode.x;
                    var currentX = (int)(isXGrowing ? j : -j);
                    var currentY = (int)(aMultiplier * currentX);

                    var vector2Int = new Vector2Int(previousNode.x + currentX, previousNode.y + currentY);

                    if (!river.Contains(vector2Int))
                    {
                        river.Add(vector2Int);
                    }

                    var xDist = Mathf.Abs(previousNode.x - currentNode.x);
                    if (Mathf.Abs(currentX) >= xDist)
                    {
                        break;
                    }

                }
            }
            else
            {
                for (float j = 0; j < lenght; j += 5)
                {
                    var currentY = (int)(previousNode.y < currentNode.y ? j : -j);
                    var currentX = (int)(currentY / aMultiplier);

                    var vector2Int = new Vector2Int(previousNode.x + currentX, previousNode.y + currentY);

                    if (!river.Contains(vector2Int))
                    {
                        river.Add(vector2Int);
                    }

                    var yDist = Mathf.Abs(previousNode.y - currentNode.y);
                    if (Mathf.Abs(currentY) >= yDist)
                    {
                        break;
                    }
                }
            }

            return currentNode;
        }

        private Vector2Int GetClosestNode(Vector2Int sourceNode, List<Vector2Int> nodes)
        {
            var minDist = float.MaxValue;
            var result = nodes[0];
            foreach (var node in nodes)
            {
                var currDist = Vector2Int.Distance(node, sourceNode);
                if (currDist < minDist)
                {
                    result = node;
                    minDist = currDist;
                }
            }

            return result;
        }

        private Vector2 GetLinearFunction(Vector2Int point1, Vector2Int point2)
        {
            float a = (float) (point1.y - point2.y) / (point1.x - point2.x);
            float b = point2.y - a * point2.x;
            return new Vector2(a, b);
        }

        private Vector2Int GetRandomTileAwayFromBorder(int minimalDistanceFromBorder)
        {
            var x = _random.Next(minimalDistanceFromBorder, _mapChanger.XResolution - minimalDistanceFromBorder);
            var y = _random.Next(minimalDistanceFromBorder, _mapChanger.ZResolution - minimalDistanceFromBorder);
            return new Vector2Int(x, y);
        }

        private Vector2Int GetRandomPointOnEdge()
        {
            var isOnBottomOrTop = _random.Bool();
            if (isOnBottomOrTop)
            {
                var isOnTop = _random.Bool();
                return isOnTop
                    ? new Vector2Int(_random.Next(_mapChanger.XResolution), _mapChanger.ZResolution - 1)
                    : new Vector2Int(_random.Next(_mapChanger.XResolution), 0);
            }
            var isOnLeft = _random.Bool();
            return isOnLeft
                ? new Vector2Int(0, _random.Next(_mapChanger.ZResolution))
                : new Vector2Int(_mapChanger.XResolution - 1, _random.Next(_mapChanger.ZResolution));
        }

        private Vector2Int GetRandomPointOnEdge(Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return new Vector2Int(_random.Next(_mapChanger.XResolution), _mapChanger.ZResolution - 1);
                case Side.Bottom:
                    return new Vector2Int(_random.Next(_mapChanger.XResolution), 0);
                case Side.Left:
                    return new Vector2Int(0, _random.Next(_mapChanger.ZResolution));
                case Side.Right:
                    return new Vector2Int(_mapChanger.XResolution - 1, _random.Next(_mapChanger.ZResolution));
            }

            throw new Exception("Invalid side");
        }

        private void BuildHills()
        {
            var seedX = _random.Next(0, _mapChanger.XResolution);
            var seedZ = _random.Next(0, _mapChanger.ZResolution);

//            var ring = _mapChanger.GetCircle(50, 50, 50);
//            var ring2 = _mapChanger.GetCircle(150, 150, 50);

            for (int i = 0; i < _mapChanger.XResolution; ++i)
            {
                for (int j = 0; j < _mapChanger.ZResolution; j++)
                {
                    float xVal = ((float) i / _mapChanger.XResolution);
                    float zVal = ((float) j / _mapChanger.ZResolution);


                    // var multiplier = ring.Contains(new Vector2Int(i, j)) || ring2.Contains(new Vector2Int(i, j)) ? 1.5f : 0.5f;
                    var perlin = GetFractalNoise(xVal, zVal, seedX, seedZ) +
                                 ((i + j) / (float) _mapChanger.XResolution);
                    _mapChanger.ChangeTileHeight(i, j, perlin);
                }
            }
            //_mapChanger.BuildMountain(20, 20, 5, 4, 1f);
//            _mapChanger.BuildMountain(60, 60, 5, 5, 1f);
        }

        public float GetPerlinNoise(float x, float z)
        {
            var perlinNoise = Mathf.PerlinNoise(x, z); //returns value between 0 and 1, but we prefere from -1 to 1
            return perlinNoise * 2 - 1;
        }

        private readonly int _octaves = 2; //wieksze aplitudy i ostrzejsze gorki
        private readonly float _lacunarity = 2; //im wiekszy tym wiecej malych faldek
        private readonly float _gain = 6; //im wiekszy tym kazda gorka wyzsza
        private readonly float _perlinScale = 2f; //im wieszy tym wiecej górek na jednostke powierzchni
        private List<Vector2Int> _mainNodes = new List<Vector2Int>();

        public float GetFractalNoise(float x, float z, int seedX, int seedZ)
        {
            float fractalNoise = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < _octaves; i++)
            {
                float xVal = x * frequency * _perlinScale * (_mapChanger.XResolution / 100f);
                float zVal = z * frequency * _perlinScale * (_mapChanger.XResolution / 100f);

                fractalNoise += amplitude * GetPerlinNoise(seedX + xVal, seedZ + zVal);
                frequency *= _lacunarity;
                amplitude *= _gain;
            }

            return fractalNoise;
        }
    }
}
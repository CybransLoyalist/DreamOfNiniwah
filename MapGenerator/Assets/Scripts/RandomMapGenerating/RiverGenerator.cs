using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapChanging;
using Assets.Scripts.Utilities;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.RandomMapGenerating
{
    public class RiverGenerator
    {
        private readonly Random _random;
        private readonly IMapChanger _mapChanger;
        private readonly List<Vector2Int> _mainNodes;
        private List<Vector2Int> _riverNodes;
        private Dictionary<Vector2Int, Side> _riverMouths;

        public RiverGenerator(Random random, IMapChanger mapChanger)
        {
            _random = random;
            _mapChanger = mapChanger;
            _mainNodes = new List<Vector2Int>();
        }

        public void BuildRiver()
        {
            var river = new List<Vector2Int>();
            var sourceEdge = EnumRangdomValueGetter.Get<Side>();
            var source = GetRandomPointOnEdge(sourceEdge);
            Vector2Int preSourceNode = GetExternalNodeOf(sourceEdge, source);

            var riverNodesCount = _random.Next(4, 6);
            _riverNodes = new List<Vector2Int> { preSourceNode, source };

            _riverMouths = new Dictionary<Vector2Int, Side>();
            for (int i = 1; i < riverNodesCount + 2; i++)
            {
                AddNode(sourceEdge, i, riverNodesCount);
            }

            switch (sourceEdge)
            {
                case Side.Top:
                    _riverNodes = _riverNodes.OrderByDescending(a => a.y).ToList();
                    break;
                case Side.Right:
                    _riverNodes = _riverNodes.OrderByDescending(a => a.x).ToList();
                    break;
                case Side.Bottom:
                    _riverNodes = _riverNodes.OrderBy(a => a.y).ToList();
                    break;
                case Side.Left:
                    _riverNodes = _riverNodes.OrderBy(a => a.x).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var riverMouthsCount =  _random.Next(1, 4);
            var riverMouthChunks = new List<Vector2Int>();


            var mainRiverMouth = _riverMouths.First().Key;
            _riverNodes.Add(mainRiverMouth);
            _riverNodes.Add(GetExternalNodeOf(_riverMouths[mainRiverMouth], mainRiverMouth));

            for (int i = 0; i < riverMouthsCount; i++)
            {
                if (i == 0)
                {
                }
                else
                {
                    float distanceToClosestMouth = 0f;
                    Vector2Int mouth = new Vector2Int(0,0);
                    Side edge = Side.Bottom;

                    int counter = 0;
                    while (distanceToClosestMouth < _mapChanger.XResolution / 8f)
                    {
                        while (edge == sourceEdge)
                        {
                            edge = EnumRangdomValueGetter.Get<Side>();
                        }
                        mouth = GetRandomPointOnEdge(edge);
                        var closestMouth = GetClosestNode(mouth, _riverMouths.Select(a => a.Key).ToList());
                        distanceToClosestMouth = Vector2Int.Distance(closestMouth, mouth);
                        ++counter;
                        if (counter == 25)
                        {
                            break;
                        }
                    }

                    _riverMouths[new Vector2Int(mouth.x, mouth.y)] = edge;
                }
            }

        
            var rivewrMouthsToBeProcessed = _riverMouths.Select(a => a.Key).Where(n => n != mainRiverMouth).ToList();
            foreach (var riverMouth in rivewrMouthsToBeProcessed)
            {

                CreateMouthRiverChunk( _riverNodes, riverMouth, riverMouthChunks);
            }

            
            river.AddRange(_riverNodes);
            river = Smoother.Smooth(river);
            river.AddRange(riverMouthChunks);

            foreach (var vector2Int in _riverNodes)
            {
                _mapChanger.ColorTile(vector2Int.x, vector2Int.y, Color.red);
            }

            foreach (var vector2Int in _riverMouths.Select(a => a.Key))
            {
                _mapChanger.ColorTile(vector2Int.x, vector2Int.y, Color.green);
            }

            foreach (var tile in river)
            {
                _mapChanger.ColorTileExact(tile.x, tile.y, Color.blue);
            }
        }

        private void AddNode(Side sourceEdge, int i, int _riverNodesCount)
        {
            const int borderOffset = 15;
            const int stripDivider = 2;

            if (sourceEdge == Side.Top || sourceEdge == Side.Bottom)
            {

                int maxSideOffset = _mapChanger.XResolution / 2;
                var stripWidth = _mapChanger.ZResolution / ((_riverNodesCount + 1) * stripDivider);

                var previousNode = _riverNodes[i - 1];
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


                if (i == _riverNodesCount + 1)
                {
                    if (sourceEdge == Side.Top)
                    {
                        _riverMouths[new Vector2Int(x, 0)] = sourceEdge.GetOpposite();
                    }
                    else
                    {
                        _riverMouths[new Vector2Int(x, _mapChanger.ZResolution - 1)] = sourceEdge.GetOpposite();
                    }
                }
                else
                {
                    _mainNodes.Add(new Vector2Int(x, y));
                    _riverNodes.Add(new Vector2Int(x, y));
                }
            }
            if (sourceEdge == Side.Left || sourceEdge == Side.Right)
            {
                int maxSideOffset = _mapChanger.ZResolution / 2;
                var stripWidth = _mapChanger.XResolution / ((_riverNodesCount + 1) * stripDivider);

                var previousNode = _riverNodes[i - 1];
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
                    maxX = (stripWidth * (stripDivider * i)); ;
                }
                var x = _random.Next(minX, maxX);


                if (i == _riverNodesCount + 1)
                {
                    if (sourceEdge == Side.Left)
                    {
                        _riverMouths[new Vector2Int(_mapChanger.XResolution - 1, y)] = sourceEdge.GetOpposite();
                    }
                    else
                    {
                        _riverMouths[new Vector2Int(0, y)] = sourceEdge.GetOpposite();
                    }
                }
                else
                {
                    _mainNodes.Add(new Vector2Int(x, y));
                    _riverNodes.Add(new Vector2Int(x, y));
                }
            }
        }



        private void CreateMouthRiverChunk(List<Vector2Int> nodes, Vector2Int riverMouth, List<Vector2Int> riverMouthChunks)
        {
            var closestNode = GetClosestNode(riverMouth, _mainNodes);
            CreateMouthRiverChunkForNode(nodes, riverMouth, riverMouthChunks, _riverMouths[riverMouth], closestNode);
        }

        private static void CreateMouthRiverChunkForNode(List<Vector2Int> nodes, Vector2Int riverMouth, List<Vector2Int> riverMouthChunks, Side edge, Vector2Int closestNode)
        {
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


    }
}
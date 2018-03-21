using System;
using System.Diagnostics;
using Assets.Scripts.MapChanging;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.RandomMapGenerating
{
    public class RandomMapGenerator
    {
        private Random _random;

        public RandomMapGenerator(int? seed = null)
        {
            if (seed == null)
            {
                seed = new Random().Next(Int32.MaxValue);
            }
            _random = new Random(seed.Value);
        }

        public void GenerateFor(IMapChanger mapChanger)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
//            mapChanger.BuildMountain(0, 50, 5, 20, 0.5f);

//             mapChanger.ColorRingsAround(20, 20, 5, 4);
//             mapChanger.ColorRingsAround(20, 20, 2, 2);
            mapChanger.BuildHollow(20,20, 5, 2, 1f);
            //             mapChanger.ColorRingsAround(10, 10, 2, 2);
            //            mapChanger.ColorRing(4,4, 2, 1, Color.magenta);
            //            mapChanger.ColorRing(10, 10, 1, 3, Color.cyan);
            //            mapChanger.ColorRing(10, 10, 2, 3, Color.magenta);
            //            mapChanger.ColorTileExact(4,4,  Color.red);
            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
        }
    }
}
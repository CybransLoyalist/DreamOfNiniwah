using System;
using System.Diagnostics;
using Assets.Scripts.MapChanging;

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
            mapChanger.BuildMountain(0, 50, 5, 20, 0.5f);
            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
        }
    }
}
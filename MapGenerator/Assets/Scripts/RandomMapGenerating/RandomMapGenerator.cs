using System;
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
        }
    }
}
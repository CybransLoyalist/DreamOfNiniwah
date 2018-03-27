using System;

namespace Assets.Scripts.RandomMapGenerating
{
    public static class EnumRangdomValueGetter
    {
        public static T Get<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            Random random = new Random();
            return (T) values.GetValue(random.Next(values.Length));
        }
    }
}
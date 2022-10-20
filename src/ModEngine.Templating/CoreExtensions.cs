using System;
using System.Linq;

namespace ModEngine.Templating
{
    public static class CoreExtensions
    {
        public static float NextFloat(this Random rand, float minValue, float maxValue, int decimalPlaces = 1)
        {
            var randNumber = rand.NextDouble() * (maxValue - minValue) + minValue;
            return Convert.ToSingle(randNumber.ToString("f" + decimalPlaces));
        }
        
        public static byte[] ToValueBytes(this string s, bool addTerminator = false) {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(s);
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return lengthByte.Concat(strBytes).Concat(new byte[1] {0}).ToArray();
        }
        
        public static string ToValueBytes(this string s, out int byteLength) {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(s);
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            byteLength = strBytes.Length + 1;
            return BitConverter.ToString(lengthByte.Concat(strBytes).ToArray());
        }
    }
}
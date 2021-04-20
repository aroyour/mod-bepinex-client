using Hazel;
using UnityEngine;

namespace PolusggSlim.Utils.Extensions
{
    public static class MessageExtensions
    {
        private const float Min = -50f;
        private const float Max = 50f;

        private static float ReverseLerp(float t)
        {
            return Mathf.Clamp((t - Min) / (Max - Min), 0f, 1f);
        }

        public static void WriteVector2(this MessageWriter writer, Vector2 value)
        {
            var x = (ushort) (ReverseLerp(value.x) * ushort.MaxValue);
            var y = (ushort) (ReverseLerp(value.y) * ushort.MaxValue);

            writer.Write(x);
            writer.Write(y);
        }

        public static Vector2 ReadVector2(this MessageReader reader)
        {
            var x = reader.ReadUInt16() / (float) ushort.MaxValue;
            var y = reader.ReadUInt16() / (float) ushort.MaxValue;

            return new Vector2(Mathf.Lerp(Min, Max, x), Mathf.Lerp(Min, Max, y));
        }

        public static Color32 ReadColor(this MessageReader reader)
        {
            return new(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }
    }
}
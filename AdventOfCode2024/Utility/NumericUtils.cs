using System.Numerics;

namespace AdventOfCode2024.Utility;

public static class NumericUtils
{
    public static Vector2 ToVector2(this Vector3 v3)
    {
        return new Vector2(v3.X, v3.Y);
    }
}

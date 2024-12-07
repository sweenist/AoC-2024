using System.Numerics;
using AdventOfCode2024.Enums;

namespace AdventOfCode2024.Utility;

public static class NumericUtils
{
    public static Vector2 ToVector2(this Vector3 v3)
    {
        return new Vector2(v3.X, v3.Y);
    }

    public static string DisplayDirection(this Vector3 v3)
    {
        return $"<{v3.X}, {v3.Y}, {(Direction)(int)v3.Z}>";
    }
}

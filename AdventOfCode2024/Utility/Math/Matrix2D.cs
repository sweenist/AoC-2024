namespace AdventOfCode2024.Utility.Math;

public class Matrix2D
{
    public int X1 { get; set; }
    public int X2 { get; set; }
    public int Y1 { get; set; }
    public int Y2 { get; set; }

    public int Determinant => (X1 * Y2) - (X2 * Y1);
}

public static class MatrixExtensions
{
    public static bool HasWholeCoefficient(this Matrix2D a1, Matrix2D a2)
    {
        var epsilon = 0.000001;
        var quotient = a2.Determinant / (float)a1.Determinant;

        var diff = System.Math.Abs(quotient) - System.Math.Floor(System.Math.Abs(quotient));
        return diff > epsilon;
    }
}

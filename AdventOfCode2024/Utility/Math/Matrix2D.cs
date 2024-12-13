namespace AdventOfCode2024.Utility.Math;

public class Matrix2D
{
    public Matrix2D(ICoordinate coord1, ICoordinate coord2, bool pivot = false)
    {
        X1 = coord1.X;
        X2 = pivot ? coord2.X : coord1.Y;
        Y1 = pivot ? coord1.Y : coord2.X;
        Y2 = coord2.Y;
    }

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
        return diff < epsilon;
    }

    public static int Divide(this Matrix2D a1, Matrix2D a2)
    {
        return a2.Determinant / a1.Determinant;
    }
}

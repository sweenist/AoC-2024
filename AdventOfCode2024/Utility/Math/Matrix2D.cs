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

    public Matrix2D(ICoordinate coord1, long x2, long y2, bool pivot = false)
    {
        X1 = coord1.X;
        X2 = pivot ? x2 : coord1.Y;
        Y1 = pivot ? coord1.Y : x2;
        Y2 = y2;
    }
    public Matrix2D(long x1, long y1, ICoordinate coord2, bool pivot = false)
    {
        X1 = x1;
        X2 = pivot ? coord2.X : y1;
        Y1 = pivot ? y1 : coord2.X;
        Y2 = coord2.Y;
    }

    public long X1 { get; set; }
    public long X2 { get; set; }
    public long Y1 { get; set; }
    public long Y2 { get; set; }

    public long Determinant => (X1 * Y2) - (X2 * Y1);
}

public static class MatrixExtensions
{
    public static bool HasWholeCoefficient(this Matrix2D a1, Matrix2D a2)
    {
        var quotient = a2.Determinant / (double)a1.Determinant;


        var diff = System.Math.Abs(quotient) - System.Math.Floor(System.Math.Abs(quotient));
        return diff < double.Epsilon;
    }

    public static long Divide(this Matrix2D a1, Matrix2D a2)
    {
        return a2.Determinant / a1.Determinant;
    }
}

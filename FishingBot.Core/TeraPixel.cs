using System;
using System.Collections.Generic;
using FishingBot.Core;

public class TeraPixel : ValueObject<TeraPixel>
{

    public int X { get; set; }

    public int Y { get; set; }


    public System.Drawing.Color Color { get; set; }

    public DeltaECalculation Calculation { get; set; }

    public TeraPixel(int x, int y)
    {
        if (x % 2 != 0 || y % 2 != 0)
        {
            //  throw new ArgumentException("Not a tera pixel");
        }

        this.X = x;
        this.Y = y;
        this.Color = System.Drawing.Color.Black;
    }

    public Dictionary<byte, (double G, double B)> precomputed = new Dictionary<byte, (double, double)>();

    public TeraPixel(int x, int y, string hexColor)
    {
        if (x % 2 != 0 || y % 2 != 0)
        {
            //throw new ArgumentException("Not a tera pixel");
        }

        this.X = x;
        this.Y = y;

        this.Color = System.Drawing.ColorTranslator.FromHtml(hexColor);
        this.Calculation = new DeltaECalculation(this.Color.R, this.Color.G, this.Color.B);
    }


    public double GetDistance(byte R, byte G, byte B)
    {
        if (R == 0)
            return 9999;
        //var precomputedDiff = precomputed[R];

        var diff = Math.Abs(R - Color.R) + Math.Abs(G - Color.G) + Math.Abs(B - Color.B);
        return  diff ;// +Math.Sqrt(Math.Abs(precomputedDiff.B - B) +  + Math.Abs(precomputedDiff.G - G)) ;
    }
    
    public double GetDistanceGrayscale(byte R, byte G, byte B)
    {
        return Math.Abs(ToGrayscale(R, G, B) - ToGrayscale(this.Color.R, this.Color.G, this.Color.B));
    }
    
    public int CompareSimple(byte R, byte G, byte B)
    {
        return 100 * (int)(
            1.0 - ((double)(
                Math.Abs(R - this.Color.R) +
                Math.Abs(G - this.Color.G) +
                Math.Abs(B - this.Color.B)
            ) / (256.0 * 3))
        );
    }

    public int DeltaECompare(byte R, byte G, byte B)
    {
        return Math.Abs(DeltaECalculation.DoFullCompare(R, G, B, this.Color.R, this.Color.G, this.Color.B));
    }

    static byte ToGrayscale(byte R, byte G, byte B)
    {
        return
            (byte)((B * .11) + //B
                (G * .59) +  //G
                (R * .3)); //R
    }


    protected override int GetHashCodeCore()
    {
        int hashCode = 0;
        hashCode = (hashCode * 397) ^ this.X.GetHashCode();
        hashCode = (hashCode * 397) ^ this.Y.GetHashCode();
        return hashCode;
    }

    protected override bool EqualsCore(TeraPixel valueObject)
    {
        return X == valueObject.X && Y == valueObject.Y && Color.Equals(valueObject.Color);
    }
}
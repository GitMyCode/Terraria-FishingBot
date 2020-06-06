using System;

namespace FishingBot.Core
{
    // Interesting DeltaE implementation found : https://blog.genreof.com/post/comparing-colors-using-delta-e-in-c#
    public class DeltaECalculation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double CieL { get; set; }
        public double CieA { get; set; }
        public double CieB { get; set; }

        public DeltaECalculation(int R, int G, int B)
        {
            RGBtoLAB(R, G, B);
        }

        public void RGBtoLAB(int R, int G, int B)
        {
            RGBtoXYZ(R, G, B);
            XYZtoLAB();
        }

        public void RGBtoXYZ(int RVal, int GVal, int BVal)
        {
            double R = Convert.ToDouble(RVal) / 255.0;       //R from 0 to 255
            double G = Convert.ToDouble(GVal) / 255.0;       //G from 0 to 255
            double B = Convert.ToDouble(BVal) / 255.0;       //B from 0 to 255

            if (R > 0.04045)
            {
                R = Math.Pow(((R + 0.055) / 1.055), 2.4);
            }
            else
            {
                R = R / 12.92;
            }
            if (G > 0.04045)
            {
                G = Math.Pow(((G + 0.055) / 1.055), 2.4);
            }
            else
            {
                G = G / 12.92;
            }
            if (B > 0.04045)
            {
                B = Math.Pow(((B + 0.055) / 1.055), 2.4);
            }
            else
            {
                B = B / 12.92;
            }

            R = R * 100;
            G = G * 100;
            B = B * 100;

            //Observer. = 2°, Illuminant = D65
            X = R * 0.4124 + G * 0.3576 + B * 0.1805;
            Y = R * 0.2126 + G * 0.7152 + B * 0.0722;
            Z = R * 0.0193 + G * 0.1192 + B * 0.9505;
        }

        public void XYZtoLAB()
        {
            // based upon the XYZ - CIE-L*ab formula at easyrgb.com (http://www.easyrgb.com/index.php?X=MATH&H=07#text7)
            double ref_X = 95.047;
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double var_X = X / ref_X;         // Observer= 2°, Illuminant= D65
            double var_Y = Y / ref_Y;
            double var_Z = Z / ref_Z;

            if (var_X > 0.008856)
            {
                var_X = Math.Pow(var_X , (1 / 3.0));
            }
            else
            {
                var_X = (7.787 * var_X) + (16 / 116.0);
            }
            if (var_Y > 0.008856)
            {
                var_Y = Math.Pow(var_Y, (1 / 3.0));
            }
            else
            {
                var_Y = (7.787 * var_Y) + (16 / 116.0);
            }
            if (var_Z > 0.008856)
            {
                var_Z = Math.Pow(var_Z, (1 / 3.0));
            }
            else
            {
                var_Z = (7.787 * var_Z) + (16 / 116.0);
            }

            CieL = (116 * var_Y) - 16;
            CieA = 500 * (var_X - var_Y);
            CieB = 200 * (var_Y - var_Z);
        }

        ///
        /// The smaller the number returned by this, the closer the colors are
        ///
        ///
        /// 
        public int CompareTo(DeltaECalculation oComparisionColor)
        {
            // Based upon the Delta-E (1976) formula at easyrgb.com (http://www.easyrgb.com/index.php?X=DELT&H=03#text3)
            double DeltaE = Math.Sqrt(Math.Pow((CieL - oComparisionColor.CieL), 2) + Math.Pow((CieA - oComparisionColor.CieA), 2) + Math.Pow((CieB - oComparisionColor.CieB), 2));
            return Convert.ToInt16(Math.Round(DeltaE));
        }

        public static int DoFullCompare(int R1, int G1, int B1, int R2, int G2, int B2)
        {
            DeltaECalculation oColor1 = new DeltaECalculation(R1, G1, B1);
            DeltaECalculation oColor2 = new DeltaECalculation(R2, G2, B2);
            return oColor1.CompareTo(oColor2);
        }
    }
}

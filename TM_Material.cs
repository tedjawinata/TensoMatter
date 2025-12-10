using System;

namespace TensoMatter
{
    /// <summary>
    /// Simple material data container for tensile / membrane logic.
    /// </summary>
    public class TM_Material
    {
        public string Name { get; set; }
        public double E { get; set; }             // Modulus elastisitas (MPa atau Pa, terserah konvensi)
        public double Thickness { get; set; }     // Tebal membrane (m)
        public double Poisson { get; set; }       // Poisson ratio
        public double WarpStiff { get; set; }     // Kekakuan arah warp
        public double WeftStiff { get; set; }     // Kekakuan arah weft
        public double MaxStrain { get; set; }     // Regangan maksimum yang diijinkan
        public double MinRadius { get; set; }     // Radius lengkung minimum (m)

        public override string ToString()
        {
            return $"{Name} | E={E}, t={Thickness}, ν={Poisson}, Warp={WarpStiff}, Weft={WeftStiff}, εmax={MaxStrain}, Rmin={MinRadius}";
        }
    }
}

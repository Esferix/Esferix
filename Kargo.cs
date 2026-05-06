using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cargo_tracker
{
    public class Kargo
    {
        public string TakipNo { get; set; }
        public string Gonderici { get; set; }
        public string Alici { get; set; }
        public string Adres { get; set; }

        public List<KargoHareket> Hareketler { get; set; }

        public Kargo()
        {
            Hareketler = new List<KargoHareket>();
        }
    }
}

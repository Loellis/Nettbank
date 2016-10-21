using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Transaksjon
    {
        [Key]
        public int TransaksjonsID { get; set; }
        public int Utkonto { get; set; }
        public int Innkonto { get; set; }
        public double Beløp { get; set; }
        public long KID { get; set; }
        public string Melding { get; set; }
        public string Tidspunkt { get; set; }
        public int TilhørendeKonto { get; set; }
    }
}
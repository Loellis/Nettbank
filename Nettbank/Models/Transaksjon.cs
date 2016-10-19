using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Transaksjon
    {
        public string utKonto { get; set; }
        public string innKonto { get; set; }
        public int beløp { get; set; }
        public long kid { get; set; }
        public string melding { get; set; }
        public string transaksjonsTidspunkt { get; set; }
    }
}
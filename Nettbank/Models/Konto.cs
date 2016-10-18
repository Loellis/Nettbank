using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Konto
    {
        public int kontoId { get; set; }
        public int personnummer { get; set; }
        public int saldo { get; set; }
    }
}
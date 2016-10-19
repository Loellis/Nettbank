using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Konto
    {
        public int kontoId { get; set; }
        public double saldo { get; set; }
        public List<Transaksjon> transaksjoner { get; set; }
    }
}
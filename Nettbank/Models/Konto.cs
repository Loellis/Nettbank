using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Konto
    {
        [Display(Name = "Kontonummer")]
        public int kontoId { get; set; }

        [Display(Name = "Kontonavn")]
        public string kontoNavn { get; set; }

        [Display(Name = "Saldo")]
        public string saldo { get; set; }

        [Display(Name = "Kundenummer")]
        public string kontoEier { get; set; }
    }
}
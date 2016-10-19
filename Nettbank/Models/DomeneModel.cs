using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    /*public class kunde
    {
        public int id { get; set; }
       
        public string fornavn { get; set; }
        public string etternavn { get; set; }
        public string adresse { get; set; }
        public string postnr { get; set; }
        public string poststed { get; set; }
    }*/

    public class jsKunde
    {
        public int id { get; set; }
        public string navn { get; set; }
    }

    // +Tilsvarende for kontoer og transaksjoner
}
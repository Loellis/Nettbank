using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Kunde
    {
        public int id { get; set; }
        public string personnummer { get; set; }
        public string fornavn { get; set; }
        public string etternavn { get; set; }
        public string adresse { get; set; }
        public string passord { get; set; }
        public string postnr { get; set; }
        public string poststed { get; set; }
    }
}
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
        [Display(Name = "Personnummer")]
        [Required(ErrorMessage ="Personnummer må oppgis")]
        public int personnummer { get; set; }
        [Display(Name = "Fornavn")]
        [Required(ErrorMessage ="Fornavn må oppgis")]
        public string fornavn { get; set; }
        [Display(Name = "Etternavn")]
        [Required(ErrorMessage = "Etternavn må oppgis")]
        public string etternavn { get; set; }
        [Display(Name = "Adresse")]
        [Required(ErrorMessage = "Adresse må oppgis")]
        public string adresse { get; set; }
        [Display(Name = "Passord")]
        [Required(ErrorMessage = "Passord må oppgis")]
        public string passord { get; set; }
        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Postnummer må oppgis")]
        public string postnr { get; set; }
        [Display(Name = "Poststed")]
        [Required(ErrorMessage = "Poststed må oppgis")]
        public string poststed { get; set; }
    }
}
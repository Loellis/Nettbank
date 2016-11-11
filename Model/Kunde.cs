using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Kunde
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Personnummer må oppgis")]
        [RegularExpression(@"[0-9]{11}", ErrorMessage = "Personnummeret må være 11 siffer.")]
        public string Personnummer { get; set; }

        [StringLength(50, ErrorMessage = "Maks 50 tegn i fornavn")]
        [Required(ErrorMessage = "Fornavn må oppgis")]
        public string Fornavn { get; set; }

        [StringLength(50, ErrorMessage = "Maks 50 tegn i etternavn")]
        [Required(ErrorMessage = "Etternavn må oppgis")]
        public string Etternavn { get; set; }

        [StringLength(50, ErrorMessage = "Maks 50 tegn i adressen")]
        [Required(ErrorMessage = "Adresse må oppgis")]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "Postnummer må oppgis")]
        [RegularExpression(@"[0-9]{4}", ErrorMessage = "Postnummer må være 4 siffer")]
        [Display(Name = "Postnummer")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Poststed må oppgis")]
        public string Poststed { get; set; }

        [Required(ErrorMessage = "Passord må oppgis")]
        public string Passord { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class kunde
    {
        [Key]
        public int id { get; set; } 
        [Required(ErrorMessage="Personnummer må oppgis")]
        public string Personnummer { get; set; }
        [Required(ErrorMessage = "Fornavn må oppgis")]
        public string Fornavn { get; set; }
        [Required(ErrorMessage = "Etternavn må oppgis")]
        public string Etternavn { get; set; }
        [Required(ErrorMessage = "Adresse må oppgis")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Postnr må oppgis")]
        public string Postnr { get; set; }
        public virtual PostSted Poststed { get; set; }
        [Required(ErrorMessage = "Passord må oppgis")]
        public string Passord { get; set; }
    }

    public class dbKunde
    {
        [Key]
        public int id { get; set; }
        public string Personnummer { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Adresse { get; set; }
        public string Postnr { get; set; }
        public virtual PostSted Poststed { get; set; }
        public byte[] Passord { get; set; }
        //public virtual List<int> Kontoer { get; set; }
    }

    public class PostSted
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }
    }

    public class konto
    {
        [Key]
        public int kontoId { get; set; }
        public double saldo { get; set; }
        public int kontoEier { get; set; }
        //public List<int> transaksjoner { get; set; }
    }

    public class transaksjon
    {
        [Key]
        public int transId { get; set; }
        public int utKontoId { get; set; }
        public int innKonto { get; set; }
        public double beløp { get; set; }
        public long KID { get; set; }
        public string melding { get; set; }
        public string transaksjonsTidspunkt { get; set; }
        public int tilhørendeKonto { get; set; }
    }

    public class KundeContext : DbContext
    {
        public KundeContext()
            : base("name=Bank1")
        {
            Database.CreateIfNotExists();
        }

        public DbSet<dbKunde> Kunder { get; set; }
        public DbSet<PostSted> Poststeder { get; set; }
        public DbSet<konto> Konti { get; set; }
        public DbSet<transaksjon> Transaksjoner { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
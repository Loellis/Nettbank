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
        public virtual List<Konto> Kontoer { get; set; }
    }

    public class PostSted
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }
    }

    public class Konto
    {
        [Key]
        public int kontoId { get; set; }
        public int saldo { get; set; }
        public virtual List<Transaksjon> transaksjoner { get; set; }
    }

    public class Transaksjon
    {
        [Key]
        public int transId { get; set; }
        public string utKonto { get; set; }
        public string innKonto { get; set; }
        public int beløp { get; set; }
        public DateTime transaksjonsTidspunkt { get; set; }
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
        //public DbSet<Konto> Konti { get; set; }
        //public DbSet<Transaksjon> Transaksjoner { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
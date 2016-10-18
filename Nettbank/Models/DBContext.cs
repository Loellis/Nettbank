﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Nettbank.Models
{
    public class Kunder
    {
        [Key]
        public int id { get; set; } //Kundenummer
        [Required(ErrorMessage="Personnummer må oppgis")]
        public int Personnummer { get; set; }
        [Required(ErrorMessage = "Fornavn må oppgis")]
        public string Fornavn { get; set; }
        [Required(ErrorMessage = "Etternavn må oppgis")]
        public string Etternavn { get; set; }
        [Required(ErrorMessage = "Adresse må oppgis")]
        public string Adresse { get; set; }
        [Required(ErrorMessage = "Passord må oppgis")]
        public string Passord { get; set; }
        public virtual PostSted Poststed { get; set; }
    }

    public class PostSted
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }
    }

    public class Kontoer
    {
        [Key]
        public int kontoId { get; set; }
        public virtual Kunder Personnummer { get; set; }
        public int saldo { get; set; }
    }


    public class KontoContext: DbContext
    {
        public KontoContext()
            : base("name=Kontoer")
        {
            Database.CreateIfNotExists();
        }

        public DbSet<Konto> Kontoer { get; set; }
        public DbSet<Kunde> Kunder { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }


    public class KundeContext : DbContext
    {
        public KundeContext()
            : base("name=Kunder")
        {
            Database.CreateIfNotExists();
        }

        public DbSet<Kunder> Kunder { get; set; }
        public DbSet<PostSted> Poststeder { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
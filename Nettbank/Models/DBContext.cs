using System;
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
        public int Personnummer { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Adresse { get; set; }
        public virtual PostSted Poststed { get; set; }
    }

    public class PostSted
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }
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
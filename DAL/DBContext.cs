using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using TrackerEnabledDbContext;

namespace DAL
{
    // [TrackChanges] er en tag som forteller TrackingEnabledDbContext-biblioteket
    // hvilke tabellers endringer som skal loggføres.
    // Dersom et felt i en tabell ikke skal loggføres kan taggen [SkipTracking] brukes.
    // Se forøvrig https://github.com/bilal-fazlani/tracker-enabled-dbcontext/wiki for bruk.

    [TrackChanges]
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
    }

    [TrackChanges]
    public class PostSted
    {
        [Key]
        public string Postnr { get; set; }
        public string Poststed { get; set; }

        //public virtual List<dbKunde> Kunder { get; set; }
    }

    [TrackChanges]
    public class konto
    {
        [Key]
        public long kontoID { get; set; }               //Ønsket å starte autoinkrementeringen på 1000 00 00000, men dette viste seg
        public string kontoNavn { get; set; }           //vanskligere enn forventet, med mindre man går inn databasemodulen til VS 
        public double saldo { get; set; }               //og endrer databasen etter den er opprettet. 
        public int kontoEier { get; set; }
    }

    [TrackChanges]
    public class transaksjon
    {
        [Key]
        public int transId { get; set; }
        public long utKontoId { get; set; }
        public long innKonto { get; set; }
        public double beløp { get; set; }
        public long KID { get; set; }
        public string melding { get; set; }
        public string transaksjonsTidspunkt { get; set; }
        public bool erGodkjent { get; set; }
    }

    // Bruker her TrackerContext istedenfor DbContext
    // for å benytte TrackingEnabledDbContext-biblioteket
    public class KundeContext : TrackerContext
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
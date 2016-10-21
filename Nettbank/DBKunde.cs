using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank.Models;
using System.Diagnostics;

namespace Nettbank
{
    public class DBKunde
    {
        public bool settInn(Kunde innKunde)
        {
            var nyKunde = new dbKunde()
            {
                Personnummer = innKunde.Personnummer,
                Fornavn = innKunde.Fornavn,
                Etternavn = innKunde.Etternavn,
                Adresse = innKunde.Adresse,
                Postnr = innKunde.Postnr,
                Passord = lagHash(innKunde.Passord)
            };

            var db = new KundeContext();
            try
            {
                var eksisterendePostnr = db.Poststeder.Find(innKunde.Postnr);

                if(eksisterendePostnr == null)
                {
                    var nyttPoststed = new PostSted()
                    {
                        Postnr = innKunde.Postnr,
                        Poststed = innKunde.Poststed
                    };
                    nyKunde.Poststed = nyttPoststed;
                }

                Debug.Print("ID = " + nyKunde.id + "\nPERSONNR = " + nyKunde.Personnummer + "\nFORNAVN = " + nyKunde.Fornavn + "\nETTERNAVN = " + nyKunde.Etternavn + "\nADRESSE = " + nyKunde.Adresse + "\nPOSTNR = " + nyKunde.Postnr + "\nPOSTSTED = " + nyKunde.Poststed + "\nPASSORD = " + nyKunde.Passord.GetType()  );
                db.Kunder.Add(nyKunde);
                db.SaveChanges();
                return true;
            }
            catch(Exception feil)
            {
                Debug.Print("ID = " + nyKunde.id + "\nPERSONNR = " + nyKunde.Personnummer + "\nFORNAVN = " + nyKunde.Fornavn + "\nETTERNAVN = " + nyKunde.Etternavn + "\nADRESSE = " + nyKunde.Adresse + "\nPOSTNR = " + nyKunde.Postnr + "\nPOSTSTED = " + nyKunde.Poststed + "\nPASSORD = " + nyKunde.Passord.GetType());
                return false;
            }
        }

        public List<Kunde> hentAlle()
        {
            var db = new KundeContext();
            List<Kunde> alleKunder = db.Kunder.Select(k => new Kunde()
            {
                id = k.id,
                Personnummer = k.Personnummer,
                Fornavn = k.Fornavn,
                Etternavn = k.Etternavn,
                Adresse = k.Adresse,
                Postnr = k.Postnr,
                Poststed = k.Poststed.Poststed
            }).ToList();

            return alleKunder;
        }

        public static bool Kunde_i_DB(Kunde innKunde)
        {
            //Innloggingstest for å finne om brukeren eksisterer 
            using (var db = new KundeContext())
            {
                byte[] passordDb = lagHash(innKunde.Passord);
                dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Passord == passordDb && b.Personnummer == innKunde.Personnummer);
                if (funnetKunde == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static byte[] lagHash(string innPassord)
        {
            //Lager et SHA256 hash av input passord for å sjekke mot hashet passord i DB

            byte[] innData, utData;
            var algoritme = System.Security.Cryptography.SHA256.Create();
            innData = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utData = algoritme.ComputeHash(innData);
            return utData;
        }
    }
}
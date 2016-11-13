using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using System.Diagnostics;

namespace DAL
{
    public class KundeDAL
    {

        //Lager en ny kunde
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

                //Debug.Print("ID = " + nyKunde.id + "\nPERSONNR = " + nyKunde.Personnummer + "\nFORNAVN = " + nyKunde.Fornavn + "\nETTERNAVN = " + nyKunde.Etternavn + "\nADRESSE = " + nyKunde.Adresse + "\nPOSTNR = " + nyKunde.Postnr + "\nPOSTSTED = " + nyKunde.Poststed + "\nPASSORD = " + nyKunde.Passord.GetType()  );
                db.Kunder.Add(nyKunde);
                db.SaveChanges();
                return true;
            }
            catch(Exception feil)
            {
                var loggFeil = new LoggFeilDAL();
                loggFeil.SkrivTilFil(feil);
                //Debug.Print("ID = " + nyKunde.id + "\nPERSONNR = " + nyKunde.Personnummer + "\nFORNAVN = " + nyKunde.Fornavn + "\nETTERNAVN = " + nyKunde.Etternavn + "\nADRESSE = " + nyKunde.Adresse + "\nPOSTNR = " + nyKunde.Postnr + "\nPOSTSTED = " + nyKunde.Poststed + "\nPASSORD = " + nyKunde.Passord.GetType());
                return false;
            }
        }

        // Adminversjon
        public bool settKunde(Kunde innKunde)
        {
            try
            {
                using (var db = new KundeContext())
                {
                    var nyKunde = new dbKunde();
                    nyKunde.Personnummer = innKunde.Personnummer;
                    nyKunde.Fornavn = innKunde.Fornavn;
                    nyKunde.Etternavn = innKunde.Etternavn;
                    nyKunde.Adresse = innKunde.Adresse;
                    nyKunde.Postnr = innKunde.Postnr;
                    nyKunde.Passord = lagHash(innKunde.Passord);

                    string innPostnr = innKunde.Postnr;

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null || funnetPostSted.Poststed == "")
                    {
                        var nyttPoststed = new PostSted()
                        {
                            Postnr = innPostnr,
                            Poststed = innKunde.Poststed
                        };
                        nyKunde.Poststed = nyttPoststed;
                    }
                    else
                    {
                        nyKunde.Poststed = funnetPostSted;
                    }

                    db.Kunder.Add(nyKunde);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeilDAL();
                loggFeil.SkrivTilFil(feil);
                //Debug.WriteLine(feil.ToString());
                return false;
            }
        }

        //Returnerer en liste med alle Kunde-objekter registrert i databasen
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

        public bool Kunde_i_DB(Kunde innKunde)
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

        public byte[] lagHash(string innPassord)
        {
            //Lager et SHA256 hash av input passord for å sjekke mot hashet passord i DB

            byte[] innData, utData;
            var algoritme = System.Security.Cryptography.SHA256.Create();
            innData = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utData = algoritme.ComputeHash(innData);
            return utData;
        }

        //Returnerer kunde med gitt brukerid som parameter
        public Kunde hentKunde(int id)
        {
            var db = new KundeContext();

            var enKunde = db.Kunder.Find(id);

            if (enKunde == null)
            {
                return null;
            }
            else
            {
                var utKunde = new Kunde()
                {
                    id = enKunde.id,
                    Fornavn = enKunde.Fornavn,
                    Etternavn = enKunde.Etternavn,
                    Adresse = enKunde.Adresse,
                    Personnummer = enKunde.Personnummer,
                    Postnr = enKunde.Postnr,
                    Poststed = enKunde.Poststed.Poststed
                };
                return utKunde;
            }
        }

        //Metode for å slette en kunde
        public bool slettKunde(int kID)
        {
            var db = new KundeContext();

            try
            {
                dbKunde slettKunde = db.Kunder.Find(kID);
                db.Kunder.Remove(slettKunde);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeilDAL();
                loggFeil.SkrivTilFil(feil);

                return false;
            }
        }

        //Metode for å endre en kunde
        public bool endreKunde(int id, Kunde innKunde)
        {
            var db = new KundeContext();

            try
            {
                dbKunde endreKunde = db.Kunder.Find(id);
                endreKunde.Fornavn = innKunde.Fornavn;
                endreKunde.Etternavn = innKunde.Etternavn;
                endreKunde.Adresse = innKunde.Adresse;
                endreKunde.Personnummer = innKunde.Personnummer;
                if(endreKunde.Postnr != innKunde.Postnr)
                {
                    PostSted eksisterendePoststed = db.Poststeder.FirstOrDefault(p => p.Postnr == innKunde.Postnr);
                    if (eksisterendePoststed == null)
                    {
                        var nyttPoststed = new PostSted()
                        {
                            Postnr = innKunde.Postnr,
                            Poststed = innKunde.Poststed
                        };
                        db.Poststeder.Add(nyttPoststed);
                    }
                    else
                    {
                        endreKunde.Postnr = innKunde.Postnr;
                    }
                };
                if(innKunde.Passord != null)
                {
                    endreKunde.Passord = lagHash(innKunde.Passord);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeilDAL();
                loggFeil.SkrivTilFil(feil);
                return false;
            }
        }


    }
}
using Nettbank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Nettbank.Controllers
{
    
    public class KontoController : Controller
    {
        public ActionResult OpprettKonto()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            return View();
        }

        [HttpPost]
        public ActionResult OpprettKonto(FormCollection innListe)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            try
            {
                using (var db = new KundeContext())
                {
                    var nyKonto = new konto();
                    // Konverter streng til double
                    nyKonto.saldo = Convert.ToDouble(innListe["Saldo"]);

                    int innKunde = Convert.ToInt32(innListe["Kontoeier"]);

                    nyKonto.kontoEier = innKunde;

                    var funnetKunde = db.Kunder.FirstOrDefault(p => p.id == innKunde);

                    //Lagre konto
                    db.Konti.Add(nyKonto);
                    db.SaveChanges();

                    return RedirectToAction("/ListKonti");
                }
            }
            catch (Exception feil)
            {
                return View();
            }
        }

        public ActionResult ListKonti()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var db = new KundeContext();
            // Har her en liste over alle kontoer -> få kun kontoer tilhørende KundeId!
            List<konto> alleKontoListe = db.Konti.ToList();

            // Session lagrer info som et objekt, må konvertere for å kunne sammenligne
            var kId = Convert.ToInt32(Session["KundeId"]); 
            
            //Finn riktig kundeobjekt
            dbKunde pKunde = db.Kunder.FirstOrDefault(k => k.id == kId);
            
            List<konto> kontoListe = new List<konto>();
            
            foreach (var konto in alleKontoListe)
            {
                if (konto.kontoEier == kId)
                {
                    kontoListe.Add(konto);
                }
            }

            
            // For å vise innlogget status påp ListKonti, testoutput
            if (Session["LoggetInn"] == null)
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
            }
            else
            {
                ViewBag.Innlogget = (bool)Session["LoggetInn"];
            }

            return View(kontoListe);
        }

        public string HentKonti(int kontoId)
        {
            using (var db = new KundeContext())
            {
                var konti = db.Konti.Where(s => s.kontoId == kontoId);
                string ut = "";
                foreach (var k in konti)
                {
                    ut += k.kontoId + "<br/>";
                }
                return ut;
            }
        }

        public JsonResult HentKonti1(int kontoId)
        {
            using (var db = new KundeContext())
            {
                List<konto> konti = db.Konti.Where(s => s.kontoId == kontoId).ToList();
                JsonResult ut = Json(konti, JsonRequestBehavior.AllowGet);
                return ut;
            }
        }

    }
















    public class TransaksjonController : Controller
    {
        /*
        public ActionResult RegistrerBetaling()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            return View();
        }

        [HttpPost]
        public ActionResult RegistrerBetaling(FormCollection innTrans)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            try
            {
                using (var db = new KundeContext())
                {
                    var nyTrans = new transaksjon();

                    nyTrans.utKontoId = Convert.ToInt32(innTrans["UtK"]);
                    nyTrans.innKonto = Convert.ToInt32(innTrans["InnK"]);
                    nyTrans.beløp = Convert.ToInt32(innTrans["Beløp"]);
                    
                    if(innTrans["KID_Meld"] is string )
                    {
                        nyTrans.melding = innTrans["KID_Meld"];
                    }
                    else
                    {
                        nyTrans.KID = Convert.ToUInt32(innTrans["KID_Meld"]);
                    }

                    //Setter transaksjonstidspunktet og formaterer det etter britisk standard
                    nyTrans.transaksjonsTidspunkt = DateTime.Now.ToString("en-GB");

                    db.Transaksjoner.Add(nyTrans);
                    db.SaveChanges();
                    return RedirectToAction("ListKonti");
                }
            }
            catch(Exception feil)
            {
                return View();
            }
        }
        */
        public ActionResult RegistrerBetaling()
        {
            //Sjekker om logget inn
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            using (var db = new KundeContext())
            {
                //Liste med alle kontoer
                List<konto> alleKonti = db.Konti.ToList();
                //Finner innlogget kundes ID og deretter tilhørende kontoer
                //som legges inn i ny tom liste
                var kId = Convert.ToInt32(Session["KundeId"]);
                dbKunde pKunde = db.Kunder.FirstOrDefault(k => k.id == kId);
                List<konto> konti = new List<konto>();

                foreach (var konto in alleKonti)
                {
                    if (konto.kontoEier == kId)
                    {
                        konti.Add(konto);
                    }
                }

                //Legger kundens kontoer inn i en nedtrekksmeny
                var nedtrekk = new List<string>();
                nedtrekk.Add("---Velg her---");
                foreach(var k in konti)
                {
                    if (!nedtrekk.Contains(k.kontoId.ToString()))
                    {
                        nedtrekk.Add(k.kontoId.ToString());
                    }
                }
                return View(nedtrekk);
            }
        }

        public ActionResult RegBet(Models.transaksjon ajaxTrans)
        {
            using (var db = new KundeContext())
            {
                db.Transaksjoner.Add(ajaxTrans);
                db.SaveChanges();
                string ut = "<table>";
                IEnumerable<transaksjon> transaksjoner = db.Transaksjoner;
                foreach(var t in transaksjoner)
                {
                    ut += "<tr><td>" + t.transId + "</td><td>" + t.utKontoId + "</td><td>" + t.innKonto + "</td><td>" + t.beløp + "</td><td>" + t.KID + "</td><td>" + t.melding + "</td><td>";

                    //Setter transaksjonstidspunktet og formaterer det etter britisk standard
                    t.transaksjonsTidspunkt = DateTime.Now.ToString();

                    ut += t.transaksjonsTidspunkt + "</td><td>";

                    t.tilhørendeKonto = t.utKontoId;

                    ut += t.tilhørendeKonto + "</td></tr>";
                }
                ut += "</table>";

                return RedirectToAction("/RegistrerBetaling");
            }
        }

        public ActionResult visTransaksjoner()
        {
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            using (var db = new KundeContext())
            {
                //Liste med alle kontoer
                List<konto> alleKonti = db.Konti.ToList();
                //Finner innlogget kundes ID og deretter tilhørende kontoer
                //som legges inn i ny tom liste
                var kId = Convert.ToInt32(Session["KundeId"]);
                dbKunde pKunde = db.Kunder.FirstOrDefault(k => k.id == kId);
                List<konto> konti = new List<konto>();

                foreach (var konto in alleKonti)
                {
                    if (konto.kontoEier == kId)
                    {
                        konti.Add(konto);
                    }
                }

                //Legger kundens kontoer inn i en nedtrekksmeny
                var nedtrekk = new List<string>();
                nedtrekk.Add("---Velg her---");
                foreach (var k in konti)
                {
                    if (!nedtrekk.Contains(k.kontoId.ToString()))
                    {
                        nedtrekk.Add(k.kontoId.ToString());
                    }
                }
                return View(nedtrekk);
            }
        }

        public JsonResult HentTrans(int kontoId)
        {
            using (var db = new KundeContext())
            {
                List<transaksjon> trans = db.Transaksjoner.Where(s => s.tilhørendeKonto == kontoId).ToList();
                JsonResult ut = Json(trans, JsonRequestBehavior.AllowGet);
                return ut;
            }
        }
    }


















    public class KundeController : Controller
    {

        public ActionResult Index()
        {
            if(Session["LoggetInn"] == null)
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
            }
            else
            {
                ViewBag.Innlogget = (bool)Session["LoggetInn"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(kunde innKunde)
        {

            if (Kunde_i_DB(innKunde) && (Session["KundeId"] == null))
            {
                ViewBag.KundeIdB = true;
                ViewBag.Innlogget = false;
                ViewBag.BankID = false;
                Session["BankID"] = false;

                // Lagre brukerens kundeID i session "KundeId"
                using (var db = new KundeContext())
                {
                    dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Personnummer == innKunde.Personnummer);
                    if (funnetKunde != null)
                    {
                        // Lagre KundeId i session
                        Session["KundeId"] = funnetKunde.id;
                        Session["kPID"] = innKunde.Personnummer;
                    }
                }
                // Kunde funnet, gå til BankID
                //return View(innKunde);
                return RedirectToAction("BankID");
            }
            else if ((bool)Session["BankID"] == true)
            {
                if (Kunde_Passord((string)Session["kPID"], innKunde))
                {
                    // Kunde i DB, riktig BankID, riktig passord -> logg inn.
                    // Lagre innlogget status i session "LoggetInn"
                    Session["LoggetInn"] = true;
                    ViewBag.Innlogget = true;

                    return View();
                }
                else
                {
                    ViewBag.PassordMelding = "Feil passord, vennligst prøv igjen.";

                    return View(innKunde);
                }
            }
            else if ((bool)Session["LoggetInn"] == true)
            {
                return View();
            }
            else
            {
                // Kunde ikke funnet
                ViewBag.KundeMelding = "Ingen kunde funnet med oppgitt personnummer, vennligst prøv igjen.";

                // Nullstill globale variabler for sikkerhetsskyld
                /*Session["LoggetInn"] = false;
                Session["KundeId"] = null;
                ViewBag.KundeIdB = false;
                ViewBag.Innlogget = false;
                ViewBag.BankID = false;
                ViewBag.BankIdMelding = null;
                ViewBag.PassordMelding = null;
*/
                return View();
            }
        }

        private static bool Kunde_i_DB(kunde innKunde)
        {
            //Innloggingstest for å finne om brukeren eksisterer 
            using (var db = new KundeContext())
            {
                dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Personnummer == innKunde.Personnummer);
                if(funnetKunde == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private static bool Kunde_Passord(string kPID, kunde innKunde)
        {
            //Innloggingstest for å sjekke om brukerens passord er riktig
            using (var db = new KundeContext())
            {
                byte[] passordDb = lagHash(innKunde.Passord);
                dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Personnummer == kPID && b.Passord == passordDb);
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

        private static byte[] lagHash(string innPassord)
        {
            //Lager et SHA256 hash av input passord for å sjekke mot hashet passord i DB

            byte[] innData, utData;
            var algoritme = System.Security.Cryptography.SHA256.Create();
            innData = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utData = algoritme.ComputeHash(innData);
            return utData;
        }

        public ActionResult ListKunder()
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside

            var db = new KundeContext();
            List<dbKunde> kundeListe = db.Kunder.ToList();
            return View(kundeListe);
        }

        // Metoder for oppretting av kunder/konti
        public ActionResult OpprettKunde()
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside
            return View();
        }

        [HttpPost]
        public ActionResult OpprettKunde(FormCollection innListe)
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside
            try
            {
                using (var db = new KundeContext())
                {
                    var nyKunde = new dbKunde();
                    nyKunde.Personnummer = innListe["Personnummer"];
                    nyKunde.Fornavn = innListe["Fornavn"];
                    nyKunde.Etternavn = innListe["Etternavn"];
                    nyKunde.Adresse = innListe["Adresse"];
                    nyKunde.Passord = lagHash(innListe["Passord"]);

                    string innPostnr = innListe["Postnummer"];

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if(funnetPostSted == null)
                    {
                        var nyttPoststed = new Models.PostSted();
                        nyttPoststed.Postnr = innListe["Postnummer"];
                        nyttPoststed.Poststed = innListe["Poststed"];
                        db.Poststeder.Add(nyttPoststed);
                    }
                    else
                    {
                        nyKunde.Poststed = funnetPostSted;
                    }
                    

                    db.Kunder.Add(nyKunde);
                    db.SaveChanges();
                    return RedirectToAction("ListKunder");
                }
            }
            catch (Exception feil)
            {
                return View();
            }
        }

        // Metode som lager to brukere med to kontoer hver
        public ActionResult TestOpprett()
        {
            try
            {
                //Sjekk om testkundene finnes id DB allerede
                var dbC = new KundeContext();

                var kundeSjekk1 = dbC.Kunder.FirstOrDefault(p => p.Personnummer == "99999111111");
                var kundeSjekk2 = dbC.Kunder.FirstOrDefault(p => p.Personnummer == "99999222222");
                
                if (kundeSjekk1 != null || kundeSjekk2 != null)
                {
                    return RedirectToAction("ListKunder");
                }

                //Opprett testkunder og testkonti
                //Testkunde1
                using (var db = new KundeContext())
                {
                    var nyKunde1 = new dbKunde();
                    nyKunde1.Personnummer = "99999111111";
                    nyKunde1.Fornavn = "Tester";
                    nyKunde1.Etternavn = "McTest";
                    nyKunde1.Adresse = "Testbakken 1";
                    nyKunde1.Passord = lagHash("test");

                    string innPostnr = "1234";

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null)
                    {
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innPostnr;
                        nyttPoststed.Poststed = "Testby";
                        db.Poststeder.Add(nyttPoststed);
                    }
                    else
                    {
                        nyKunde1.Poststed = funnetPostSted;
                    }
                    db.Kunder.Add(nyKunde1);
                    db.SaveChanges();
                }

                //Testkunde2
                using (var db = new KundeContext())
                {
                    var nyKunde2 = new dbKunde();
                    nyKunde2.Personnummer = "99999222222";
                    nyKunde2.Fornavn = "Fru Test";
                    nyKunde2.Etternavn = "McTest";
                    nyKunde2.Adresse = "Testbakken 1";
                    nyKunde2.Passord = lagHash("test");

                    string innPostnr = "1234";

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null)
                    {
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innPostnr;
                        nyttPoststed.Poststed = "Testby";
                        db.Poststeder.Add(nyttPoststed);
                    }
                    else
                    {
                        nyKunde2.Poststed = funnetPostSted;
                    }
                    db.Kunder.Add(nyKunde2);
                    db.SaveChanges();
                }

                //Testkonti for testkunde1
                using (var db = new KundeContext())
                {
                    var nyKonto1 = new konto();
                    
                    nyKonto1.saldo = 1066601;
                    var k1 = db.Kunder.FirstOrDefault(p => p.Personnummer == "99999111111");
                    nyKonto1.kontoEier = k1.id;
                    
                    //Lagre konto
                    db.Konti.Add(nyKonto1);
                    db.SaveChanges();

                    var nyKonto2 = new konto();

                    nyKonto2.saldo = 10201;
                    
                    nyKonto2.kontoEier = k1.id;

                    //Lagre konto
                    db.Konti.Add(nyKonto2);
                    db.SaveChanges();
                }

                //Testkonti for testkunde2
                using (var db = new KundeContext())
                {
                    var nyKonto1 = new konto();

                    nyKonto1.saldo = 123456;
                    var k1 = db.Kunder.FirstOrDefault(p => p.Personnummer == "99999222222");
                    nyKonto1.kontoEier = k1.id;

                    //Lagre konto
                    db.Konti.Add(nyKonto1);
                    db.SaveChanges();

                    var nyKonto2 = new konto();

                    nyKonto2.saldo = 54951;

                    nyKonto2.kontoEier = k1.id;

                    //Lagre konto
                    db.Konti.Add(nyKonto2);
                    db.SaveChanges();
                }

                return RedirectToAction("ListKunder");
            }
            catch (Exception feil)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult SlettDB()
        {
            var db = new KundeContext();

            System.Data.Entity.Database.Delete("Bank1");

            return RedirectToAction("Index");
        }

        public ActionResult LoggUt()
        {
            // Nullstill globale variabler
            Session["LoggetInn"] = false;
            Session["KundeId"] = null;
            Session["kPID"] = null;
            ViewBag.KundeIdB = false;
            ViewBag.Innlogget = false;
            ViewBag.BankID = false;
            Session["BankID"] = false;
            ViewBag.KundeMelding = null;
            ViewBag.BankIdMelding = null;
            ViewBag.PassordMelding = null;

            return RedirectToAction("Index");
        }

        public ActionResult BankID()
        {
            //Sjekker om logget inn
            if (Session["KundeId"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.BankID = false;
            ViewBag.KundeIdB = true;
            ViewBag.BankIdMelding = "";
            return View();
        }

        [HttpPost]
        public ActionResult BankID(string bIdInput)
        {
            //int bId = Convert.ToInt32(bIdInput);
            string bId = bIdInput;
            if (bId == "123456")
            {
                ViewBag.BankID = true;
                Session["BankID"] = true;
                ViewBag.BankIdMelding = "";
                return RedirectToAction("Index");
            }
            ViewBag.BankIdMelding = "Feil BankID, vennligst prøv igjen.";
            ViewBag.ErrorMsg = bIdInput;
            return View();
        }

    }
}
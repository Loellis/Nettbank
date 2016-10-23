using Nettbank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /*
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
        */

        [HttpPost]
        public ActionResult OpprettKonto(Konto innKonto)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var kontoDB = new DBKonto();
            bool insertOK = kontoDB.lagKonto(innKonto);

            if (insertOK)
            {
                return RedirectToAction("ListKonti");
            }
            return View();
        }

        public ActionResult ListKonti()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var kontoDB = new DBKonto();
            var kId = Convert.ToInt32(Session["KundeId"]);
            List<Konto> kontiListe = kontoDB.hentTilhørendeKonti(kId);
            return View(kontiListe);
        }

        /*
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
        }*/

            /*
        public string HentKonti(int kontoId)
        {
            using (var db = new KundeContext())
            {
                var konti = db.Konti.Where(s => s.kontoID == kontoId);
                string ut = "";
                foreach (var k in konti)
                {
                    ut += k.kontoID + "<br/>";
                }
                return ut;
            }
        }

        public JsonResult HentKonti1(int kontoId)
        {
            using (var db = new KundeContext())
            {
                List<konto> konti = db.Konti.Where(s => s.kontoID == kontoId).ToList();
                JsonResult ut = Json(konti, JsonRequestBehavior.AllowGet);
                return ut;
            }
        }
        */
    }
















    public class TransaksjonController : Controller
    {

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
        public ActionResult RegistrerBetaling(Transaksjon trans)
        {
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            //if (ModelState.IsValid)
            //{
                var transDB = new DBTransaksjoner();
                var kId = Convert.ToInt32(Session["KundeId"]);
                bool insertOK = transDB.regBetaling(trans, kId);

                if (insertOK)
                {
                    return RedirectToAction("visTransaksjoner");
                }
            //}

            return View();

        }

        /*
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

        /*
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
                if (!nedtrekk.Contains(k.kontoID.ToString()))
                {
                    nedtrekk.Add(k.kontoID.ToString());
                }
            }
            return View(nedtrekk);
        }
    }
    */

        /*
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

                    ut += t.transaksjonsTidspunkt + "</td></tr>";

                }
                ut += "</table>";

                return RedirectToAction("/RegistrerBetaling");
            }
        }
        */


        public ActionResult visTransaksjoner([DefaultValue(0)] int id)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var tDB = new DBTransaksjoner();

            if(id == 0)
            {
                List<Transaksjon> tList = tDB.hentAlleTransaksjoner();
                return View(tList);
            }

            List<Transaksjon> tListe = tDB.hentTilhørendeTransaksjon(id);
            return View(tListe);
        }

        public ActionResult Slett(int id)
        {
            var tDB = new DBTransaksjoner();
            Transaksjon trans = tDB.hentTransaksjon(id);
            return View(trans);
        }

        [HttpPost]
        public ActionResult Slett(int id, Transaksjon slettTrans)
        {
            var tDB = new DBTransaksjoner();
            bool slettOK = tDB.slettTransaksjon(id);
            if (slettOK)
            {
                return RedirectToAction("visTransaksjoner");
            }
            return View();
        }

        public ActionResult velgKonto()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var kontoDB = new DBKonto();
            var kId = Convert.ToInt32(Session["KundeId"]);
            List<Konto> kontiListe = kontoDB.hentTilhørendeKonti(kId);
            return View(kontiListe);
        }

        /*
        public JsonResult HentTransaksjoner(int kontoID)
        {
            var db = new KundeContext();

            List<transaksjon> alleTrans = db.Transaksjoner.ToList();
            List<Transaksjon> trans = new List<Transaksjon>();

            var kId = kontoID;

            foreach (var t in alleTrans)
            {
                if (t.utKontoId == kId)
                {
                    var nyT = new Transaksjon()
                    {
                        TransaksjonsID = t.transId,
                        Utkonto = t.utKontoId.ToString(),
                        Innkonto = t.innKonto.ToString(),
                        Beløp = t.beløp.ToString(),
                        KID = t.KID.ToString(),
                        Melding = t.melding,
                        Tidspunkt = t.transaksjonsTidspunkt

                    };
                    trans.Add(nyT);
                }
            }
            JsonResult ut = Json(trans, JsonRequestBehavior.AllowGet);
            return ut;
        }
        */
    }


















    public class KundeController : Controller
    {
        public ActionResult Hjem()
        {
            if (Session["LoggetInn"] == null || (bool)Session["BankID"] == false)
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
                Session["BankID"] = false;
                ViewBag.Avbrutt = true;
                Session["Avbrutt"] = true;
            }
            else
            {
                ViewBag.Avbrutt = true;
                ViewBag.Innlogget = (bool)Session["LoggetInn"];
            }
            return View();
        }

        public ActionResult Index()
        {
            if(Session["LoggetInn"] == null || (bool)Session["BankID"] == false)
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
                Session["BankID"] = false;
                ViewBag.Avbrutt = true;
                Session["Avbrutt"] = true;
            }
            else if ((bool)Session["BankID"] == true && (bool)Session["LoggetInn"] == false)
            {
                ViewBag.Innlogget = false;
                ViewBag.Avbrutt = false;
            }
            else
            {
                ViewBag.Avbrutt = false;
                ViewBag.Innlogget = (bool)Session["LoggetInn"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(Kunde innKunde)
        {

            if (Kunde_i_DB(innKunde))
            {
                ViewBag.KundeIdB = true;
                ViewBag.Innlogget = false;
                ViewBag.BankID = false;
                ViewBag.Avbrutt = false;
                Session["BankID"] = false;
                Session["Avbrutt"] = false;

                // Lagre brukerens kundeID i session "KundeId"
                using (var db = new KundeContext())
                {
                    dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Personnummer == innKunde.Personnummer);
                    if (funnetKunde != null)
                    {
                        // Lagre KundeId i session
                        Session["KundeId"] = funnetKunde.id;
                        Session["kPID"] = funnetKunde.Personnummer;
                        Session["KundeNavn"] = funnetKunde.Fornavn;
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
                    Session["Avbrutt"] = false;

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

        private static bool Kunde_i_DB(Kunde innKunde)
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

        private static bool Kunde_Passord(string kPID, Kunde innKunde)
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

        /*
        public ActionResult ListKunder()
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside

            var db = new KundeContext();
            List<dbKunde> kundeListe = db.Kunder.ToList();
            List<Kunde> kListe = new List<Kunde>();

            // Trekk ut info fra kundeListe(dbKunde) og legg inn i kListe(Kunde)
            foreach(dbKunde dbK in kundeListe)
            {
                Kunde tempKunde = new Kunde();
                tempKunde.id = dbK.id;
                tempKunde.Personnummer = dbK.Personnummer;
                tempKunde.Fornavn = dbK.Fornavn;
                tempKunde.Etternavn = dbK.Etternavn;
                tempKunde.Adresse = dbK.Adresse;
                tempKunde.Postnr = dbK.Postnr;
                if (dbK.Poststed != null)
                {
                    tempKunde.Poststed = dbK.Poststed.Poststed;
                }
                else
                {
                    tempKunde.Poststed = "ERROR";
                }
                
                kListe.Add(tempKunde);
            }
            // Returner en List<Kunde> til viewet.
            return View(kListe);
        }
        */

        public ActionResult ListKunder()
        {
            var kundeDB = new DBKunde();
            List<Kunde> alleK = kundeDB.hentAlle();
            return View(alleK);
        }

        // Metoder for oppretting av kunder
        public ActionResult OpprettKunde()
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside
            return View();
        }
        /*
        [HttpPost]
        public ActionResult OpprettKunde(Kunde innKunde)
        {
            if (ModelState.IsValid)
            {
                var kundeDB = new DBKunde();
                bool insertOK = kundeDB.settKunde(innKunde);
                if (insertOK)
                {
                    return RedirectToAction("ListKunder");
                }
            }
            return View();
        } */

        /*[HttpPost]
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
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innListe["Postnummer"];
                        nyttPoststed.Poststed = innListe["Poststed"];
                        db.Poststeder.Add(nyttPoststed);
                        //db.SaveChanges();
                        //Context.Entry<T>(entity).Reload()
                        //db.Entry<PostSted>(nyttPoststed).Reload();
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
        */

        [HttpPost]
        public ActionResult OpprettKunde(Kunde innKunde)
        {
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside
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

                //Opprett PostSted
                using (var db = new KundeContext())
                {
                    var nyttPoststed = new PostSted();
                    nyttPoststed.Postnr = "1234";
                    nyttPoststed.Poststed = "Testby";
                    db.Poststeder.Add(nyttPoststed);
                    db.SaveChanges();
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
                    nyKunde1.Postnr = "1234";
                    nyKunde1.Passord = lagHash("test");

                    string innPostnr = "1234";

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null || funnetPostSted.Poststed == "")
                    {
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innPostnr;
                        nyttPoststed.Poststed = "Testby";
                        db.Poststeder.Add(nyttPoststed);
                        db.SaveChanges();
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
                    nyKunde2.Postnr = "1234";
                    nyKunde2.Passord = lagHash("test");

                    string innPostnr = "1234";

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null || funnetPostSted.Poststed == "")
                    {
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innPostnr;
                        nyttPoststed.Poststed = "Testby";
                        db.Poststeder.Add(nyttPoststed);
                        db.SaveChanges();
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

                //Test transaksjoner for testkunde2
                using (var db = new KundeContext())
                {
                    var nyTrans1 = new transaksjon();

                    var k1 = db.Konti.FirstOrDefault(k => k.kontoID == 3);
                    nyTrans1.utKontoId = k1.kontoID;
                    nyTrans1.innKonto = 999;
                    nyTrans1.beløp = 100;
                    nyTrans1.KID = 45645464;
                    nyTrans1.transaksjonsTidspunkt = DateTime.Now.ToString();
                    nyTrans1.erGodkjent = true;

                    db.Transaksjoner.Add(nyTrans1);
                    db.SaveChanges();

                    var nyTrans2 = new transaksjon();

                    var k2 = db.Konti.FirstOrDefault(k => k.kontoID == 4);
                    nyTrans2.utKontoId = k2.kontoID;
                    nyTrans2.innKonto = 777;
                    nyTrans2.beløp = 555;
                    nyTrans2.melding = "Hei på deg";
                    nyTrans2.transaksjonsTidspunkt = DateTime.Now.ToString();
                    nyTrans2.erGodkjent = false;

                    db.Transaksjoner.Add(nyTrans2);
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
            Session["KundeNavn"] = null;
            Session["Avbrutt"] = true;

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
            ViewBag.Avbrutt = false;
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
                ViewBag.Avbrutt = false;
                Session["Avbrutt"] = false;

                return RedirectToAction("Index");
            }
            ViewBag.BankIdMelding = "Feil BankID, vennligst prøv igjen.";
            ViewBag.ErrorMsg = "Inntastet BankID-kode: " + bIdInput;
            ViewBag.Avbrutt = false;
            return View();
        }

    }
}
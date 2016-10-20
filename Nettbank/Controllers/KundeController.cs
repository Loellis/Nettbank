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
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            return View();
        }

        [HttpPost]
        public ActionResult OpprettKonto(FormCollection innListe)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
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
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var db = new KundeContext();
            // Har her en liste over alle kontoer -> få kun kontoer tilhørende kundeId!
            List<konto> alleKontoListe = db.Konti.ToList();

            // Session lagrer info som et objekt, må konvertere for å kunne sammenligne
            var kId = Convert.ToInt32(Session["kundeId"]); 
            
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
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            return View();
        }

        [HttpPost]
        public ActionResult RegistrerBetaling(FormCollection innTrans)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
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
            if ((Session["LoggetInn"] == null) || (Session["kundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            using (var db = new KundeContext())
            {
                //Liste med alle kontoer
                List<konto> alleKonti = db.Konti.ToList();
                //Finner innlogget kundes ID og deretter tilhørende kontoer
                //som legges inn i ny tom liste
                var kId = Convert.ToInt32(Session["kundeId"]);
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
            if (Kunde_i_DB(innKunde))
            {
                // Lagre innlogget status i session "LoggetInn"
                
                Session["LoggetInn"] = true;
                

                // Få lagret brukerens kundeID i session "kundeId"
                using(var db = new KundeContext())
                {
                    dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Personnummer == innKunde.Personnummer);
                    if (funnetKunde != null)
                    {
                        // Trenger kun en, men her er tre måter å lagre globalt
                        ViewData["kundeId"] = funnetKunde.id;
                        ViewBag.kId = funnetKunde.id;
                        Session["kundeId"] = funnetKunde.id;
                    }
                }

                ViewBag.Innlogget = true;
                return View();
            }
            else
            {
                Session["LoggetInn"] = false;
                ViewBag.Innlogget = false;
                return View();
            }
        }

        private static bool Kunde_i_DB(kunde innKunde)
        {
            //Innloggingstest for å finne om brukeren eksisterer 
            using (var db = new KundeContext())
            {
                byte[] passordDb = lagHash(innKunde.Passord);
                dbKunde funnetKunde = db.Kunder.FirstOrDefault(b => b.Passord == passordDb && b.Personnummer == innKunde.Personnummer);
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

    }
}
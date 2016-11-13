using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DAL;

namespace Nettbank.Controllers
{
    /*
     *  KontoController
     * 
     */
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
        public ActionResult OpprettKonto(Konto innKonto)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            //Finner kundens ID
            var id = Convert.ToInt32(Session["KundeId"]);

            //Aksesserer databasen
            var kontoDB = new KontoBLL();
            //Sjekker om konto kan lages med oppgitte verdier i viewet + kundens ID
            bool insertOK = kontoDB.lagKonto(innKonto, id);

            //Hvis opprettelsen ble godkjent sendes man så til kontooversikt
            if (insertOK)
            {
                return RedirectToAction("ListKonti");
            }
            //Hvis ikke vil man få opp igjen samme vindu (opprettKonto-viewet)
            return View();
        }

        public ActionResult ListKonti()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            //Aksesserer databasen
            var kontoDB = new KontoBLL();
            //Finner kundens ID
            var kId = Convert.ToInt32(Session["KundeId"]);
            //Henter alle kontoer som tilhører kundens ID
            List<Konto> kontiListe = kontoDB.hentTilhørendeKonti(kId);

            return View(kontiListe);
        }

        public string HentKonti(int kontoID)
        {
            using (var db = new KundeContext())
            {
                var konto = db.Konti.Where(k => k.kontoID == kontoID);
                string ut = "";
                foreach (var k in konto)
                {
                    ut += k.kontoID + "<br/>";
                }
                return ut;
            }
        }

        public JsonResult HentKonti1(int kontoID)
        {
            using (var db = new KundeContext())
            {
                List<konto> konti = db.Konti.Where(k => k.kontoID == kontoID).ToList();
                JsonResult ut = Json(konti, JsonRequestBehavior.AllowGet);
                return ut;
            }
        }
    } // end KontoController

    /*
     *  TransaksjonsController
     * 
     */
    public class TransaksjonController : Controller
    {

        public ActionResult RegistrerBetaling()
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var db = new KontoBLL();

            List<Konto> konti = db.hentTilhørendeKonti(Convert.ToInt32(Session["KundeId"]));
            Transaksjon ny = new Transaksjon();
            var nedtrekk = new List<string>();

            nedtrekk.Add("--- Velg Konto ---");
            foreach(var k in konti)
            {
                nedtrekk.Add(k.kontoId.ToString());
            }

            var tupleReturn = new Tuple<Transaksjon, List<string>>(ny, nedtrekk);
            //Returnerer en tuple til viewet slik at man kan anvende seg av Javascript og bruke transaksjonsmodellen samtidig
            return View(tupleReturn);
        }

        [HttpPost]
        public ActionResult RegistrerBetaling([Bind(Prefix = "Item1")] Transaksjon trans, string utKonto)
        {
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            
            if (trans == null)
            {
                var lF = new LoggFeilDAL();
                lF.SkrivTilFil(new Exception("trans er null, hvorfor er modellen null?"));
            }
            
            //trans.Utkonto = bruke name fra select?
            //TEST TEST TEST
            /*try
            {
                trans.Utkonto = utKonto;
            }*/
            //trans.Utkonto = utKonto;
            var nyTrans = new Transaksjon();
            try
            {
                nyTrans.Utkonto = utKonto;
                nyTrans.Innkonto = trans.Innkonto;
                nyTrans.Beløp = trans.Beløp;
                nyTrans.KID = trans.KID;
                nyTrans.Melding = trans.Melding;
                nyTrans.Tidspunkt = trans.Tidspunkt;
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);
            }

            // Sjekk om dato er gyldig
            DateTime transDato;
            bool datoFeil = false;
            if (!(trans.Tidspunkt == null || trans.Tidspunkt == ""))
            {
                // Dato er oppgitt av kunde, sjekk gyldighet av denne
                if (!DateTime.TryParse(trans.Tidspunkt, out transDato))
                {
                    // Innskrevet dato er på et ugjennkjennelig format
                    ViewBag.TidErrMsg = "Skriv dato på format: dd/mm/åååå eller la være blank";
                    datoFeil = true;
                }
                else if (DateTime.Compare(transDato.Date, DateTime.Now.Date) < 0)
                {
                    // Dato er før dagens dato
                    ViewBag.TidErrMsg = "Dato må være dagens dato eller frem i tid";
                    datoFeil = true;
                }
            }

            // Denne kodesnutten ble brukt for hvilke data som fantes i trans.
            //var loggFeil1 = new LoggFeilDAL();
            //string tmp = "KC->Innkonto: " + nyTrans.Innkonto + ". Utkonto: " + nyTrans.Utkonto + ". Beløp: " + nyTrans.Beløp + ". Melding: " + nyTrans.Melding + ".";
            //loggFeil1.SkrivTilFil(new Exception(tmp));
            
            // Lag konto-nedtrekksliste
            var db = new KontoBLL();
            List<Konto> konti = db.hentTilhørendeKonti(Convert.ToInt32(Session["KundeId"]));
            Transaksjon ny = new Transaksjon();
            var nedtrekk = new List<string>();
            nedtrekk.Add("--- Velg Konto ---");
            foreach (var k in konti)
            {
                nedtrekk.Add(k.kontoId.ToString());
            }

            // To modeller skal sendes til Viewet. Her brukes Tuple for å sende
            // Transaksjon (Item1) og List<string> (Item2) til Viewet.
            var tupleReturn = new Tuple<Transaksjon, List<string>>(ny, nedtrekk);

            if (datoFeil)
            {
                // Datofeil, returner med ViewBag.TidErrMsg og tupleReturn
                return View(tupleReturn);
            }

            // Forsøk å sette inn transaksjon i DB
            var transDB = new TransaksjonBLL();
            var kId = Convert.ToInt32(Session["KundeId"]);
            bool insertOK = transDB.regBetaling(nyTrans, kId);

            if (insertOK)
            {
                TempData["YesMsg"] = "Transaksjonen lagt til i transaksjonsregisteret.";
                ViewBag.ErrMsg = null;
                return RedirectToAction("visTransaksjoner", tupleReturn);
            }
            else
            {
                TempData["YesMsg"] = null;
                ViewBag.ErrMsg = "Transaksjonen mislyktes. Sjekk at opplysningene er korrekt eller prøv igjen senere";
                ViewBag.YesMsg = null;
            }
            ViewBag.TidErrMsg = null;

            return View(tupleReturn);
        }

        public ActionResult visTransaksjoner([DefaultValue(0)] int id)
        {
            // Send bruker til innlogging dersom ikke innlogget
            if ((Session["LoggetInn"] == null) || (Session["KundeId"] == null))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            else if (id != 0)
            {
                bool tilgang = false;
                // Sjekk at innlogget bruker har tilgang på denne siden
                try
                {
                    var kontoDB = new KontoBLL();
                    List<Konto> kontoListe = kontoDB.hentTilhørendeKonti((int)Session["KundeId"]);
                    foreach (var konto in kontoListe)
                    {
                        if (konto.kontoId == id)
                        {
                            // Innlogget bruker er kontoeier
                            tilgang = true;
                            break;
                        }
                    }
                }
                catch (Exception feil)
                {
                    var loggFeil = new LoggFeil();
                    loggFeil.SkrivTilFil(feil);
                }

                // Dersom bruker ikke har tilgang til denne kontoen/kontos transaksjoner, 
                // vis kundes transaksjoner
                if (!tilgang)
                {
                    return RedirectToAction("ListKonti", "Konto");
                }
            }

            // Hent eventuell melding  fra TempData

            ViewBag.YesMsg = TempData["YesMsg"];

            var tDB = new TransaksjonBLL();

            if(id == 0)
            {
                List<Transaksjon> alleKundesTrans = tDB.hentKundesTransaksjoner((int)Session["KundeId"]);
                return View(alleKundesTrans);
            }

            List<Transaksjon> tListe = tDB.hentTilhørendeTransaksjon(id);
            return View(tListe);
        }

        public ActionResult godkjennBetaling(int id)
        {
            var tDB = new TransaksjonBLL();
            Transaksjon trans = tDB.hentTransaksjon(id);
            return View(trans);
        }

        [HttpPost]
        public ActionResult godkjennBetaling(int id, Transaksjon godkjennTrans)
        {
            var tDB = new TransaksjonBLL();
            bool godkjent = tDB.sjekkTransaksjon(id);
            if (godkjent)
            {
                return RedirectToAction("visTransaksjoner");
            }
            return RedirectToAction("visTransaksjoner");
        }

        public ActionResult Slett(int id)
        {
            var tDB = new TransaksjonBLL();
            Transaksjon trans = tDB.hentTransaksjon(id);
            return View(trans);
        }

        [HttpPost]
        public ActionResult Slett(int id, Transaksjon slettTrans)
        {
            var tDB = new TransaksjonBLL();
            bool slettOK = tDB.slettTransaksjon(id);
            if (slettOK)
            {
                return RedirectToAction("visTransaksjoner");
            }
            return View();
        }

        public ActionResult Endre([DefaultValue(0)] int id)
        {
            if(id == 0)
            {
                return RedirectToAction("visTransaksjoner");
            }

            var tDB = new TransaksjonBLL();
            Transaksjon trans = tDB.hentTransaksjon(id);
            return View(trans);
        }

        [HttpPost]
        public ActionResult Endre(int id, Transaksjon endreTrans)
        {
            var tDB = new TransaksjonBLL();

            if (ModelState.IsValid)
            {
                bool endreOK = tDB.endreTrans(id, endreTrans);
                if (endreOK)
                {
                    return RedirectToAction("visTransaksjoner");
                }
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

            var kontoDB = new KontoBLL();
            var kId = Convert.ToInt32(Session["KundeId"]);
            List<Konto> kontiListe = kontoDB.hentTilhørendeKonti(kId);
            return View(kontiListe);
        }
    }// end TransaksjonController

    /*
     * KundeController
     * 
     */
    public class KundeController : Controller
    {
        public ActionResult Hjem()
        {
            AdminOpprett();

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
            if (Session["LoggetInn"] == null || (bool)Session["BankID"] == false)
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
        
        //Metode for å slette kunde
        public ActionResult Slett([DefaultValue(0)] int id)
        {
            if (id == 0)
            {
                return RedirectToAction("ListKunder");
            }

            var kDB = new KundeBLL();
            Kunde kunde = kDB.hentKunde(id);
            return View(kunde);
        }

        [HttpPost]
        public ActionResult Slett(int id, Transaksjon slettTrans)
        {
            var kDB = new KundeBLL();
            bool slettOK = kDB.slettKunde(id);
            if (slettOK)
            {
                return RedirectToAction("ListKunder");
            }
            return View();
        }

        //Metode for å endre/oppdatere kunde
        public ActionResult Oppdater([DefaultValue(0)] int id)
        {
            if (id == 0)
            {
                return RedirectToAction("ListKunder");
            }

            var kDB = new KundeBLL();
            Kunde kunde = kDB.hentKunde(id);
            return View(kunde);
        }

        [HttpPost]
        public ActionResult Oppdater(int id, Kunde endreKunde)
        {
            var kDB = new KundeBLL();

            if (ModelState.IsValid)
            {
                bool endreOK = kDB.endreKunde(id, endreKunde);
                if (endreOK)
                {
                    return RedirectToAction("ListKunder");
                }
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
                        Session["KundeEtternavn"] = funnetKunde.Etternavn;
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

        public ActionResult ListKunder()
        {
            if ((Session["LoggetInn"] == null) || (Convert.ToInt32(Session["KundeId"]) != 1))
            {
                return RedirectToAction("/Index", "Kunde");
            }

            var kundeDB = new KundeBLL();
            List<Kunde> alleK = kundeDB.hentAlle();
            return View(alleK);
        }

        // Metoder for oppretting av kunder
        public ActionResult OpprettKunde()
        {
            if ((Session["LoggetInn"] == null) || (Convert.ToInt32(Session["KundeId"]) != 1))
            {
                return RedirectToAction("/Index", "Kunde");
            }
            // Ingen innloggingssjekk her, foreløpig er dette en "admin"/testside
            return View();
        }

        [HttpPost]
        public ActionResult OpprettKunde(Kunde innKunde)
        {
            if ((Session["LoggetInn"] == null) || (Convert.ToInt32(Session["KundeId"]) != 1))
            {
                return RedirectToAction("/Index", "Kunde");
            }
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
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);

                return View();
            }
        }
        
        //Metode som skal automatisk opprette Admin om den ikke allerede finnes
        public ActionResult AdminOpprett()
        {
            try
            {
                var DB = new KundeContext();

                var adminSjekk = DB.Kunder.FirstOrDefault(p => p.Personnummer == "00000000000");

                if(adminSjekk != null)
                {
                    return View();
                }
                else
                {
                    SlettDB();
                    using (var db = new KundeContext())
                    {
                        var Admin = new dbKunde();
                        Admin.Personnummer = "00000000000";
                        Admin.Fornavn = "Admin";
                        Admin.Etternavn = "admin";
                        Admin.Adresse = "Local";
                        Admin.Postnr = "0000";
                        Admin.Passord = lagHash("admin");

                        string innPostnr = "0000";

                        var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                        if (funnetPostSted == null || funnetPostSted.Poststed == "")
                        {
                            var nyttPoststed = new PostSted();
                            nyttPoststed.Postnr = innPostnr;
                            nyttPoststed.Poststed = "Admin";
                            db.Poststeder.Add(nyttPoststed);
                            db.SaveChanges();
                        }
                        else
                        {
                            Admin.Poststed = funnetPostSted;
                        }
                        db.Kunder.Add(Admin);
                        db.SaveChanges();
                    }

                    return View();
                }
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);

                return RedirectToAction("Index");
            }
        } 

        /*
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

                //Opprett Admin
                using (var db = new KundeContext())
                {
                    var Admin = new dbKunde();
                    Admin.Personnummer = "00000000000";
                    Admin.Fornavn = "Admin";
                    Admin.Etternavn = "admin";
                    Admin.Adresse = "Local";
                    Admin.Postnr = "0000";
                    Admin.Passord = lagHash("admin");

                    string innPostnr = "0000";

                    var funnetPostSted = db.Poststeder.FirstOrDefault(p => p.Postnr == innPostnr);

                    if (funnetPostSted == null || funnetPostSted.Poststed == "")
                    {
                        var nyttPoststed = new PostSted();
                        nyttPoststed.Postnr = innPostnr;
                        nyttPoststed.Poststed = "Admin";
                        db.Poststeder.Add(nyttPoststed);
                        db.SaveChanges();
                    }
                    else
                    {
                        Admin.Poststed = funnetPostSted;
                    }
                    db.Kunder.Add(Admin);
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

                using (var db = new KundeContext())
                {
                    var startKonto = new konto();
                    startKonto.saldo = 99999;
                    startKonto.kontoEier = 1;
                    startKonto.kontoID = 10000000001;
                    db.Konti.Add(startKonto);
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
                    nyTrans2.innKonto = k1.kontoID;
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
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);

                return RedirectToAction("Index");
            }
        } */

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
            Session["KundeEtternavn"] = null;
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
    }// end KundeController
}// end Controller
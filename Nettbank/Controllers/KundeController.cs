using Nettbank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Nettbank.Controllers
{
    // Nappet fra MVC-ajax-a for testing

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
                Session["LoggetInn"] = true;
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
            byte[] innData, utData;
            var algoritme = System.Security.Cryptography.SHA256.Create();
            innData = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utData = algoritme.ComputeHash(innData);
            return utData;
        }

        public ActionResult ListKunder()
        {
            var db = new KundeContext();
            List<dbKunde> kundeListe = db.Kunder.ToList();
            return View(kundeListe);
        }

        public ActionResult OpprettKunde()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OpprettKunde(FormCollection innListe)
        {
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

        /*public string hentAlleNavn()
        {
            var db = new KundeDB();
            List<kunde> alleKunder = db.hentAlleKunder();
            var alleNavn = new List<jsKunde>();
            foreach (kunde k in alleKunder)
            {
                var ettNavn = new jsKunde();
                ettNavn.id = k.id;
                ettNavn.navn = k.fornavn + " " + k.etternavn;
                alleNavn.Add(ettNavn);
            }
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(alleNavn);
            return json;
        }

        public string hentKundeInfo(int id)
        {
            var db = new KundeDB();
            Kunde enKunde = db.hentEnKunde(id);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(enKunde);
            return json;
        }

        public string register(kunde innKunde)
        {
            var db = new KundeDB();
            db.lagreEnKunde(innKunde);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize("OK");
        }*/
    }
}
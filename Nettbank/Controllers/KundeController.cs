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
        public ActionResult Index(Kunder innKunde)
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

        private static bool Kunde_i_DB(Kunder innKunde)
        {
            using (var db = new KundeContext())
            {
                byte[] passordDb = lagHash(innKunde.Passord);
                dbKunder funnetKunde = db.Kunder.FirstOrDefault(b => b.Passord == passordDb && b.Personnummer == innKunde.Personnummer);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank.Models;

namespace Nettbank
{
    public class DBKonto
    {

        public bool lagKonto(Konto innKonto)
        {
            var nyKonto = new konto()
            {
                kontoNavn = innKonto.kontoNavn,
                saldo = 0,
                kontoEier = Convert.ToInt32(innKonto.kontoEier)
            };

            var db = new KundeContext();

            try
            {
                db.Konti.Add(nyKonto);
                db.SaveChanges();
                return true;
            }
            catch(Exception feil)
            {
                return false;
            }
        }
    }
}
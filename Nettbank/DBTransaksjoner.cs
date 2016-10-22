using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank.Models;
using System.Web.Mvc;

namespace Nettbank
{
    public class DBTransaksjoner
    {
        public bool regBetaling(Transaksjon trans)
        {
            var nyTrans = new transaksjon()
            {
                utKontoId = Convert.ToInt32(trans.Utkonto),
                innKonto = Convert.ToInt32(trans.Innkonto),
                beløp = Convert.ToDouble(trans.Beløp),
                KID = Convert.ToInt64(trans.KID),
                melding = trans.Melding,
                transaksjonsTidspunkt = trans.Tidspunkt
            };

            var db = new KundeContext();

            try
            {
                db.Transaksjoner.Add(nyTrans);
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
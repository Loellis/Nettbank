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
        public bool regBetaling(Transaksjon trans, int id)
        {
            var db = new KundeContext();
            var kontoDB = new DBKonto();

            //Sjekk datogyldighet
            string transTid;
            DateTime transDato;
            if (trans.Tidspunkt == null || trans.Tidspunkt == "")
            {
                transTid = DateTime.Now.ToString();
            }
            else if (!DateTime.TryParse(trans.Tidspunkt, out transDato))
            {
                return false;
            }
            else
            {
                transTid = transDato.ToString();
            }

            //Sjekk kontogyldighet
            if (Convert.ToInt32(trans.Innkonto) < 1)
            {
                return false;
            }
            else if (Convert.ToInt32(trans.Innkonto) == Convert.ToInt32(trans.Utkonto))
            {
                return false;
            }

            List<Konto> kontoer = kontoDB.hentTilhørendeKonti(id);
            List<int> kontoID = new List<int>();

            foreach (var k in kontoer)
            {
                try
                {
                    if (k.kontoId == Convert.ToUInt32(trans.Utkonto))
                    {
                        var nyTrans = new transaksjon()
                        {
                            utKontoId = Convert.ToInt32(trans.Utkonto),
                            innKonto = Convert.ToInt32(trans.Innkonto),
                            beløp = Convert.ToDouble(trans.Beløp),
                            KID = Convert.ToInt64(trans.KID),
                            melding = trans.Melding,
                            transaksjonsTidspunkt = transTid,
                            erGodkjent = false
                        };

                        try
                        {
                            db.Transaksjoner.Add(nyTrans);
                            db.SaveChanges();
                            return true;
                        }
                        catch (Exception feil)
                        {
                            return false;
                        }
                    }
                }
                catch (OverflowException oe)
                {
                    return false;
                }
            }
            return false;
        }

        //Finner transaksjoner som tilhører bestemt konto
        public List<Transaksjon> hentTilhørendeTransaksjon(int id)
        {
            var db = new KundeContext();

            List<transaksjon> alleTransListe = db.Transaksjoner.ToList();
            List<Transaksjon> transListe = new List<Transaksjon>();

            foreach (var t in alleTransListe)
            {
                if (t.transId == id)
                {
                    var nyTrans = new Transaksjon()
                    {
                        TransaksjonsID = t.transId,
                        Utkonto = t.utKontoId.ToString(),
                        Innkonto = t.innKonto.ToString(),
                        Beløp = t.beløp.ToString(),
                        KID = t.KID.ToString(),
                        Melding = t.melding,
                        Tidspunkt = t.transaksjonsTidspunkt,
                        Bekreftet = t.erGodkjent.ToString()
                    };
                    transListe.Add(nyTrans);
                }
            }

            return transListe;
        }

        //Finner alle transaksjoner
        public List<Transaksjon> hentAlleTransaksjoner()
        {
            var db = new KundeContext();

            List<transaksjon> alleTransListe = db.Transaksjoner.ToList();
            List<Transaksjon> transListe = new List<Transaksjon>();

            foreach (var t in alleTransListe)
            {
                var nyTrans = new Transaksjon()
                {
                    TransaksjonsID = t.transId,
                    Utkonto = t.utKontoId.ToString(),
                    Innkonto = t.innKonto.ToString(),
                    Beløp = t.beløp.ToString(),
                    KID = t.KID.ToString(),
                    Melding = t.melding,
                    Tidspunkt = t.transaksjonsTidspunkt,
                    Bekreftet = t.erGodkjent.ToString()
                };
                transListe.Add(nyTrans);
            }
            return transListe;
        }

        public Transaksjon hentTransaksjon(int id)
        {
            var db = new KundeContext();

            var enTrans = db.Transaksjoner.Find(id);

            if(enTrans == null)
            {
                return null;
            }
            else
            {
                var utTrans = new Transaksjon()
                {
                    TransaksjonsID = enTrans.transId,
                    Utkonto = enTrans.utKontoId.ToString(),
                    Innkonto = enTrans.innKonto.ToString(),
                    Beløp = enTrans.beløp.ToString(),
                    KID = enTrans.KID.ToString(),
                    Melding = enTrans.melding,
                    Tidspunkt = enTrans.transaksjonsTidspunkt,
                    Bekreftet = enTrans.erGodkjent.ToString()
                };
                return utTrans;
            }
        }

        public bool slettTransaksjon(int tID)
        {
            var db = new KundeContext();

            try
            {
                transaksjon slettTrans = db.Transaksjoner.Find(tID);
                db.Transaksjoner.Remove(slettTrans);
                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                return false;
            }
        }
    }
}
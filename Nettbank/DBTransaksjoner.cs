using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank.Models;
using System.Web.Mvc;
using BLL;

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
            if (Convert.ToInt64(trans.Innkonto) < 1)
            {
                return false;
            }
            else if (Convert.ToInt64(trans.Innkonto) == Convert.ToInt64(trans.Utkonto))
            {
                return false;
            }

            List<Konto> kontoer = kontoDB.hentTilhørendeKonti(id);
            List<int> kontoID = new List<int>();

            foreach (var k in kontoer)
            {
                try
                {
                    if (k.kontoId == Convert.ToInt64(trans.Utkonto))
                    {
                        var nyTrans = new transaksjon()
                        {
                            utKontoId = Convert.ToInt64(trans.Utkonto),
                            innKonto = Convert.ToInt64(trans.Innkonto),
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
                            var loggFeil = new LoggFeil();
                            loggFeil.SkrivTilFil(feil);

                            return false;
                        }
                    }
                }
                catch (Exception feil)
                {
                    var loggFeil = new LoggFeil();
                    loggFeil.SkrivTilFil(feil);

                    return false;
                }
            }
            return false;
        }

        //Finner transaksjoner som tilhører bestemt konto
        public List<Transaksjon> hentTilhørendeTransaksjon(int id)
        {
            var db = new KundeContext();
            var KDB = new DBKonto();


            //List<Konto> TilhørendeKonti = KDB.hentTilhørendeKonti(id);

            List<transaksjon> alleTransListe = db.Transaksjoner.ToList();
            List<Transaksjon> transListe = new List<Transaksjon>();
            /*Konto konto = new Konto();
            bool kontoFins = false;
            foreach (var k in TilhørendeKonti)
            {
                if (k.kontoId == (long)id)
                {
                    konto = k;
                    kontoFins = true;
                    break;
                }
            }*/
            
            try
            {
                foreach (var t in alleTransListe)
                {
                    /*if (kontoFins)
                    {*/
                        //if (konto.kontoId == t.utKontoId)
                        if (id == t.utKontoId)
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
                    //}
                }
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);
            }

            return transListe;
        }

        // Finner alle transaksjoner tilhørende en kunde
        public List<Transaksjon> hentKundesTransaksjoner(int id)
        {
            var db = new KundeContext();
            var KDB = new DBKonto();


            List<Konto> TilhørendeKonti = KDB.hentTilhørendeKonti(id);

            List<transaksjon> alleTransListe = db.Transaksjoner.ToList();
            List<Transaksjon> transListe = new List<Transaksjon>();
            
            try
            {
                foreach (var t in alleTransListe)
                {
                    foreach (var k in TilhørendeKonti)
                    {
                        if (k.kontoId == t.utKontoId)
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
                }
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);
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
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);

                return false;
            }
        }

        public bool endreTrans(int id, Transaksjon innTrans)
        {
            var db = new KundeContext();

            try
            {
                transaksjon endreTrans = db.Transaksjoner.Find(id);
                endreTrans.utKontoId = Convert.ToInt64(innTrans.Utkonto);
                endreTrans.innKonto = Convert.ToInt64(innTrans.Innkonto);
                endreTrans.beløp = Convert.ToDouble(innTrans.Beløp);
                endreTrans.KID = Convert.ToInt64(innTrans.KID);
                endreTrans.melding = innTrans.Melding;

                db.SaveChanges();
                return true;
            }
            catch (Exception feil)
            {
                var loggFeil = new LoggFeil();
                loggFeil.SkrivTilFil(feil);

                return false;
            }
        }
    }
}
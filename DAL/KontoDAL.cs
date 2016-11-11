﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace DAL
{
    public class KontoDAL
    {
        public bool lagKonto(Konto innKonto, int id)
        {
            var nyKonto = new konto()
            {
                kontoNavn = innKonto.kontoNavn,
                saldo = 0,
                kontoEier = id
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
                var loggFeil = new LoggFeilDAL();
                loggFeil.SkrivTilFil(feil);

                return false;
            }
        }

        public List<Konto> hentTilhørendeKonti(int id)
        {
            var db = new KundeContext();
            
            List<konto> alleKontoListe = db.Konti.ToList();
            List<Konto> kontoListe = new List<Konto>();
          
            var kId = id;

            foreach (var konto in alleKontoListe)
            {
                if ((int)konto.kontoEier == kId)
                {
                    var nyKonto = new Konto()
                    {
                        kontoId = konto.kontoID,
                        kontoNavn = konto.kontoNavn,
                        saldo = konto.saldo.ToString(),
                        kontoEier = kId.ToString()

                    };
                    kontoListe.Add(nyKonto);
                }
            }

            return kontoListe;
        }

        public bool eksisterendeKonto(long id)
        {
            var db = new KundeContext();

            List<konto> alleKontoListe = db.Konti.ToList();

            foreach(var konto in alleKontoListe)
            {
                if(id == konto.kontoID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
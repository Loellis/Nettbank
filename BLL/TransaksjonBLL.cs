using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DAL;

namespace BLL
{
    public class TransaksjonBLL
    {
        // Registrerer en betaling
        public bool regBetaling(Transaksjon trans, int id)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.regBetaling(trans, id);
        }

        //Finner transaksjoner som tilhører bestemt konto
        public List<Transaksjon> hentTilhørendeTransaksjon(int id)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.hentTilhørendeTransaksjon(id);
        }

        // Finner alle transaksjoner tilhørende en kunde
        public List<Transaksjon> hentKundesTransaksjoner(int id)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.hentKundesTransaksjoner(id);
        }

        //Finner alle transaksjoner
        public List<Transaksjon> hentAlleTransaksjoner()
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.hentAlleTransaksjoner();
        }

        // Finner en transaksjon
        public Transaksjon hentTransaksjon(int id)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.hentTransaksjon(id);
        }

        //Sletter en transaksjon
        public bool slettTransaksjon(int tID)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.slettTransaksjon(tID);
        }

        // Endrer en transaksjon
        public bool endreTrans(int id, Transaksjon innTrans)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.endreTrans(id, innTrans);
        }

        // Sjekker en transaksjon
        public bool sjekkTransaksjon(int id)
        {
            var transaksjonDAL = new TransaksjonDAL();
            return transaksjonDAL.sjekkTransaksjon(id);
        }
    }
}

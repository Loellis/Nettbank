using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Model;

namespace BLL
{
    public class KontoBLL
    {
        // Oppretter en konto
        public bool lagKonto(Konto innKonto, int id)
        {
            var kontoDAL = new KontoDAL();
            return kontoDAL.lagKonto(innKonto, id);
        }

        // Henter kontoer tilhørende en kunde
        public List<Konto> hentTilhørendeKonti(int id)
        {
            var kontoDAL = new KontoDAL();
            return kontoDAL.hentTilhørendeKonti(id);
        }

        // Sjekker om en konto eksisterer fra før av
        public bool eksisterendeKonto(long id)
        {
            var kontoDAL = new KontoDAL();
            return kontoDAL.eksisterendeKonto(id);
        }
    }
}

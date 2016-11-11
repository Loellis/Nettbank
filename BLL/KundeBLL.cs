using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Model;

namespace BLL
{
    public class KundeBLL
    {
        // Opprett kunde
        public bool settInn(Kunde innKunde)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.settInn(innKunde);
        }
        // Admin-versjon av settInn
        public bool settKunde(Kunde innKunde)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.settKunde(innKunde);
        }
        public List<Kunde> hentAlle()
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.hentAlle();
        }
        public bool Kunde_i_DB(Kunde innKunde)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.Kunde_i_DB(innKunde);
        }
        public byte[] lagHash(string innPassord)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.lagHash(innPassord);
        }
        public Kunde hentKunde(int id)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.hentKunde(id);
        }
        public bool slettKunde(int kID)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.slettKunde(kID);
        }
        public bool endreKunde(int id, Kunde innKunde)
        {
            var kundeDAL = new KundeDAL();
            return kundeDAL.endreKunde(id, innKunde);
        }
    }
}

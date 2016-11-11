using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL
{
    public class DBaksess
    {
        // For å gi direkte databaseaksess til Nettbank
        public KundeContext GiAksess()
        {
            var dbA = new KundeContext();
            return dbA;
        }
    }
}

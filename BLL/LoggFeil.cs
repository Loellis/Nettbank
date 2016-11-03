using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Klasse med en metode med til formål: Brukes for å loggføre feilsituasjoner.

    // Dette er mellomlaget mellom DAL og omverdenen.
    public class LoggFeil
    {
        public void SkrivTilFil(Exception feil)
        {
            var loggDAL = new LoggFeilDAL();
            loggDAL.SkrivTilFil(feil);
        }
    }
}

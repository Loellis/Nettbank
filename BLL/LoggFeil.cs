using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Klasse med en metode med til formål: Brukes for å loggføre feilsituasjoner.
    public class LoggFeil
    {
        public void SkrivTilFil(Exception feil)
        {
            // For å bruke Feilskriving til logg, legg til kode i catch som under:
            //try
            //{
            //    int zero = 0;
            //    int result = 100 / zero;
            //}
            //catch (Exception feil)
            //{
            //    var loggFeil = new LoggFeil();
            //    loggFeil.SkrivTilFil(feil);
            //}


            // Solution path:
            // Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            // Hardcode path:
            // string filePath = @"C:\Error.txt";

            // Kode inspirert av: http://stackoverflow.com/questions/21307789/how-to-save-exception-in-txt-file
            // og http://stackoverflow.com/questions/18813475/c-sharp-saving-a-txt-file-to-the-project-root

            // Denne strengen bruker Path.Combine med tilhørende argumenter til å sette
            // loggfilens path til prosjektets rot, og kaller loggfilen "Feilsituasjoner".
            string loggFil = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Feilsituasjoner.txt");

            // StreamWriter oppretter tekstfilen dersom den ikke allerede eksisterer.
            using (StreamWriter writer = new StreamWriter(loggFil, true))
            {
                writer.WriteLine("Message :" + feil.Message + "<br/>" + Environment.NewLine + "StackTrace :" + feil.StackTrace + "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}

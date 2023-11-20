using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

/*
 * Obrisati sve podatke o email adresama sa domenom 
 * @yahoo.com za donore koji imaju veci broj godina od korisnickog unosa.
 */

namespace BrisanjeApp1 {
    internal class Program {
        static void Main(string[] args) {

            OracleConnection con = null;
            string conString = "Data Source = 160.99.12.92/GISLAB_PD; User Id = S18234; Password = Princered!05";

            try {

                Console.Write("Unesite broj godina: ");
                int br_god;
            unos:
                br_god = Int32.Parse(Console.ReadLine());

                if (br_god > 65) { 
                    Console.WriteLine("Pogresan unos - broj godina veci od 65! Pokusajte ponovo.");
                    goto unos;
                }

                con = new OracleConnection(conString);
                con.Open();

                StringBuilder cmdDelete = new StringBuilder();

                cmdDelete.Append("DELETE FROM DON_MAIL WHERE EMAIL LIKE '%@yahoo.com' ");
                cmdDelete.Append("AND MBR_DONORA IN ( SELECT JMBG FROM DONOR ");
                cmdDelete.Append("WHERE EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ > :godine ) ");

                OracleCommand cmd = new OracleCommand(cmdDelete.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;

                OracleParameter paramGod = new OracleParameter("godine", OracleDbType.Int32);
                paramGod.Value = br_god;

                cmd.Parameters.Add(paramGod);

                Console.WriteLine("Brisanje izvrseno! (Obrisano torki: " + cmd.ExecuteNonQuery() + ")");

            }
            catch (Exception ec) {
                Console.WriteLine("Doslo je do greske pri izvrsavanju programa: " + ec.Message);
            }
            finally {
                if (con != null && con.State == System.Data.ConnectionState.Open) {
                    con.Close();
                    con.Dispose();
                }
                con = null;
            }
        }
    }
}

/* IZLAZ NA KONZOLI:

Unesite broj godina: 45
Brisanje izvrseno! (Obrisano torki: 5)
 */
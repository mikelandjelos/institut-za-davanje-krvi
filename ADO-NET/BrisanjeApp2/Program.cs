using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

/*
 * Obrisati sve podatke o email adresama sa domenom 
 * @yahoo.com za donore koji imaju veci broj godina od korisnickog unosa.
 */

namespace BrisanjeApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            OracleConnection con = null;
            string conString = "Data Source = 160.99.12.92/GISLAB_PD; User Id = S18234; Password = Princered!05;";

            try
            {

                Console.Write("Unesite broj godina: ");
                int br_god;
            unos:
                br_god = Int32.Parse(Console.ReadLine());

                if (br_god > 65)
                {
                    Console.WriteLine("Pogresan unos - broj godina veci od 65! Pokusajte ponovo.");
                    goto unos;
                }

                con = new OracleConnection(conString);
                con.Open();

                StringBuilder cmdDelete = new StringBuilder();

                cmdDelete.Append("SELECT * FROM DON_MAIL WHERE EMAIL LIKE '%@yahoo.com' ");
                cmdDelete.Append("AND MBR_DONORA IN ( SELECT JMBG FROM DONOR ");
                cmdDelete.Append("WHERE EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ > :godine ) ");

                OracleCommand cmd = new OracleCommand(cmdDelete.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;

                OracleParameter paramGod = new OracleParameter("godine", OracleDbType.Int32);
                paramGod.Value = br_god;

                cmd.Parameters.Add(paramGod);

                // konekcija i komanda kreirane, parametar dodat

                DataSet ds = new DataSet();
                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds, "EMAIL");

                OracleCommandBuilder cmdBuilder = new OracleCommandBuilder(da);

                con.Close();

                int i, brojac = 0;
                for (i = ds.Tables["EMAIL"].Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = ds.Tables["EMAIL"].Rows[i];
                    con.Open();
                    OracleTransaction tr = con.BeginTransaction();
                    dr.Delete();
                    brojac += da.Update(ds, "EMAIL");
                    tr.Commit();
                    con.Close();
                }
                Console.WriteLine("Brisanje izvrseno! (Obrisano torki: " + brojac.ToString() + ")");
            }
            catch (Exception ec)
            {
                Console.WriteLine("Doslo je do greske: " + ec.Message);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                con = null;
            }
        }
    }
}

/* IZLAZ KONZOLE:

Unesite broj godina: 50
Brisanje izvrseno! (Obrisano torki: 15)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

/*  Connection, Command, DataReader i Parameter - azuriranje prva aplikacija
 *  Potrebno je sve pacijente koji su poceli svoje 
 *  lecenje meseca koji unosi korisnik (unosi redni broj meseca)
 *  i lece se u zdravstvenoj ustanovi ciji naziv unosi korisnik
 *  prebaciti u zdravstvenu ustanovu cije ime takodje unosi korisnik;
 */

namespace AzuriranjeApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            OracleConnection con = null;
            string conString = "Data Source = <ip-addr>/GISLAB_PD; User Id = S18234; Password = <my-pass>";

            try
            {

                string nazivUstanove1, nazivUstanove2;
                int mesec;

                // korisnik unosi parametre

                Console.WriteLine("Unesite naziv ustanove iz koje zelite da prebacite pacijente: ");
                nazivUstanove1 = Console.ReadLine();
            unos:
                Console.Write("Unesite redni broj meseca (Jan -> 1): ");
                mesec = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Unesite naziv ustanove u koju zelite da prebacite pacijente: ");
                nazivUstanove2 = Console.ReadLine();

                if (mesec > 12 || mesec <= 0)
                {
                    Console.WriteLine("Pogresan unos! Pokusajte ponovo.");
                    goto unos;
                }

                // korisnik uneo parametre -> kreiramo i otvaramo konekciju
                con = new OracleConnection(conString);
                con.Open();

                // 'cupamo' id ustanove u koju treba da prebacimo pacijente

                string cmdStr = "SELECT ID_USTANOVE FROM ZDRAVSTVENA_USTANOVA WHERE NAZIV = :naziv2";
                OracleCommand cmd2 = new OracleCommand(cmdStr, con);
                cmd2.CommandType = System.Data.CommandType.Text;

                OracleParameter paramNaziv2 = new OracleParameter("naziv2", OracleDbType.Varchar2);
                paramNaziv2.Value = nazivUstanove2;
                cmd2.Parameters.Add(paramNaziv2);

                int novi_id = Convert.ToInt32(cmd2.ExecuteScalar());
                cmd2.Dispose();

                // sada je id u lokalnoj promenljivoj id

                // sada pravimo lokalne kopije podataka koje treba da update-ujemo (iz tabele PACIJENT)

                StringBuilder cmdSelectSB = new StringBuilder();
                // IME, ADRESA, JMBG, KRVNA_GRUPA, RH_FAKTOR, POL, ID_USTANOVE, DAT_POCETKA_LECENJA
                cmdSelectSB.Append("SELECT * FROM PACIJENT ");
                cmdSelectSB.Append("WHERE EXTRACT(MONTH FROM DAT_POCETKA_LECENJA) = :mesec ");
                cmdSelectSB.Append("AND ID_USTANOVE = ( ");
                cmdSelectSB.Append("SELECT ID_USTANOVE FROM ZDRAVSTVENA_USTANOVA ");
                cmdSelectSB.Append("WHERE UPPER(NAZIV) = UPPER(:naziv1)) ");

                OracleCommand cmd1 = new OracleCommand(cmdSelectSB.ToString(), con);
                cmd1.CommandType = System.Data.CommandType.Text;

                OracleParameter paramNaziv1 = new OracleParameter("naziv1", OracleDbType.Varchar2);
                paramNaziv1.Value = nazivUstanove1;
                OracleParameter paramMesec = new OracleParameter("mesec", OracleDbType.Int32);
                paramMesec.Value = mesec;

                cmd1.Parameters.Add(paramNaziv1);
                cmd1.Parameters.Add(paramMesec);
                cmd1.BindByName = true;

                DataSet ds = new DataSet();
                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = cmd1;
                da.Fill(ds, "PACIJENTI");
                // cmd1.Dispose();

                // lokalne napravljene

                OracleCommandBuilder cmdBuilder = new OracleCommandBuilder(da);

                con.Close();
                int i = 0;
                foreach (DataRow r in ds.Tables["PACIJENTI"].Rows)
                {

                    // za svaku torku izmeni id_ustanove na novi_id koji smo malopre dobili

                    r["ID_USTANOVE"] = novi_id;

                    da.Update(ds, "PACIJENTI");

                    i++;
                }

                cmd1.Dispose();
                Console.WriteLine("Azurirano je " + i.ToString() + " podataka.");
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

/* IZLAZ NA KONZOLI:
 * 
 * 1.
 * 
 * Unesite naziv ustanove iz koje zelite da prebacite pacijente:
 * Vojnomedicinska akademija
 * Unesite redni broj meseca (Jan -> 1): 9
 * Unesite naziv ustanove u koju zelite da prebacite pacijente:
 * Opsta bolnica Leskovac
 * Azurirano je 7 podataka.
 * 
 * 2.
 * 
 * Unesite naziv ustanove iz koje zelite da prebacite pacijente:
 * Opsta bolnica Jagodina
 * Unesite redni broj meseca (Jan -> 1): 9
 * Unesite naziv ustanove u koju zelite da prebacite pacijente:
 * Univerzitetski klinicki centar Nis
 * Azurirano je 3 podataka.
 */
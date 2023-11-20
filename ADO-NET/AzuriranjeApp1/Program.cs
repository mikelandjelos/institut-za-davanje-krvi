using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

/*  Connection, Command, DataReader i Parameter - azuriranje prva aplikacija
 *  Potrebno je sve pacijente koji su poceli svoje 
 *  lecenje meseca koji unosi korisnik (unosi redni broj meseca)
 *  i lece se u zdravstvenoj ustanovi ciji naziv unosi korisnik
 *  prebaciti u zdravstvenu ustanovu 'Vojnomedicinska akademija';
 */

namespace Azuriranje_faza5_app1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            OracleConnection con = null;
            string conString = "Data Source = <ip-addr>/GISLAB_PD; User Id = S18234; Password = <my-pass>";

            try
            {

                string nazivUstanove;
                int mesec;

                Console.WriteLine("Unesite naziv ustanove: ");
                nazivUstanove = Console.ReadLine();
            unos:
                Console.Write("Unesite redni broj meseca (Jan -> 1): ");
                mesec = Int32.Parse(Console.ReadLine());

                if (mesec > 12 || mesec <= 0)
                {
                    Console.WriteLine("Pogresan unos! Pokusajte ponovo.");
                    goto unos;
                }

                // kreiranje i otvaranje konekcije ka bazi
                con = new OracleConnection(conString);
                con.Open();

                // korisnik je obavio unos i konekcija ka bazi je kreirana
                // -> sada idemo na kreiranje i izvrsenje komande

                StringBuilder cmdSB = new StringBuilder();

                cmdSB.Append("UPDATE PACIJENT SET ID_USTANOVE = ( ");
                cmdSB.Append("SELECT ID_USTANOVE FROM ZDRAVSTVENA_USTANOVA ");
                cmdSB.Append("WHERE UPPER(NAZIV) = UPPER('Vojnomedicinska akademija')) ");
                cmdSB.Append("WHERE EXTRACT(MONTH FROM DAT_POCETKA_LECENJA) = :mesec ");
                cmdSB.Append("AND ID_USTANOVE = ( ");
                cmdSB.Append("SELECT ID_USTANOVE FROM ZDRAVSTVENA_USTANOVA ");
                cmdSB.Append("WHERE UPPER(NAZIV) = UPPER(:naziv))");

                OracleCommand cmd = new OracleCommand(cmdSB.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;

                // komanda je kreirana treba joj dodati jos i parametre koje u njoj koristimo

                OracleParameter paramNaziv = new OracleParameter("naziv", OracleDbType.Varchar2);
                paramNaziv.Value = nazivUstanove;
                OracleParameter paramMesec = new OracleParameter("mesec", OracleDbType.Int32);
                paramMesec.Value = mesec;

                cmd.Parameters.Add(paramNaziv);
                cmd.Parameters.Add(paramMesec);
                cmd.BindByName = true;

                Console.WriteLine("Azurirano je " + cmd.ExecuteNonQuery() + " podataka.");

            }
            catch (Exception ec)
            {
                Console.WriteLine("Doslo je do greske pri izvrsenju programa: " + ec.Message);
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
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

Unesite naziv ustanove:
Institut za kardio-hirurske bolesti "Banjica"
Unesite redni broj meseca (Jan -> 1): 9
Azurirano je 3 podataka.

 */
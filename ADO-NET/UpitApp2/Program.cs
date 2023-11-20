using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;

/*
 * Connection, Command, DataReader i Parameter - prva aplikacija, SLOZENI UPIT
 * 
 * Pronaci i ispisati datum donacije, redni broj, puno ime donora, 
 * status i kolicinu donirane krvi za sve donacije 
 * obavljene trazenog meseca - omoguciti unos meseca (int) preko komandne linije;
 * (trenutno postoje samo donacije obavljene septembra i decembra)
 */


namespace UpitApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            OracleConnection con = null;
            string conString = "Data Source = <ip-addr>/GISLAB_PD; User Id = S18234; Password = <my-pass>";

            try
            {

                // korisnik unosi redni broj meseca
                Console.WriteLine("Unesite redni broj meseca (Jan -> 1):");
            unos:
                int mesec = Int32.Parse(Console.ReadLine());

                if (mesec <= 0 || mesec > 12)
                {
                    Console.WriteLine("Pogresan unos! Pokusajte ponovo.");
                    goto unos;
                }

                // kreiranje i otvaranje konekcije
                con = new OracleConnection(conString);
                con.Open();

                // kreiramo nas upit pomocu StringBuilder-a
                StringBuilder query = new StringBuilder();
                query.Append("SELECT DONOR.LICNO_IME || ' ' || DONOR.PREZIME AS IME,");
                query.Append("DONACIJA.RBR_DONACIJE AS REDNI_BROJ, ");
                query.Append("DONACIJA.DATUM_DONACIJE AS DATUM, ");
                query.Append("DONACIJA.STATUS AS STATUS, ");
                query.Append("DONACIJA.KOLICINA AS KOLICINA_KRVI ");
                query.Append("FROM DONOR, DONACIJA ");
                query.Append("WHERE DONOR.JMBG = DONACIJA.MBR_DONORA ");
                query.Append("AND EXTRACT(MONTH FROM DONACIJA.DATUM_DONACIJE) = :mesec");

                // kreiranje komande i parametara
                OracleCommand cmd = new OracleCommand(query.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;

                OracleParameter paramMesec = new OracleParameter("mesec", OracleDbType.Int32);
                paramMesec.Value = mesec;

                cmd.Parameters.Add(paramMesec);
                // kreirana naredba sa parametrom

                // kreiranje DataAdaptera
                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = cmd;

                // kreiranje DataSet objekta (kontejnera za kesiranje podataka)
                DataSet ds = new DataSet();

                // 'punjenje' DataSet-a pomocu DataAdapter-a
                da.Fill(ds, "DONACIJE");
                // i zatvaranje konekcije -> nije nam vise potrebna
                // zato sto imamo podatke u lokalu (unutar nase operativne memorije)
                con.Close();

                int i = 1;
                foreach (DataRow r in ds.Tables["Donacije"].Rows)
                {
                    string punoIme = Convert.ToString(r[0]);
                    int rbrDonacije = Convert.ToInt32(r[1]);
                    string datumDonacije = Convert.ToString(r[2]);
                    string statusDonacije = Convert.ToString(r[3]);
                    int kolicinaKrvi = Convert.ToInt32(r[4]);

                    Console.Write(i.ToString() + ". " + punoIme + ", ");
                    Console.Write(rbrDonacije.ToString() + ", " + datumDonacije.ToString() + ", ");
                    Console.WriteLine(statusDonacije + ", " + kolicinaKrvi.ToString() + "ml ;");
                    i++;
                    // ukoliko bi trebali da vrsimo neku izmenu datuma donacije
                    // mogli bi njegovo 'hvatanje' iz objekta r (klasni tip DataRow)
                    // da izvrsimo na sledeci nacin:
                    // -> DateTime datumDonacije = Convert.ToDateTime(r[2]);
                    // ali to u ovom programu nije neophodno tako da i tip string zadovoljava nase potrebe
                }
                if (i == 1)
                    Console.WriteLine("Ne postoje donacije obavljene " + mesec + ". meseca.");
            }
            catch (Exception ec)
            {
                Console.WriteLine("Doslo je do greske: " + ec.Message);
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

Unesite redni broj meseca (Jan -> 1):
9
1. Ratimir Grgurovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
2. Miomir Darkovic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
3. Borivoje Janketic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
4. Dubravko Savic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
5. Drazen Brdjanin, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
6. Nebojsa Obradovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
7. Velizar Nikolic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
8. Zoran Tasic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
9. Obren Moldovan, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
10. Drazen Filipovic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
11. Vasilije Darkovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
12. Radomir Despotovic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
13. Una Gavrilovic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
14. Mirna Marinkovic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
15. Gordana Despotovic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
16. Zaklina Grgurovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
17. Draginja Popovic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
18. Ljiljana Carapic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
19. Zivota Lazic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
20. Nenad Bacic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
21. Spasoje Zoric, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
22. Marko Evic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
23. Miroslav Cvetkovic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
24. Dragomir Djordjevic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
25. Tomislav Velimirovic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
26. Velimir Gojkovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
27. Arsenije Vladic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
28. Bogdan Zoric, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
29. Vojkan Urosevic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
30. Teodor Borisov, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
31. Gabrijel Urosevic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
32. Nemanja Darkovic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
33. Dusan Cvetkovic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
34. Gojko Lazic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
35. Anastasije Vujic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
36. Andrej Bojanic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
37. Vojislava Nedeljkovic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
38. Vojislava Nestorovski, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
39. Jadranka Nanusevski, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
40. Radana Vasiljevic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
41. Ruza Gavrilovic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
42. Draginja Brdjanin, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
43. Ljubisav Markovic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
44. Branko Karanovic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
45. Radojko Golubovic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;
46. Sinisa Jocic, 1, 9/1/2021 12:00:00 AM, iskoriscena, 450ml ;
47. Jakov Georgijevic, 1, 9/2/2021 12:00:00 AM, iskoriscena, 450ml ;
48. Kosta Nesic, 1, 9/3/2021 12:00:00 AM, iskoriscena, 450ml ;
49. Mila Tasic, 1, 9/6/2021 12:00:00 AM, iskoriscena, 450ml ;
50. Mila Simic, 1, 9/7/2021 12:00:00 AM, iskoriscena, 450ml ;

 */
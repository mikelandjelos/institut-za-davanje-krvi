using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

/*
 * Connection, Command, DataReader i Parameter - prva aplikacija, SLOZENI UPIT
 * 
 * Pronaci i ispisati datum donacije, redni broj, puno ime donora, 
 * status i kolicinu donirane krvi za sve donacije 
 * obavljene trazenog meseca - omoguciti unos meseca (int) preko komandne linije;
 * (trenutno postoje samo donacije obavljene septembra i decembra)
 */

namespace App1_upit_faza4
{
    internal class Program
    {
        static void Main(string[] args)
        {

            OracleConnection con = null;
            string conString = "Data Source = <ip-addr>/GISLAB_PD; User Id = S18234; Password = <my-pass>";

            try
            {

                int mesec;
            unos:
                // korisnik unosi redni broj meseca
                Console.WriteLine("Unesite redni broj meseca (Januar -> 1, Decembar -> 12):");
                mesec = Int32.Parse(Console.ReadLine());

                if (mesec <= 0 || mesec > 12)
                {
                    Console.WriteLine("Pogresan unos! Pokusajte ponovo.");
                    goto unos;
                }

                // kreiramo novu konekciju ka nasoj bazi podataka
                con = new OracleConnection(conString);
                // a zatim je otvaramo
                con.Open();

                // zbog velicine string-a tj. naseg upita koristimo StringBuilder
                StringBuilder query = new StringBuilder();
                query.Append("SELECT DONOR.LICNO_IME || ' ' || DONOR.PREZIME AS IME,");
                query.Append("DONACIJA.RBR_DONACIJE AS REDNI_BROJ, ");
                query.Append("DONACIJA.DATUM_DONACIJE AS DATUM, ");
                query.Append("DONACIJA.STATUS AS STATUS, ");
                query.Append("DONACIJA.KOLICINA AS KOLICINA_KRVI ");
                query.Append("FROM DONOR, DONACIJA ");
                query.Append("WHERE DONOR.JMBG = DONACIJA.MBR_DONORA ");
                query.Append("AND EXTRACT(MONTH FROM DONACIJA.DATUM_DONACIJE) = :mesec");

                // kreiranje komande pomocu postojece konekcije i objekta query koji pretvaramo u string
                // koristeci metodu ToString nad njime
                OracleCommand cmd = new OracleCommand(query.ToString(), con);
                cmd.CommandType = System.Data.CommandType.Text;

                // kreiranje parametra 
                OracleParameter paramMesec = new OracleParameter("mesec", OracleDbType.Int32);
                paramMesec.Value = mesec;

                // dodavanje parametra komandi
                cmd.Parameters.Add(paramMesec);

                // kreiranje objekta klasnog tipa DataReader zbog toga sto ce rezultat
                // nase SQL komande biti relacija sa vise torki (odnosno nece biti skalar)
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {

                    int i = 1;

                    while (dr.Read())
                    {

                        // uzimanje celija torke koja se trenutno nalazi u DataReader-u
                        // u lokalne promenljive radi njihovog stampanja na std izlaz
                        string punoImeDonora = dr.GetString(0);
                        int rbrDonacije = dr.GetInt32(1);
                        DateTime datumDonacije = dr.GetDateTime(2);
                        string statusDonacije = dr.GetString(3);
                        int kolicinaKrvi = dr.GetInt32(4);

                        Console.Write(i.ToString() + ". " + punoImeDonora + ", ");
                        Console.Write(rbrDonacije.ToString() + ", " + datumDonacije.ToString() + ", ");
                        Console.WriteLine(statusDonacije + ", " + kolicinaKrvi.ToString() + "ml ;");
                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("Ne postoje donacije obavljenje " + mesec + ". meseca u godini!");
                }

                dr.Close();

            }
            catch (Exception ec)
            {
                Console.WriteLine("Doslo je do greske prilikom izvrsavanja: " + ec.Message);
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

Unesite redni broj meseca (Januar -> 1, Decembar -> 12):
12
1. Ratimir Grgurovic, 4, 12/19/2021 12:00:00 AM, na stanju, 450ml ;
2. Ratimir Grgurovic, 3, 12/12/2021 12:00:00 AM, na stanju, 450ml ;
3. Ratimir Grgurovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
4. Miomir Darkovic, 4, 12/19/2021 12:00:00 AM, na stanju, 450ml ;
5. Miomir Darkovic, 3, 12/12/2021 12:00:00 AM, na stanju, 450ml ;
6. Miomir Darkovic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
7. Borivoje Janketic, 3, 12/13/2021 12:00:00 AM, na stanju, 450ml ;
8. Borivoje Janketic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
9. Dubravko Savic, 3, 12/16/2022 12:00:00 AM, na stanju, 450ml ;
10. Dubravko Savic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
11. Drazen Brdjanin, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
12. Nebojsa Obradovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
13. Velizar Nikolic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
14. Zoran Tasic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
15. Obren Moldovan, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
16. Drazen Filipovic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
17. Vasilije Darkovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
18. Radomir Despotovic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
19. Una Gavrilovic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
20. Mirna Marinkovic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
21. Gordana Despotovic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
22. Zaklina Grgurovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
23. Draginja Popovic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
24. Ljiljana Carapic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
25. Zivota Lazic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
26. Nenad Bacic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
27. Spasoje Zoric, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
28. Marko Evic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
29. Miroslav Cvetkovic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
30. Dragomir Djordjevic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
31. Tomislav Velimirovic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
32. Velimir Gojkovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
33. Arsenije Vladic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
34. Bogdan Zoric, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
35. Vojkan Urosevic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
36. Teodor Borisov, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
37. Gabrijel Urosevic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
38. Nemanja Darkovic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
39. Dusan Cvetkovic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
40. Gojko Lazic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
41. Anastasije Vujic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
42. Andrej Bojanic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
43. Vojislava Nedeljkovic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
44. Vojislava Nestorovski, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
45. Jadranka Nanusevski, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
46. Radana Vasiljevic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
47. Ruza Gavrilovic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
48. Draginja Brdjanin, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
49. Ljubisav Markovic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
50. Branko Karanovic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
51. Radojko Golubovic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;
52. Sinisa Jocic, 2, 12/6/2021 12:00:00 AM, na stanju, 450ml ;
53. Jakov Georgijevic, 2, 12/7/2021 12:00:00 AM, na stanju, 450ml ;
54. Kosta Nesic, 2, 12/8/2021 12:00:00 AM, na stanju, 450ml ;
55. Mila Tasic, 2, 12/9/2021 12:00:00 AM, na stanju, 450ml ;
56. Mila Simic, 2, 12/10/2021 12:00:00 AM, na stanju, 450ml ;

*/
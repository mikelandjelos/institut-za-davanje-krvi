                /* AZURIRANJE PODATAKA - 2 primera */

/* Primer 1:
- Potrebno je sve pacijente koji su poceli svoje lecenje novembra 
i lece se u zdravstvenoj ustanovi 'Univerzitetski klinicki centar Nis' 
prebaciti u zdravstvenu ustanovu 'Vojna bolnica Nis'
(postoji FK ID_USTANOVE u relaciji PACIJENT koji referencira istoimeni PK tabele
ZDRAVSTVENA_USTANOVA; treba pronaci nacin da atribut ID_USTANOVE za odgovarajuce torke u relaciji PACIJENT
promenimo u vrednost id-a za ustanovu ciji je naziv 'Vojna bolnica Nis');
*/

/* Upit za ispitivanje torke koje ispunjavaju uslov, pre azuriranja relacije: */
SELECT PACIJENT.IME, PACIJENT.DAT_POCETKA_LECENJA, ZDRAVSTVENA_USTANOVA.NAZIV
FROM PACIJENT JOIN ZDRAVSTVENA_USTANOVA
ON PACIJENT.ID_USTANOVE = ZDRAVSTVENA_USTANOVA.ID_USTANOVE 
WHERE EXTRACT(MONTH FROM PACIJENT.DAT_POCETKA_LECENJA) = 11
AND PACIJENT.ID_USTANOVE = (
    SELECT ID_USTANOVE
    FROM ZDRAVSTVENA_USTANOVA
    WHERE UPPER(NAZIV) = UPPER('Univerzitetski klinicki centar Nis'));

/* Komanda za azuriranje relacije: */
UPDATE PACIJENT
SET ID_USTANOVE = (
    SELECT ID_USTANOVE
    FROM ZDRAVSTVENA_USTANOVA
    WHERE UPPER(NAZIV) = UPPER('Vojna bolnica Nis'))
WHERE EXTRACT(MONTH FROM DAT_POCETKA_LECENJA) = 11
AND ID_USTANOVE = (
    SELECT ID_USTANOVE
    FROM ZDRAVSTVENA_USTANOVA
    WHERE UPPER(NAZIV) = UPPER('Univerzitetski klinicki centar Nis'));

/* Upit za ispitivanje stanja nakon azuriranja relacije: */
SELECT PACIJENT.IME, PACIJENT.DAT_POCETKA_LECENJA, ZDRAVSTVENA_USTANOVA.NAZIV
FROM PACIJENT JOIN ZDRAVSTVENA_USTANOVA
ON PACIJENT.ID_USTANOVE = ZDRAVSTVENA_USTANOVA.ID_USTANOVE 
WHERE EXTRACT(MONTH FROM PACIJENT.DAT_POCETKA_LECENJA) = 11
AND PACIJENT.ID_USTANOVE = (
    SELECT ID_USTANOVE
    FROM ZDRAVSTVENA_USTANOVA
    WHERE UPPER(NAZIV) = UPPER('Vojna bolnica Nis')); 

/* Primer 2:
 - Azurirati podatke u bazi tako da banka krvi
 'Banka krvi UKC Beograd' prebaci deljenje svojih zaliha sa
 banke krvi 'Banka krvi OB Uzice' na 'Banka krvi UKC Nis';
*/
/* mozemo da kreiramo pogled koji cemo iskoristiti za proveravanje
   uspesnosti naseg azuriranja 
Napomena: Kreirani pogled ne predstavlja jedan od primera za poglede u okviru ovog domaceg zadatka 
vec samo sluzi svrsi da olaksa proveru rezultata azuriranja. */
CREATE VIEW DELI_ZALIHE_PREGLED AS
SELECT BD.NAZIV AS BANKA_DELITELJ, BK.NAZIV AS BANKA_KORISNIK
FROM BANKA_KRVI BD JOIN DELI_ZALIHE
ON BD.ID_BANKE = DELI_ZALIHE.ID_BANKE_D
JOIN BANKA_KRVI BK
ON DELI_ZALIHE.ID_BANKE_K = BK.ID_BANKE
ORDER BY BD.ID_BANKE ASC;
/* komanda za azuriranje */
UPDATE DELI_ZALIHE
SET ID_BANKE_K = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi UKC Nis'
)
WHERE ID_BANKE_K = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi OB Uzice'
)
AND ID_BANKE_D = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi UKC Beograd'
);

CREATE VIEW DELI_ZALIHE_PREGLED AS
SELECT BD.NAZIV AS BANKA_DELITELJ, BK.NAZIV AS BANKA_KORISNIK
FROM BANKA_KRVI BD JOIN DELI_ZALIHE
ON BD.ID_BANKE = DELI_ZALIHE.ID_BANKE_D
JOIN BANKA_KRVI BK
ON DELI_ZALIHE.ID_BANKE_K = BK.ID_BANKE
ORDER BY BD.ID_BANKE ASC;
/* komanda za azuriranje */
UPDATE DELI_ZALIHE
SET ID_BANKE_K = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi UKC Nis'
)
WHERE ID_BANKE_K = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi OB Uzice'
)
AND ID_BANKE_D = (
    SELECT ID_BANKE FROM BANKA_KRVI WHERE NAZIV = 'Banka krvi UKC Beograd'
);

                /* BRISANJE PODATAKA - 2 primera */

/* Primer 1:
 - Obrisati podatke o bankama krvi koje ne snabdevaju zalihama nijednu
 zdravstvenu ustanovu; (brisemo torku iz relacije BANKA_KRVI, da bi nasli
 id bolnice koja ne snabdeva nijednu zdravstvenu ustanovu koristicemo
 podatke iz tabela SNABDEVA i BANKA_KRVI)
*/

DELETE FROM BANKA_KRVI
WHERE ID_BANKE NOT IN (
    SELECT DISTINCT ID_BANKE
    FROM SNABDEVA
);

/* Primer 2:
 - Obrisati sve podatke o email adresama sa domenom @yahoo.com 
 za donore koji su stariji od 50 godina.
*/
/* Upit1 za pronalazenje torki koje se brisu:
=> vratice relaciju bez torki ukoliko je brisanje uspesno */
SELECT JMBG, LICNO_IME || ' ' || PREZIME AS IME, EMAIL
FROM DONOR JOIN DON_MAIL
ON JMBG = MBR_DONORA
WHERE EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ > 50
AND EMAIL LIKE '%@yahoo.com';

SELECT COUNT(*) BROJ_ADRESA FROM DON_MAIL;

/* komanda za brisanje podataka */
DELETE FROM DON_MAIL
WHERE EMAIL LIKE '%@yahoo.com'
AND MBR_DONORA IN (
    SELECT JMBG
    FROM DONOR
    WHERE EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ > 50
);
/* dodatni upit za proveru stanja nakon brisanja 
   => ukoliko je brisanje uspesno trebalo bi da vrati
   relaciju bez torki, a ukoliko nije uspesno trebalo bi
   da vrati jednu rec u kojoj je konstanta 0 */
SELECT DISTINCT 0 FROM DONOR
WHERE EXISTS (
    SELECT 0
    FROM DON_MAIL
    WHERE MBR_DONORA = JMBG
    AND EMAIL LIKE '%@yahoo.com')
AND EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ > 50;
                /* POGLEDI - 2 primera */

/* Primer 1:
 - Kreirati pogled koji prikazuje nazive zdravstvenih ustanova,
 njihove lokacije i broj pacijenata koji se u njima leci, kao
 i broj banki krvi koji ih snabdeva svojim zalihama;
*/
/* Rezultat "unutrasnjeg" upita, kome je dat alijas USTANOVE je
relacija koja sadrzi trazene podatke iz relacije ZDRAVSTVENA_USTANOVA
kao i broj pacijenata koji se u njima leci; spoljasnji upit spaja
novodobijenu relaciju USTANOVE i postojecu relaciju SNABDEVA kako bi
se za svaku ustanovu prebrojale i banke koje je snabdevaju */
CREATE VIEW USTANOVE_INFO AS
SELECT USTANOVE.USTANOVA, USTANOVE.LOKACIJA,
USTANOVE.BROJ_PACIJENATA, 
COUNT(*) BROJ_BANKI
FROM SNABDEVA JOIN ( /* "unutrasnji" upit */
    SELECT ZDRAVSTVENA_USTANOVA.ID_USTANOVE,
    NAZIV AS USTANOVA, LOKACIJA,
    COUNT(*) AS BROJ_PACIJENATA
    FROM ZDRAVSTVENA_USTANOVA, PACIJENT
    WHERE ZDRAVSTVENA_USTANOVA.ID_USTANOVE = PACIJENT.ID_USTANOVE
    GROUP BY ZDRAVSTVENA_USTANOVA.ID_USTANOVE, NAZIV, LOKACIJA) USTANOVE
ON SNABDEVA.ID_USTANOVE = USTANOVE.ID_USTANOVE
GROUP BY USTANOVE.USTANOVA, USTANOVE.LOKACIJA, USTANOVE.BROJ_PACIJENATA;

/* Primer 2:
 - Kreirati pogled koji prikazuje informacije o donorima i to jmbg, puno ime,
krvnu grupu i Rh faktor, starost, broj donacija ciji je status 'na stanju';
*/
CREATE VIEW DONOR_INFO AS
SELECT JMBG, LICNO_IME, PREZIME,
(EXTRACT YEAR FROM CURRENT_YEAR) - GOD_RODJ AS STAROST,
KRVNA_GRUPA, RH_FAKTOR, COUNT(*) AS BROJ_DONACIJA
FROM DONOR JOIN DONACIJA
ON JMBG = MBR_DONORA
WHERE UPPER(STATUS) = 'NA STANJU'
GROUP BY JMBG, LICNO_IME, PREZIME,
(EXTRACT YEAR FROM CURRENT_YEAR) - GOD_RODJ,
KRVNA_GRUPA, RH_FAKTOR;

                /* INDEKSIRANJE - 2 primera */

/* Primer 1: 
 - Napisati naredbu kojom se nad kolonom MBR_DONORA u tabeli DONACIJA
 formira indeks. */

CREATE INDEX DONACIJA_MBRDON_IND ON DONACIJA(MBR_DONORA);

/* Primer 2: 
 - Napisati naredbu kojom se formira indeks nad kolonom 
 ID_USTANOVE u tabeli PACIJENT
*/

CREATE INDEX PACIJENT_IDUST_IND ON PACIJENT(ID_USTANOVE);
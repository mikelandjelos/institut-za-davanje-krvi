/*SQL UPITI*/

/*-----------------------------------------------------------------------*/

/* PROSTI SQL UPITI */

/*
ZADATAK 1: - KLAUZULA WHERE I RELACIONI OPERATOR <
 - Pronaci i ispisati imena, prezimena, maticne brojeve i godine rodjenja
   svih donora koji su rodjeni pre 1990. godine;
*/
SELECT LICNO_IME || ' ' || PREZIME AS IME, JMBG AS MATICNI_BROJ,
GOD_RODJ AS GODINA_RODJENJA
FROM DONOR
WHERE GOD_RODJ < 1990;

/*
ZADATAK 1.5: - KLAUZULA LIKE
 - Pronaci i ispisati imena i prezimena svih pacijenata cije
   prezime pocinje sa A (ime i prezime u tabeli PACIJENT predstavljaju
   jedan podatak pod nazivom IME, ali su sva imena i prezimena odvojena
   po jednim blanko znakom);
*/
SELECT IME FROM PACIJENT
WHERE UPPER(IME) LIKE ('% A%');

/*
ZADATAK 2: - KLAUZULA BETWEEN i LOGICKI OPERATOR AND
 - Pronaci i ispisati datume, maticne brojeve donora i statuse svih donacija
   koje su obavljene u periodu od 6 do 8 decembra 2021. godine,
   ukljucujuci njih;
*/
SELECT MBR_DONORA, DATUM_DONACIJE, STATUS FROM DONACIJA
WHERE EXTRACT(DAY FROM DATUM_DONACIJE) BETWEEN 6 AND 8
AND EXTRACT(MONTH FROM DATUM_DONACIJE) = 12
AND EXTRACT(YEAR FROM DATUM_DONACIJE) = 2021;

/*
ZADATAK 3: IN, IS NULL, OR
 - Pronaci i ispisati imena i krvne grupe zajedno sa Rh faktorom 
   svih donora za koje se ne zna adresa stanovanja ili im je ime 'Marko' ili 'Mila';
 - U okviru ovog zadatka bilo je potrebno dodati nove vrednosti u BP
   zbog toga sto nije postojao donor za koga se nije znala adresa stanovanja;
*/ 
INSERT INTO DONOR VALUES('Slavoljub', 'Popovic', (NULL), 4466276850217, 1999, 'A', '-', 'M', 88);
INSERT INTO DONOR VALUES('Vojkan', 'Krsmanovic', (NULL), 4019302768105, 1993, 'B', '+', 'M', 76);
INSERT INTO DONOR VALUES('Marko', 'Jankovic', (NULL), 9998712011690, 1956, 'AB', '-', 'M', 101);

SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
OR ADRESA IS NULL;

/* Moze i pomocu UNION */

SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
UNION
SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE ADRESA IS NULL;

/*
    Da li postoji neko ko ispunjava oba uslova prethodnog zadatka? (INTERSECT)
*/
SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
AND ADRESA IS NULL;

SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
INTERSECT
SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE ADRESA IS NULL;

/*
    Uraditi prethodni zadatak ali tako da se ne pojavljuju torke koje ispunjavaju
    oba uslova; (MINUS)
*/
SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
OR ADRESA IS NULL
MINUS
SELECT LICNO_IME || ' ' || PREZIME AS IME, KRVNA_GRUPA || RH_FAKTOR AS TIP_KRVI
FROM DONOR
WHERE UPPER(LICNO_IME) IN ('MARKO', 'MILA')
AND ADRESA IS NULL;


/*-----------------------------------------------------------------------*/

/* SLOZENI SQL UPITI */

/*
    ZADATAK 4: INNER JOIN
    - Pronaci i ispisati datum donacije, redni broj, puno ime donora, status
    i kolicinu donirane krvi za sve donacije obavljene septembra (godina i dan su nebitni);
*/
SELECT DONOR.LICNO_IME || ' ' || DONOR.PREZIME AS IME,
DONACIJA.RBR_DONACIJE AS REDNI_BROJ,
DONACIJA.DATUM_DONACIJE AS DATUM,
DONACIJA.STATUS AS STATUS,
DONACIJA.KOLICINA AS KOLICINA_KRVI
FROM DONOR, DONACIJA
WHERE DONOR.JMBG = DONACIJA.MBR_DONORA
AND EXTRACT(MONTH FROM DONACIJA.DATUM_DONACIJE) = 9;

SELECT DONOR.LICNO_IME || ' ' || DONOR.PREZIME AS IME,
DONACIJA.RBR_DONACIJE AS REDNI_BROJ,
DONACIJA.DATUM_DONACIJE AS DATUM,
DONACIJA.STATUS AS STATUS,
DONACIJA.KOLICINA AS KOLICINA_KRVI
FROM DONOR INNER JOIN DONACIJA
ON DONOR.JMBG = DONACIJA.MBR_DONORA
WHERE EXTRACT(MONTH FROM DONACIJA.DATUM_DONACIJE) = 9;

/*
    ZADATAK 5: LEFT OUTER JOIN
    - Prikazati licna imena svih donora rodjenih pre 1995. godine
    i njihove email adrese, ukoliko ne postoji podatak o email adresi
    ispisati samo licno ime donora;
*/
SELECT DONOR.LICNO_IME AS IME,
DON_MAIL.EMAIL AS EMAIL
FROM DONOR, DON_MAIL
WHERE DONOR.JMBG = DON_MAIL.MBR_DONORA(+)
AND DONOR.GOD_RODJ < 1995;

SELECT DONOR.LICNO_IME AS IME,
DON_MAIL.EMAIL AS EMAIL
FROM DONOR LEFT JOIN DON_MAIL
ON DONOR.JMBG = DON_MAIL.MBR_DONORA
WHERE DONOR.GOD_RODJ < 1995;

/*
    ZADATAK 6: RIGHT OUTER JOIN
    - Prikazati sve nazive zdravstvenih ustanova i nazive banaka krvi
    koje ih snabdevaju. Ukoliko banka krvi ne snabdeva nijednu ustanovu
    prikazati njen naziv.
*/
INSERT INTO BANKA_KRVI VALUES (6, 'Banka krvi UKC Pristina', 'Pristina', '038500600');

SELECT ZDRAVSTVENA_USTANOVA.NAZIV AS ZDRAVSTVENA_USTANOVA,
BANKA_KRVI.NAZIV AS BANKA_KRVI
FROM ZDRAVSTVENA_USTANOVA INNER JOIN SNABDEVA
ON ZDRAVSTVENA_USTANOVA.ID_USTANOVE = SNABDEVA.ID_BANKE
RIGHT JOIN BANKA_KRVI
ON SNABDEVA.ID_BANKE = BANKA_KRVI.ID_BANKE;

SELECT ZDRAVSTVENA_USTANOVA.NAZIV AS ZDRAVSTVENA_USTANOVA,
BANKA_KRVI.NAZIV AS BANKA_KRVI
FROM ZDRAVSTVENA_USTANOVA, SNABDEVA, BANKA_KRVI
WHERE ZDRAVSTVENA_USTANOVA.ID_USTANOVE(+) = SNABDEVA.ID_BANKE
AND SNABDEVA.ID_BANKE(+) = BANKA_KRVI.ID_BANKE;

/*
    ZADATAK 7: NEKORELISANI UGNJEZDENI UPITI
    - Pronaci i prikazati podatke o najstarijem donoru;
*/
SELECT * FROM (
SELECT JMBG, LICNO_IME || ' ' || PREZIME AS IME,
ADRESA, KRVNA_GRUPA, RH_FAKTOR, POL, TEZINA,
EXTRACT(YEAR FROM CURRENT_DATE) - GOD_RODJ AS STAROST
FROM DONOR
ORDER BY STAROST DESC
)
WHERE ROWNUM = 1;

/*
    ZADATAK 8: GROUP BY, HAVING, COUNT (FUNKCIJA AGREGACIJE)
    - Prikazati podatke za donore koji imaju broj donacija veci od 2;
    - Trenutno svi donori u bazi imaju po 2 donacije, pa je potrebno
    dodati nekoliko donacija kako bi upit imao smislen rezultat;
*/
INSERT INTO DONACIJA VALUES(4466816950637, 3, '12-Dec-21', 450, 2, 'na stanju');
INSERT INTO DONACIJA VALUES(4010408568105, 3, '12-Dec-21', 450, 3, 'na stanju');
INSERT INTO DONACIJA VALUES(9976771011690, 3, '13-Dec-21', 450, 4, 'na stanju');
INSERT INTO DONACIJA VALUES(6295020821067, 3, '16-Dec-22', 450, 5, 'na stanju');
INSERT INTO DONACIJA VALUES(4466816950637, 4, '19-Dec-21', 450, 2, 'na stanju');
INSERT INTO DONACIJA VALUES(4010408568105, 4, '19-Dec-21', 450, 3, 'na stanju');

SELECT *
FROM DONOR, (SELECT MBR_DONORA, COUNT(*) AS BROJ_DONACIJA
FROM DONACIJA
GROUP BY MBR_DONORA
HAVING COUNT(*) > 2) UNUTRASNJA
WHERE DONOR.JMBG = UNUTRASNJA.MBR_DONORA;

/*
    ZADATAK 9: Korelisani upiti
    - Prikazi sve podatke o pacijentima koji su smesteni u zdravstvenoj ustanovi
    sa identifikacionim brojem 2 ('Vojna bolnica Nis');
    - Primer moze da se uradi vrlo lako, 
    ali je uradjen pomocu korelisanih upita;
*/
SELECT * FROM PACIJENT WHERE ID_USTANOVE = 2; /*vrlo prosto resenje*/

/*malo komplikovanije resenje koje koristi korelisane upite*/
SELECT * FROM PACIJENT
WHERE EXISTS(
SELECT ID_USTANOVE FROM ZDRAVSTVENA_USTANOVA
WHERE ZDRAVSTVENA_USTANOVA.ID_USTANOVE = PACIJENT.ID_USTANOVE /*ovo je korelacija*/
AND ZDRAVSTVENA_USTANOVA.ID_USTANOVE = 2);

/*
    ZADATAK 10:
    - Prikazati nazive zdravstvenih ustanova i za svaku od njih broj
    pacijenata koji se u njima leci. Sortirati rezultat u rastucem poretku.
*/
SELECT ZDRAVSTVENA_USTANOVA.NAZIV || ', ' || ZDRAVSTVENA_USTANOVA.LOKACIJA USTANOVA,
COUNT(*) BROJ_PACIJENATA
FROM ZDRAVSTVENA_USTANOVA, PACIJENT
WHERE ZDRAVSTVENA_USTANOVA.ID_USTANOVE = PACIJENT.ID_USTANOVE
GROUP BY ZDRAVSTVENA_USTANOVA.NAZIV || ', ' || ZDRAVSTVENA_USTANOVA.LOKACIJA
ORDER BY BROJ_PACIJENATA ASC;

/*-----------------------------------------------------------------------*/
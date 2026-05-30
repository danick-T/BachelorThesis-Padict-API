-- CoachAssist seed data
-- Doel: realistische testdata voor 1 club.
-- Dit script voert pas iets uit wanneer je het zelf bewust tegen Neon draait.
-- Het script ruimt alleen de eigen seedclub en seedlogins op.

BEGIN;

-- 1. Ruim bestaande seeddata op, zodat het script opnieuw uitvoerbaar blijft.
WITH seed_club AS (
    SELECT clubid
    FROM club
    WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid
    FROM team
    WHERE clubid IN (SELECT clubid FROM seed_club)
),
seed_spelers AS (
    SELECT spelerid
    FROM speler
    WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_wedstrijden AS (
    SELECT wedstrijdid
    FROM wedstrijd
    WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_trainingen AS (
    SELECT trainingid
    FROM training
    WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_opstellingen AS (
    SELECT opstellingid
    FROM opstelling
    WHERE wedstrijdid IN (SELECT wedstrijdid FROM seed_wedstrijden)
)
DELETE FROM opstellingspeler
WHERE opstellingid IN (SELECT opstellingid FROM seed_opstellingen);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
),
seed_wedstrijden AS (
    SELECT wedstrijdid FROM wedstrijd WHERE teamid IN (SELECT teamid FROM seed_teams)
)
DELETE FROM opstelling
WHERE wedstrijdid IN (SELECT wedstrijdid FROM seed_wedstrijden);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
),
seed_spelers AS (
    SELECT spelerid FROM speler WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_wedstrijden AS (
    SELECT wedstrijdid FROM wedstrijd WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_trainingen AS (
    SELECT trainingid FROM training WHERE teamid IN (SELECT teamid FROM seed_teams)
)
DELETE FROM spelerstatistiek
WHERE spelerid IN (SELECT spelerid FROM seed_spelers)
   OR wedstrijdid IN (SELECT wedstrijdid FROM seed_wedstrijden);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
),
seed_spelers AS (
    SELECT spelerid FROM speler WHERE teamid IN (SELECT teamid FROM seed_teams)
),
seed_trainingen AS (
    SELECT trainingid FROM training WHERE teamid IN (SELECT teamid FROM seed_teams)
)
DELETE FROM trainingaanwezigheid
WHERE spelerid IN (SELECT spelerid FROM seed_spelers)
   OR trainingid IN (SELECT trainingid FROM seed_trainingen);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
),
seed_spelers AS (
    SELECT spelerid FROM speler WHERE teamid IN (SELECT teamid FROM seed_teams)
)
DELETE FROM teamaanvraag
WHERE spelerid IN (SELECT spelerid FROM seed_spelers)
   OR teamid IN (SELECT teamid FROM seed_teams);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
)
DELETE FROM clubaanvraag
WHERE clubid IN (SELECT clubid FROM seed_club);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
)
DELETE FROM wedstrijd
WHERE teamid IN (SELECT teamid FROM seed_teams);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
)
DELETE FROM training
WHERE teamid IN (SELECT teamid FROM seed_teams);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
),
seed_teams AS (
    SELECT teamid FROM team WHERE clubid IN (SELECT clubid FROM seed_club)
)
DELETE FROM speler
WHERE teamid IN (SELECT teamid FROM seed_teams);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
)
DELETE FROM team
WHERE clubid IN (SELECT clubid FROM seed_club);

WITH seed_club AS (
    SELECT clubid FROM club WHERE naam = 'KFC Beersel-Drogenbos'
)
DELETE FROM terrein
WHERE clubid IN (SELECT clubid FROM seed_club);

DELETE FROM club
WHERE naam = 'KFC Beersel-Drogenbos';

DELETE FROM gebruiker
WHERE email LIKE '%@coachassist.test';

-- 2. Club en terreinen.
INSERT INTO club (naam, stad)
VALUES ('KFC Beersel-Drogenbos', 'Beersel');

INSERT INTO terrein (naam, adres, clubid)
SELECT v.naam, v.adres, c.clubid
FROM club c
CROSS JOIN (
    VALUES
        ('Terrein A', 'Lotsesteenweg 64, 1650 Beersel'),
        ('Terrein B', 'Lotsesteenweg 64, 1650 Beersel')
) AS v(naam, adres)
WHERE c.naam = 'KFC Beersel-Drogenbos';

-- 3. Gebruikers: admin, coaches en een selectie spelers met login.
INSERT INTO gebruiker (email, passwordhash, rol, voornaam, achternaam)
VALUES
    ('admin@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Admin'::gebruikersrol, 'Sofie', 'Vermeiren'),
    ('coach.u13@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Coach'::gebruikersrol, 'Thomas', 'De Smet'),
    ('coach.u15@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Coach'::gebruikersrol, 'Karim', 'Benali'),
    ('coach.u17@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Coach'::gebruikersrol, 'Jeroen', 'Peeters'),
    ('coach.u21@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Coach'::gebruikersrol, 'Niels', 'Van den Broeck'),
    ('coach.eerste@coachassist.test', 'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==', 'Coach'::gebruikersrol, 'Ruben', 'Claes');

WITH teams AS (
    SELECT *
    FROM (VALUES
        ('U13', 'u13'),
        ('U15', 'u15'),
        ('U17', 'u17'),
        ('U21', 'u21'),
        ('Eerste ploeg', 'eerste')
    ) AS t(teamnaam, slug)
),
login_spelers AS (
    SELECT *
    FROM (VALUES
        ('U13', 1, 'Milan', 'Verhaert'),
        ('U13', 2, 'Noah', 'Jacobs'),
        ('U13', 3, 'Luca', 'Peeters'),
        ('U13', 4, 'Mathis', 'De Wolf'),
        ('U13', 5, 'Finn', 'Maes'),
        ('U15', 1, 'Senne', 'Goossens'),
        ('U15', 2, 'Tibo', 'Willems'),
        ('U15', 3, 'Yanis', 'Bakkali'),
        ('U15', 4, 'Arthur', 'Lenaerts'),
        ('U15', 5, 'Adam', 'El Idrissi'),
        ('U17', 1, 'Rayan', 'Messaoudi'),
        ('U17', 2, 'Jules', 'Vercammen'),
        ('U17', 3, 'Mats', 'Van Acker'),
        ('U17', 4, 'Lennert', 'Cools'),
        ('U17', 5, 'Ilyas', 'Bouazza'),
        ('U21', 1, 'Kobe', 'Vranckx'),
        ('U21', 2, 'Niels', 'Segers'),
        ('U21', 3, 'Bram', 'Michiels'),
        ('U21', 4, 'Samir', 'Ait Omar'),
        ('U21', 5, 'Jarne', 'De Ridder'),
        ('Eerste ploeg', 1, 'Wout', 'Vandeputte'),
        ('Eerste ploeg', 2, 'Glenn', 'Dierckx'),
        ('Eerste ploeg', 3, 'Olivier', 'Hermans'),
        ('Eerste ploeg', 4, 'Mehdi', 'Rahmani'),
        ('Eerste ploeg', 5, 'Arne', 'Van Damme')
    ) AS s(teamnaam, nr, voornaam, achternaam)
)
INSERT INTO gebruiker (email, passwordhash, rol, voornaam, achternaam)
SELECT
    'speler.' || teams.slug || '.' || lpad(login_spelers.nr::text, 2, '0') || '@coachassist.test',
    'AQAAAAIAAYagAAAAEBEyU3SVttf4GTpbfJ2+3yC0TZS7jK49nHiW6lzebUaL4wCRQYxB4L8NRfep+IJs4A==',
    'Speler'::gebruikersrol,
    login_spelers.voornaam,
    login_spelers.achternaam
FROM teams
JOIN login_spelers ON login_spelers.teamnaam = teams.teamnaam;

-- 4. Teams koppelen aan coaches.
INSERT INTO team (naam, seizoen, clubid, coachid)
SELECT v.teamnaam, '2025-2026', c.clubid, g.gebruikerid
FROM club c
JOIN (
    VALUES
        ('U13', 'coach.u13@coachassist.test'),
        ('U15', 'coach.u15@coachassist.test'),
        ('U17', 'coach.u17@coachassist.test'),
        ('U21', 'coach.u21@coachassist.test'),
        ('Eerste ploeg', 'coach.eerste@coachassist.test')
) AS v(teamnaam, coach_email) ON true
JOIN gebruiker g ON g.email = v.coach_email
WHERE c.naam = 'KFC Beersel-Drogenbos';

-- 5. Spelers: 16 spelers per team.
WITH speler_basis AS (
    SELECT *
    FROM (VALUES
        ('U13', 1, 'Milan', 'Verhaert'),
        ('U13', 2, 'Noah', 'Jacobs'),
        ('U13', 3, 'Luca', 'Peeters'),
        ('U13', 4, 'Mathis', 'De Wolf'),
        ('U13', 5, 'Finn', 'Maes'),
        ('U13', 6, 'Remi', 'Smet'),
        ('U13', 7, 'Tuur', 'Wouters'),
        ('U13', 8, 'Lou', 'Geerts'),
        ('U13', 9, 'Vince', 'De Backer'),
        ('U13', 10, 'Emiel', 'Dumont'),
        ('U13', 11, 'Nolan', 'Verbruggen'),
        ('U13', 12, 'Daan', 'Coppens'),
        ('U13', 13, 'Ferre', 'Lauwers'),
        ('U13', 14, 'Jasper', 'Moens'),
        ('U13', 15, 'Bas', 'Cornelis'),
        ('U13', 16, 'Ties', 'Renard'),
        ('U15', 1, 'Senne', 'Goossens'),
        ('U15', 2, 'Tibo', 'Willems'),
        ('U15', 3, 'Yanis', 'Bakkali'),
        ('U15', 4, 'Arthur', 'Lenaerts'),
        ('U15', 5, 'Adam', 'El Idrissi'),
        ('U15', 6, 'Louis', 'Martens'),
        ('U15', 7, 'Ruben', 'Vanden Eynde'),
        ('U15', 8, 'Kasper', 'Smets'),
        ('U15', 9, 'Nassim', 'El Amrani'),
        ('U15', 10, 'Stan', 'Bogaerts'),
        ('U15', 11, 'Vic', 'De Clercq'),
        ('U15', 12, 'Mauro', 'Palmers'),
        ('U15', 13, 'Elias', 'Hendrickx'),
        ('U15', 14, 'Jef', 'Baert'),
        ('U15', 15, 'Amin', 'Haddadi'),
        ('U15', 16, 'Robin', 'Vermeulen'),
        ('U17', 1, 'Rayan', 'Messaoudi'),
        ('U17', 2, 'Jules', 'Vercammen'),
        ('U17', 3, 'Mats', 'Van Acker'),
        ('U17', 4, 'Lennert', 'Cools'),
        ('U17', 5, 'Ilyas', 'Bouazza'),
        ('U17', 6, 'Seppe', 'Vandenberghe'),
        ('U17', 7, 'Kyan', 'Nguyen'),
        ('U17', 8, 'Dries', 'Lievens'),
        ('U17', 9, 'Nabil', 'Chakir'),
        ('U17', 10, 'Tuur', 'De Meyer'),
        ('U17', 11, 'Sander', 'Volders'),
        ('U17', 12, 'Quinten', 'Pauwels'),
        ('U17', 13, 'Raf', 'Roosen'),
        ('U17', 14, 'Amine', 'Khalfi'),
        ('U17', 15, 'Lars', 'Van Hoof'),
        ('U17', 16, 'Xander', 'Willemsen'),
        ('U21', 1, 'Kobe', 'Vranckx'),
        ('U21', 2, 'Niels', 'Segers'),
        ('U21', 3, 'Bram', 'Michiels'),
        ('U21', 4, 'Samir', 'Ait Omar'),
        ('U21', 5, 'Jarne', 'De Ridder'),
        ('U21', 6, 'Mauro', 'De Wilde'),
        ('U21', 7, 'Gilles', 'Maertens'),
        ('U21', 8, 'Imran', 'El Kaddouri'),
        ('U21', 9, 'Rik', 'Verstraeten'),
        ('U21', 10, 'Tobias', 'Van Loon'),
        ('U21', 11, 'Aaron', 'Boon'),
        ('U21', 12, 'Simon', 'Hermans'),
        ('U21', 13, 'Jonas', 'Van Camp'),
        ('U21', 14, 'Younes', 'Saidi'),
        ('U21', 15, 'Kjell', 'Peeters'),
        ('U21', 16, 'Liam', 'Devos'),
        ('Eerste ploeg', 1, 'Wout', 'Vandeputte'),
        ('Eerste ploeg', 2, 'Glenn', 'Dierckx'),
        ('Eerste ploeg', 3, 'Olivier', 'Hermans'),
        ('Eerste ploeg', 4, 'Mehdi', 'Rahmani'),
        ('Eerste ploeg', 5, 'Arne', 'Van Damme'),
        ('Eerste ploeg', 6, 'Kevin', 'Janssens'),
        ('Eerste ploeg', 7, 'Dylan', 'Buelens'),
        ('Eerste ploeg', 8, 'Zakaria', 'Ben Moussa'),
        ('Eerste ploeg', 9, 'Gert', 'Pieters'),
        ('Eerste ploeg', 10, 'Jelle', 'Verlinden'),
        ('Eerste ploeg', 11, 'Mounir', 'Tazi'),
        ('Eerste ploeg', 12, 'Tom', 'De Pauw'),
        ('Eerste ploeg', 13, 'Bjorn', 'Vansina'),
        ('Eerste ploeg', 14, 'Nico', 'Vercruysse'),
        ('Eerste ploeg', 15, 'Sami', 'Bennani'),
        ('Eerste ploeg', 16, 'Joachim', 'Lambrechts')
    ) AS s(teamnaam, nr, voornaam, achternaam)
),
teams AS (
    SELECT t.teamid, t.naam, lower(replace(t.naam, ' ', '')) AS slug
    FROM team t
    JOIN club c ON c.clubid = t.clubid
    WHERE c.naam = 'KFC Beersel-Drogenbos'
)
INSERT INTO speler (voornaam, achternaam, rugnummer, vastepositie, teamid, gebruikerid)
SELECT
    s.voornaam,
    s.achternaam,
    s.nr,
    CASE
        WHEN s.nr = 1 THEN 'Keeper'::vaste_positie
        WHEN s.nr BETWEEN 2 AND 6 THEN 'Verdediger'::vaste_positie
        WHEN s.nr BETWEEN 7 AND 11 THEN 'Middenvelder'::vaste_positie
        ELSE 'Aanvaller'::vaste_positie
    END,
    teams.teamid,
    g.gebruikerid
FROM teams
JOIN speler_basis s ON s.teamnaam = teams.naam
LEFT JOIN gebruiker g
    ON g.email = 'speler.' ||
        CASE teams.naam
            WHEN 'Eerste ploeg' THEN 'eerste'
            ELSE lower(teams.naam)
        END ||
        '.' || lpad(s.nr::text, 2, '0') || '@coachassist.test'
    AND s.nr <= 5;

-- 6. Club- en teamaanvragen voor realistische statusdata.
INSERT INTO clubaanvraag (gebruikerid, clubid, status, datumaanvraag)
SELECT g.gebruikerid, c.clubid, 'goedgekeurd'::aanvraagstatus, DATE '2025-08-01'
FROM gebruiker g
CROSS JOIN club c
WHERE c.naam = 'KFC Beersel-Drogenbos'
  AND g.email LIKE '%@coachassist.test';

INSERT INTO teamaanvraag (spelerid, teamid, status, datumaanvraag)
SELECT s.spelerid, s.teamid, 'goedgekeurd'::aanvraagstatus, DATE '2025-08-15'
FROM speler s
JOIN team t ON t.teamid = s.teamid
JOIN club c ON c.clubid = t.clubid
WHERE c.naam = 'KFC Beersel-Drogenbos';

-- 7. Trainingen: veel verleden, enkele recente en toekomstige trainingen.
WITH team_data AS (
    SELECT
        t.teamid,
        t.naam,
        row_number() OVER (ORDER BY t.teamid) - 1 AS team_offset
    FROM team t
    JOIN club c ON c.clubid = t.clubid
    WHERE c.naam = 'KFC Beersel-Drogenbos'
),
training_schema AS (
    SELECT
        td.teamid,
        td.naam,
        gs.nr,
        DATE '2026-02-03' + ((gs.nr - 1) * INTERVAL '7 days') + (td.team_offset * INTERVAL '1 day') AS datum,
        CASE WHEN td.team_offset % 2 = 0 THEN TIME '18:30' ELSE TIME '19:30' END AS starttijd,
        CASE WHEN td.team_offset % 2 = 0 THEN TIME '20:00' ELSE TIME '21:00' END AS eindtijd,
        CASE
            WHEN gs.nr IN (6, 14) THEN 'afgelast'::trainingstatus
            WHEN gs.nr IN (10) THEN 'uitgesteld'::trainingstatus
            ELSE 'gepland'::trainingstatus
        END AS status,
        CASE
            WHEN gs.nr IN (6, 14) THEN 'Training afgelast door weersomstandigheden.'
            WHEN gs.nr IN (10) THEN 'Training verplaatst naar later in de week.'
            ELSE NULL
        END AS opmerking
    FROM team_data td
    CROSS JOIN generate_series(1, 18) AS gs(nr)
)
INSERT INTO training (datum, starttijd, eindtijd, teamid, terreinid, status, opmerking)
SELECT
    ts.datum::date,
    ts.starttijd,
    ts.eindtijd,
    ts.teamid,
    tr.terreinid,
    ts.status,
    ts.opmerking
FROM training_schema ts
JOIN team t ON t.teamid = ts.teamid
JOIN club c ON c.clubid = t.clubid
JOIN terrein tr
    ON tr.clubid = c.clubid
   AND tr.naam = CASE WHEN ts.nr % 2 = 0 THEN 'Terrein B' ELSE 'Terrein A' END
WHERE c.naam = 'KFC Beersel-Drogenbos';

-- 8. Aanwezigheden voor trainingen tot en met 25 mei 2026.
INSERT INTO trainingaanwezigheid (trainingid, spelerid, aanwezig, reden)
SELECT
    tr.trainingid,
    s.spelerid,
    CASE
        WHEN (s.rugnummer + extract(day from tr.datum)::int) % 9 = 0 THEN false
        ELSE true
    END,
    CASE
        WHEN (s.rugnummer + extract(day from tr.datum)::int) % 9 = 0 THEN 'School, blessure of familieafspraak'
        ELSE NULL
    END
FROM training tr
JOIN team t ON t.teamid = tr.teamid
JOIN club c ON c.clubid = t.clubid
JOIN speler s ON s.teamid = t.teamid
WHERE c.naam = 'KFC Beersel-Drogenbos'
  AND tr.datum <= DATE '2026-05-25'
  AND tr.status = 'gepland'::trainingstatus;

-- 9. Wedstrijden: verleden, heden en toekomst.
WITH team_data AS (
    SELECT
        t.teamid,
        t.naam,
        row_number() OVER (ORDER BY t.teamid) - 1 AS team_offset
    FROM team t
    JOIN club c ON c.clubid = t.clubid
    WHERE c.naam = 'KFC Beersel-Drogenbos'
),
wedstrijd_schema AS (
    SELECT
        td.teamid,
        td.naam,
        gs.nr,
        DATE '2026-03-01' + ((gs.nr - 1) * INTERVAL '14 days') + (td.team_offset * INTERVAL '1 day') AS datum,
        CASE WHEN td.team_offset % 2 = 0 THEN TIME '10:30' ELSE TIME '15:00' END AS starttijd,
        (ARRAY[
            'KHO Huizingen',
            'FC Dworp',
            'VK Linkebeek',
            'SK Leeuw',
            'RSC Anderlecht Girls/Youth',
            'Tempo Overijse',
            'KV Zuun',
            'FC Pepingen',
            'Olympia Wijgmaal',
            'KFC Strombeek'
        ])[gs.nr] AS tegenstander,
        CASE
            WHEN gs.nr <= 6 THEN 'gespeeld'::wedstrijdstatus
            WHEN gs.nr = 7 THEN 'uitgesteld'::wedstrijdstatus
            WHEN gs.nr = 8 THEN 'geannuleerd'::wedstrijdstatus
            ELSE 'gepland'::wedstrijdstatus
        END AS status,
        CASE WHEN gs.nr <= 6 THEN ((gs.nr + td.team_offset) % 5) ELSE NULL END AS thuisscore,
        CASE WHEN gs.nr <= 6 THEN ((gs.nr + 2 + td.team_offset) % 4) ELSE NULL END AS uitscore,
        CASE
            WHEN gs.nr = 7 THEN 'Wedstrijd uitgesteld door onbeschikbaar terrein.'
            WHEN gs.nr = 8 THEN 'Tegenstander gaf forfait.'
            ELSE NULL
        END AS opmerking
    FROM team_data td
    CROSS JOIN generate_series(1, 10) AS gs(nr)
)
INSERT INTO wedstrijd (datum, starttijd, tegenstander, thuisscore, uitscore, teamid, terreinid, status, opmerking)
SELECT
    ws.datum::date,
    ws.starttijd,
    ws.tegenstander,
    ws.thuisscore,
    ws.uitscore,
    ws.teamid,
    tr.terreinid,
    ws.status,
    ws.opmerking
FROM wedstrijd_schema ws
JOIN team t ON t.teamid = ws.teamid
JOIN club c ON c.clubid = t.clubid
JOIN terrein tr
    ON tr.clubid = c.clubid
   AND tr.naam = CASE WHEN ws.nr % 2 = 0 THEN 'Terrein B' ELSE 'Terrein A' END
WHERE c.naam = 'KFC Beersel-Drogenbos';

-- 10. Statistieken voor gespeelde wedstrijden.
INSERT INTO spelerstatistiek (spelerid, wedstrijdid, goals, assists, gelekaarten, rodekaarten, gespeeldeminuten)
SELECT
    s.spelerid,
    w.wedstrijdid,
    CASE
        WHEN s.vastepositie = 'Aanvaller'::vaste_positie AND s.rugnummer IN (12, 14) THEN (w.wedstrijdid + s.rugnummer) % 3
        WHEN s.vastepositie = 'Middenvelder'::vaste_positie AND s.rugnummer = 10 THEN (w.wedstrijdid + s.rugnummer) % 2
        ELSE 0
    END,
    CASE
        WHEN s.vastepositie IN ('Middenvelder'::vaste_positie, 'Aanvaller'::vaste_positie) THEN (w.wedstrijdid + s.rugnummer) % 2
        ELSE 0
    END,
    CASE WHEN s.rugnummer IN (4, 6, 8) AND w.wedstrijdid % 3 = 0 THEN 1 ELSE 0 END,
    CASE WHEN s.rugnummer = 5 AND w.wedstrijdid % 11 = 0 THEN 1 ELSE 0 END,
    CASE
        WHEN s.rugnummer <= 11 THEN 90
        WHEN s.rugnummer IN (12, 13, 14) THEN 25 + ((w.wedstrijdid + s.rugnummer) % 30)
        ELSE 0
    END
FROM wedstrijd w
JOIN team t ON t.teamid = w.teamid
JOIN club c ON c.clubid = t.clubid
JOIN speler s ON s.teamid = t.teamid
WHERE c.naam = 'KFC Beersel-Drogenbos'
  AND w.status = 'gespeeld'::wedstrijdstatus
  AND s.rugnummer <= 14;

-- 11. Opstellingen voor gespeelde wedstrijden en eerstvolgende geplande wedstrijden.
WITH gekozen_wedstrijden AS (
    SELECT wedstrijdid, teamid, row_number() OVER (PARTITION BY teamid ORDER BY datum) AS rn
    FROM wedstrijd
    WHERE status IN ('gespeeld'::wedstrijdstatus, 'gepland'::wedstrijdstatus)
),
inserted AS (
    INSERT INTO opstelling (wedstrijdid, formatie)
    SELECT wedstrijdid, CASE WHEN rn % 2 = 0 THEN '4-4-2' ELSE '4-3-3' END
    FROM gekozen_wedstrijden
    WHERE rn <= 5
    RETURNING opstellingid, wedstrijdid
),
basis_posities AS (
    SELECT *
    FROM (VALUES
        (1, 'Keeper'::opstelling_positie, 50, 6),
        (2, 'Rechtsback'::opstelling_positie, 82, 25),
        (3, 'Centrale verdediger'::opstelling_positie, 62, 22),
        (4, 'Centrale verdediger'::opstelling_positie, 38, 22),
        (5, 'Linksback'::opstelling_positie, 18, 25),
        (6, 'Verdedigende middenvelder'::opstelling_positie, 50, 43),
        (7, 'Centrale middenvelder'::opstelling_positie, 35, 55),
        (8, 'Centrale middenvelder'::opstelling_positie, 65, 55),
        (9, 'Rechtsbuiten'::opstelling_positie, 82, 75),
        (10, 'Spits'::opstelling_positie, 50, 82),
        (11, 'Linksbuiten'::opstelling_positie, 18, 75)
    ) AS p(rugnummer, positie, x, y)
)
INSERT INTO opstellingspeler (opstellingid, spelerid, positienaam, veldpositiex, veldpositiey)
SELECT i.opstellingid, s.spelerid, p.positie, p.x, p.y
FROM inserted i
JOIN wedstrijd w ON w.wedstrijdid = i.wedstrijdid
JOIN speler s ON s.teamid = w.teamid
JOIN basis_posities p ON p.rugnummer = s.rugnummer;

COMMIT;

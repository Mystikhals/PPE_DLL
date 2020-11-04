delimiter ;

DROP TABLE IF EXISTS Pilote;
CREATE TABLE Pilote
( brevet VARCHAR(6),nom VARCHAR(20), nbHVol INT, comp VARCHAR(4),
CONSTRAINT pk_Pilote PRIMARY KEY(brevet))ENGINE=INNODB;

INSERT INTO Pilote VALUES ('PL-1', 'Gilles Laborde', 2450, 'AF');
INSERT INTO Pilote VALUES ('PL-2', 'Frederic D''Almeyda', 900, 'AF');
INSERT INTO Pilote VALUES ('PL-3', 'Florence Perissel', 1000, 'SING');
INSERT INTO Pilote VALUES ('PL-4', 'Sonia Dietrich', 2450, 'SING');
INSERT INTO Pilote VALUES ('PL-5', 'Christine Royo', 200, 'AF');
INSERT INTO Pilote VALUES ('PL-6', 'Aurelia Ente', 2450, 'SING');

SELECT * FROM Pilote;

delimiter $

-- ****************************
-- Procédure SANS paramètres
-- Compte le nombre de pilotes
-- ****************************
DROP PROCEDURE IF EXISTS ComptePilotes$
CREATE PROCEDURE ComptePilotes() 
BEGIN
	DECLARE nb INT;
	SELECT COUNT(*)
	INTO nb
	FROM Pilote;
	
	SELECT nb;
END;
$

delimiter ;

-- APPEL
CALL ComptePilotes();

delimiter $

-- ********************************************
-- Procédure AVEC un paramètre
-- Extrait tous les pilotes pour une compagnie
-- ********************************************
DROP PROCEDURE IF EXISTS SelPilote$
CREATE PROCEDURE SelPilote(IN pcomp VARCHAR(4)) 
BEGIN
	DECLARE nb SMALLINT;

	-- a. Nombre de pilotes
	SELECT COUNT(*) 
	INTO nb 
	FROM Pilote
	WHERE comp = pcomp;

	-- b. Si AUCUN ou PLUSIEURS
	IF (nb = 0) THEN 
		SELECT 'AUCUN';
	ELSE
		-- c. Recherche des informations du pilote
		SELECT brevet, nom, nbHVol
		FROM Pilote
		WHERE comp = pcomp;
	END IF;
END;
$

delimiter ;

-- APPEL
CALL SelPilote('AF');


delimiter $

-- **************************************************************
-- Procédure avec DEUX paramètres
-- Pilotes d'une compagnie passée en paramètre
-- dont le nombre d'heures de vol >= au nombre passé en paramètre
-- *************************************************************** 
DROP PROCEDURE IF EXISTS PlusNbHeures$
CREATE PROCEDURE PlusNbHeures(IN pcomp VARCHAR(4), IN pheures INT) 
BEGIN
	SELECT nom, nbHVol
	FROM Pilote
	WHERE comp = pcomp
	AND nbHVol >= pheures;
END;
$

delimiter ;

-- APPEL
SET @vs_compa = 'AF';
SET @vs_heures = 500;
CALL PlusNbHeures(@vs_compa, @vs_heures);

delimiter $

-- ****************************************
-- Augmentation de 10 heures de vol
-- pour tous les pilotes dont la compagnie
-- est passée en paramètre.
-- ****************************************
SELECT * FROM Pilote$
DROP PROCEDURE IF EXISTS MajPilote$
CREATE PROCEDURE MajPilote(IN pcomp VARCHAR(4)) 
BEGIN
	UPDATE Pilote
	SET nbHVol = nbHVol + 10
	WHERE comp = pcomp;
END;
$

delimiter ;

-- APPEL
CALL MajPilote('AF');
SELECT * FROM Pilote;

delimiter $

-- ***************************************************************
-- Procédure étudiée en cours, avec la compagnie pour paramètre.
-- Recherche le pilote ayant le plus grand nombre d'heures de vol.
-- Si PLUSIEURS : affichage du message "PLUSIEURS".
-- Si AUCUN : affichage du message "AUCUN"
-- ****************************************************************
DROP PROCEDURE IF EXISTS PlusExperimente$
CREATE PROCEDURE PlusExperimente(IN pcomp VARCHAR(4)) 
BEGIN
	DECLARE nb SMALLINT;
	DECLARE nomPil VARCHAR(20);

	-- a. Nombre de pilotes
	SELECT  COUNT(*) 
	INTO nb 
	FROM Pilote
	WHERE nbHVol = (SELECT MAX(nbHVol) FROM Pilote WHERE comp = pcomp)
	AND comp = pcomp;

	-- b. Si AUCUN ou PLUSIEURS
	IF (nb = 0) THEN 
		SET nomPil := 'AUCUN';
	ELSEIF nb > 1 THEN
		SET nomPil := 'PLUSIEURS';
	ELSE
		-- c. Recherche des informations du pilote
		SELECT nom 
		INTO nomPil 
		FROM Pilote
		WHERE nbHVol =  (SELECT MAX(nbHVol) FROM Pilote WHERE comp = pcomp)
		AND comp = pcomp;
	END IF;

	-- Valeur de retour
	SELECT nomPil;
END;
$

delimiter ;

-- APPEL
SET @vs_compa = 'AF';
CALL PlusExperimente(@vs_compa);

delimiter $

-- *********************************************************
-- Procédure étudiée en cours. Paramètres :
-- 			En ENTREE : compagnie
--          En SORTIE : nom du pilote et le nombre d'heures
-- **********************************************************
DROP PROCEDURE IF EXISTS PlusExperimenteOUT$
CREATE PROCEDURE PlusExperimenteOUT(IN pcomp VARCHAR(4), OUT pnomPil VARCHAR(20), OUT pheuresVol INT) 
BEGIN
	DECLARE nb SMALLINT;

	-- a. Nombre de pilotes
	SELECT COUNT(*)
	INTO nb
	FROM Pilote
	WHERE nbHVol =  (SELECT MAX(nbHVol) FROM Pilote WHERE comp = pcomp)
	AND comp = pcomp;

	-- b. Si AUCUN ou PLUSIEURS
	IF (nb = 0) THEN 
		SET pnomPil := 'AUCUN';
		SET pheuresVol := 0;
	ELSEIF nb > 1 THEN
		SET pnomPil := 'PLUSIEURS';
		SET pheuresVol := 0;
	ELSE
		-- c. Recherche des informations du pilote
		SELECT nom, nbHVol 
		INTO pnomPil, pheuresVol 
		FROM Pilote
		WHERE nbHVol = (SELECT MAX(nbHVol) FROM Pilote WHERE comp = pcomp)
		AND comp = pcomp;
	END IF;
END;
$

delimiter ;

-- APPEL
SET @vs_compa = 'AF';
SET @vs_nompil = '';
SET @vs_heures = '';

CALL PlusExperimenteOUT(@vs_compa,@vs_nompil,@vs_heures);
SELECT  @vs_compa,@vs_nompil,@vs_heures;
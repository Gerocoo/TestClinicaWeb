CREATE DATABASE TestClinica;
GO
USE TestClinica;
GO

CREATE TABLE PartiCorpo (
    Id INT IDENTITY PRIMARY KEY, 
    Nome VARCHAR(50) NOT NULL
);

CREATE TABLE Ambulatori (
    Id INT IDENTITY PRIMARY KEY, 
    Nome VARCHAR(50) NOT NULL
);

CREATE TABLE Esami (
    Id INT IDENTITY PRIMARY KEY,
    CodiceMinisteriale VARCHAR(10),
    CodiceInterno VARCHAR(10),
    DescrizioneEsame VARCHAR(100),
    ParteCorpoId INT FOREIGN KEY REFERENCES PartiCorpo(Id)
);

CREATE TABLE EsamiAmbulatori (
    EsameId INT FOREIGN KEY REFERENCES Esami(Id),
    AmbulatorioId INT FOREIGN KEY REFERENCES Ambulatori(Id),
    PRIMARY KEY (EsameId, AmbulatorioId)
);

INSERT INTO PartiCorpo (Nome) VALUES ('Testa'), ('Arti superiori'), ('Addome'), ('Torace');
INSERT INTO Ambulatori (Nome) VALUES ('Radiologia'), ('Tac1'), ('Risonanza'), ('EcografiaPrivitera'), ('EcografiaMassimino');

-- Inserimento Esame 1: Eco Addome (Addome = ID 3)
INSERT INTO Esami (CodiceMinisteriale, CodiceInterno, DescrizioneEsame, ParteCorpoId) VALUES ('M123', 'I123', 'Eco Addome', 3);
INSERT INTO EsamiAmbulatori (EsameId, AmbulatorioId) VALUES (1, 4), (1, 5);

-- Inserimento Esame 2: RMN cranio (Testa = ID 1)
INSERT INTO Esami (CodiceMinisteriale, CodiceInterno, DescrizioneEsame, ParteCorpoId) VALUES ('M124', 'I124', 'RMN cranio', 1);
INSERT INTO EsamiAmbulatori (EsameId, AmbulatorioId) VALUES (2, 3);

-- Inserimento Esame 3: RX mano Dx (Arti superiori = ID 2)
INSERT INTO Esami (CodiceMinisteriale, CodiceInterno, DescrizioneEsame, ParteCorpoId) VALUES ('M125', 'I125', 'RX mano Dx', 2);
INSERT INTO EsamiAmbulatori (EsameId, AmbulatorioId) VALUES (3, 1);
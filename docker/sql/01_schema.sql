CREATE DATABASE ResearchAssistant;
GO

USE ResearchAssistant;
GO

CREATE TABLE Directors (
                           Id INT PRIMARY KEY IDENTITY(1,1),
                           Name NVARCHAR(100) NOT NULL,
                           Nationality NVARCHAR(50),
                           BirthYear INT
);

CREATE TABLE Actors (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(100) NOT NULL,
                        Nationality NVARCHAR(50),
                        BirthYear INT
);

CREATE TABLE Genres (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Movies (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Title NVARCHAR(200) NOT NULL,
                        Year INT NOT NULL,
                        Rating DECIMAL(3,1),
                        RuntimeMinutes INT,
                        Plot NVARCHAR(MAX)
);

CREATE TABLE MovieDirectors (
                                MovieId INT FOREIGN KEY REFERENCES Movies(Id),
                                DirectorId INT FOREIGN KEY REFERENCES Directors(Id),
                                PRIMARY KEY (MovieId, DirectorId)
);

CREATE TABLE MovieActors (
                             MovieId INT FOREIGN KEY REFERENCES Movies(Id),
                             ActorId INT FOREIGN KEY REFERENCES Actors(Id),
                             PRIMARY KEY (MovieId, ActorId)
);

CREATE TABLE MovieGenres (
                             MovieId INT FOREIGN KEY REFERENCES Movies(Id),
                             GenreId INT FOREIGN KEY REFERENCES Genres(Id),
                             PRIMARY KEY (MovieId, GenreId)
);

CREATE TABLE Reviews (
                         Id INT PRIMARY KEY IDENTITY(1,1),
                         MovieId INT FOREIGN KEY REFERENCES Movies(Id),
                         ReviewText NVARCHAR(MAX) NOT NULL,
                         Score DECIMAL(3,1)
);
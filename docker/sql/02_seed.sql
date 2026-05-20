USE ResearchAssistant;
GO

-- Genres
INSERT INTO Genres (Name) VALUES
('Action'), ('Drama'), ('Sci-Fi'), ('Thriller'), ('Comedy'),
('Crime'), ('Adventure'), ('Animation'), ('Horror'), ('Romance');

-- Directors
INSERT INTO Directors (Name, Nationality, BirthYear) VALUES
                                                         ('Christopher Nolan', 'British', 1970),
                                                         ('Steven Spielberg', 'American', 1946),
                                                         ('Martin Scorsese', 'American', 1942),
                                                         ('Ridley Scott', 'British', 1937),
                                                         ('James Cameron', 'Canadian', 1954),
                                                         ('Quentin Tarantino', 'American', 1963),
                                                         ('David Fincher', 'American', 1962),
                                                         ('Denis Villeneuve', 'Canadian', 1967);

-- Actors
INSERT INTO Actors (Name, Nationality, BirthYear) VALUES
                                                      ('Leonardo DiCaprio', 'American', 1974),
                                                      ('Matt Damon', 'American', 1970),
                                                      ('Cillian Murphy', 'Irish', 1976),
                                                      ('Tom Hanks', 'American', 1956),
                                                      ('Sigourney Weaver', 'American', 1949),
                                                      ('Brad Pitt', 'American', 1963),
                                                      ('Morgan Freeman', 'American', 1937),
                                                      ('Jodie Foster', 'American', 1962),
                                                      ('Timothée Chalamet', 'American', 1995),
                                                      ('Zendaya', 'American', 1996),
                                                      ('Edward Norton', 'American', 1969),
                                                      ('Kate Winslet', 'British', 1975),
                                                      ('Christian Bale', 'British', 1974),
                                                      ('Heath Ledger', 'Australian', 1979),
                                                      ('Matthew McConaughey', 'American', 1969);

-- Movies
INSERT INTO Movies (Title, Year, Rating, RuntimeMinutes, Plot) VALUES
                                                                   ('Inception', 2010, 8.8, 148, 'A thief who steals corporate secrets through dream-sharing technology is given the task of planting an idea into the mind of a CEO.'),
                                                                   ('Interstellar', 2014, 8.6, 169, 'A team of explorers travel through a wormhole in space in an attempt to ensure humanitys survival on a new planet.'),
                                                                   ('Oppenheimer', 2023, 8.9, 180, 'The story of American scientist J. Robert Oppenheimer and his role in the development of the atomic bomb during World War II.'),
                                                                   ('The Dark Knight', 2008, 9.0, 152, 'Batman faces the Joker, a criminal mastermind who seeks to create chaos in Gotham City.'),
                                                                   ('Schindlers List', 1993, 9.0, 195, 'In German-occupied Poland during World War II, Oskar Schindler gradually becomes concerned for his Jewish workforce.'),
                                                                   ('Saving Private Ryan', 1998, 8.6, 169, 'Following the Normandy landings, a group of US soldiers go behind enemy lines to retrieve a paratrooper whose brothers have been killed in action.'),
                                                                   ('Alien', 1979, 8.4, 117, 'The crew of a commercial spacecraft encounter a deadly extraterrestrial creature after investigating a mysterious transmission.'),
                                                                   ('Gladiator', 2000, 8.5, 155, 'A former Roman general sets out to exact vengeance against the corrupt emperor who murdered his family.'),
                                                                   ('Titanic', 1997, 7.9, 194, 'A seventeen-year-old aristocrat falls in love with a kind but poor artist aboard the luxurious, ill-fated R.M.S. Titanic.'),
                                                                   ('The Departed', 2006, 8.5, 151, 'An undercover cop and a mole in the police attempt to identify each other while both report to their respective bosses.'),
                                                                   ('Pulp Fiction', 1994, 8.9, 154, 'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.'),
                                                                   ('Se7en', 1995, 8.6, 127, 'Two detectives hunt a serial killer who uses the seven deadly sins as his motives.'),
                                                                   ('The Silence of the Lambs', 1991, 8.6, 118, 'A young FBI cadet seeks the help of an imprisoned cannibal killer to catch another serial killer.'),
                                                                   ('Dune', 2021, 8.0, 155, 'A noble family becomes embroiled in a war for control over the galaxy most valuable asset while its heir becomes troubled by visions of a dark future.'),
                                                                   ('Dune Part Two', 2024, 8.5, 166, 'Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.');

-- MovieDirectors
INSERT INTO MovieDirectors (MovieId, DirectorId) VALUES
                                                     (1, 1), (2, 1), (3, 1), (4, 1),
                                                     (5, 2), (6, 2),
                                                     (7, 4), (8, 4),
                                                     (9, 5),
                                                     (10, 3),
                                                     (11, 6),
                                                     (12, 7), (13, 7),
                                                     (14, 8), (15, 8);

-- MovieActors
INSERT INTO MovieActors (MovieId, ActorId) VALUES
                                               (1, 1), (1, 13),
                                               (2, 15), (2, 13),
                                               (3, 3),
                                               (4, 13), (4, 14),
                                               (5, 6),
                                               (6, 2), (6, 4),
                                               (7, 5),
                                               (8, 6),
                                               (9, 1), (9, 12),
                                               (10, 1), (10, 6), (10, 7),
                                               (11, 6),
                                               (12, 6), (12, 7),
                                               (13, 8),
                                               (14, 9), (14, 10),
                                               (15, 9), (15, 10);

-- MovieGenres
INSERT INTO MovieGenres (MovieId, GenreId) VALUES
                                               (1, 3), (1, 4), (1, 1),
                                               (2, 3), (2, 4), (2, 7),
                                               (3, 2), (3, 4),
                                               (4, 1), (4, 6), (4, 4),
                                               (5, 2),
                                               (6, 2), (6, 1),
                                               (7, 3), (7, 9),
                                               (8, 1), (8, 2), (8, 7),
                                               (9, 2), (9, 10),
                                               (10, 6), (10, 4), (10, 2),
                                               (11, 6), (11, 4),
                                               (12, 6), (12, 4), (12, 9),
                                               (13, 9), (13, 4), (13, 6),
                                               (14, 3), (14, 7), (14, 2),
                                               (15, 3), (15, 7), (15, 2);

-- Reviews
INSERT INTO Reviews (MovieId, ReviewText, Score) VALUES
                                                     (1, 'A mind-bending masterpiece that challenges the boundaries of reality. Nolan at his absolute best.', 9.5),
                                                     (1, 'Visually stunning with a complex narrative that rewards repeat viewings. A modern classic.', 8.5),
                                                     (2, 'An emotional and visually spectacular journey through space. Hans Zimmers score elevates every scene.', 9.0),
                                                     (2, 'Ambitious and thought-provoking but slightly overlong. Still one of the best sci-fi films in years.', 7.5),
                                                     (3, 'A towering achievement in cinema. Cillian Murphy delivers a career-defining performance.', 9.5),
                                                     (3, 'Dense and demanding but ultimately rewarding. A film that stays with you long after the credits roll.', 8.5),
                                                     (4, 'Heath Ledgers Joker is one of the greatest villain performances in cinema history. A near perfect film.', 9.8),
                                                     (4, 'Redefines what a superhero film can be. Dark, complex and utterly gripping from start to finish.', 9.0),
                                                     (5, 'One of the most powerful films ever made. Spielberg at his most restrained and effective.', 9.5),
                                                     (6, 'The opening sequence alone is worth the price of admission. A brutal and moving war epic.', 8.5),
                                                     (7, 'Still terrifying after all these years. Ridley Scott creates unbearable tension in every scene.', 8.8),
                                                     (8, 'A spectacular epic with a career-best performance from Russell Crowe.', 8.0),
                                                     (9, 'A timeless romance wrapped around a spectacular disaster. DiCaprio and Winslet are magnetic together.', 8.0),
                                                     (10, 'Scorseses best film in decades. An intricate crime thriller with outstanding performances throughout.', 9.0),
                                                     (11, 'Revolutionary and endlessly quotable. Tarantino rewrote the rules of storytelling with this film.', 9.2),
                                                     (12, 'Dark, stylish and deeply unsettling. Fincher crafts a thriller that gets under your skin.', 9.0),
                                                     (13, 'One of the finest thrillers ever made. Jodie Foster and Anthony Hopkins are unforgettable.', 9.3),
                                                     (14, 'A stunning visual achievement with a rich and complex story. Villeneuve proves himself a master.', 8.5),
                                                     (15, 'An even better sequel. Zendaya and Chalamet have incredible chemistry. Epic cinema at its finest.', 9.0);
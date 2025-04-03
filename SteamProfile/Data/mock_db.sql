-- BEFORE CREATING AN ACCOUNT
INSERT INTO Users (email, username, hashed_password, developer, last_login) VALUES
	('alice@example.com', 'AliceGamer', 'hashed_password_1', 1, '2025-03-20 14:25:00'),
	('bob@example.com', 'BobTheBuilder', 'hashed_password_2', 0, '2025-03-21 10:12:00'),
	('charlie@example.com', 'CharlieX', 'hashed_password_3', 0, '2025-03-22 18:45:00'),
	('diana@example.com', 'DianaRocks', 'hashed_password_4', 0, '2025-03-19 22:30:00'),
	('eve@example.com', 'Eve99', 'hashed_password_5', 1, '2025-03-23 08:05:00'),
	('frank@example.com', 'FrankTheTank', 'hashed_password_6', 0, '2025-03-24 16:20:00'),
	('grace@example.com', 'GraceSpeed', 'hashed_password_7', 0, '2025-03-25 11:40:00'),
	('harry@example.com', 'HarryWizard', 'hashed_password_8', 0, '2025-03-20 20:15:00'),
	('ivy@example.com', 'IvyNinja', 'hashed_password_9', 0, '2025-03-22 09:30:00'),
	('jack@example.com', 'JackHacks', 'hashed_password_10', 1, '2025-03-24 23:55:00');


INSERT INTO Wallet (user_id, points, money_for_games) VALUES
	(1, 10, 200),
	(2, 10, 200),
	(3, 10, 200),
	(4, 10, 200),
	(5, 10, 200),
	(6, 10, 200),
	(7, 10, 200),
	(8, 10, 200),
	(9, 10, 200),
	(10, 10, 200);

INSERT INTO UserProfiles (user_id, profile_picture, bio, last_modified)
VALUES 
    (1, 'ms-appx:///Assets\Collections\image.jpg', 'Gaming enthusiast and software developer', GETDATE()),
    (2, 'ms-appx:///Assets\download.jpg', 'Game developer and tech lover', GETDATE()),
    (3, 'ms-appx:///Assets\download.jpg', 'Casual gamer and streamer',�GETDATE()),
	(4, 'ms-appx:///Assets\Collections\image.jpg', 'Casual gamer and streamer',�GETDATE()),
	(5, 'ms-appx:///Assets\download.jpg', 'Casual gamer and streamer',�GETDATE()),
	(6, 'ms-appx:///Assets\default_picture.jpg', 'Casual gamer and streamer',�GETDATE()),
	(7, 'ms-appx:///Assets\default_picture.jpg', 'Casual gamer and streamer',�GETDATE()),
	(8, 'ms-appx:///Assets\default_picture.jpg', 'Casual gamer and streamer',�GETDATE()),
	(9, 'ms-appx:///Assets\default_picture.jpg', 'Casual gamer and streamer',�GETDATE()),
	(10, 'ms-appx:///Assets\default_picture.jpg', 'Casual gamer and streamer',�GETDATE());

INSERT INTO Collections (user_id, name, cover_picture, is_public, created_at)
VALUES
	(1, 'All Owned Games', '/Assets/Collections/allgames.jpg', 1, '2022-02-21'),
	(2, 'Shooters', '/Assets/Collections/shooters.jpg', 1, '2025-03-21'),
	(1, 'Sports', '/Assets/Collections/sports.jpg', 1, '2023-03-21'),
	(1, 'Chill Games', '/Assets/Collections/chill.jpg', 1, '2024-03-21'),
	(2, 'Pets', '/Assets/Collections/pets.jpg', 0, '2025-01-21'),
	(1, 'X-Mas', '/Assets/Collections/xmas.jpg', 0, '2025-02-21');

INSERT INTO Feature_User (user_id, feature_id, equipped) VALUES
	(5, 1, 1),
	(5, 5, 0),
	(5, 2, 0);

INSERT INTO Achievements (achievement_name, description, achievement_type, points) 
VALUES
	('FRIENDSHIP1', 'You made a friend, you get a point', 'Friendships', 1),
	('FRIENDSHIP2', 'You made 5 friends, you get 3 points', 'Friendships', 3),
	('FRIENDSHIP3', 'You made 10 friends, you get 5 points', 'Friendships', 5),
	('FRIENDSHIP4', 'You made 50 friends, you get 10 points', 'Friendships', 10),
	('FRIENDSHIP5', 'You made 100 friends, you get 15 points', 'Friendships', 15),
	('OWNEDGAMES1', 'You own 1 game, you get 1 point', 'Owned Games', 1),
	('OWNEDGAMES2', 'You own 5 games, you get 3 points', 'Owned Games', 3),
	('OWNEDGAMES3', 'You own 10 games, you get 5 points', 'Owned Games', 5),
	('OWNEDGAMES4', 'You own 50 games, you get 10 points', 'Owned Games', 10),
	('SOLDGAMES1', 'You sold 1 game, you get 1 point', 'Sold Games', 1),
	('SOLDGAMES2', 'You sold 5 games, you get 3 points', 'Sold Games', 3),
	('SOLDGAMES3', 'You sold 10 games, you get 5 points', 'Sold Games', 5),
	('SOLDGAMES4', 'You sold 50 games, you get 10 points', 'Sold Games', 10),
	('REVIEW1', 'You gave 1 review, you get 1 point', 'Number of Reviews Given', 1),
	('REVIEW2', 'You gave 5 reviews, you get 3 points', 'Number of Reviews Given', 3),
	('REVIEW3', 'You gave 10 reviews, you get 5 points', 'Number of Reviews Given', 5),
	('REVIEW4', 'You gave 50 reviews, you get 10 points', 'Number of Reviews Given', 10),
	('REVIEWR1', 'You got 1 review, you get 1 point', 'Number of Reviews Received', 1),
	('REVIEWR2', 'You got 5 reviews, you get 3 points', 'Number of Reviews Received', 3),
	('REVIEWR3', 'You got 10 reviews, you get 5 points', 'Number of Reviews Received', 5),
	('REVIEWR4', 'You got 50 reviews, you get 10 points', 'Number of Reviews Received', 10),
	('DEVELOPER', 'You are a developer, you get 10 points', 'Developer', 10),
	('ACTIVITY1', 'You have been active for 1 year, you get 1 point', 'Years of Activity', 1),
	('ACTIVITY2', 'You have been active for 2 years, you get 3 points', 'Years of Activity', 3),
	('ACTIVITY3', 'You have been active for 3 years, you get 5 points', 'Years of Activity', 5),
	('ACTIVITY4', 'You have been active for 4 years, you get 10 points', 'Years of Activity', 10),
	('POSTS1', 'You have made 1 post, you get 1 point', 'Number of Posts', 1),
	('POSTS2', 'You have made 5 posts, you get 3 points', 'Number of Posts', 3),
	('POSTS3', 'You have made 10 posts, you get 5 points', 'Number of Posts', 5),
	('POSTS4', 'You have made 50 posts, you get 10 points', 'Number of Posts', 10)
	;

update Achievements 
set icon_url = 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'
where achievement_id > 0


-- AFTER CREATING AN ACCOUNT

INSERT INTO Collections (user_id, name, cover_picture, is_public, created_at)
VALUES
	(11, 'All Owned Games', '/Assets/Collections/allgames.jpg', 1, '2022-02-21'),
	(11, 'Shooters', '/Assets/Collections/shooters.jpg', 1, '2025-03-21'),
	(11, 'Sports', '/Assets/Collections/sports.jpg', 1, '2023-03-21'),
	(11, 'Chill Games', '/Assets/Collections/chill.jpg', 1, '2024-03-21'),
	(11, 'Pets', '/Assets/Collections/pets.jpg', 0, '2025-01-21'),
	(11, 'X-Mas', '/Assets/Collections/xmas.jpg', 0, '2025-02-21');


-- SHOOTERS (game_id 1�3)
INSERT INTO OwnedGames (user_id, title, description, cover_picture)
VALUES
	(11, 'Call of Duty: MWIII', 'First-person military shooter', '/Assets/Games/codmw3.png'),
	(11, 'Overwatch 2', 'Team-based hero shooter', '/Assets/Games/overwatch2.png'),
	(11, 'Counter-Strike 2', 'Tactical shooter', '/Assets/Games/cs2.png');

-- SPORTS (game_id 4�6)
INSERT INTO OwnedGames (user_id, title, description, cover_picture)
VALUES
	(11, 'FIFA 25', 'Football simulation', '/Assets/Games/fifa25.png'),
	(11, 'NBA 2K25', 'Basketball simulation', '/Assets/Games/nba2k25.png'),
	(11, 'Tony Hawk Pro Skater', 'Skateboarding sports game', '/Assets/Games/thps.png');

-- CHILL (game_id 7)
INSERT INTO OwnedGames (user_id, title, description, cover_picture)
VALUES
	(11, 'Stardew Valley', 'Relaxing farming game', '/Assets/Games/stardewvalley.png');

-- PETS (game_id 8�10)
INSERT INTO OwnedGames (user_id, title, description, cover_picture)
VALUES
	(11, 'The Sims 4: Cats & Dogs', 'Life sim with pets', '/Assets/Games/sims4pets.png'),
	(11, 'Nintendogs', 'Pet care simulation', '/Assets/Games/nintendogs.png'),
	(11, 'Pet Hotel', 'Manage a hotel for pets', '/Assets/Games/pethotel.png');

-- X-Mas (game_id 11)
INSERT INTO OwnedGames (user_id, title, description, cover_picture)
VALUES
	(11, 'Christmas Wonderland', 'Festive hidden object game', '/Assets/Games/xmas.png');

select * from OwnedGames;

select * from users;


INSERT INTO Feature_User (user_id, feature_id, equipped) VALUES
	(11, 1, 0),
	(11, 5, 0),
	(11,2,0);

insert into UserAchievements (user_id, achievement_id, unlocked_at) values
	(11, 1, GETDATE()),
	(11, 6, GETDATE());


-- AFTER CREATING MORE ACCOUNTS 
INSERT INTO Friendships (user_id, friend_id)
VALUES 
    (11, 12),
	(12, 11),
    (11, 13),
	(13,11);
GO

insert into UserAchievements (user_id, achievement_id, unlocked_at) values
	(12, 1, GETDATE()),
	(12, 6, GETDATE()),
	(12, 14, GETDATE());

INSERT INTO Feature_User (user_id, feature_id, equipped) VALUES
	(12, 1, 0),
	(12, 5, 0),
	(12, 2, 0);

select * from Users;
select * from Friendships;
select * from�UserProfiles;
select * from Wallet;
select * from Achievements;
select  * from UserAchievements;
select * from Features;
select * from Feature_User;

delete from users

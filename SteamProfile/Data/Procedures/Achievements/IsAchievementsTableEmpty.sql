CREATE PROCEDURE IsAchievementsTableEmpty
AS
BEGIN
	SELECT COUNT(1) FROM Achievements
END
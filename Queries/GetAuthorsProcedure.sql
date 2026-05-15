CREATE PROCEDURE GetAuthors
AS 
BEGIN 
   SELECT Id, Name, Rating, Bio, Email, NULL AS NumBooks FROM Authors
END;
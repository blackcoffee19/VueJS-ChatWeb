USE [ChatDB]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_AddFriends 
	@ReqId INT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @NewGroupID INT
    DECLARE @RequesterId INT
	DECLARE @AssigneeId INT
	DECLARE @Code VARCHAR(200)
	UPDATE Requirements SET Status = 0, Type = 2, ModifiedDate = GETDATE() WHERE RId = @ReqId
	SET @RequesterId = (SELECT RequesterId FROM Requirements WHERE RId = @ReqId)
	SET @AssigneeId = (SELECT AssigneeId FROM Requirements WHERE RId = @ReqId)
	DECLARE @Check1 INT
	SET @Check1 = (SELECT COUNT(*) 
					FROM Relationships 
					WHERE ((User_1_Id = @RequesterId AND User_2_Id = @AssigneeId ) OR 
							(User_2_Id = @RequesterId AND User_1_Id = @AssigneeId )) AND isnull(UserDeleted,0)=0 AND Status = 1)
	IF(@Check1 =0)
	BEGIN
		SET @Code = 'XG' + CAST((SELECT COUNT(*) FROM GroupChatTb) + 1 AS NVARCHAR)

		INSERT INTO GroupChatTb (Code, UserCreated, CreatedDate) VALUES (@Code, @RequesterId, GETDATE())
		SET @NewGroupID = SCOPE_IDENTITY();
	
		INSERT INTO MemeberGroupTb (GroupId, UserId, CreatedDate) VALUES (@NewGroupID, @RequesterId, GETDATE()),(@NewGroupID, @AssigneeId, GETDATE())
		INSERT INTO Relationships (Type,User_1_Id, User_2_Id, Counting,Status,UserCreated ,CreatedDate)VALUES (1, @RequesterId, @AssigneeId, 0,1 ,@RequesterId, GETDATE())
	END
END
GO

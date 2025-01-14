USE [ChatDB]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_Unfriend
	@Userid int,
	@UserReq int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @GroupID INT
	SET @GroupID = (SELECT GrId FROM GroupChatTb g 
								LEFT JOIN MemeberGroupTb m ON m.GroupId= g.GrId 
								LEFT JOIN MemeberGroupTb m2 ON m2.GroupId= g.GrId 
								WHERE m.UserId = @Userid AND m2.UserId=@UserReq AND isnull(g.UserDeleted,0)=0 ) 
	IF(ISNULL(@GroupID,0) != 0)
	BEGIN
		UPDATE Relationships SET Status=0, UserDeleted = @UserReq, DeletedDate = GETDATE() WHERE (User_1_Id = @Userid AND User_2_Id = @UserReq ) OR (User_1_Id = @UserReq AND User_2_Id = @Userid)

		UPDATE MemeberGroupTb SET UserDeleted = @UserReq, DeletedDate =GETDATE() WHERE GroupId = @GroupID
		UPDATE GroupChatTb SET UserDeleted = @UserReq, DeletedDate = GETDATE() WHERE GrId = @GroupID
    END
END
GO

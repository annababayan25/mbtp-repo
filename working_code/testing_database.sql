ALTER PROCEDURE dbo.RetrieveBlackoutState
    @StartDate DATE,
    @EndDate   DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        b.PCID,
        pc.Description AS ProfitCenterName,
        b.StartDate,
        b.EndDate,
        b.Reason
    FROM BlackoutDate b
    INNER JOIN ProfitCenter pc 
        ON b.PCID = pc.PCID
    WHERE 
        b.StartDate <= @EndDate
        AND b.EndDate >= @StartDate
    ORDER BY 
        b.PCID, b.StartDate;
END

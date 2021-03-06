
DECLARE @it INT = 2001;
DECLARE @part INT;
DECLARE @centreRnd FLOAT;
DECLARE @lungRnd FLOAT;
DECLARE @heartRand FLOAT;
DECLARE @respRand FLOAT;


while @it <= 4000
BEGIN
	SET @part = 76
	WHILE @part <= 306
	BEGIN
		SET @centreRnd = RAND();
		SET @lungRnd = RAND();
		SET @heartRand = RAND();
		SET @respRand = RAND();
	   INSERT INTO [dabtrial_com_participantdata].[dbo].[Strata]
           ([Iteration]
           ,[ParticipantNo]
           ,[StudyCentreId]
           ,[RespSupportTypeID]
           ,[HasChronicLungDisease]
           ,[HasCyanoticHeartDisease])
     VALUES
           (@it,
            @part,
            CASE
				WHEN @centreRnd<0.6667 THEN 1
				WHEN @centreRnd<0.8267 THEN 2
				WHEN @centreRnd<0.92 THEN 3
				ELSE 4
			END,
           CASE
				WHEN @respRand<0.0133 THEN 2
				WHEN @respRand<0.6267 THEN 3
				WHEN @respRand<0.9333 THEN 4
				ELSE 5
			END
           ,CASE
				WHEN @lungRnd>0.92 THEN 1 ELSE 0
			END
           ,CASE
				WHEN @heartRand>0.9867 THEN 1 ELSE 0
			END)
	   SET @part = @part + 1;
	END
	SET @it = @it + 1;
	CHECKPOINT
END;
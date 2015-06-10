USE SenderSheduller;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Split') AND type = N'IF')
	DROP FUNCTION Split
GO

CREATE FUNCTION Split ( 
	@StringToSplit varchar(max),
	@Separator varchar(max))
RETURNS TABLE 
AS 
	RETURN WITH indices AS
	( 
		SELECT CAST(0 AS BIGINT) S, CAST(1 AS BIGINT) E
		UNION ALL
		SELECT E, CHARINDEX(@Separator, @StringToSplit, E) + LEN(@Separator) 
		FROM indices
		WHERE E > S AND @StringToSplit<>'' AND @StringToSplit<>@Separator
	)
	SELECT 
		SUBSTRING(@StringToSplit,S, 
		CASE WHEN E > LEN(@Separator) THEN E-S-LEN(@Separator) ELSE LEN(@StringToSplit) - S + 1 END) String,
		S StartIndex        
	FROM indices
	WHERE S >0
GO

IF OBJECT_ID (N'GetStringHash', N'FN') IS NOT NULL
	DROP FUNCTION SaveUser;
GO

CREATE FUNCTION GetStringHash (@SourceString NVARCHAR(MAX))
RETURNS NVARCHAR(32)
AS
BEGIN
	RETURN SUBSTRING(master.dbo.fn_varbintohexstr(HASHBYTES('MD5', @SourceString)),3,32);
END;

GO

IF OBJECT_ID (N'SaveUser', N'P') IS NOT NULL
	DROP PROCEDURE SaveUser;

GO

CREATE PROCEDURE SaveUser
(
	@Login NVARCHAR(200),
	@Password NVARCHAR(MAX),
	@IsEnabled BIT,
	@CallbackServiceUrl NVARCHAR(MAX) = NULL
)
AS
BEGIN
	DECLARE @UserId UNIQUEIDENTIFIER;
	SELECT @UserId = Id FROM dbo.Users WHERE Login = @Login
	IF (@UserId IS NULL)
	BEGIN
		SET @UserId = NEWID();
		INSERT INTO dbo.Users
			    (Id, [Login], PassHash, IsEnabled, CallbackServiceUrl )
		VALUES  ( 
			      @UserId, -- UNIQUEIDENTIFIER,
			      @Login, -- Login - nvarchar(200)
				  dbo.GetStringHash(@Password), -- PassHash - nvarchar(200)
			      @IsEnabled,  -- IsEnabled - bit
			      @CallbackServiceUrl
	        );
	END
	ELSE
	BEGIN
		UPDATE dbo.Users 
			SET PassHash = dbo.GetStringHash(@Password), IsEnabled = @IsEnabled, CallbackServiceUrl = @CallbackServiceUrl
				WHERE Id = @UserId;
	END
	SELECT @UserId;
END;

GO

IF OBJECT_ID (N'GetUser', N'P') IS NOT NULL
	DROP PROCEDURE GetUser;

GO

CREATE PROCEDURE GetUser 
(
	@Login NVARCHAR(200),
	@Password NVARCHAR(MAX)
)
AS
BEGIN
	SELECT Id AS UserId FROM dbo.Users WITH(NOLOCK) 
		WHERE Login = @Login AND PassHash = dbo.GetStringHash(@Password) AND IsEnabled = 1;
END;

GO

IF OBJECT_ID (N'SetProviderMetrics', N'P') IS NOT NULL
	DROP PROCEDURE SetProviderMetrics;

GO

CREATE PROCEDURE SetProviderMetrics(
	@ProviderId UNIQUEIDENTIFIER,
	@QualityMetrics REAL,
	@ChannelBrandwidth REAL
	)
AS
BEGIN
	UPDATE dbo.Providers 
		SET QualityMetrics = @QualityMetrics, ChannelBrandwidth = @ChannelBrandwidth 
			WHERE Id = @ProviderId; 
END;

GO

IF OBJECT_ID (N'GetProviders', N'P') IS NOT NULL
	DROP PROCEDURE GetProviders;

GO
	
CREATE PROCEDURE GetProviders(@UserId UNIQUEIDENTIFIER = NULL)
AS
BEGIN
	SELECT PV.Id, PV.Name FROM dbo.Providers PV WITH(NOLOCK)
		LEFT JOIN dbo.SenderNames SN WITH(NOLOCK) 
			ON PV.Id = SN.ProviderId
		 WHERE @UserId IS NULL OR SN.UserId = @UserId
		GROUP BY PV.Id, PV.Name
END;

GO

IF OBJECT_ID (N'GetProvider', N'P') IS NOT NULL
	DROP PROCEDURE GetProvider;

GO

CREATE PROCEDURE GetProvider(@ProviderId UNIQUEIDENTIFIER)
AS
BEGIN
	SELECT * FROM dbo.Providers WITH(NOLOCK)
	WHERE Id = @ProviderId
END;

GO

IF OBJECT_ID (N'CreateSenderName', N'P') IS NOT NULL
	DROP PROCEDURE CreateSenderName;

GO

CREATE PROCEDURE CreateSenderName(
	@UserId UNIQUEIDENTIFIER,
	@ProviderId UNIQUEIDENTIFIER,
	@SenderName NVARCHAR(200))
AS
BEGIN
	DECLARE @SenderNameId UNIQUEIDENTIFIER;
	DECLARE @UserId2 UNIQUEIDENTIFIER;
	SELECT @SenderNameId = Id, @UserId2 = UserId FROM dbo.SenderNames WITH(NOLOCK) WHERE Name = @SenderName AND ProviderId = @ProviderId
	IF (@UserId2 IS NOT NULL AND @UserId2<>@UserId)
	BEGIN
		SET @SenderNameId = NULL;
	END
	ELSE IF (@SenderNameId IS NULL)
	BEGIN
		SET @SenderNameId = NEWID();
		INSERT INTO dbo.SenderNames
			    ( Id ,
				  UserId ,
				  ProviderId ,
			      Name
				)
		VALUES  ( @SenderNameId , -- Id - uniqueidentifier
				  @UserId , -- UserId - uniqueidentifier
				  @ProviderId , -- ProviderId - uniqueidentifier
				  @SenderName -- Name - nvarchar(200)
				);
	END;
	SELECT @SenderNameId;
END;

GO

IF OBJECT_ID (N'DisableSenderName', N'P') IS NOT NULL
	DROP PROCEDURE DisableSenderName;

GO

CREATE PROCEDURE DisableSenderName(@SenderNameId UNIQUEIDENTIFIER)
AS
BEGIN
	UPDATE dbo.SenderNames SET IsEnabled = 0 WHERE Id = @SenderNameId;
END;

GO

IF OBJECT_ID (N'GetSenderNames', N'P') IS NOT NULL
	DROP PROCEDURE GetSenderNames;

GO

CREATE PROCEDURE GetSenderNames(@UserId UNIQUEIDENTIFIER)
AS
BEGIN
	SELECT * FROM dbo.SenderNames WITH(NOLOCK) 
		WHERE UserId = @UserId AND IsEnabled = 1
END;

GO

IF OBJECT_ID (N'EnqueueSMS', N'P') IS NOT NULL
	DROP PROCEDURE EnqueueSMS;

GO

CREATE PROCEDURE EnqueueSMS(
	@UserId UNIQUEIDENTIFIER,
	@SenderNameId UNIQUEIDENTIFIER,
	@ProviderId UNIQUEIDENTIFIER,
	@ClientId NVARCHAR(200) = NULL,
	@DistributionId NVARCHAR(200) = NULL,
	@ExtSMSId NVARCHAR(200) = NULL,
	@sequence_number SMALLINT,
	@service_type VARCHAR(6) = NULL,
	@source_addr_ton SMALLINT = NULL,
	@source_addr_npi SMALLINT = NULL,
	@source_addr VARCHAR(21) = NULL,
	@number_of_dests SMALLINT = 1,
	@esm_class SMALLINT,
    @protocol_id SMALLINT,
	@priority_flag SMALLINT,
    @schedule_delivery_time VARCHAR(17) = NULL,
    @validity_period VARCHAR(17) = NULL,
	@registered_delivery SMALLINT = 1,
	@replace_if_present_flag SMALLINT = 1,
	@data_coding SMALLINT = 0,
	@sm_default_msg_id SMALLINT = NULL,
	@sm_length SMALLINT,
	@short_message VARCHAR(508),
	@TLVTable XML,
	@Destanations XML,
	@CustomParameters NVARCHAR(MAX) = NULL
	)
AS
BEGIN
	DECLARE @Id UNIQUEIDENTIFIER;
	BEGIN TRAN
	SET @Id = NEWID();
	INSERT INTO dbo.SMSQueue
	        ( Id ,
	          UserId ,
	          SenderNameId ,
	          ProviderId ,
	          ClientId ,
	          DistributionId ,
	          ExtSMSId ,
	          sequence_number ,
	          service_type ,
	          source_addr_ton ,
	          source_addr_npi ,
	          source_addr ,
	          number_of_dests ,
	          esm_class ,
	          protocol_id ,
	          priority_flag ,
	          schedule_delivery_time ,
	          validity_period ,
	          registered_delivery ,
	          replace_if_present_flag ,
	          data_coding ,
	          sm_default_msg_id ,
	          sm_length ,
	          short_message
	        )
	VALUES  ( @Id , -- Id - uniqueidentifier
	          @UserId , -- UserId - uniqueidentifier
	          @SenderNameId , -- SenderNameId - uniqueidentifier
	          @ProviderId , -- ProviderId - uniqueidentifier
	          @ClientId , -- ClientId - nvarchar(200)
	          @DistributionId , -- DistributionId - nvarchar(200)
	          @ExtSMSId , -- ExtSMSId - nvarchar(200)
	          @sequence_number, -- sequence_number - int
	          @service_type , -- service_type - varchar(6)
	          @source_addr_ton , -- source_addr_ton - smallint
	          @source_addr_npi , -- source_addr_npi - smallint
	          @source_addr , -- source_addr - smallint
	          @number_of_dests , -- number_of_dests - smallint
	          @esm_class , -- esm_class - smallint
	          @protocol_id , -- protocol_id - smallint
	          @priority_flag , -- priority_flag - smallint
	          @schedule_delivery_time , -- schedule_delivery_time - varchar(17)
	          @validity_period , -- validity_period - varchar(17)
	          @registered_delivery , -- registered_delivery - smallint
	          @replace_if_present_flag , -- replace_if_present_flag - smallint
	          @data_coding , -- data_coding - smallint
	          @sm_default_msg_id , -- sm_default_msg_id - smallint
	          @sm_length , -- sm_length - smallint
	          @short_message  -- short_message - varchar(254)
	        );
	   INSERT INTO dbo.SMSTLV
	          (SmsId, Tag, Value )
	   SELECT
			@Id,
			TLV.Item.value('./*[local-name()=''Key''][1]','SMALLINT') AS Tag,
			TLV.Item.value('./*[local-name()=''Value''][1]','VARCHAR(508)') AS Value
	   FROM @TLVTable.nodes('//*[local-name()=''KeyValueOfanyTypeanyType'']') AS TLV(Item);
		
	   INSERT INTO dbo.DestinationMap
	          ( SMSId ,
	            dest_addr_ton ,
	            dest_addr_npi ,
	            destination_addr ,
	            IsDistributionList
	          )
	   SELECT
	        @Id,	
			Adressses.Item.value('./*[local-name()=''DestinationAddressTon''][1]','SMALLINT') AS Ton,
			Adressses.Item.value('./*[local-name()=''DestinationAddressNpi''][1]','SMALLINT') AS Npi,
			Adressses.Item.value('./*[local-name()=''DestAddress''][1]','VARCHAR(21)') AS Destination,
			Adressses.Item.value('if (./*[local-name()=''IsDistributionList''][1] = ''true'') then 1 else 0','BIT') AS IsDistributionList
	   FROM @Destanations.nodes('//*[local-name()=''DestinationAddress'']') AS Adressses(Item);	  
	   
	   IF (@CustomParameters IS NOT NULL)
	   BEGIN
			INSERT INTO CustomFields
			(SmsId, [Key], Value)
				select 
				@Id,
					(select top 1 String from dbo.Split(String, '=') order by StartIndex), 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc)
				from
					(select String from dbo.Split(@CustomParameters, ';')) as t1
				OPTION (MAXRECURSION 1000)
	   END
	  COMMIT TRAN;
	  SELECT @Id;
END;
GO

IF OBJECT_ID (N'GetSMSForCancel', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSForCancel;

GO

CREATE PROCEDURE GetSMSForCancel(
	@ClientId NVARCHAR(200),
	@DistributionId NVARCHAR(200) = NULL,
	@ExtSmsId NVARCHAR(200) = NULL,
	@Id UNIQUEIDENTIFIER = NULL
) AS 
BEGIN
	UPDATE dbo.SMSQueue SET Status = 6 WHERE ClientId = @ClientId AND
	(@DistributionId IS NULL OR DistributionId = @DistributionId) AND
	(@ExtSmsId IS NULL OR ExtSMSId = @ExtSmsId) AND
	(@Id IS NULL OR Id = @Id) AND
	[Status] IN (0,1); 
	
	SELECT SQ.Id, SQ.SMSId, SQ.Status, SQ.ProviderId FROM dbo.SMSQueue SQ WITH(NOLOCK) 
	WHERE SQ.ClientId = @ClientId AND
	(@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
	(@ExtSmsId IS NULL OR SQ.ExtSMSId = @ExtSmsId) AND
	(@Id IS NULL OR SQ.Id = @Id) AND
	[Status] = 6
END;

GO

IF OBJECT_ID (N'UpdateSMSStatus', N'P') IS NOT NULL
	DROP PROCEDURE UpdateSMSStatus;

GO

CREATE PROCEDURE UpdateSMSStatus(
		@Id UNIQUEIDENTIFIER,
		@SmsId NVARCHAR(200), 
		@Status SMALLINT, 
		@ProviderId UNIQUEIDENTIFIER, 
		@ProviderStatus SMALLINT
	)
AS 
BEGIN
	IF (@id IS NOT NULL)
	BEGIN
		UPDATE dbo.SMSQueue SET SMSId = ISNULL(@SmsId, SMSId), Status = ISNULL(@Status, Status), ProviderId = ISNULL(@ProviderId, ProviderId), PSStatus = ISNULL(@ProviderStatus, PSStatus)
		 WHERE Id = @Id
		SELECT Q.Id, Q.ClientId, Q.DistributionId, U.CallbackServiceUrl FROM dbo.SMSQueue Q WITH(NOLOCK)
			INNER JOIN dbo.Users U WITH(NOLOCK) ON U.Id = Q.UserId  
				WHERE Q.Id = @Id;
	END
	ELSE IF (@SmsId IS NOT NULL AND @ProviderId IS NOT NULL)
	BEGIN
		UPDATE dbo.SMSQueue SET Status = ISNULL(@Status, Status), PSStatus = ISNULL(@ProviderStatus, PSStatus)
		 WHERE SMSId = @SmsId AND ProviderId = @ProviderId;
	    SELECT Q.Id, Q.ClientId, Q.DistributionId, U.CallbackServiceUrl FROM dbo.SMSQueue Q WITH(NOLOCK)
			INNER JOIN dbo.Users U WITH(NOLOCK) ON U.Id = Q.UserId   
				WHERE Q.SMSId = @SmsId AND Q.ProviderId = @ProviderId;
	END
END;

GO

IF OBJECT_ID (N'GetSMSParams', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSParams;

GO

CREATE PROCEDURE GetSMSParams(
		@Id UNIQUEIDENTIFIER
	)
AS 
BEGIN
	SELECT [Key], Value FROM dbo.CustomFields WITH(NOLOCK) WHERE SmsId = @Id
END;

GO

IF OBJECT_ID (N'GetSMSToSend', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSToSend;

GO

CREATE PROCEDURE GetSMSToSend(
	@Count BIGINT
	)
AS
BEGIN 
	DECLARE @SendIds TABLE
	(
	   id uniqueidentifier
	);
	
	INSERT INTO @SendIds
		SELECT TOP(@Count) SQ.Id FROM dbo.SMSQueue SQ WITH(NOLOCK) 
			WHERE Status = 0;
	
	UPDATE dbo.SMSQueue SET LastSendTime = GETDATE()
		FROM @SendIds WHERE [@SendIds].id = SMSQueue.Id;
	
	SELECT * FROM dbo.SMSQueue WITH(NOLOCK)
		INNER JOIN @SendIds ON dbo.SMSQueue.Id = [@SendIds].id 		
		
END;

GO

IF OBJECT_ID (N'GetSMSToUpdateStatus', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSToUpdateStatus;

GO

CREATE PROCEDURE GetSMSToUpdateStatus(
	@Count BIGINT,
	@CheckPeriod DATETIME
) AS
BEGIN
	DECLARE @CheckIds TABLE
	(
	   id uniqueidentifier
	);
	
	INSERT INTO @CheckIds
		SELECT TOP(@Count) SQ.Id FROM dbo.SMSQueue SQ WITH(NOLOCK) 
			WHERE Status = 1 AND LastStatusCheck<@CheckPeriod;
			
	SELECT * FROM dbo.SMSQueue WITH(NOLOCK)
		INNER JOIN @CheckIds ON dbo.SMSQueue.Id = [@CheckIds].id 	
END;

GO

IF OBJECT_ID (N'GetSMSCountInStatus', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSCountInStatus;

GO

CREATE PROCEDURE GetSMSCountInStatus(
	@UserId UNIQUEIDENTIFIER,
	@Status SMALLINT = NULL, 
	@ClientId NVARCHAR(200) = NULL,
	@DistributionId NVARCHAR(200) = NULL,
	@StartDate DATETIME = NULL, 
	@EndDate DATETIME = NULL,
	@CustomsLimits NVARCHAR(max) = NULL)
AS
BEGIN
	SELECT COUNT(SQ.Id) AS Quantity, SQ.Status FROM dbo.SMSQueue SQ WITH(NOLOCK)
	LEFT JOIN dbo.CustomFields CF WITH(NOLOCK) ON SQ.Id = CF.SmsId
			LEFT JOIN (
				select 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex) AS [Key], 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc) AS Value
				from
					(select String from dbo.Split(@CustomsLimits, ';')) as t1
			) LIM ON LIM.[Key] = CF.[Key] AND LIM.Value = CF.Value
	WHERE SQ.UserId = @UserId AND
		  (@CustomsLimits IS NULL OR LIM.Value IS NOT NULL) AND
	      (@Status IS NULL OR SQ.Status = @Status) AND
	      (@ClientId IS NULL OR SQ.ClientId = @ClientId) AND
	      (@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
	      (@StartDate IS NULL OR SQ.EuqueueTime >= @StartDate) AND
	      (@EndDate IS NULL OR SQ.EuqueueTime <= @EndDate)
	GROUP BY SQ.Status
END;

GO

IF OBJECT_ID (N'GetSMSStatuses', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSStatuses;

GO

CREATE PROCEDURE GetSMSStatuses(
		@UserId UNIQUEIDENTIFIER, 
		@ClientId NVARCHAR(200) = NULL, 
		@DistributionId NVARCHAR(200) = NULL, 
		@ExtSmsId NVARCHAR(200) = NULL, 
		@Id UNIQUEIDENTIFIER = NULL,
		@StartDate DATETIME = NULL, 
		@EndDate DATETIME = NULL, 
		@RowsPerPage BIGINT = 20, 
		@PageNumber BIGINT = 1,
		@CustomsLimits NVARCHAR(max) = NULL
)AS
BEGIN
	IF (@RowsPerPage IS NULL)
	BEGIN
		SELECT SQ.Id, ROW_NUMBER() OVER (ORDER BY SQ.EuqueueTime DESC) AS Row, COUNT(SQ.Id) OVER () AS Records, ExtSMSId, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime FROM dbo.SMSQueue SQ WITH(NOLOCK)
		LEFT JOIN dbo.CustomFields CF WITH(NOLOCK) ON SQ.Id = CF.SmsId
			LEFT JOIN (
				select 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex) AS [Key], 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc) AS Value
				from
					(select String from dbo.Split(@CustomsLimits, ';')) as t1
			) LIM ON LIM.[Key] = CF.[Key] AND LIM.Value = CF.Value
			WHERE SQ.UserId = @UserId AND
			(@CustomsLimits IS NULL OR LIM.Value IS NOT NULL) AND
			(@ClientId IS NULL OR SQ.ClientId = @ClientId) AND
			(@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
			(@ExtSmsId IS NULL OR SQ.ExtSMSId = @ExtSmsId) AND
			(@Id IS NULL OR SQ.Id = @Id) AND
			(@StartDate IS NULL OR SQ.EuqueueTime >= @StartDate) AND
			(@EndDate IS NULL OR SQ.EuqueueTime<=@EndDate)
			GROUP BY SQ.Id, ExtSMSId, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime
			ORDER BY SQ.EuqueueTime DESC
	END
	ELSE
	BEGIN
		IF (@PageNumber IS NULL)
			SET @PageNumber = 1;
		SELECT Id, Row, Records, ExtSMSId, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime FROM
		(
			SELECT TOP(@RowsPerPage) ROW_NUMBER() OVER (ORDER BY SQ.EuqueueTime DESC) AS Row, COUNT(SQ.Id) OVER() AS Records, SQ.Id, SQ.ExtSMSId, SQ.Status,SQ.ProviderId, SQ.PSStatus, SQ.LastStatusCheck, SQ.EuqueueTime FROM dbo.SMSQueue SQ WITH(NOLOCK)
			LEFT JOIN dbo.CustomFields CF WITH(NOLOCK) ON SQ.Id = CF.SmsId
			LEFT JOIN (
				select 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex) AS [Key], 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc) AS Value
				from
					(select String from dbo.Split(@CustomsLimits, ';')) as t1
			) LIM ON LIM.[Key] = CF.[Key] AND LIM.Value = CF.Value
			WHERE SQ.UserId = @UserId AND
			(@CustomsLimits IS NULL OR LIM.Value IS NOT NULL) AND
			(@ClientId IS NULL OR SQ.ClientId = @ClientId) AND
			(@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
			(@ExtSmsId IS NULL OR SQ.ExtSMSId = @ExtSmsId) AND
			(@Id IS NULL OR SQ.Id = @Id) AND
			(@StartDate IS NULL OR SQ.EuqueueTime >= @StartDate) AND
			(@EndDate IS NULL OR SQ.EuqueueTime<=@EndDate)
			GROUP BY SQ.Id, ExtSMSId, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime
		) T
		WHERE T.Row>=(@PageNumber-1)*@RowsPerPage AND T.Row<=@PageNumber*@RowsPerPage
		ORDER BY T.Row DESC
	END
END;

GO

IF OBJECT_ID (N'GetSMSDetails', N'P') IS NOT NULL
	DROP PROCEDURE GetSMSDetails;

GO

CREATE PROCEDURE GetSMSDetails(
		@UserId UNIQUEIDENTIFIER, 
		@ClientId NVARCHAR(200) = NULL, 
		@DistributionId NVARCHAR(200) = NULL, 
		@ExtSmsId NVARCHAR(200) = NULL, 
		@Id UNIQUEIDENTIFIER = NULL,
		@StartDate DATETIME = NULL, 
		@EndDate DATETIME = NULL, 
		@RowsPerPage BIGINT = 20, 
		@PageNumber BIGINT = 1,
		@CustomsLimits NVARCHAR(max) = NULL
)AS
BEGIN
	IF (@RowsPerPage IS NULL)
	BEGIN
		SELECT SQ.Id, ROW_NUMBER() OVER (ORDER BY SQ.EuqueueTime DESC) AS Row, COUNT(SQ.Id) OVER() AS Records, ExtSMSId, SQ.source_addr, short_message,  destination_addr, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime, data_coding FROM dbo.SMSQueue SQ WITH(NOLOCK)
		INNER JOIN dbo.DestinationMap DM WITH(NOLOCK) ON SQ.Id = DM.SMSId
		LEFT JOIN dbo.CustomFields CF WITH(NOLOCK) ON SQ.Id = CF.SmsId
			LEFT JOIN (
				select 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex) AS [Key], 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc) AS Value
				from
					(select String from dbo.Split(@CustomsLimits, ';')) as t1
			) LIM ON LIM.[Key] = CF.[Key] AND LIM.Value = CF.Value
			WHERE SQ.UserId = @UserId AND
			(@CustomsLimits IS NULL OR LIM.Value IS NOT NULL) AND
			(@ClientId IS NULL OR SQ.ClientId = @ClientId) AND
			(@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
			(@ExtSmsId IS NULL OR SQ.ExtSMSId = @ExtSmsId) AND
			(@Id IS NULL OR SQ.Id = @Id) AND
			(@StartDate IS NULL OR SQ.EuqueueTime >= @StartDate) AND
			(@EndDate IS NULL OR SQ.EuqueueTime<=@EndDate)
			GROUP BY SQ.Id, ExtSMSId, SQ.source_addr, short_message,  destination_addr, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime, data_coding
			ORDER BY SQ.EuqueueTime DESC
				OPTION (MAXRECURSION 1000)
	END
	ELSE
	BEGIN
		IF (@PageNumber IS NULL)
			SET @PageNumber = 1;
		SELECT Id, Row, Records, ExtSMSId, source_addr, short_message, destination_addr, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime, data_coding FROM
		(
			SELECT ROW_NUMBER() OVER (ORDER BY SQ.EuqueueTime DESC) AS Row, COUNT(SQ.Id) OVER() AS Records, SQ.Id, SQ.ExtSMSId, SQ.source_addr, short_message, destination_addr, SQ.Status,SQ.ProviderId, SQ.PSStatus, SQ.LastStatusCheck, SQ.EuqueueTime, SQ.data_coding FROM dbo.SMSQueue SQ WITH(NOLOCK)
			INNER JOIN dbo.DestinationMap DM WITH(NOLOCK) ON SQ.Id = DM.SMSId
			LEFT JOIN dbo.CustomFields CF WITH(NOLOCK) ON SQ.Id = CF.SmsId
			LEFT JOIN (
				select 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex) AS [Key], 
					(select top 1 String from dbo.Split(String, '=') order by StartIndex desc) AS Value
				from
					(select String from dbo.Split(@CustomsLimits, ';')) as t1
			) LIM ON LIM.[Key] = CF.[Key] AND LIM.Value = CF.Value
			WHERE SQ.UserId = @UserId AND
			(@CustomsLimits IS NULL OR LIM.Value IS NOT NULL) AND
			(@ClientId IS NULL OR SQ.ClientId = @ClientId) AND
			(@DistributionId IS NULL OR SQ.DistributionId = @DistributionId) AND
			(@ExtSmsId IS NULL OR SQ.ExtSMSId = @ExtSmsId) AND
			(@Id IS NULL OR SQ.Id = @Id) AND
			(@StartDate IS NULL OR SQ.EuqueueTime >= @StartDate) AND
			(@EndDate IS NULL OR SQ.EuqueueTime<=@EndDate)
			GROUP BY SQ.Id, ExtSMSId, SQ.source_addr, short_message,  destination_addr, Status, ProviderId, PSStatus, LastStatusCheck, EuqueueTime, data_coding
		) T
		WHERE T.Row>(@PageNumber-1)*@RowsPerPage AND T.Row<=@PageNumber*@RowsPerPage
		ORDER BY T.Row ASC
		OPTION (MAXRECURSION 1000)
	END
END;

GO

IF (NOT EXISTS (SELECT Id FROM dbo.Users WHERE Login = 'Admin'))
BEGIN

	DECLARE @UserId UNIQUEIDENTIFIER, @ProviderId UNIQUEIDENTIFIER;

	EXEC dbo.SaveUser @Login = 'Admin', -- nvarchar(200)
		@Password = 'colonel', -- nvarchar(max)
		@IsEnabled = 1 --bit
	    

	SELECT @UserId = Id FROM dbo.Users WHERE Login = 'Admin';

	SELECT @ProviderId = Id FROM dbo.Providers WHERE Name = 'Zanzara';

	EXEC dbo.CreateSenderName @UserId = @UserId, -- uniqueidentifier
		@ProviderId = @ProviderId, -- uniqueidentifier
		@SenderName = 'Development' -- nvarchar(200)

	SELECT @ProviderId = Id FROM dbo.Providers WHERE Name = 'Zanzara_Production';
	
	EXEC dbo.CreateSenderName @UserId = @UserId, -- uniqueidentifier
		@ProviderId = @ProviderId, -- uniqueidentifier
		@SenderName = 'Development' -- nvarchar(200)
	
	SELECT @ProviderId = Id FROM dbo.Providers WHERE Name = 'Moscow';
	
	EXEC dbo.CreateSenderName @UserId = @UserId, -- uniqueidentifier
		@ProviderId = @ProviderId, -- uniqueidentifier
		@SenderName = 'Development' -- nvarchar(200)
	
END;
    

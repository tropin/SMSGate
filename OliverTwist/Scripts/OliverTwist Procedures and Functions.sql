USE [OliverTwist]

GO

IF OBJECT_ID (N'ChangeClientAccount', N'P') IS NOT NULL
	DROP PROCEDURE ChangeClientAccount;

GO

--Процедура изменения счета

CREATE PROCEDURE [dbo].[ChangeClientAccount]
    @AccountId AS BIGINT,
    @Amount DECIMAL,
	@Cost DECIMAL, 
	@MoneyVolume DECIMAL,
	@SelectedRangeId BIGINT,
	@Comment NVARCHAR(max),
	@ManagerId UNIQUEIDENTIFIER,
	@RealClientId BIGINT,
	@OperationalClientId BIGINT
    
AS
BEGIN
      DECLARE @Result BIT; 
      SET @Result = 0; --По-умаолчанию у нас "ничего не вышло"
      
      DECLARE @CostRangeId BIGINT; --NULL
      DECLARE @QuickCost DECIMAL; --NULL
      
      --Определим сработал ли какой-либо рендж в чистом виде
      SELECT @CostRangeId = CRH.VersionId, @QuickCost = CRH.Cost FROM dbo.CostRangesHistory CRH WITH(NOLOCK)
      INNER JOIN 
      ( --Берем Id последних версий диапазонов по заданному счету
		SELECT MAX(VersionId) AS VersionId FROM dbo.CostRangesHistory WITH(NOLOCK)
		WHERE AccountId = @AccountId
		GROUP BY Id
      ) VersionIds
      ON CRH.VersionId = VersionIds.VersionId
      WHERE CRH.Cost = @Cost AND CRH.Volume <= @Amount
      
      IF (@CostRangeId IS NULL) -- Если рендж не сработал, то наша цена есть "быстрая цена"
		SET @QuickCost = ISNULL(@Cost, 0);
      
      BEGIN TRAN
         DECLARE @AvailableOwnerAmount DECIMAL;
         DECLARE @AvailableClientAmount DECIMAL;
         /*Дальше будут хитрые блокировки, которые держатся до конца транзации, что объявлена выше. Тут не дается любым инструкциям из других транзакций апдетить счета
          по тем айдишникам, то мы выбрали и для таблицы "блокировок счета" блокируем всю таблицу(чтобы новые блокировки нельязя было поставить)*/
         --Доступный баланс держателя (инетересен, только если тип счета предоплатный)
         SELECT @AvailableOwnerAmount = MIN(ACC.Amount)-ISNULL(SUM(LCK.Amount),0) FROM dbo.Account ACC WITH(ROWLOCK,UPDLOCK, HOLDLOCK) LEFT JOIN
         dbo.AccountLocks LCK WITH(TABLOCK, HOLDLOCK) ON ACC.Id = LCK.AccountId INNER JOIN
         dbo.Clients C WITH(NOLOCK) ON C.AccountId = ACC.Id
         WHERE C.Id = @OperationalClientId  AND AccountType = 0 AND LCK.IsDeleted <> 1
         -- Доступный баланс клиента
         SELECT @AvailableClientAmount = ISNULL(MIN(ACC.Amount),0)-ISNULL(SUM(LCK.Amount),0) FROM dbo.Account ACC WITH(ROWLOCK, UPDLOCK, HOLDLOCK) LEFT JOIN
         dbo.AccountLocks LCK WITH(NOLOCK) ON ACC.Id = LCK.AccountId
         WHERE ACC.Id = @AccountId AND LCK.IsDeleted <> 1
         
         
         IF (@AvailableOwnerAmount IS NULL OR @AvailableOwnerAmount>=@Amount) -- Если у нас не постоплатка или просто достаточно доступных СМС
         BEGIN
			-- PRINT 'Можем начислять';
			IF (@AvailableClientAmount+@Amount<0) -- Пытаемся вернуть себе баланс засчет клиента, загоняя его в минус (не канает)
			BEGIN
				ROLLBACK TRAN
				-- PRINT 'Пытаемся вернуть себе баланс засчет клиента';
				RETURN @Result; 
			END	
			
			--Обновим счет получателя
			DECLARE @PayerHistroyId BIGINT;
			
			UPDATE Account SET  Amount = Amount + @Amount WHERE Id=@AccountId
		 
			INSERT INTO dbo.AccountHistory
			        ( Id ,
			          CostRangeId ,
			          Amount ,
			          AmountDelta ,
			          QuickCost ,
			          MoneyVolume ,
			          ManagerId ,
			          RealClientId ,
			          OperationalClientId ,
			          Comment
			        )
			SELECT Id, @CostRangeId, Amount, @Amount, @QuickCost, @MoneyVolume, @ManagerId, @RealClientId, @OperationalClientId, @Comment
			FROM dbo.Account WITH(NOLOCK) WHERE Id = @AccountId;
			
			SET @PayerHistroyId = SCOPE_IDENTITY();
			
				--Обновим счет владельца
			UPDATE Account SET Amount = Amount - @Amount
			FROM dbo.Clients C WITH(NOLOCK)
			WHERE C.Id = @OperationalClientId AND C.AccountId = Account.Id
			
			INSERT INTO dbo.AccountHistory
			        ( Id ,
			          CostRangeId ,
			          Amount ,
			          AmountDelta ,
			          QuickCost ,
			          MoneyVolume ,
			          ManagerId ,
			          RealClientId ,
			          OperationalClientId ,
			          TargetAccountId,
			          Comment
			        )
			SELECT A.Id, @CostRangeId, A.Amount, -@Amount, @QuickCost, @MoneyVolume, @ManagerId, @RealClientId, @OperationalClientId, @PayerHistroyId, @Comment
			FROM dbo.Account A WITH(NOLOCK) INNER JOIN
				 dbo.Clients C WITH(NOLOCK) ON A.Id = C.AccountId
			WHERE C.Id = @OperationalClientId
			
			SET @Result = 1;
		 END
         COMMIT TRAN;
         RETURN @Result;
END

GO

IF OBJECT_ID (N'DeleteUserInternal', N'P') IS NOT NULL
	DROP PROCEDURE DeleteUserInternal;
GO

-- Процедура удаления пользователя в подсистеме Membership

CREATE PROCEDURE DeleteUserInternal
(
	@UserId UNIQUEIDENTIFIER -- Удаляемый пользователь подсистемы Membership
)
AS BEGIN
	DECLARE @ApplicationName NVARCHAR(MAX);
	DECLARE @TablesDeleted INT;
	DECLARE @UserName NVARCHAR(MAX);
	SET @TablesDeleted = 0;
	SELECT @ApplicationName = A.ApplicationName, @UserName = U.UserName FROM dbo.aspnet_Users U WITH(NOLOCK) 
			INNER JOIN dbo.aspnet_Applications A WITH(NOLOCK)
			ON U.ApplicationId = A.ApplicationId
		 WHERE U.UserId = @UserId;
	IF (@ApplicationName IS NOT NULL)
	BEGIN
		EXEC dbo.aspnet_Users_DeleteUser @ApplicationName = @ApplicationName, -- nvarchar(256)
			@UserName = @UserName, -- nvarchar(256)
			@TablesToDeleteFrom = 15, -- int (удалять будем все)
			@NumTablesDeletedFrom = @TablesDeleted out  -- int (сколько удалилось)
	END;
	RETURN @TablesDeleted;
END

GO

IF OBJECT_ID (N'CreateClient', N'P') IS NOT NULL
	DROP PROCEDURE CreateClient;
GO
--Процедура создания нового клиента и всего ему пологающегося
CREATE PROCEDURE CreateClient
(
	@UserId UNIQUEIDENTIFIER, --Имя пользователя для которого создается клиент (если процесс упадет, пользователь будет удален со всеми потрохами)
	@OrganizationName NVARCHAR(MAX), --Имя организации
	@IsDealler BIT, --Диллером ли клиент будет?
	@OwnerClientId BIGINT = NULL, --Клиент, для которого создаваемый будет дочерним
	@InitialAccountAmount DECIMAL = NULL, --Стартовое значение СМС на счету (если нет готового шаблона счета)
	@InitialCost DECIMAL = NULL, -- Стартовая стоимость за одну СМС (если нет готового шаблона счета)
	@InitialAccountType INT = NULL, --Тип счета по-умлочанию
	@InitialAccountDebtingType INT = NULL, -- Тип списания по-умолчанию
	@TemplateAccountId BIGINT = NULL, --Счет с которого надо скопировать состояние (шаблон счета)
	@ClientStatus INT = NULL, --Статус клиента
	@FirstName NVARCHAR(MAX) = NULL, --Имя
	@MiddleName NVARCHAR(MAX) = NULL, --Отчество
	@LastName NVARCHAR(MAX) = NULL, --Фамилия
	@City NVARCHAR(MAX) = NULL, --Город
	@MobilePhone NVARCHAR(MAX) = NULL, --Номер мобильного
	@TimeZone NVARCHAR(MAX) = NULL, --Временная зона
	@Sex INT = NULL --Пол
)
AS BEGIN
	DECLARE @UseTemplateAccount BIT;
	DECLARE @TargetAccountId BIGINT;
		SET @UseTemplateAccount = NULL;
		IF (@TemplateAccountId IS NOT NULL AND EXISTS(SELECT Id FROM dbo.Account WITH(NOLOCK) WHERE Id = @TemplateAccountId AND IsDeleted = 0))
			SET @UseTemplateAccount = 1;
		ELSE IF (@InitialAccountAmount IS NOT NULL AND @InitialCost IS NOT NULL AND 
			@InitialAccountType IS NOT NULL AND 
			@InitialAccountType IN (0,1) AND
			@InitialAccountDebtingType IS NOT NULL AND
			@InitialAccountDebtingType IN (0,1)
			)     
		    SET @UseTemplateAccount = 0;
		IF (@UseTemplateAccount IS NULL)
		BEGIN
			EXEC dbo.DeleteUserInternal @UserId = @UserId -- uniqueidentifier
			RAISERROR(N'Для клиента не может быть создан счет. Передано не достаточно параметров или они некорректны', 18, 1);
			RETURN;
		END;
		IF (@OwnerClientId IS NOT NULL AND
		    (SELECT TOP 1 DeallerOfClientId FROM dbo.Clients WITH(NOLOCK) WHERE Id = @OwnerClientId AND IsDeleted = 0) IS NULL
		    )
		BEGIN
			EXEC dbo.DeleteUserInternal @UserId = @UserId -- uniqueidentifier
			RAISERROR(N'Создающий клиент должен быть диллером', 18, 1);
			RETURN;
		END;	
	BEGIN TRY
	DECLARE @NewClientId BIGINT;
	SELECT @NewClientId = ClientId FROM dbo.Users2Clients WITH(NOLOCK) WHERE UserId = @UserId;
	IF (@NewClientId IS NULL)
	BEGIN
	BEGIN TRAN CLIENT_CREATE
		DECLARE @OwnerAccountType INT;
		SELECT @OwnerAccountType = AccountType FROM dbo.Account ACC WITH(ROWLOCK, HOLDLOCK)
		INNER JOIN dbo.Clients C WITH(NOLOCK) ON C.AccountId = ACC.Id
		WHERE C.Id = @OwnerClientId
		
		IF (@OwnerAccountType = 0 AND @InitialAccountType<>0) --Предоплатники могут создавать только предоплатников
		BEGIN
			RAISERROR(N'Клиенты с предоплатным типом счета могут создавать только клиентов с предоплатным типом счета', 18, 1);
		END
		
		IF (@UseTemplateAccount = 0)
			BEGIN
				--У нас создается счет "с нуля"
				INSERT INTO dbo.Account
				        ( Amount ,
				          AccountType ,
				          DebtingType 
				        )
				VALUES  ( @InitialAccountAmount , -- Amount - decimal
				          @InitialAccountType , -- AccountType - int
				          @InitialAccountDebtingType -- DebtingType - int
				        );
			    SET @TargetAccountId = SCOPE_IDENTITY();
			    --Создаем стоимость для 1-ой СМС
			    INSERT INTO dbo.CostRanges
			            ( AccountId ,
			              Cost ,
			              Volume
			            )
			    VALUES  ( @TargetAccountId , -- AccountId - bigint
			              @InitialCost , -- Cost - decimal
			              1 -- Volume - bigint
			            );
			    --Не забываем про историю
			    INSERT INTO dbo.CostRangesHistory
			             ( Id ,
			               AccountId ,
			               Cost ,
			               Volume ,
			               Action,
			               IsDeleted
			             )
			     SELECT Id, AccountId, Cost, Volume, 0, IsDeleted FROM dbo.CostRanges WHERE Id = SCOPE_IDENTITY();
			END
		ELSE
			BEGIN
				--Используется шаблон счета
				INSERT INTO dbo.Account
				        ( Amount ,
				          AccountType ,
				          DebtingType,
				          IsDeleted 
				        )
			    SELECT Amount, AccountType, DebtingType, IsDeleted FROM dbo.Account WITH(NOLOCK) WHERE Id = @TemplateAccountId;
			    SET @TargetAccountId = SCOPE_IDENTITY();
			    INSERT INTO dbo.CostRanges
			            ( AccountId ,
			              Cost ,
			              Volume,
			              IsDeleted
			            )
			    SELECT @TargetAccountId, Cost, Volume, IsDeleted FROM dbo.CostRanges WITH(NOLOCK) WHERE AccountId = @TemplateAccountId AND IsDeleted = 0;
			END;
	    --Создаем историю счета - первую запись
	    INSERT INTO dbo.AccountOptionHistory
	            ( Id ,
	              AccountType ,
	              DebtingType ,
	              Action,
	              IsDeleted
	            )
			    SELECT Id, AccountType, DebtingType, 0, IsDeleted FROM dbo.Account WITH(NOLOCK) WHERE Id = @TargetAccountId;
	    -- Операционная история
	    INSERT INTO dbo.AccountHistory
	            ( Id,
	              CostRangeId,
	              Amount,
	              AmountDelta,
	              QuickCost,
	              MoneyVolume
	            )
	    SELECT Id, NULL, Amount, Amount, 0, 0 FROM dbo.Account WITH(NOLOCK) WHERE Id = @TargetAccountId;
		--Создаем профиль пользователя
		INSERT INTO dbo.UserProfile
		        ( UserId ,
		          FirstName ,
		          MiddleName ,
		          LastName ,
		          MobilePhone ,
		          Sex ,
		          TimeZone ,
		          City
		        )
		VALUES  ( @UserId , -- UserId - uniqueidentifier
		          @FirstName , -- FirstName - nvarchar(4000)
		          @MiddleName , -- MiddleName - nvarchar(4000)
		          @LastName , -- LastName - nvarchar(4000)
		          @MobilePhone , -- MobilePhone - nvarchar(4000)
		          @Sex , -- Sex - int
		          @TimeZone , -- TimeZone - nvarchar(4000)
		          @City  -- City - nvarchar(4000)
		        );
		--Создаем клиента
		DECLARE @DealledWithClient BIGINT;
		IF (@IsDealler = 1)
			SET @DealledWithClient = @OwnerClientId;
		INSERT INTO dbo.Clients
		        ( OrganizationName ,
		          Status ,
		          DeallerOfClientId ,
		          CreatedByClientId ,
		          AccountId
		        )
		VALUES  ( @OrganizationName , -- OrganizationName - nvarchar(4000)
		          @ClientStatus , -- Status - int
		          @DealledWithClient , -- DeallerOfClientId - bigint
		          @OwnerClientId , -- CreatedByClientId - bigint
		          @TargetAccountId -- AccountId - bigint
		        );
		SET @NewClientId =  SCOPE_IDENTITY();
		--Создаем историю клиента
		INSERT INTO dbo.ClientsHistory
		        ( Id ,
		          OrganizationName ,
		          Status ,
		          DeallerOfClientId ,
		          DateCreated ,
		          CreatedByClientId ,
		          AccountId ,
		          IsDeleted ,
		          Action
		        )
		SELECT Id, OrganizationName, Status, DeallerOfClientId, DateCreated, CreatedByClientId, AccountId, IsDeleted, 0
			FROM dbo.Clients WITH(NOLOCK) WHERE Id = @NewClientId;
	    --Создаем связку клиент - пользователь
	    INSERT INTO dbo.Users2Clients
	            ( UserId, ClientId)
	    VALUES  ( @UserId, -- UserId - uniqueidentifier
	              @NewClientId -- ClientId - bigint
	              );
	COMMIT TRAN CLIENT_CREATE;
	END;
	SELECT * FROM dbo.Clients WITH(NOLOCK) WHERE id = @NewClientId;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN CLIENT_CREATE;
		EXEC dbo.DeleteUserInternal @UserId = @UserId -- uniqueidentifier
		DECLARE @OrigError NVARCHAR(MAX);
		SET @OrigError = ERROR_MESSAGE();
	    RAISERROR(N'Невозможно создать клиента, оригинальная ошибка:  %s',18 , 1, @OrigError);
	END CATCH
END;

GO

IF OBJECT_ID (N'GetClearBallance', N'FN') IS NOT NULL
	DROP FUNCTION GetClearBallance;
GO
--Функция получения баланса клиента
CREATE FUNCTION GetClearBallance
(
	@ClientId BIGINT
)
RETURNS DECIMAL
AS 
BEGIN
	DECLARE @Result DECIMAL;
	SELECT @Result = ISNULL(MIN(ACC.Amount),0) - ISNULL(SUM(LCK.Amount),0)
	FROM dbo.Account ACC WITH(ROWLOCK, UPDLOCK, HOLDLOCK) INNER JOIN
	dbo.Clients C WITH(NOLOCK) ON C.AccountId = ACC.Id
	LEFT JOIN dbo.AccountLocks LCK WITH(ROWLOCK, UPDLOCK, HOLDLOCK) ON LCK.AccountId = ACC.Id
	WHERE C.Id = @ClientId AND LCK.IsDeleted<>1;
	RETURN @Result;
END

GO

--Получить весь граф клиентов котороые ПОД указанным (Клиенты, Клиенты-Клиентов и т.д.)
-- TODO: Когда будут графы более 100 уровней подчинения (что будет нескоро), надо будет решать проблему  OPTION (MAXRECURSION XXX)

IF OBJECT_ID (N'GetUnderControlClients', N'IF') IS NOT NULL
	DROP FUNCTION GetUnderControlClients;
GO

CREATE FUNCTION GetUnderControlClients
(
	@RootClient BIGINT
)
RETURNS TABLE
AS
RETURN
(
WITH
 Rec (id, CreatedByClientId, OrganizationName, ChildLevel)
 AS (
   SELECT id, CreatedByClientId, OrganizationName, 0 FROM dbo.Clients CL WITH(NOLOCK) WHERE CL.Id = @RootClient AND CL.IsDeleted = 0
   UNION ALL
   SELECT C.id, C.CreatedByClientId, C.OrganizationName, R.ChildLevel + 1
   FROM Rec R
   INNER JOIN dbo.Clients C WITH(NOLOCK) 
    ON C.CreatedByClientId = R.Id
    WHERE C.IsDeleted = 0
   )
 SELECT * FROM Rec
 );
 
GO

IF OBJECT_ID (N'GetOverDraftBallance', N'FN') IS NOT NULL
	DROP FUNCTION GetOverDraftBallance;
GO
--Функция получения баланса клиента
CREATE FUNCTION GetOverDraftBallance
(
	@ClientId BIGINT
)
RETURNS DECIMAL
AS 
BEGIN
	DECLARE @Result DECIMAL;
	SELECT @Result = (SELECT dbo.GetClearBallance(C.Id)-
	(
		SELECT ISNULL(SUM(dbo.GetClearBallance(UC.Id)),0) FROM GetUndercontrolClients(C.Id) UC WHERE 
		 ChildLevel>0 AND dbo.GetClearBallance(UC.Id)<0
	)
	FROM Clients C WITH(NOLOCK)
    WHERE C.Id = @ClientId)
	RETURN @Result;
END

GO

IF OBJECT_ID (N'LockSMS', N'P') IS NOT NULL
	DROP PROCEDURE LockSMS;
GO
--Процедура блокировки СМС со счета
CREATE PROCEDURE LockSMS
(
   @ClientId BIGINT,
   @Count BIGINT = 1,
   @DistributionId BIGINT = NULL,
   @ExtDistributionId NVARCHAR(300) = NULL
)
AS
BEGIN
	BEGIN TRAN
		DECLARE @LockId BIGINT, @AccountId BIGINT;
		DECLARE @CurDate DATETIME;
		DECLARE @Result BIGINT;
		DECLARE @IsPrePayed BIT;
		
		IF (@Count IS NULL)
			SET @Count = 1;
		
		SET @Result = 0;

		SET @CurDate = GETDATE();
		
		SELECT @AccountId = A.Id, @IsPrePayed = DebtingType FROM dbo.Account A WITH(NOLOCK) INNER JOIN dbo.Clients C WITH(NOLOCK)
					ON C.AccountId = A.Id 
						WHERE C.Id = @ClientId;
		
		IF ( 
				(@IsPrePayed = 1 AND dbo.GetOverDraftBallance(@ClientId)>=@Count) 
				OR @IsPrePayed = 0
		    )
		BEGIN
		
			SELECT @LockId = AL.Id, @AccountId = A.Id FROM dbo.AccountLocks AL WITH(ROWLOCK, UPDLOCK, HOLDLOCK) INNER JOIN
				dbo.Account A WITH(NOLOCK) ON A.Id = AL.AccountId INNER JOIN
				dbo.Clients C WITH(NOLOCK) ON C.AccountId = A.Id
												WHERE  C.Id = @ClientId AND AL.IsFinal=0 AND AL.IsDeleted=0 AND
												(											       
													 (
														(@DistributionId IS NULL OR AL.DistributionId = @DistributionId) AND
														(@ExtDistributionId IS NULL OR AL.ExtDistributionId = @ExtDistributionId) 
													  ) AND
													  (	
														 DAY(AL.LockDate)   = DAY(@CurDate) AND
														 MONTH(AL.LockDate) = MONTH(@CurDate) AND
														 YEAR(AL.LockDate)  = YEAR(@CurDate)
													  )
												)
			IF (@LockId IS NULL)
			BEGIN
				IF (@AccountId IS NOT NULL)
				BEGIN
					INSERT INTO dbo.AccountLocks
							( AccountId ,
							  DistributionId ,
							  Amount ,
							  IsDeleted ,
							  IsFinal ,
							  ExtDistributionId ,
							  LockDate,
							  DeleteTime
							)
					VALUES  ( @AccountId , -- AccountId - bigint
							  @DistributionId , -- DistributionId - bigint
							  @Count , -- Amount - bigint
							  0 , -- IsDeleted - bit
							  0 , -- IsFinal - bit
							  @ExtDistributionId , -- ExtDistributionId - nvarchar(300)
							  GETDATE(), -- LockDate - datetime
							  NULL -- DeleteTime - datetime
							);
					SET @LockId = SCOPE_IDENTITY();
					SELECT Id, AccountId, Amount FROM dbo.AccountLocks WITH(NOLOCK) WHERE Id = @LockId;
					SET @Result = 1;
				END;
			END
			ELSE
			BEGIN
				UPDATE dbo.AccountLocks SET Amount = Amount+@Count WHERE Id = @LockId;
				SELECT Id, AccountId, Amount FROM dbo.AccountLocks WITH(NOLOCK) WHERE Id = @LockId;
				SET @Result = 1;
			END
		END;
	COMMIT TRAN
	RETURN @Result;
END

GO

IF OBJECT_ID (N'FinalizeSMSLock', N'P') IS NOT NULL
	DROP PROCEDURE FinalizeSMSLock;
GO
--Процедура блокировки СМС со счета
CREATE PROCEDURE FinalizeSMSLock
(
   @ClientId BIGINT,
   @Count BIGINT = 1,
   @DistributionId BIGINT = NULL,
   @ExtDistributionId NVARCHAR(300) = NULL,
   @IsCommit BIT = 0
)
AS
BEGIN
	BEGIN TRAN
	DECLARE @LockId BIGINT;
	DECLARE @CurrentAmount BIGINT;
	DECLARE @Result BIGINT;
	DECLARE @AccountId BIGINT;
	DECLARE @LockTime DATETIME;
	
	IF (@Count IS NULL)
			SET @Count = 1;
	
	SET @Result = 0;
	
	SELECT @AccountId = A.Id FROM dbo.Account A WITH(NOLOCK) INNER JOIN dbo.Clients C WITH(NOLOCK)
					ON C.AccountId = A.Id 
						WHERE C.Id = @ClientId;
						
    --SELECT @AccountId AS AccountId;
	
	DECLARE LockCursor CURSOR FORWARD_ONLY READ_ONLY FAST_FORWARD FOR
		SELECT AL.Id, AL.Amount, AL.LockDate FROM dbo.AccountLocks AL INNER JOIN
				dbo.Account A WITH(NOLOCK) ON A.Id = AL.AccountId INNER JOIN
				dbo.Clients C WITH(NOLOCK) ON C.AccountId = A.Id
												WHERE  C.Id = @ClientId AND AL.IsFinal=0 AND AL.IsDeleted=0 AND
													   (AL.DistributionId = @DistributionId OR (@DistributionId IS NULL AND AL.DistributionId IS NULL)) AND
													   (AL.ExtDistributionId = @ExtDistributionId OR (@ExtDistributionId IS NULL AND AL.ExtDistributionId IS NULL))
												ORDER BY LockDate DESC;
    OPEN LockCursor;
    FETCH NEXT FROM LockCursor 
		INTO @LockId, @CurrentAmount, @LockTime;
		
	--SELECT @LockId, @CurrentAmount, @LockTime;
	
	WHILE @@FETCH_STATUS = 0
    BEGIN
		IF (@IsCommit = 1)
			BEGIN
				DECLARE @FinalLockId BIGINT
				SET @FinalLockId = 
				(SELECT TOP 1 AL.Id FROM dbo.AccountLocks AL WITH(ROWLOCK, UPDLOCK, HOLDLOCK) INNER JOIN
				dbo.Account A WITH(NOLOCK) ON A.Id = AL.AccountId INNER JOIN
				dbo.Clients C WITH(NOLOCK) ON C.AccountId = A.Id
												WHERE  C.Id = @ClientId AND AL.IsFinal=1 AND AL.IsDeleted<>1 AND
													(AL.DistributionId = @DistributionId OR (@DistributionId IS NULL AND AL.DistributionId IS NULL)) AND
													(AL.ExtDistributionId = @ExtDistributionId OR (@ExtDistributionId IS NULL AND AL.ExtDistributionId IS NULL))
												ORDER BY LockDate DESC);
			    --SELECT @FinalLockId AS FinalLockId;
				IF (@CurrentAmount>@Count)
					BEGIN
						UPDATE dbo.AccountLocks SET Amount = Amount-@Count WHERE Id = @LockId;
						IF (@FinalLockId IS NULL)
							BEGIN
							    IF (@AccountId IS NOT NULL)
									BEGIN
										INSERT dbo.AccountLocks
												( AccountId ,
												  DistributionId ,
												  Amount ,
												  IsDeleted ,
												  IsFinal ,
												  ExtDistributionId ,
												  LockDate ,
												  DeleteTime
												)
										VALUES  ( @AccountId , -- AccountId - bigint
												  @DistributionId , -- DistributionId - bigint
												  @Count , -- Amount - bigint
												  0 , -- IsDeleted - bit
												  1 , -- IsFinal - bit
												  @ExtDistributionId , -- ExtDistributionId - nvarchar(300)
												  @LockTime , -- LockDate - datetime
												  NULL  -- DeleteTime - datetime
												);
										SET @Result = 1;
									END
								ELSE SET @Result = 0;
							END;
						ELSE
							BEGIN
								UPDATE dbo.AccountLocks SET Amount = Amount+@Count WHERE Id = @FinalLockId;
								SET @Result = 1;
							END
						SET @Count = 0;
					END
				ELSE
					BEGIN
						UPDATE dbo.AccountLocks SET IsDeleted = 1, DeleteTime = GETDATE() WHERE Id = @LockId;
						IF (@FinalLockId IS NULL)
							BEGIN
								IF (@AccountId IS NOT NULL)
									BEGIN
										INSERT dbo.AccountLocks
												( AccountId ,
												  DistributionId ,
												  Amount ,
												  IsDeleted ,
												  IsFinal ,
												  ExtDistributionId ,
												  LockDate ,
												  DeleteTime
												)
										VALUES  ( @AccountId , -- AccountId - bigint
												  @DistributionId , -- DistributionId - bigint
												  @CurrentAmount , -- Amount - bigint
												  0 , -- IsDeleted - bit
												  1 , -- IsFinal - bit
												  @ExtDistributionId , -- ExtDistributionId - nvarchar(300)
												  @LockTime , -- LockDate - datetime
												  NULL  -- DeleteTime - datetime
												);
										SET @Result = 1;
									END
								ELSE SET @Result = 0;
							END;
						ELSE
							BEGIN
								UPDATE dbo.AccountLocks SET Amount = Amount+@CurrentAmount WHERE Id = @FinalLockId;
								SET @Result = 1;
							END
						SET @Count = @Count - @CurrentAmount;
					END
			END
		ELSE
			BEGIN
				IF (@CurrentAmount>@Count)
					BEGIN
						UPDATE dbo.AccountLocks SET Amount = Amount-@Count WHERE Id = @LockId;
						SET @Count = 0;
					END
				ELSE
					BEGIN
						UPDATE dbo.AccountLocks SET IsDeleted = 1, DeleteTime = GETDATE() WHERE Id = @LockId;
						SET @Count = @Count - @CurrentAmount;
					END
				SET @Result = 1;
			END
		
		IF (@Count = 0 OR @Result = 0)
			BREAK;
			
		FETCH NEXT FROM LockCursor 
		INTO @LockId, @CurrentAmount, @LockTime;
	END
	
	CLOSE LockCursor;
	DEALLOCATE LockCursor;

	IF (@Count<>0)
	    SET @Result =0;
	
	IF (@Result = 1)
		COMMIT TRAN;
	ELSE
		ROLLBACK TRAN;
	
	RETURN @Result;
END
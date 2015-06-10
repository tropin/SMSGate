--CREATE DATABASE OliverTwist
--USE OliverTwist

--Таблица клиентов
CREATE TABLE Clients
(
    Id BIGINT CONSTRAINT PK_Clients PRIMARY KEY IDENTITY,
    UserId UNIQUEIDENTIFIER CONSTRAINT FK_UserToLogin REFERENCES dbo.aspnet_Users(UserId) NULL,
    FirstName NVARCHAR(4000) NOT NULL,
    MiddleName NVARCHAR(4000) NULL,
    LastName NVARCHAR(4000) NOT NULL,
    City NVARCHAR(4000) NULL,
    OrganizationName NVARCHAR(4000) NULL,
    MobilePhone NVARCHAR(4000) NOT NULL,
    Sex INT, -- 0 Девочка, 1 Мальчик, 2-она-он, 3-он-она, 4-хз
    Email NVARCHAR(4000) NOT NULL,
    TimeZone NVARCHAR(4000) NULL,
    Status INT, -- Статусы 0 - неактивированный, 1-Активный, 2- валидированный, 3 - блокированый
    DeallerOfUserId BIGINT CONSTRAINT FK_DealdBy REFERENCES Clients(Id) NULL,
    DateCreated DATETIME CONSTRAINT DF_DateCreated DEFAULT GETDATE(),
    CreatedByClientId BIGINT CONSTRAINT FK_CreatedBy REFERENCES Clients(Id) NULL,
);

CREATE INDEX IDX_FirstName ON Clients(FirstName);
CREATE INDEX IDX_LastName ON Clients(LastName);
CREATE INDEX IDX_OrganizationName ON Clients(OrganizationName);
CREATE INDEX IDX_Email ON Clients(Email);
CREATE INDEX IDX_MobilePhone ON Clients(MobilePhone);

-- История работы с таблицей клиентоа
CREATE TABLE ClientsHistory
(
	Id BIGINT CONSTRAINT PK_ClientHistory PRIMARY KEY IDENTITY,
	ClientId BIGINT,
	Action INT,
	OldValue NVARCHAR(max) NOT NULL,
	NewValue NVARCHAR(max) NULL,
	Date DATETIME CONSTRAINT DF_Date DEFAULT GETDATE(),
	ActionByUserId BIGINT CONSTRAINT FK_ActionBy REFERENCES Clients(Id) NOT NULL
);

CREATE INDEX IDX_ClientId ON ClientsHistory(ClientId);

--Имена отправителей для СМС
CREATE TABLE SenderNames 
(
	Id BIGINT CONSTRAINT PK_SenderName PRIMARY KEY IDENTITY,
	ClientId BIGINT CONSTRAINT FK_OwnedBy REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	ApproveManagerId BIGINT CONSTRAINT FK_ApproveManager REFERENCES Clients(Id) NULL,
	ApproveDate DATETIME NULL,
	Name NVARCHAR(2000) NOT NULL,
	DateCreated DATETIME CONSTRAINT DF_SN_DateCreated DEFAULT GETDATE(),
);

CREATE INDEX IDX_Name ON SenderNames(Name);
CREATE INDEX IDX_ClientId ON SenderNames(ClientId);

--Группы клиентов
CREATE TABLE ClientGroups 
(
	Id BIGINT CONSTRAINT PK_ClientGroup PRIMARY KEY IDENTITY,
	Name NVARCHAR(max) NOT NULL,
	ClientId BIGINT CONSTRAINT FK_CG_OwnedBy REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	DateCreated DATETIME CONSTRAINT DF_CG_DateCreated DEFAULT GETDATE()
);

CREATE INDEX IDX_ClientId ON ClientGroups(ClientId);

--Группы клиентов, назначения
CREATE TABLE Groups2Clients
(
	Id BIGINT CONSTRAINT PK_Assignment PRIMARY KEY IDENTITY,
	ClientId BIGINT CONSTRAINT FK_Assignee REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	GroupId BIGINT CONSTRAINT FK_AssignedGroup REFERENCES ClientGroups(Id) NOT NULL,
	SlaveGroupId BIGINT CONSTRAINT FK_AssigneeGroup REFERENCES ClientGroups(Id) NULL,
	GroupType INT CONSTRAINT DF_GroupType DEFAULT 0,-- Для будущего использовангия 
	DateCreated DATETIME CONSTRAINT DF_G2C_DateCreated DEFAULT GETDATE()
);
    
CREATE INDEX IDX_ClientId ON Groups2Clients(ClientId);
CREATE INDEX IDX_GroupId ON Groups2Clients(GroupId);
CREATE INDEX IDX_SlaveGroupId ON Groups2Clients(SlaveGroupId);

--Шаблоны сообщений
CREATE TABLE MessageTemplates
(
	Id BIGINT CONSTRAINT PK_MessageTemplate PRIMARY KEY IDENTITY,
	ClientId BIGINT CONSTRAINT FK_MT_OwnedBy REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	Name NVARCHAR(max) NULL,
	Text NVARCHAR(max) NOT NULL,
	DateCreated DATETIME CONSTRAINT DF_MT_DateCreated DEFAULT GETDATE()
)

CREATE INDEX IDX_ClientId ON MessageTemplates(ClientId);

--Рассылки
CREATE TABLE Distributions
(
  Id BIGINT CONSTRAINT PK_Distribution PRIMARY KEY IDENTITY,
  Name NVARCHAR(max) NOT NULL,
  Recipients NVARCHAR(max) NOT NULL,
  [Type] INT NOT NULL, --0 TEXT, 1-FLASH, 2-WAP-PUSH
  [STOPList] NVARCHAR(max) NULL,
  SenderNameId BIGINT CONSTRAINT FK_SenderName REFERENCES SenderNames(Id) NOT NULL,
  SmsTTL DATETIME NULL,
  Text NVARCHAR(max) NOT NULL,
  TemplateId BIGINT CONSTRAINT FK_Template REFERENCES MessageTemplates(Id) NULL,
  Transliterate BIT CONSTRAINT DF_Translit DEFAULT 1,
  DistributionCount INT NOT NULL,
  CreatedByClientId BIGINT CONSTRAINT FK_D_CreatedBy REFERENCES Clients(Id) NOT NULL,
  DateCreated DATETIME CONSTRAINT DF_D_DateCreated DEFAULT GETDATE()
);

CREATE INDEX IDX_SenderNameId ON Distributions(SenderNameId);
CREATE INDEX IDX_TemplateId ON Distributions(TemplateId);
CREATE INDEX IDX_CreatedByClientId ON Distributions(CreatedByClientId);

--Счет пользователя
CREATE TABLE Account
(
	Id BIGINT CONSTRAINT PK_Account PRIMARY KEY IDENTITY,
	ClientId BIGINT CONSTRAINT FK_A_OwnedBy REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	Amount DECIMAL NOT NULL,
	Cost DECIMAL NOT NULL
);

CREATE INDEX IDX_ClientId ON Account(ClientId);

--История изменения счетра
CREATE TABLE AccountHistory
(
	Id BIGINT CONSTRAINT PK_AccountHistory PRIMARY KEY IDENTITY,
	ManagerId BIGINT CONSTRAINT FK_Manager REFERENCES Clients(Id) NOT NULL,
	AccountId BIGINT CONSTRAINT FK_Account REFERENCES Account(Id) ON DELETE CASCADE NOT NULL,
	OldCostValue DECIMAL NOT NULL,
	NewCostValue DECIMAL NOT NULL,
	DateCreated DATETIME CONSTRAINT DF_AH_DateCreated DEFAULT GETDATE()		
);

CREATE INDEX IDX_ManagerId ON AccountHistory(ManagerId);
CREATE INDEX IDX_AccountId ON AccountHistory(AccountId);

--Лог действий менеджера
CREATE TABLE ActionLog
(
	Id BIGINT CONSTRAINT PK_ActionLog PRIMARY KEY IDENTITY,
	ManagerId BIGINT CONSTRAINT FK_AL_Manager REFERENCES Clients(Id) NOT NULL,
	ActionType NVARCHAR(max),
	ActionContent NVARCHAR(max),
	DateCreated DATETIME CONSTRAINT DF_AL_DateCreated DEFAULT GETDATE()	
);

CREATE INDEX IDX_ManagerId ON ActionLog(ManagerId);

-- Адресная книга, по которой будут формироваться рассыки
CREATE TABLE AddressBook
(
    Id BIGINT CONSTRAINT PK_AddressBook PRIMARY KEY IDENTITY, 
    ClientId BIGINT CONSTRAINT FK_ClientHolder REFERENCES Clients(Id) NOT NULL,
    FirstName NVARCHAR(4000) NULL,
    MiddleName NVARCHAR(4000) NULL,
    LastName NVARCHAR(4000) NULL,
    Sex INT NULL, -- 0 Девочка, 1 Мальчик, 2-она-он, 3-он-она, 4-хз
    BirthDay DATETIME NULL,
    MobilePhone NVARCHAR(4000) NOT NULL,
    City NVARCHAR(2000) NULL,
    [Description] NVARCHAR(2000) NULL
);

CREATE INDEX IDX_AB_ClientId ON AddressBook(ClientId);
CREATE INDEX IDX_AB_City ON AddressBook(City);
CREATE INDEX IDX_AB_Description ON AddressBook([Description]);
CREATE INDEX IDX_AB_Sex ON AddressBook(Sex);
CREATE INDEX IDX_AB_BirthDay ON AddressBook(ClientId);

DROP TABLE dbo.Groups2Clients;
DROP TABLE dbo.ClientGroups;

--Группы клиентов
CREATE TABLE AddressGroups 
(
	Id BIGINT CONSTRAINT PK_ClientGroup PRIMARY KEY IDENTITY,
	Name NVARCHAR(max) NOT NULL,
	ClientId BIGINT CONSTRAINT FK_CG_OwnedBy REFERENCES Clients(Id) ON DELETE CASCADE NOT NULL,
	DateCreated DATETIME CONSTRAINT DF_CG_DateCreated DEFAULT GETDATE()
);

CREATE INDEX IDX_ClientId ON AddressGroups(ClientId);

--Группы адресов, назначения
CREATE TABLE Groups2Addresses
(
	Id BIGINT CONSTRAINT PK_Assignment PRIMARY KEY IDENTITY,
	AddressId BIGINT CONSTRAINT FK_Assignee REFERENCES AddressBook(Id) ON DELETE CASCADE NOT NULL,
	GroupId BIGINT CONSTRAINT FK_AssignedGroup REFERENCES AddressGroups(Id) NOT NULL,
	SlaveGroupId BIGINT CONSTRAINT FK_AssigneeGroup REFERENCES AddressGroups(Id) NULL,
	GroupType INT CONSTRAINT DF_GroupType DEFAULT 0,-- Для будущего использовангия 
	DateCreated DATETIME CONSTRAINT DF_G2C_DateCreated DEFAULT GETDATE()
);

-----11.10.2010-------------- 
ALTER TABLE Clients
DROP CONSTRAINT FK_UserToLogin;
ALTER TABLE Clients
DROP COLUMN UserId;

--Таблица соответствия клиентов и пользователей за ними закрепленных
CREATE TABLE Users2Clients
(
	Id BIGINT CONSTRAINT PK_Users2Clients PRIMARY KEY IDENTITY,
	UserId UNIQUEIDENTIFIER CONSTRAINT FK_Users REFERENCES dbo.aspnet_Users(UserId),
	ClientId BIGINT CONSTRAINT FK_Clients REFERENCES dbo.Clients(id)
);

DECLARE @ApplicationId UNIQUEIDENTIFIER;

EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName = '/', -- nvarchar(256)
    @ApplicationId = @ApplicationId OUT; -- uniqueidentifier

INSERT INTO dbo.aspnet_Roles
        ( ApplicationId ,
          RoleName ,
          LoweredRoleName ,
          Description
        )
VALUES  (
          @ApplicationId,
          'Admin' , -- RoleName - nvarchar(256)
          'admin' , -- LoweredRoleName - nvarchar(256)
          'Администратор клиента'  -- Description - nvarchar(256)
        )
INSERT INTO dbo.aspnet_Roles
        ( ApplicationId ,
          RoleName ,
          LoweredRoleName ,
          Description
        )
VALUES  (
          @ApplicationId, 
          'Manager' , -- RoleName - nvarchar(256)
          'manager' , -- LoweredRoleName - nvarchar(256)
          'Менеджер клиента'  -- Description - nvarchar(256)
        )
        
-- 16.10.2010 -----Тропин Павел-------

BEGIN TRAN

DROP INDEX Clients.IDX_FirstName;
DROP INDEX Clients.IDX_LastName;
DROP INDEX Clients.IDX_Email;
DROP INDEX Clients.IDX_MobilePhone;

ALTER TABLE Clients
DROP COLUMN FirstName, LastName, MiddleName, Sex, TimeZone, Email, MobilePhone, City;

ALTER TABLE Clients
ADD AccountId BIGINT CONSTRAINT FK_ClientAccount REFERENCES Account(Id);
EXEC sys.sp_rename @objname = 'dbo.Clients.DeallerOfUserId', -- nvarchar(1035)
    @newname = 'DeallerOfClientId', -- sysname
    @objtype = 'column' -- varchar(13)
    
CREATE INDEX IDX_Client2Account ON dbo.Clients(AccountId);
    
--Профиль пользователя
CREATE TABLE UserProfile
(
	Id BIGINT CONSTRAINT PK_UserProfile PRIMARY KEY IDENTITY,
	UserId UNIQUEIDENTIFIER CONSTRAINT FK_ProfileToUser REFERENCES dbo.aspnet_Users(UserId) NULL,
	FirstName NVARCHAR(4000) NOT NULL,
    MiddleName NVARCHAR(4000) NULL,
    LastName NVARCHAR(4000) NOT NULL,
    MobilePhone NVARCHAR(4000) NOT NULL,
    City NVARCHAR(4000) NULL,
    Sex INT, -- 0 Девочка, 1 Мальчик, 2-она-он, 3-он-она, 4-хз
    TimeZone NVARCHAR(4000) NULL
)

CREATE INDEX IDX_FirstName ON UserProfile(FirstName);
CREATE INDEX IDX_LastName ON UserProfile(LastName);
CREATE INDEX IDX_MobilePhone ON UserProfile(MobilePhone);
CREATE INDEX IDX_Profile2User ON UserProfile(UserId);
CREATE INDEX IDX_ProfileCity ON UserProfile(City);

ALTER TABLE dbo.AddressBook
ADD TimeZone NVARCHAR(4000) NULL;

DROP INDEX dbo.Account.IDX_ClientId;

ALTER TABLE dbo.Account
DROP CONSTRAINT FK_A_OwnedBy, COLUMN ClientId, Cost;
ALTER TABLE dbo.Account
ADD AccountType INT NOT NULL CONSTRAINT DF_AccountType DEFAULT(0), --Тип счета (предоплата = 0, постоплата = 1)
    DebtingType INT NOT NULL CONSTRAINT DF_DebtingType DEFAULT(0) -- Тип списания (отправленные = 0, доставленные = 1)
;

--Диапазон стоисомтей СМС
CREATE TABLE CostRanges
(
	Id BIGINT CONSTRAINT PK_CostRange PRIMARY KEY IDENTITY,
	AccountId BIGINT CONSTRAINT FK_RangeAccount REFERENCES dbo.Account(Id),
	Cost DECIMAL NOT NULL,
	Volume BIGINT NOT NULL,
	LowerSumm AS Cost*Volume PERSISTED,
	IsDeleted BIT NOT NULL CONSTRAINT DF_CostRangesIsDeleted DEFAULT(0)
);

CREATE INDEX IDX_CostRange2Account ON CostRanges(AccountId);

ALTER TABLE dbo.SenderNames
DROP CONSTRAINT FK_ApproveManager,COLUMN ApproveManagerId;
ALTER TABLE dbo.SenderNames
ADD ApproveManagerId UNIQUEIDENTIFIER NULL,
CONSTRAINT FK_ApproveManager FOREIGN KEY (ApproveManagerId) REFERENCES dbo.aspnet_Users(UserId);

DROP TABLE dbo.ClientsHistory;
DROP TABLE dbo.ActionLog;

ALTER TABLE dbo.Distributions
ADD [Status] INT NOT NULL CONSTRAINT DF_DistributionStatus DEFAULT(0); -- 0 Создана, 1 - Выполняется,2 - Выполнена,3 - Отменена

ALTER TABLE dbo.Account
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_AccountIsDeleted DEFAULT(0);

ALTER TABLE dbo.Clients
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_ClientIsDeleted DEFAULT(0);

ALTER TABLE dbo.Users2Clients
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_Users2ClientIsDeleted DEFAULT(0);

ALTER TABLE dbo.MessageTemplates
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_MessageTemplateIsDeleted DEFAULT(0);

ALTER TABLE dbo.SenderNames
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_SenderNameIsDeleted DEFAULT(0);

ALTER TABLE dbo.Distributions
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_DistributionIsDeleted DEFAULT(0);

ALTER TABLE dbo.AddressGroups
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_AddressGroupIsDeleted DEFAULT(0);

ALTER TABLE dbo.AddressBook
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_AddressBookIsDeleted DEFAULT(0);

ALTER TABLE dbo.Groups2Addresses
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_Groups2AddressIsDeleted DEFAULT(0);

--Таблица блоикровок счета. Только при предоплате
CREATE TABLE dbo.AccountLocks
(
	Id BIGINT CONSTRAINT PK_AccountLocks PRIMARY KEY IDENTITY,
	AccountId BIGINT CONSTRAINT FK_AccountLocks REFERENCES dbo.Account(Id),
	DistributionId BIGINT CONSTRAINT FK_ByDistributionLock REFERENCES dbo.Distributions(Id),
	Amount BIGINT NOT NULL,
	IsDeleted BIT NOT NULL CONSTRAINT DF_AccountLocksIsDeleted DEFAULT(0)
);

COMMIT TRAN


-- Работа с историческими таблицами
BEGIN TRAN

DROP TABLE dbo.AccountHistory;
--История интервалов стоимостей
CREATE TABLE CostRangesHistory
(
	VersionId BIGINT CONSTRAINT PK_CostRangeHistory PRIMARY KEY IDENTITY,
	Id BIGINT NOT NULL CONSTRAINT FK_MasterCostRange REFERENCES dbo.CostRanges(id),
	AccountId BIGINT NOT NULL CONSTRAINT FK_RangeHistoryAccount REFERENCES dbo.Account(Id),
	Cost DECIMAL NOT NULL,
	Volume BIGINT NOT NULL,
	IsDeleted BIT NOT NULL,
	[Action] INT NULL, -- 0 добавление, 1 изменение, 2 удаление
	ManagerId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_CostRangesHistoryManager REFERENCES aspnet_Users(UserId),
	RealClientId BIGINT NOT NULL CONSTRAINT FK_CostRangesHistoryRealClient REFERENCES Clients(Id),
	OperationalClientId BIGINT NOT NULL CONSTRAINT FK_CostRangesHistoryOperationalClient REFERENCES Clients(Id),
	VersionDate DateTime NOT NULL CONSTRAINT DF_CostRangesHistoryVersionDate DEFAULT(GETDATE())
);

--История операций со счетом
CREATE TABLE AccountHistory
(
	VersionId BIGINT CONSTRAINT PK_AccountHistory PRIMARY KEY IDENTITY,
	Id BIGINT NOT NULL CONSTRAINT FK_MasterAccount REFERENCES dbo.Account(id),
	CostRangeId BIGINT NULL CONSTRAINT FK_MatchingRange REFERENCES dbo.CostRangesHistory(VersionId),
	Amount DECIMAL NOT NULL,
	AmountDelta DECIMAL NOT NULL,
	QuickCost DECIMAL NOT NULL,
	MoneyVolume DECIMAL NOT NULL,
	Comment NVARCHAR(max) NULL,
	ManagerId UNIQUEIDENTIFIER NULL CONSTRAINT FK_AccountHistoryManager REFERENCES aspnet_Users(UserId),
	RealClientId BIGINT NULL CONSTRAINT FK_AccountHistoryRealClient REFERENCES Clients(Id),
	OperationalClientId BIGINT NULL CONSTRAINT FK_AccountHistoryOperationalClient REFERENCES Clients(Id),
	VersionDate DateTime NOT NULL CONSTRAINT DF_AccountHistoryVersionDate DEFAULT(GETDATE())
);

--История параметров счета
CREATE TABLE AccountOptionHistory
(
	VersionId BIGINT CONSTRAINT PK_AccountOptionHistory PRIMARY KEY IDENTITY,
	Id BIGINT NOT NULL CONSTRAINT FK_MasterOptionAccount REFERENCES dbo.Account(id),
	AccountType INT NOT NULL,
    DebtingType INT NOT NULL,
	IsDeleted BIT NOT NULL,
	[Action] INT NULL, -- 0 добавление, 1 изменение, 2 удаление
	ManagerId UNIQUEIDENTIFIER NULL CONSTRAINT FK_AccountOptionHistoryManager REFERENCES aspnet_Users(UserId),
	RealClientId BIGINT NOT NULL CONSTRAINT FK_AccountOptionHistoryRealClient REFERENCES Clients(Id),
	OperationalClientId BIGINT NOT NULL CONSTRAINT FK_AccountOptionHistoryOperationalClient REFERENCES Clients(Id),
	VersionDate DateTime NOT NULL CONSTRAINT DF_AccountOptionVersionDate DEFAULT(GETDATE())
);

--История клиентов
CREATE TABLE ClientsHistory
(
	VersionId BIGINT CONSTRAINT PK_ClientsHistory PRIMARY KEY IDENTITY,
	Id BIGINT NOT NULL CONSTRAINT FK_MasterClient REFERENCES dbo.Clients(id),
	OrganizationName nvarchar(4000) NULL,
    [Status] int NULL,
    DeallerOfClientId bigint NULL,
    DateCreated datetime NULL,
    CreatedByClientId bigint NULL,
    AccountId bigint NULL,
    IsDeleted bit NOT NULL,
    [Action] INT NULL, -- 0 добавление, 1 изменение, 2 удаление
	ManagerId UNIQUEIDENTIFIER NULL CONSTRAINT FK_ClientsHistoryManager REFERENCES aspnet_Users(UserId),
	RealClientId BIGINT NOT NULL CONSTRAINT FK_ClientsHistoryRealClient REFERENCES Clients(Id),
	OperationalClientId BIGINT NOT NULL CONSTRAINT FK_ClientsHistoryOperationalClient REFERENCES Clients(Id),
	VersionDate DateTime NOT NULL CONSTRAINT DF_ClientsHistoryVersionDate DEFAULT(GETDATE())
);

--История рассылки (только статусы)
CREATE TABLE DistributionHistory
(
	VersionId BIGINT CONSTRAINT PK_DistributionHistory PRIMARY KEY IDENTITY,
	Id BIGINT NOT NULL CONSTRAINT FK_MasterDistribution REFERENCES dbo.Distributions(id),
	[Status] [int] NOT NULL,
	IsDeleted bit NOT NULL,
    [Action] INT NULL, -- 0 добавление, 1 изменение, 2 удаление
	ManagerId UNIQUEIDENTIFIER NULL CONSTRAINT FK_DistributionHistoryManager REFERENCES aspnet_Users(UserId),
	RealClientId BIGINT NOT NULL CONSTRAINT FK_DistributionHistoryRealClient REFERENCES Clients(Id),
	OperationalClientId BIGINT NOT NULL CONSTRAINT FK_DistributionHistoryOperationalClient REFERENCES Clients(Id),
	VersionDate DateTime NOT NULL CONSTRAINT DF_DistributionHistoryVersionDate DEFAULT(GETDATE())
);

--Считать это поле нет смысла, слишком динамичное
ALTER TABLE dbo.Distributions
DROP COLUMN DistributionCount;

COMMIT TRAN
--После всех этих изменений базу придется очистить
DELETE dbo.AddressBook
DELETE dbo.CostRangesHistory
DELETE dbo.CostRanges
DELETE dbo.Users2Clients
DELETE dbo.ClientsHistory
DELETE dbo.Clients
DELETE dbo.AccountHistory
DELETE dbo.AccountOptionHistory
DELETE dbo.Account
DELETE dbo.UserProfile
DELETE dbo.aspnet_Membership
DELETE dbo.aspnet_UsersInRoles
DELETE dbo.aspnet_Users

--Добавляем возможность создавать шаблоны счетов
ALTER TABLE dbo.Account
ADD TemplateOfClient BIGINT NULL CONSTRAINT FK_TemplateOwnerClient REFERENCES dbo.Clients(Id);

CREATE INDEX IDX_AccountTemplateOwner ON dbo.Account(TemplateOfClient);

ALTER TABLE dbo.ClientsHistory
ALTER COLUMN OperationalClientId BIGINT NULL

ALTER TABLE dbo.ClientsHistory
ALTER COLUMN RealClientId BIGINT NULL

ALTER TABLE dbo.CostRangesHistory
ALTER COLUMN OperationalClientId BIGINT NULL

ALTER TABLE dbo.CostRangesHistory
ALTER COLUMN RealClientId BIGINT NULL

ALTER TABLE dbo.CostRangesHistory
ALTER COLUMN ManagerId UNIQUEIDENTIFIER NULL

ALTER TABLE dbo.AccountOptionHistory
ALTER COLUMN OperationalClientId BIGINT NULL

ALTER TABLE dbo.AccountOptionHistory
ALTER COLUMN RealClientId BIGINT NULL

ALTER TABLE dbo.AccountOptionHistory
ALTER COLUMN ManagerId UNIQUEIDENTIFIER NULL

ALTER TABLE dbo.UserProfile
DROP CONSTRAINT FK_ProfileToUser 

DROP INDEX dbo.UserProfile.IDX_Profile2User;

ALTER TABLE dbo.UserProfile
ALTER COLUMN UserId UNIQUEIDENTIFIER NOT NULL

ALTER TABLE dbo.UserProfile
ADD CONSTRAINT FK_ProfileToUser FOREIGN KEY (UserId) REFERENCES dbo.aspnet_Users(UserId) 

CREATE INDEX IDX_Profile2User ON UserProfile(UserId);

ALTER TABLE dbo.UserProfile
ALTER COLUMN FirstName NVARCHAR(4000) NULL

ALTER TABLE dbo.UserProfile
ALTER COLUMN LastName NVARCHAR(4000) NULL

ALTER TABLE dbo.UserProfile
ALTER COLUMN MobilePhone NVARCHAR(4000) NULL

ALTER TABLE dbo.UserProfile
ALTER COLUMN Sex INT NULL

DECLARE @AppId UNIQUEIDENTIFIER;
SELECT @AppId = ApplicationId FROM dbo.aspnet_Applications WITH(NOLOCK) WHERE ApplicationName = '/'

INSERT INTO dbo.aspnet_Roles
        ( ApplicationId ,
          RoleName ,
          LoweredRoleName ,
          Description
        )
VALUES  (
          @AppId,
          'RootAdmin' , -- RoleName - nvarchar(256)
          'rootadmin' , -- LoweredRoleName - nvarchar(256)
          'Центральный администратор'  -- Description - nvarchar(256)
        );
          
 
-- Версия 0.2----------------
ALTER TABLE dbo.UserProfile
ADD IsDeleted BIT NOT NULL CONSTRAINT DF_UserProfileIsDeleted DEFAULT (0);

ALTER TABLE dbo.AccountHistory
ADD DistributionId BIGINT NULL CONSTRAINT FK_AcoountHistoryDistribution REFERENCES dbo.Distributions(Id),
TargetAccountId BIGINT NULL CONSTRAINT FK_AccountOutcomeHistoryId REFERENCES dbo.AccountHistory(VersionId);

CREATE INDEX IDX_AccountHistoryDistribution ON dbo.AccountHistory(DistributionId);
CREATE INDEX IDX_AccountHistoryOutcomeLink ON dbo.AccountHistory(TargetAccountId);

DROP INDEX dbo.Account.IDX_AccountTemplateOwner;

ALTER TABLE dbo.Account 
DROP
CONSTRAINT FK_TemplateOwnerClient,
COLUMN TemplateOfClient


--Шаблоны счетов для создания новых клиентов
CREATE TABLE AccountTemplates
(
	Id BIGINT CONSTRAINT PK_AccountTemplates PRIMARY KEY IDENTITY,
	Name NVARCHAR(4000) NOT NULL,
	AccountId BIGINT NOT NULL CONSTRAINT FK_TemplateAccount REFERENCES dbo.Account(Id),
	ClientOwnerId BIGINT NOT NULL CONSTRAINT FK_ClientOwner REFERENCES dbo.Clients(Id)
)

ALTER TABLE Groups2Addresses
ALTER COLUMN AddressId BIGINT NULL

-- Версия 0.3. -------------------
ALTER TABLE dbo.AccountLocks
ADD IsFinal BIT NOT NULL CONSTRAINT DF_Final DEFAULT(0),
	ExtDistributionId NVARCHAR(300) NULL,
	LockDate DATETIME NOT NULL CONSTRAINT DF_LockDate DEFAULT(GETDATE());
	
CREATE INDEX IDX_Lock_ExtDistributionId ON AccountLocks(ExtDistributionId);
CREATE INDEX IDX_Lock_Date ON AccountLocks(LockDate);

ALTER TABLE dbo.AccountLocks
ADD DeleteTime DATETIME NULL;
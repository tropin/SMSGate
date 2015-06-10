CREATE DATABASE SenderSheduller;

GO

USE SenderSheduller;

---Пользователи шлюза------
CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY 
    CONSTRAINT DF_UserId DEFAULT NEWID(),
    [Login] NVARCHAR(200) NOT NULL,
    [PassHash] NVARCHAR(200) NOT NULL    
)
 
CREATE INDEX IDX_Users ON Users([Login], [PassHash]) WITH (FILLFACTOR = 75, PAD_INDEX = ON);

-- Провайдеры, через которых мы будем осуществлять доставку
CREATE TABLE Providers
(
	Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Providers PRIMARY KEY
	CONSTRAINT DF_ProviderId DEFAULT NEWID(),
	Name NVARCHAR(200) NOT NULL,
	Configuration XML NOT NULL,	--Конфигурация провайдера поставщика
	QualityMetrics REAL NULL, --Соотношение доставленых/отправленых (будет обновляться каждый день ночью)
	ChannelBrandwidth REAL NULL -- Усредненная разница между запрошенным временем отправки и фактической доставкой (будет обновляться каждый день ночью)
);

--Имена отправителей, зарегистрированные за конкретными клиентами
CREATE TABLE SenderNames
(
	Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SenderNames PRIMARY KEY
	CONSTRAINT DF_SenderNamesId DEFAULT NEWID(),
	UserId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_SenderNames2Users REFERENCES Users(Id),
	ProviderId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_Provider REFERENCES Providers(Id),
	Name NVARCHAR(200) NOT NULL
);

CREATE INDEX IDX_Users ON SenderNames(UserId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_Providers ON SenderNames(ProviderId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);

--Очередь сообщений
CREATE TABLE SMSQueue
(
	Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SMSQueue PRIMARY KEY
	CONSTRAINT DF_SMSQueueId DEFAULT NEWID(),
	UserId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_SMSQueue2Users REFERENCES Users(Id),
	SenderNameId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_SMSQueue2SenderNames REFERENCES SenderNames(Id),
	ProviderId UNIQUEIDENTIFIER NULL CONSTRAINT FK_SMSProvider REFERENCES Providers(Id),
	ClientId NVARCHAR(200) NULL,
	DistributionId NVARCHAR(200) NULL,
	SMSId NVARCHAR(200) NULL, -- Id СМС у внешнего поставщика
	[Status] SMALLINT NOT NULL CONSTRAINT DF_SMSStatus DEFAULT 0,
	/*
		Доступные статусы
		0 - Cтоит в очереди
		1 - Отправлен поставщику
		2 - Доставлен поставщиком
		3 - Ошибка отправки
		4 - Ошибка валидации
		5 - Отменена
		6 - Попытка отмены
	*/
	PSStatus SMALLINT NULL, -- Специфичный для провайдера статус сообщения
	DeliveryTime DATETIME NULL, -- Время доставки сообщения
	EuqueueTime DATETIME NOT NULL CONSTRAINT DF_EqueueTime DEFAULT GETDATE(), -- Время постановки в очередь
	LastSendTime DATETIME NULL, -- Время последней попытки отправки поставщику
	RetryCount SMALLINT NOT NULL CONSTRAINT DF_RetryCount DEFAULT 0, -- Количество повторов отправки
	sequence_number INT NOT NULL,
	service_type VARCHAR(6) NULL,
	source_addr_ton SMALLINT NULL,
	source_addr_npi SMALLINT NULL,
	source_addr VARCHAR(21) NULL,
	number_of_dests SMALLINT NOT NULL CONSTRAINT DF_DestanationsCount DEFAULT 1,
	esm_class SMALLINT NOT NULL,
	protocol_id SMALLINT NOT NULL,
	priority_flag SMALLINT NOT NULL,
	schedule_delivery_time VARCHAR(17) NULL,
	validity_period VARCHAR(17) NULL,
	registered_delivery SMALLINT NOT NULL CONSTRAINT DF_RegistredSMS DEFAULT 1,
	replace_if_present_flag SMALLINT NOT NULL CONSTRAINT DF_ReplaceIfPresent DEFAULT 1,
	data_coding SMALLINT NOT NULL CONSTRAINT DF_SMSCoding DEFAULT 0,
	/*
		Доступные кодировки
		0 - SMSC Default Alphabet
		1 - IA5 (CCITT T.50)/ASCII (ANSI X3.4) b
		2 - Octet unspecified (8-bit binary) b
		3 - Latin 1 (ISO-8859-1) b
		4 - Octet unspecified (8-bit binary) a
		5 -  JIS (X 0208-1990) b
		6 - Cyrllic (ISO-8859-5) b
		7 - Latin/Hebrew (ISO-8859-8) b
		8 - UCS2 (ISO/IEC-10646) a
		9 - Pictogram Encoding b
		10 - ISO-2022-JP (Music Codes) b
		13 - Extended Kanji JIS(X 0212-1990) b
		14 - KS C 5601 b
	*/
	sm_default_msg_id SMALLINT NULL,
	sm_length SMALLINT NOT NULL,
	short_message VARCHAR(254) NOT NULL
);

CREATE INDEX IDX_Users ON dbo.SMSQueue(UserId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_Providers ON dbo.SMSQueue(ProviderId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_SenderNames ON dbo.SMSQueue(SenderNameId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_ClientDistributionId ON dbo.SMSQueue(ClientId, DistributionId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_SmsId ON dbo.SMSQueue(SMSId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_DeliveryTime ON dbo.SMSQueue(DeliveryTime) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_EnqueueTime ON dbo.SMSQueue(EuqueueTime) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_Status ON dbo.SMSQueue([Status]) WITH (FILLFACTOR = 75, PAD_INDEX = ON);

--Таблица списка назначений одной СМС (актуально если текст не меняется от получателя к получателю)
CREATE TABLE DestinationMap
(
	Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_DestinationMap PRIMARY KEY
	CONSTRAINT DF_DestinationMapId DEFAULT NEWID(),
	SMSId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_DestinationMap2SMSQueue REFERENCES SMSQueue(Id),
	dest_addr_ton SMALLINT NOT NULL,
	dest_addr_npi SMALLINT NOT NULL,
	destination_addr VARCHAR(21) NOT NULL
);

CREATE INDEX IDX_SmsId ON DestinationMap(SMSId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);
CREATE INDEX IDX_Destanation ON DestinationMap(destination_addr) WITH (FILLFACTOR = 75, PAD_INDEX = ON);

--Допзначения с коммандой SMPP
CREATE TABLE SMSTLV
(
   Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SMSTLV PRIMARY KEY
	CONSTRAINT DF_SMSTLVId DEFAULT NEWID(),
   SmsId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_SMSTLV2SMSQueue REFERENCES dbo.SMSQueue(Id),
   Tag SMALLINT NOT NULL,
   Length SMALLINT NOT NULL,
   Value VARCHAR(254) NOT NULL
);

CREATE INDEX IDX_SMSId ON dbo.SMSTLV(SmsId) WITH (FILLFACTOR = 75, PAD_INDEX = ON);

ALTER TABLE dbo.Users
ADD IsEnabled BIT NOT NULL CONSTRAINT DF_UserIsEnabled DEFAULT 1;

ALTER TABLE dbo.SenderNames 
ADD IsEnabled BIT NOT NULL CONSTRAINT DF_SenderNameIsEnabled DEFAULT 1;

ALTER TABLE DestinationMap
ADD IsDistributionList BIT NOT NULL CONSTRAINT DF_IsDistributionList DEFAULT 0;

ALTER TABLE SMSTLV
DROP COLUMN Length;

ALTER TABLE dbo.SMSQueue
ADD ExtSMSId NVARCHAR(200) NULL;

ALTER TABLE dbo.SMSQueue
ALTER COLUMN short_message VARCHAR(280) NOT NULL;

INSERT INTO dbo.Providers 
        ( Name ,
          Configuration ,
          QualityMetrics ,
          ChannelBrandwidth
        )
VALUES  ( 'Zanzara' , -- Name - nvarchar(200)
          '<ProviderConfiguration xmlns="http://schemas.datacontract.org/2004/07/Csharper.SenderService" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                          <BindingTypes xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet.Request">
                            <a:SmppBind.BindingType>BindAsTransceiver</a:SmppBind.BindingType>
                          </BindingTypes>
                          <DestinationNumberings>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                          </DestinationNumberings>
                          <EnqureLinkInterval>PT1M</EnqureLinkInterval>
                          <Host>213.242.207.57</Host>
                          <Password>4wangn6z</Password>
                          <PayloadType>WDPMessage</PayloadType>
                          <Port>2775</Port>
                          <SourceNumberings>
                            <TonNpiPair>
                              <Npi>0</Npi>
                              <Ton>5</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>0</Npi>
                              <Ton>3</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>0</Ton>
                            </TonNpiPair>
                          </SourceNumberings>
                          <SupportedSMPPVersions xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet">
                            <a:Pdu.SmppVersionType>Version3_4</a:Pdu.SmppVersionType>
                          </SupportedSMPPVersions>
                          <SystemId>test.csharper.ru</SystemId>
                          <Support7Bit>false</Support7Bit>
				</ProviderConfiguration>' , -- Configuration - xml
          1.0 , -- QualityMetrics - real
          10.0  -- ChannelBrandwidth - real
        );
        
INSERT INTO dbo.Providers 
        ( Name ,
          Configuration ,
          QualityMetrics ,
          ChannelBrandwidth
        )
VALUES  ( 'Zanzara_Production' , -- Name - nvarchar(200)
          '<ProviderConfiguration xmlns="http://schemas.datacontract.org/2004/07/Csharper.SenderService" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                          <BindingTypes xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet.Request">
                            <a:SmppBind.BindingType>BindAsTransceiver</a:SmppBind.BindingType>
                          </BindingTypes>
                          <DestinationNumberings>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                          </DestinationNumberings>
                          <EnqureLinkInterval>PT1M</EnqureLinkInterval>
                          <Host>213.242.207.57</Host>
                          <Password>4wangn6z</Password>
                          <PayloadType>WDPMessage</PayloadType>
                          <Port>2775</Port>
                          <SourceNumberings>
                            <TonNpiPair>
                              <Npi>0</Npi>
                              <Ton>5</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>0</Npi>
                              <Ton>3</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>0</Ton>
                            </TonNpiPair>
                          </SourceNumberings>
                          <SupportedSMPPVersions xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet">
                            <a:Pdu.SmppVersionType>Version3_4</a:Pdu.SmppVersionType>
                          </SupportedSMPPVersions>
                          <SystemId>csharper.ru</SystemId>
                          <Support7Bit>false</Support7Bit>
				</ProviderConfiguration>'  , -- Configuration - xml
          1.0 , -- QualityMetrics - real
          10.0  -- ChannelBrandwidth - real
        );
        
ALTER TABLE SMSQueue
ADD LastStatusCheck DATETIME CONSTRAINT DF_LastStatusCheck DEFAULT(GETDATE());

ALTER TABLE SMSQueue
ALTER COLUMN LastStatusCheck DATETIME NOT NULL

INSERT INTO dbo.Providers 
        ( Name ,
          Configuration ,
          QualityMetrics ,
          ChannelBrandwidth
        )
VALUES  ( 'Moscow' , -- Name - nvarchar(200)
          '<ProviderConfiguration xmlns="http://schemas.datacontract.org/2004/07/Csharper.SenderService" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                          <BindingTypes xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet.Request">
                            <a:SmppBind.BindingType>BindAsTransceiver</a:SmppBind.BindingType>
                          </BindingTypes>
                          <DestinationNumberings>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                          </DestinationNumberings>
                          <EnqureLinkInterval>PT30S</EnqureLinkInterval>
                          <Host>fenix.qtelecom.ru</Host>
                          <Password>30522768</Password>
                          <PayloadType>WDPMessage</PayloadType>
                          <Port>8056</Port>
                          <SourceNumberings>
                            <TonNpiPair>
                              <Npi>0</Npi>
                              <Ton>5</Ton>
                            </TonNpiPair>
                            <TonNpiPair>
                              <Npi>1</Npi>
                              <Ton>1</Ton>
                            </TonNpiPair>
                          </SourceNumberings>
                          <SupportedSMPPVersions xmlns:a="http://schemas.datacontract.org/2004/07/RoaminSMPP.Packet">
                            <a:Pdu.SmppVersionType>Version3_4</a:Pdu.SmppVersionType>
                          </SupportedSMPPVersions>
                          <SystemId>12923.1</SystemId>
                          <Support7Bit>true</Support7Bit>
				</ProviderConfiguration>' , -- Configuration - xml
          1.0 , -- QualityMetrics - real
          20.0  -- ChannelBrandwidth - real
        );
        
ALTER TABLE dbo.SMSQueue
ALTER COLUMN short_message VARCHAR(508);

UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  delete /x:ProviderConfiguration/x:Need7BitPacking[1]
  ');

UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  delete /x:ProviderConfiguration/x:Support7Bit[1]
  ');
  
UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  insert <x:Support7Bit>true</x:Support7Bit> after /x:ProviderConfiguration[1]/x:SourceNumberings[1]')
  
UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  insert <x:Need7BitPacking>false</x:Need7BitPacking> after /x:ProviderConfiguration[1]/x:Host[1]')
  
UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  insert <x:TimeShift>0</x:TimeShift> after /x:ProviderConfiguration[1]/x:SystemId[1]')
 
  
/*UPDATE Providers SET Configuration.modify('
  declare namespace x="http://schemas.datacontract.org/2004/07/Csharper.SenderService";
  replace value of (/x:ProviderConfiguration/x:Support7Bit/text())[1]
  with "true"');*/
  
ALTER TABLE SMSTLV
ALTER COLUMN Value VARCHAR(508);

CREATE TABLE CustomFields
(
	Id BIGINT IDENTITY NOT NULL CONSTRAINT PK_CustomFields PRIMARY KEY,
	SmsId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_SMSCustoms FOREIGN KEY REFERENCES SMSQueue(Id),
	[Key] NVARCHAR(300) NOT NULL,
	[Value] NVARCHAR(MAX)
);

CREATE INDEX IDX_CustomsToSms ON CustomFields(SmsId);
CREATE INDEX IDX_CustomsKeys ON CustomFields([Key]);

ALTER TABLE Users
ADD CallbackServiceUrl NVARCHAR(MAX) NULL

UPDATE Users SET CallbackServiceUrl = 'http://localhost:9049/Services/GatewayCallback.svc';
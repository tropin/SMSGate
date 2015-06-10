using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.UnidecodeSharp;
using Csharper.SenderService.DAL;
using RoaminSMPP.Packet.Request;
using RoaminSMPP.Packet;
using System.Xml.Linq;
using RoaminSMPP.Utility;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;
using Csharper.Common;
using Csharper.Common;

namespace Csharper.SenderService
{   
    public class Service
    {
        private static SenderShedullerEntities _context;

        private static DataContractSerializer _addressSerializer = new DataContractSerializer(typeof(DestinationAddress[]));
        private static DataContractSerializer _tlvSerializer = new DataContractSerializer(typeof(Hashtable));
        private static DataContractSerializer _providerConfigurationSerializer = new DataContractSerializer(typeof(ProviderConfiguration));

        private const int SMS_UNICODE_LENGTH = 70;
        private const int SMS_UNICODE_CONCAT_LENGTH = 67;
        private const int SMS_7BIT_LENGTH = 160;
        private const int SMS_8BIT_LENGTH = 140;
        private const int SMS_7BIT_CONCAT_LENGTH = 153;
        private const int SMS_8BIT_CONCAT_LENGTH = 134;

        private static SendingProcessor sender;
        private static CheckStatusProcessor statusChecker;

        private static byte concatRefNum = 0;

        static Service()
        {
            sender = new SendingProcessor(Properties.Settings.Default.BatchSendInterval);
            statusChecker = new CheckStatusProcessor(Properties.Settings.Default.CheckStatusInterval);
        }

        private static SenderShedullerEntities Context
        {
            get
            {
                if (_context == null)
                    _context = new SenderShedullerEntities();
                return _context;
            }
        }

        public Guid? Login(string login, string password)
        {
            return Context.GetUser(login, password).FirstOrDefault();
        }

        public bool HaveSenderName(Guid userId, string senderName)
        {
            return Context.GetSenderNames(userId).Any(name => name.Name == senderName);
        }

        public Guid? SaveUser(string login, string password, bool isEnabled = true, string callbackServiceUrl = null)
        {
            return Context.SaveUser(login, password, isEnabled, callbackServiceUrl).FirstOrDefault();
        }

        public List<IdNameList> GetProviders(Guid userId)
        {
            return Context.GetProviders(userId).ToList();
        }

        public List<SMSSeriesId> SendSms(Guid userId, string senderName, List<string> addresses, string message, 
                                             bool transliterate = false,
                                             bool alertOnDelivery = false,
                                             bool registredDelivery = true,
                                             bool replaceIfPresent = true,
                                             string callbackNumber = null,
                                             string callbackNumberDisplay = null,
                                             Pdu.PriorityType priority = Pdu.PriorityType.Level2,
                                             Pdu.PrivacyType  privacy =  Pdu.PrivacyType.Nonrestricted,                                                                                          
                                             string clientId = null, 
                                             string distributionId = null, 
                                             string smsId = null,
                                             DateTime? deliveryTime = null,
                                             TimeSpan? validalityPeriod = null,
                                             ushort smsSignal = 0,
                                             ushort sourcePort = 0,
                                             ushort userMessageReference = 0,
                                             Dictionary<string,string> customParameters = null)
        {
            if (validalityPeriod == null)
            {
                validalityPeriod = TimeSpan.FromMinutes(10);
            }

            List<SMSSeriesId> result = new List<SMSSeriesId>();
            //Здесь будет получение провайдера для данного пользователя, исходя из имени отправителя, требуемого качества канала, жизни поставщика, статуса пользователя, клиента
            bool useRussian = true;
            //---------------------------------------------------------
            SenderNames sName = Context.GetSenderNames(userId).Where(sn => sn.Name == senderName).OrderByDescending(sn => sn.Providers.ChannelBrandwidth).FirstOrDefault();
            if (sName != null)
            {
            //---------------------------------------------------------
                //--Конфигурация провайдера, должна покрыть некоторые опции----
                ProviderConfiguration configuration = _providerConfigurationSerializer.Deserialize<ProviderConfiguration>(sName.Providers.Configuration);
                Pdu.SmppVersionType smppVersion = configuration.SupportedSMPPVersions.First();
                Pdu.PayloadTypeType payloadType = configuration.PayloadType;

                TonNpiPair outNumbering = sName.Name.IsDigital()?
                    configuration.SourceNumberings.Where(sn=>sn.Ton == Pdu.TonType.International).First():
                    outNumbering = configuration.SourceNumberings.Where(sn=>sn.Ton == Pdu.TonType.Alphanumeric).First();
                TonNpiPair destinationNumbering = configuration.DestinationNumberings
                    .Where(dn=>dn.Ton == Pdu.TonType.International).First();
                //-------------------------------------------------------------
                string[] messages;
                if (!message.ContainsNonASCII() || transliterate)
                {
                    if (transliterate)
                        message = message.Unidecode();
                    if (configuration.Support7Bit)
                    {
                        if (message.Length > SMS_7BIT_LENGTH)
                        {
                            messages = message.SplitByLength(SMS_7BIT_CONCAT_LENGTH);
                        }
                        else
                            messages = message.SplitByLength(SMS_7BIT_LENGTH);
                    }
                    else
                    {
                        if (message.Length > SMS_8BIT_LENGTH)
                            messages = message.SplitByLength(SMS_8BIT_CONCAT_LENGTH);
                        else
                            messages = message.SplitByLength(SMS_8BIT_LENGTH);
                    }
                    useRussian = false;
                }
                else
                {
                    if (message.Length>SMS_UNICODE_LENGTH)
                        messages = message.SplitByLength(SMS_UNICODE_CONCAT_LENGTH);
                    else
                        messages = message.SplitByLength(SMS_UNICODE_LENGTH);
                }
                bool concatenated = messages.Length > 1;
                if (concatenated)
                    concatRefNum = concatRefNum.CycleInc();
                for (byte i = 0; i < messages.Length; ++i)
                {
                    MessageLcd2 req;

                    if (addresses.Count > 1)
                        req = new SmppSubmitMulti();
                    else
                        req = new SmppSubmitSm();

                    byte[] bMessage;
                    
                    if (configuration.Support7Bit && !useRussian)
                    {
                        bMessage = new GSMEncoding().GetBytes(messages[i]);
                        if (configuration.Need7BitPacking)
                            bMessage = GSMEncoding.Encode7Bit(bMessage, concatenated ? 5 : 0);
                    }
                    else if (!useRussian)
                    {
                       bMessage = Encoding.ASCII.GetBytes(messages[i]);
                    }
                    else
                    {
                       bMessage = Encoding.UTF8.GetBytes(messages[i].Endian2UTF());
                    }
                    if (useRussian)
                        req.DataCoding = RoaminSMPP.Packet.Pdu.DataCodingType.Unicode;
                    else 
                        req.DataCoding = Pdu.DataCodingType.SMSCDefault;
                    req.AlertOnMsgDelivery = Convert.ToByte(alertOnDelivery);
                    if (useRussian)
                        req.LanguageIndicator = Pdu.LanguageType.UCS2_16Bit;
                    else if (configuration.Support7Bit)
                        req.LanguageIndicator = Pdu.LanguageType.GSM7BitDefaultAlphabet;
                    else
                        req.LanguageIndicator = Pdu.LanguageType.Unspecified;
                    req.PayloadType = payloadType;
                    req.PriorityFlag = priority;
                    req.PrivacyIndicator = privacy;
                    req.ProtocolId = smppVersion;
                    req.RegisteredDelivery = registredDelivery?
                        Pdu.RegisteredDeliveryType.OnSuccessOrFailure:
                        Pdu.RegisteredDeliveryType.None;
                    req.ReplaceIfPresentFlag = replaceIfPresent;
                    req.ScheduleDeliveryTime = deliveryTime == null ? null : deliveryTime.Value.GetDateString(configuration.TimeShift);
                    req.SmsSignal = smsSignal;
                    req.SourceAddress = sName.Name;
                    req.SourceAddressTon = outNumbering.Ton;
                    req.SourceAddressNpi = outNumbering.Npi;
                    req.SourceAddressSubunit = RoaminSMPP.Packet.Pdu.AddressSubunitType.MobileEquipment;
                    req.SourcePort = sourcePort;
                    req.UserMessageReference = userMessageReference;
                    req.ValidityPeriod = validalityPeriod == null ? null : validalityPeriod.Value.GetDateString();
                    if (!string.IsNullOrEmpty(callbackNumber))
                    {
                        req.CallbackNum = callbackNumber;
                        if (string.IsNullOrEmpty(callbackNumberDisplay))
                            req.CallbackNumAtag = callbackNumberDisplay;
                        req.CallbackNumPresInd = 1;
                    }
                    if (concatenated)
                    {
                        //Добавляем EI UDH для сочленяемых сообщений
                        byte[] concatBytes = new byte[6];
                        concatBytes[0] = 5;
                        concatBytes[1] = 0;
                        concatBytes[2] = 3;
                        concatBytes[3] = concatRefNum;
                        concatBytes[4] = (byte)messages.Length;
                        concatBytes[5] = (byte)(i + 1);
                        byte[] resulting = new byte[bMessage.Length + concatBytes.Length];
                        concatBytes.CopyTo(resulting, 0);
                        bMessage.CopyTo(resulting, concatBytes.Length);
                        bMessage = resulting;
                    }
                    req.ShortMessage = bMessage;
                    if (req is SmppSubmitSm)
                    {
                        SmppSubmitSm simple = req as SmppSubmitSm;
                        simple.DestinationAddress = addresses[0];
                        simple.DestinationAddressTon = destinationNumbering.Ton;
                        simple.DestinationAddressNpi = destinationNumbering.Npi;
                    }
                    if (req is SmppSubmitMulti)
                    {
                        SmppSubmitMulti complex = req as SmppSubmitMulti;
                        complex.DestinationAddresses = addresses.Select
                            (
                               address => new DestinationAddress(destinationNumbering.Ton, destinationNumbering.Npi, address)
                            ).ToArray();
                    }
                    if (concatenated && !string.IsNullOrEmpty(callbackNumber))
                        req.EsmClass = 195;
                    else if (concatenated)
                        req.EsmClass = 67;
                    else if (!string.IsNullOrEmpty(callbackNumber))
                        req.EsmClass = 131;
                    else
                        req.EsmClass = 3;
                    SMSSeriesId id = EnqueueSMS(userId, sName.Id, sName.ProviderId, req, clientId, distributionId, smsId, customParameters);
                    if (!id.Equals(SMSSeriesId.Empty))
                        result.Add(id);
                }
            }
            return result;
        }

        public List<SMSCheckItem> CheckSMSStatus(Guid userId, string clientId = null, string distibutionId = null, string extSmsId = null, Guid? id = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null)
        {
            return
                Context.GetSMSStatuses(userId, clientId, distibutionId, extSmsId, id, dateStart, dateEnd, rowsPerPage, pageNumber, customLimits==null?null: Tools.Merge(customLimits))
                .Select(
                    item => new SMSCheckItem()
                    {
                       Id = item.Id,
                       MessageId = item.ExtSMSId,
                       Status = (SMSStatus)item.Status,
                       LastCheckUTC = item.LastStatusCheck.ToUniversalTime(),
                       Row = item.Row,
                       Records = item.Records
                    }
                )
                .ToList();
        }

        public List<SMSDetail> GetSmsDetalization(Guid userId, string clientId = null, string distibutionId = null, string extSmsId = null, Guid? id = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null)
        {
            return Context.GetSMSDetails(userId, clientId, distibutionId, extSmsId, id, dateStart, dateEnd, rowsPerPage, pageNumber, customLimits == null? null: Tools.Merge(customLimits))
                .Select(rec =>
                    new SMSDetail()
                    {
                        Id = rec.Id,
                        EnqueueTime = rec.EuqueueTime,
                        StatusTime = rec.LastStatusCheck,
                        Status = (SMSStatus)rec.Status,
                        Sender = rec.source_addr,
                        Destination = rec.destination_addr,
                        Text = rec.short_message.GetStringFromHexInCoding(rec.data_coding),
                        Records = rec.Records,
                        Row = rec.Row
                    }
                ).ToList();
        }

        public List<SMSCounter> GetSmsCounter(Guid userId, SMSStatus? status = null, string clientId = null, string distibutionId = null, DateTime? dateStart = null, DateTime? dateEnd = null, Dictionary<string, string> customLimits = null)
        {
            return Context.GetSMSCountInStatus(userId, (short?)status, clientId, distibutionId, dateStart, dateEnd, customLimits == null? null: Tools.Merge(customLimits))
                .Select(rec =>
                    new SMSCounter()
                    {
                        Status = (SMSStatus)rec.Status,
                        Count = rec.Quantity??0
                    }
                ).ToList();
        }

        private SMSSeriesId EnqueueSMS(Guid userId, Guid senderNameId, Guid providerId, MessageLcd2 req, 
                                             string clientId = null, 
                                             string distributionId = null, 
                                             string smsId = null,
                                             Dictionary<string, string> customParameters = null)
        {
            SMSSeriesId result = SMSSeriesId.Empty;
            DestinationAddress[] dadress;
            if (req is SmppSubmitMulti)
                dadress = (req as SmppSubmitMulti).DestinationAddresses;
            else
            {
                SmppSubmitSm simple = req as SmppSubmitSm;
                dadress = new[] 
                { 
                    new DestinationAddress(simple.DestinationAddressTon, simple.DestinationAddressNpi, simple.DestinationAddress)
                };
            }
            var enqueueResult =
            Context.EnqueueSMS(userId, senderNameId, providerId, clientId, distributionId, smsId,
                               (short?)req.SequenceNumber, req.ServiceType, (short?)req.SourceAddressTon, (short?)req.SourceAddressNpi,
                               req.SourceAddress, req is SmppSubmitMulti ? (short?)(req as SmppSubmitMulti).NumberOfDestinations : 1,
                               req.EsmClass, (short?)req.ProtocolId, (short?)req.PriorityFlag, req.ScheduleDeliveryTime, req.ValidityPeriod,
                               (short?)req.RegisteredDelivery, req.ReplaceIfPresentFlag?(short?)1:(short?)0, (short?)req.DataCoding, req.SmDefaultMessageId,
                               req.SmLength, (req.ShortMessage as byte[]).GetHexString(),
                               _tlvSerializer.Serialize(new Hashtable(req.TlvTable.tlvTable)),
                               _addressSerializer.Serialize(dadress),
                               customParameters == null? null : Tools.Merge(customParameters)
                               );
            Guid? baseSmsId = enqueueResult.FirstOrDefault();
            byte position = 0;
            try
            {
                position = req.SarSegmentSeqnum;
            }
            catch{ /*Не выставлена */}
            if (baseSmsId != null)
                result = new SMSSeriesId()
                {
                    Id = baseSmsId.Value,
                    ExternalId = smsId,
                    SeriesPosition = position
                };
            return result;
        }
    }
}

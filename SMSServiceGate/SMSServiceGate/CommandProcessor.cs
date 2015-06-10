using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BinaryAnalysis.UnidecodeSharp;
using SmsService = Csharper.SMSServiceGate.ADEService.ADEService;

namespace Csharper.SMSServiceGate
{
    public static class CommandProcessor
    {
        private static SmsService service = new SmsService();
        private static SessionHolder sh = new SessionHolder();

        internal static Wrappers.SendSMSResp.response ProcessSendSMS(Wrappers.SendSMSReq.request request)
        { 
            Wrappers.SendSMSResp.response resp = new Wrappers.SendSMSResp.response();
            List<XElement> elements = new List<XElement>(); 
            foreach (var destination in request.phone_to)
            {
                string sessionKey = sh.GetSessionKey(request.login, request.pwd, request.originator.FirstOrDefault(source=> source.num_message == destination.num_message)
                    .TypedValue);
                resp.method = "SendSMS";
                string[] sentMessages;
                bool isError = false;
                try
                {
                    sentMessages = service.SendSms(
                                sessionKey: sessionKey,
                                addresses: new[] { destination.TypedValue },
                                messageId: null,
                                distibutionId: null,
                                message: request.message.FirstOrDefault(
                                    mess => mess.num_message == destination.num_message
                                    ).TypedValue,
                                transliterate: false, 
                                transliterateSpecified: false,
                                deliveryTime: null,
                                deliveryTimeSpecified: false,
                                validalityPeriod: null)
                           .Select(message=>message.Id).ToArray();
                }
                catch (Exception ex)
                {
                    sentMessages = new[] { 
                        ex.Message.Split(new string[]{"--->"}, StringSplitOptions.RemoveEmptyEntries)
                        .LastOrDefault()
                        .Trim()
                        .RemoveUnredableSymbols()
                        .Unidecode()
                    };
                    isError = true;
                }
                for (int i = 1; i <= sentMessages.Length; ++i)
                {
                    if (!isError)
                    {
                        elements.Add(new Wrappers.SendSMSResp.response.smsLocalType()
                         {
                             num_message = destination.num_message,
                             parts = (uint)sentMessages.Length,
                             part_no = (uint)i,
                             id = sentMessages[i - 1]
                         }.Untyped);
                    }
                    else
                    {
                        XElement element = new XElement(@"msg");
                        element.Add(
                                new XAttribute(@"num_message", destination.num_message),
                                sentMessages[i - 1]
                            );
                        elements.Add(element);
                    }
                }   
            }
            elements.ForEach(resp.Untyped.Add);
            return resp; 
        }

        internal static Wrappers.CheckSMSResp.response ProcessCheckSMS(Wrappers.CheckSMSReq.request request)
        {
            Wrappers.CheckSMSResp.response resp = new Wrappers.CheckSMSResp.response();
            resp.method = "CheckSMSFull";
            foreach (var checkInstr in request.sms_id)
            {
                var status = service.CheckSMS(request.login, request.pwd, checkInstr.ToString());
                
                resp.sms.Add(new Wrappers.CheckSMSResp.response.smsLocalType()
                {
                    id = checkInstr,
                    state_id = status.Status.GetStatus(),
                    state_update = status.LastCheckUTC.ToString("yyyy-MM-dd hh:mm:ss"),
                });   
            }
            return resp;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Linq;
using Csharper.SMSServiceGate.Properties;
using System.Diagnostics;
using System.Xml;

namespace Csharper.SMSServiceGate
{
    /// <summary>
    /// Summary description for BatchSend
    /// </summary>
    public class BatchSend : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Csharper.SMSServiceGate.Wrappers.response err_resp = new Wrappers.response();
            err_resp.method = "ErrorSMS";
            err_resp.msg = Resources.Error_Unknown;
            string responce = err_resp.ToString();
            try
            {
                XDocument doc = XDocument.Load(XmlReader.Create(context.Request.InputStream));
                XAttribute attr = doc.Root.Attribute(@"method");
                if (attr != null)
                {

                    switch (attr.Value)
                    {
                        case "SendSMS":
                            responce = CommandProcessor.ProcessSendSMS(Csharper.SMSServiceGate.Wrappers.SendSMSReq.request.Load(new StringReader(doc.ToString())))
                                .ToString();
                            break;
                        case "CheckSMSFull":
                            responce = CommandProcessor.ProcessCheckSMS(Csharper.SMSServiceGate.Wrappers.CheckSMSReq.request.Load(new StringReader(doc.ToString())))
                                .ToString();
                            break;
                        default:
                            err_resp.msg = Resources.Error_UnknownCommand;
                            responce = err_resp.ToString();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                err_resp.msg = ex.Message;
                Trace.TraceError(ex.ToString());
                responce = err_resp.ToString();
            }
            context.Response.Write(responce);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
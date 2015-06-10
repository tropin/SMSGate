using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoaminSMPP.Utility;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;
using Csharper.SenderService;
using RoaminSMPP.Packet;
using System.Xml;
using Csharper.Common;

namespace SerializationTest
{
    public class Program
    {
        public static int Main(params string[] clparams)
        {
            string test = "This must be one complete sms message encoded by 7bit alphabet wihtout any parts or chunks it must be onecompete long test without interruptions or any other th";
            GSMEncoding gsmEnc = new GSMEncoding();
            Encoding utf8Enc = new System.Text.UTF8Encoding();
            byte[] gsmBytes = utf8Enc.GetBytes(test);
            byte[] utf8Bytes = Encoding.Convert(gsmEnc, utf8Enc, gsmBytes);
            test = utf8Enc.GetString(utf8Bytes);

            DataContractSerializer scs = new DataContractSerializer(typeof(DestinationAddress[])); 
            DataContractSerializer scs2 = new DataContractSerializer(typeof(Hashtable));
            DataContractSerializer scs3 = new DataContractSerializer(typeof(ProviderConfiguration));
            TlvTable tlbTab = new TlvTable();
            tlbTab.SetOptionalParamString(0x0501, "1234");
            tlbTab.SetOptionalParamString(0x0006, "4321");

            DestinationAddress[] dadress = new RoaminSMPP.Utility.DestinationAddress[]
            {
                new DestinationAddress(RoaminSMPP.Packet.Pdu.TonType.International, RoaminSMPP.Packet.Pdu.NpiType.National, "79225443636"),
                new DestinationAddress(RoaminSMPP.Packet.Pdu.TonType.International, RoaminSMPP.Packet.Pdu.NpiType.National, "79225443637")
            };

            ProviderConfiguration pc = new ProviderConfiguration()
            {
                BindingTypes = new List<RoaminSMPP.Packet.Request.SmppBind.BindingType>(new[] { RoaminSMPP.Packet.Request.SmppBind.BindingType.BindAsTransceiver }),
                SourceNumberings = new List<TonNpiPair>
                (
                    new[] {
                            new TonNpiPair(Pdu.TonType.Alphanumeric, Pdu.NpiType.Unknown), //Цифры и буквы                    
                            new TonNpiPair(Pdu.TonType.International, Pdu.NpiType.ISDN), //Международный формат
                            new TonNpiPair(Pdu.TonType.NetworkSpecific, Pdu.NpiType.Unknown), //Короткий номер
                            new TonNpiPair(Pdu.TonType.Unknown, Pdu.NpiType.ISDN) //Внутренние номера
                          }
                ),
                DestinationNumberings = new List<TonNpiPair>
                (
                    new[] {
                            new TonNpiPair(Pdu.TonType.International, Pdu.NpiType.ISDN) //Международный формат
                          }
                ),
                EnqureLinkInterval = new TimeSpan(0, 0, 30),
                PayloadType = Pdu.PayloadTypeType.WDPMessage,
                SupportedSMPPVersions = new List<Pdu.SmppVersionType>(new[] { Pdu.SmppVersionType.Version3_4 }),
                SystemId = "test.csharper.ru",
                Password = "4wangn6z",
                Host = "213.242.207.57",
                Port = 2775,
                TimeShift = -3
            };

            Stream stream = /*File.OpenWrite(@"D:\Temp.txt");*/
                Console.OpenStandardOutput();
            XmlWriter xw = XmlWriter.Create(stream, new XmlWriterSettings()
            {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Auto
            });
            scs.WriteObject(xw, dadress);
            Console.WriteLine();
            scs2.WriteObject(xw, new Hashtable(tlbTab.tlvTable));
            Console.WriteLine();
            scs3.WriteObject(xw, pc);
            xw.Close();
            Console.WriteLine();
            Console.ReadKey();                          
            return 0;
        }
    }
}

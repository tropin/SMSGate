using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using RoaminSMPP.Packet;
using System.Globalization;
using Csharper.Common;

namespace Csharper.SenderService
{
    public static class Extensions
    {
        private static readonly Regex ALPHANUMERIC = new Regex(@"([a-zA-Z\d]+){1}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex SHORTCODE = new Regex(@"\d{1,5}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static TonNpiPair GetNpiTonPair(this IEnumerable<TonNpiPair> available, string senderName)
        {
            TonNpiPair result = null;
            if (ALPHANUMERIC.IsMatch(senderName))
            {
                result = available.Where(avail =>
                    avail.Npi == RoaminSMPP.Packet.Pdu.NpiType.Unknown &&
                    avail.Ton == RoaminSMPP.Packet.Pdu.TonType.Alphanumeric)
                    .FirstOrDefault();
            }
            else if (SHORTCODE.IsMatch(senderName))
            {
                result = available.Where(avail =>
                    avail.Npi == RoaminSMPP.Packet.Pdu.NpiType.Unknown &&
                    avail.Ton == RoaminSMPP.Packet.Pdu.TonType.NetworkSpecific)
                    .FirstOrDefault();
            }
            else if (senderName.StartsWith("+"))
                result = available.Where(avail =>
                    avail.Npi == RoaminSMPP.Packet.Pdu.NpiType.ISDN &&
                    avail.Ton == RoaminSMPP.Packet.Pdu.TonType.International)
                    .FirstOrDefault();
            else if (string.IsNullOrWhiteSpace(senderName))
            {
                result = available.Where(avail =>
                    avail.Npi == RoaminSMPP.Packet.Pdu.NpiType.Unknown &&
                    avail.Ton == RoaminSMPP.Packet.Pdu.TonType.Unknown)
                    .FirstOrDefault();
            }
            else
            {
                result = available.Where(avail =>
                    avail.Npi == RoaminSMPP.Packet.Pdu.NpiType.ISDN &&
                    avail.Ton == RoaminSMPP.Packet.Pdu.TonType.Unknown)
                    .FirstOrDefault();
            }
            return result;
        }

        public static SMSStatus ConvertStatus(this Pdu.MessageStateType type)
        {
            SMSStatus result;
            switch (type)
            {
                case Pdu.MessageStateType.Accepted:
                    result = SMSStatus.Send;
                    break;
                case Pdu.MessageStateType.Deleted:
                    result = SMSStatus.Cancelled;
                    break;
                case Pdu.MessageStateType.Delivered:
                    result = SMSStatus.Delivered;
                    break;
                case Pdu.MessageStateType.Enroute:
                    result = SMSStatus.Send;
                    break;
                case Pdu.MessageStateType.Expired:
                    result = SMSStatus.Cancelled;
                    break;
                case Pdu.MessageStateType.Rejected:
                    result = SMSStatus.ValidationError;
                    break;
                case Pdu.MessageStateType.Undeliverable:
                    result = SMSStatus.SendError;
                    break;
                case Pdu.MessageStateType.Unknown:
                    result = SMSStatus.Send;
                    break;
                default:
                    result = SMSStatus.Send;
                    break;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Globalization;

namespace Csharper.Common
{
    public static class Extensions
    {
        private static readonly Random RANDOM = new Random();
        
        /// <summary>
        /// Получить 16x отображение байтового массива
        /// </summary>
        /// <param name="byteKey"></param>
        /// <returns></returns>
        public static string Stringify(this byte[] byteKey)
        {
            return Convert.ToBase64String(byteKey);
        }

        public static byte[] GetBytes(this string stringKey)
        {
            return Convert.FromBase64String(stringKey);
        }

        public static string GetStringFromHexInCoding(this string hexString, int dataCoding)
        {
            string result = string.Empty;
            byte[] bytes = hexString.GetBytesFromHex();
            //сначала надо определить не конкат ли случаем?
            if (bytes.Length > 6 && bytes[0] == 5 && bytes[1] == 0 && bytes[2] == 3)
            {
                //Конкат, убираем лишние 6 байт в начале
                bytes = bytes.Skip(6).ToArray();
            }
            if (dataCoding == 8)
            {
                //Unicode
                result = Encoding.UTF8.GetString(bytes).UTF2Endian();
            }
            else
            {
                //8bit
                result = new GSMEncoding().GetString(bytes);
            }
            return result;
        }

        public static byte CycleInc(this byte value)
        {
            if (value == byte.MaxValue)
            {
                return 0;
            }
            else return (byte)(value + 1);
        }

        public static bool ContainsNonASCII(this string content)
        {
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(content)).Contains("?");
        }

        public static ushort GetRandom()
        {
            return (ushort)RANDOM.Next(ushort.MinValue, ushort.MaxValue);
        }

        public static string Serialize(this DataContractSerializer serializer, object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(this DataContractSerializer serializer, string serialized)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string GetDateString(this DateTime pTime, int shifting)
        {
            if (pTime == DateTime.MinValue || pTime == null)
                return "";
            return pTime.ToString(string.Format("yyMMddHHmmss0{0}{1}", Math.Abs(shifting), shifting >= 0 ? "+" : "-"));
        }

        public static string GetDateString(this TimeSpan pSpan)
        {
            if (pSpan == TimeSpan.MinValue || pSpan == null)
                return "";
            return string.Format("0000{0}{1}000R", (pSpan.Days > 31 ? 31 : pSpan.Days).ToString("00"), pSpan.ToString("hhmmss"));
        }

        public static byte GetDataCoding(this string text)
        {
            if (UTF2Endian(text).Length != ASCII2Endian(text).Length)
            {
                return 8;
            }
            else
            {
                return 0;
            }
        }

        public static string UTF2Endian(this string s)
        {
            Encoding ui = Encoding.BigEndianUnicode;
            Encoding u8 = Encoding.UTF8;
            return ui.GetString(u8.GetBytes(s));
        }

        public static string Endian2UTF(this string s)
        {
            Encoding ui = Encoding.BigEndianUnicode;
            Encoding u8 = Encoding.UTF8;
            return u8.GetString(ui.GetBytes(s));
        }


        public static string ASCII2Endian(this string s)
        {
            Encoding ui = Encoding.BigEndianUnicode;
            Encoding a = Encoding.ASCII;
            return ui.GetString(a.GetBytes(s));
        }

        public static string Endian2ASCII(this string s)
        {
            Encoding ui = Encoding.BigEndianUnicode;
            Encoding a = Encoding.ASCII;
            return a.GetString(ui.GetBytes(s));
        }

        public static bool IsDigital(this string s)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"^[\d]+$");
            return re.Match(s).Success;
        }

        public static string GetHexString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            if (bytes != null)
            {
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("X2"));
                }
            }
            return sb.ToString();
        }

        public static byte[] GetBytesFromHex(this string hexString)
        {
            int i = 0;
            List<byte> result = new List<byte>();
            while (i < hexString.Length)
            {
                result.Add(byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber));
                i = i + 2;
            }
            return result.ToArray();
        }

        

        public static string[] SplitByLength(this string content, int itemLength, bool byWord = false)
        {
            List<string> items = new List<string>();
            if (content.Length <= itemLength)
            {
                items.Add(content);
            }
            else
            {
                char[] chars = content.ToArray();
                while (chars.Length != 0)
                {
                    int indexTo = Math.Min(itemLength - 1, chars.Length - 1);
                    if (chars[indexTo] != ' ' && byWord)
                    {
                        for (int i = indexTo; i >= 0; --i)
                        {
                            if (chars[i] == ' ')
                            {
                                indexTo = i;
                            }
                        }
                    }
                    items.Add(new String(chars.Take(indexTo + 1).ToArray()));
                    chars = chars.Skip(indexTo + 1).ToArray();
                }
            }
            return items.ToArray();
        }
    }
}
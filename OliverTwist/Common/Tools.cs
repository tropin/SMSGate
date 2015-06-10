using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Csharper.Common
{
    public static class Tools
    {
        private const string MergeSymbol = ";";
        private const string KeyValueSymbol = "=";
        
        private static string GetHtmlUtfChars(string value)
        {
            string encoded = string.Empty;
            foreach (char ch in value)
            {
                encoded += string.Format("&#{0}#", ((int)ch).ToString());
            }
            return encoded;
        }
        
        public static string EscapeSpecialSymbols(string value, params string[] specialSymbols)
        {
            string result = value;
            foreach (string symbol in specialSymbols)
            {
                result = result.Replace(symbol, GetHtmlUtfChars(symbol));
            }
            return result;
        }
        
        public static string Merge(Dictionary<string,string> values)
        {
            return Merge(values.Select(x => EscapeSpecialSymbols(x.Key, KeyValueSymbol) + KeyValueSymbol + EscapeSpecialSymbols(x.Value, KeyValueSymbol)).ToList());
        }

        public static string Merge(IList values)
        {
            if (values == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            if (values.Count > 0)
            {
                sb.Append(EscapeSpecialSymbols(values[0].ToString(), MergeSymbol));
                for (int i = 1; i < values.Count; i++)
                {
                    sb.Append(MergeSymbol);
                    sb.Append(EscapeSpecialSymbols(values[i].ToString(), MergeSymbol));
                }
            }
            return sb.ToString();
        }
    }
}

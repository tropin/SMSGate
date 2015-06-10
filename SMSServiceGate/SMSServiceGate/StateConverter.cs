using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Csharper.SMSServiceGate.ADEService;

namespace Csharper.SMSServiceGate
{
    public static class StateConverter
    {
        public static string RemoveUnredableSymbols(this string s)
        {
            StringBuilder newresult = new StringBuilder(); 
            try
            {
                foreach (char c in s)
                {
                    if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                        newresult.Append(c);
                }
            }
            catch 
            {
                //Совершенно не интересны падения
            }
            return newresult.ToString();
        }

        public static string GetStatus(this SMSStatus statusCode)
        {
            string result = "partly_deliver";
            switch (statusCode)
            {
                case SMSStatus.Cancelled:
                    //return 'Сообщение отменено';
                    result = "not_deliver";
                    break;

                case SMSStatus.CancellationPending:
                    //return 'Сообщение отменяется';
                    result = "not_deliver";
                    break;

                case SMSStatus.InQueue:
                    //return 'Сообщение находится в очереди';
                    result = "partly_deliver";
                    break;

                case SMSStatus.Send:
                    //return 'Сообщение отправлено';
                    result = "partly_deliver";
                break;

                case SMSStatus.Delivered:
                    //return 'Сообщение доставлено';
                    result = "deliver";
                break;

                case SMSStatus.ValidationError:
                //return 'Ошибка валидации запроса';
                result = "not_deliver";
                break;

                case SMSStatus.SendError:
                    //return 'Ошибка: сообщение отклонено СМС центром';
                    result = "not_deliver";
                break;

                default:
                    //return 'Статус не распознан';
                    result = "not_deliver";
                break;
            }
            return result;
        }
    }
}
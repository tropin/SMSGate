using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Csharper.Common;

namespace Csharper.OliverTwist.Services
{
    [ServiceContract]
    public interface IADEService
    {
        /// <summary>
        /// Вход на сервис
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <param name="senderName">Имя отправителя</param>
        /// <returns>Сессионный ключ</returns>
        [OperationContract]
        string Login(string login, string password, string senderName);

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="addresses">Адреса доставки сообщения</param>
        /// <param name="message">Сообщение</param>
        /// <param name="distibutionId">Идентификатор рассылки (необязателен)</param>
        /// <param name="messageId">Идентификатор сообщения (необязателен)</param>
        /// <param name="transliterate">Транслитерация (необязательна, по-умолчанию выключена)</param>
        /// <param name="deliveryTime">Время доставки (необязательно, по-умолчанию немедленно)</param>
        /// <param name="validalityPeriod">Время жизни (необязательно, по-умолчанию неделя)</param>
        /// <returns>Информация об отправленных смс</returns>
        [OperationContract]
        List<SMSSeriesId> SendSms(string sessionKey, List<string> addresses, string message, string distibutionId = null, string messageId = null, bool transliterate = false, DateTime? deliveryTime = null,
                                             TimeSpan? validalityPeriod = null);

        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="distributionId">Идентификатор рассылки</param>
        /// <param name="rowsPerPage">Строк на странице</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <returns>Список статусов</returns>
        [OperationContract]
        List<SMSCheckItem> CheckSMSByDistribution(string sessionKey, string distributionId, long? rowsPerPage = null, long? pageNumber = null);

        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="messageId">Номер сообщения</param>
        /// <param name="rowsPerPage">Строк на странице</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <returns>Список статусов</returns>
        [OperationContract]
        List<SMSCheckItem> CheckSMSByMessageId(string sessionKey, string messageId, long? rowsPerPage = null, long? pageNumber = null);

        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="Id">Уникальный идентификатор сообщения</param>
        /// <returns>Статус</returns>
        [OperationContract]
        SMSCheckItem CheckSMSById(string sessionKey, Guid id);

        /// <summary>
        /// Получение статуса СМС прямым запросом
        /// </summary>
        /// <param name="Id">Идентификатор СМС</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Статус</returns>
        [OperationContract]
        SMSCheckItem CheckSMS(string login, string password, Guid Id);

        /// <summary>
        /// Выход с сервиса
        /// </summary>
        /// <param name="sessionKey"></param>
        [OperationContract]
        void LogOff(string sessionKey);
    }
}

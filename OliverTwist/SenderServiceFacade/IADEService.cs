using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Csharper.SenderService;
using Csharper.SenderService.DAL;
using Csharper.Common;

namespace Csharper.SenderServiceFacade
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
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <param name="distibutionId">Идентификатор рассылки (необязателен)</param>
        /// <param name="messageId">Идентификатор сообщения (необязателен)</param>
        /// <param name="transliterate">Транслитерация (необязательна, по-умолчанию выключена)</param>
        /// <param name="deliveryTime">Время доставки (необязательно, по-умолчанию немедленно)</param>
        /// <param name="validalityPeriod">Время жизни (необязательно, по-умолчанию неделя)</param>
        /// <param name="customParameters">Дополнительная информация с СМС</param>
        /// <returns>Информация об отправленных смс</returns>
        [OperationContract]
        List<SMSSeriesId> SendSms(string sessionKey, List<string> addresses, string message, string clientId = null, string distibutionId = null, string messageId = null, bool transliterate = false, DateTime? deliveryTime = null,
                                             TimeSpan? validalityPeriod = null, Dictionary<string, string> customParameters = null);

        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <param name="distributionId">Идентификатор рассылки (необязателен)</param>
        /// <param name="rowsPerPage">Строк на странице</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="dateStart">Дата начала отчета</param>
        /// <param name="dateEnd">Дата окончания отчета</param>
        /// <param name="customLimits">Список прямых равенств доп. параметров</param>
        /// <returns>Список статусов</returns>
        [OperationContract]
        List<SMSCheckItem> CheckSMSStatuses(string sessionKey, string clientId = null, string distributionId = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null);

        /// <summary>
        /// Получение счетчиков по статусам СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="status">Интересующий статус (необязателен)</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <param name="distibutionId">Идентификатор рассылки (необязателен)</param>
        /// <param name="dateStart">Дата начала отчета</param>
        /// <param name="dateEnd">Дата окончания отчета</param>
        /// <param name="customLimits">Список прямых равенств доп. параметров</param>
        /// <returns>Счетчики СМС по статусам</returns>
        [OperationContract]
       List<SMSCounter> GetSMSCounters(string sessionKey, SMSStatus? status = null, string clientId = null, string distibutionId = null, DateTime? dateStart = null, DateTime? dateEnd = null, Dictionary<string, string> customLimits = null);

        /// <summary>
        /// Получение детализации по СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <param name="distibutionId">Идентификатор рассылки (необязателен)</param>
        /// <param name="extSmsId">Идентификатор СМС во внешней системе</param>
        /// <param name="id">Идентификатор СМС</param>
        /// <param name="dateStart">Дата начала отчета</param>
        /// <param name="dateEnd">Дата окончания отчета</param>
        /// <param name="rowsPerPage">Строк на странице</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="customLimits">Список прямых равенств доп. параметров</param>
        /// <returns></returns>
        [OperationContract]
        List<SMSDetail> GetSMSDetalization(string sessionKey, string clientId = null, string distibutionId = null, string extSmsId = null, Guid? id = null, DateTime? dateStart = null, DateTime? dateEnd = null, long? rowsPerPage = null, long? pageNumber = null, Dictionary<string, string> customLimits = null);


        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="messageId">Номер сообщения</param>
        /// <param name="rowsPerPage">Строк на странице</param>
        /// <param name="pageNumber">Номер страницы</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <returns>Список статусов</returns>
        [OperationContract]
        List<SMSCheckItem> CheckSMSByMessageId(string sessionKey, string messageId, long? rowsPerPage = null, long? pageNumber = null, string clientId = null);

        /// <summary>
        /// Получение статусов СМС
        /// </summary>
        /// <param name="sessionKey">Сессия работы с сервисом</param>
        /// <param name="Id">Уникальный идентификатор сообщения</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <returns>Статус</returns>
        [OperationContract]
        SMSCheckItem CheckSMSById(string sessionKey, Guid id, string clientId = null);

        /// <summary>
        /// Получение статуса СМС прямым запросом
        /// </summary>
        /// <param name="Id">Идентификатор СМС</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="clientId">Идентификатор клиента (необязателен)</param>
        /// <returns>Статус</returns>
        [OperationContract]
        SMSCheckItem CheckSMS(string login, string password, Guid Id, string clientId = null);

        /// <summary>
        /// Выход с сервиса
        /// </summary>
        /// <param name="sessionKey"></param>
        [OperationContract]
        void LogOff(string sessionKey);
    }
}

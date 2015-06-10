using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcFlan.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csharper.OliverTwist.Model
{
    /// <summary>
    /// Модель записи в истории счета.
    /// </summary>
    public class ClientAccountActionModel
    {
        /// <summary>
        /// Дата и время проведения операции.
        /// </summary>
        [SearchFilter]
        [OrderBy(IsDefault = true)]
        [DisplayName("Дата операции")]
        public DateTime? VersionDate { get; set; }

        /// <summary>
        /// Id записи в истории.
        /// </summary>
        [ScaffoldColumn(false)]
        public long VersionId { get; set; }

        /// <summary>
        /// Id счета клиента.
        /// </summary>
        [ScaffoldColumn(false)]
        public long AccounId { get; set; }

        /// <summary>
        /// Id стоимости смс.
        /// </summary>
        [ScaffoldColumn(false)]
        public long? CostRangeId { get; set; }

        /// <summary>
        /// Баланс.
        /// </summary>
        [OrderBy]
        [DisplayName("Баланс")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Изменение баланса.
        /// </summary>
        [OrderBy]
        [DisplayName("Изменение баланса")]
        public decimal AmountDelta { get; set; }

        /// <summary>
        /// Комментарий к операции.
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Id  менеджера, зачислившего на баланс.
        /// </summary>
        [ScaffoldColumn(false)]
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// Менеджер, произведший операцию зачисления.
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Пользователь")]
        public string ManagerName { get; set; }

        /// <summary>
        /// Id клиента.
        /// </summary>
        [ScaffoldColumn(false)]
        public long? RealClientId { get; set; }

        /// <summary>
        /// Клиент.
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Клиент")]
        public string RealClientName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ScaffoldColumn(false)]
        public long? OperationalClientId { get; set; }

        [SearchFilter]
        [OrderBy]
        [DisplayName("От имени клиента")]
        public string OperationalClientName { get; set; }

        /// <summary>
        /// Id рассылки.
        /// </summary>
        [ScaffoldColumn(false)]
        public long? DistributionId { get; set; }

        /// <summary>
        /// Счет клиента, на который была перечислена дельта.
        /// </summary>
        [ScaffoldColumn(false)]
        public long? TargetAccountId { get; set; }

        [SearchFilter]
        [ScaffoldColumn(false)]
        public string TargetAccountOrganizationName { get; set; }

        /// <summary>
        /// Внесенная сумма.
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Внесенная сумма")]
        public decimal? MoneyVolume { get; set; }

        /// <summary>
        /// Стоимость одной СМС.
        /// </summary>
        [SearchFilter]
        [OrderBy]
        [DisplayName("Стоимость одной СМС")]
        public decimal? QuickCost { get; set; }

    }
}

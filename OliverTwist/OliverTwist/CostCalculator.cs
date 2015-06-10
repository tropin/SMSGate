using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist
{
    public class CostCalculator
    {
        private CostCalculatorMode _mode;

        public CostCalculator(CostCalculatorMode mode)
        {
            _mode = mode;
        }

        public void RecalcModel(ChangeClientAccountModel account)
        {
            switch (_mode)
            {
                case CostCalculatorMode.FixedMoney:
                    account.OneSMSCost = account.InputMoney/account.AddingAmount;
                    break;
                default: //Если фиксированная цена или количество СМС
                    account.InputMoney = account.AddingAmount * account.OneSMSCost;
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OliverTwist.Converters;
using System.Linq.Expressions;
using LinqKit;

namespace Csharper.OliverTwist.Model.Extensions
{
    public static class ModelExtensions
    {       

        public static Expression<Func<AddressBook, AddressModel>> AddressModelSelectExpression
        {
            get
            {
                return address => new AddressModel()
                {
                    Id = address.Id,
                    BirthDay = address.BirthDay,
                    City = address.City,
                    Description = address.Description,
                    FirstName = address.FirstName,
                    InGroups = address.Assigments.GetGroups(),
                    LastName = address.LastName,
                    MiddleName = address.MiddleName,
                    MobilePhone = address.MobilePhone,
                    Sex = address.Sex,
                    TimeZone = address.TimeZone
                };
            }
        }

        public static Expression<Func<CostRange, CostRangeModel>> CostRangeModelSelectExpression
        {
            get
            {
                return costRange => new CostRangeModel()
                {
                    Id = costRange.Id,
                    Cost = costRange.Cost,
                    Volume = costRange.Volume,
                    LowerSum = costRange.LowerSumm ?? 0
                };
            }
        }

        public static AddressModel ToModel(this AddressBook address)
        {
            return AddressModelSelectExpression.Invoke(address);
        }

        public static Dictionary<long, string> GetGroups(this IEnumerable<Groups2Address> assigments)
        {
            Dictionary<long, string> result = new Dictionary<long, string>();
            foreach (Groups2Address assigment in assigments)
            {
                result.Add(assigment.Group.Id, assigment.Group.Name);
            }
            return result;
        }

        public static AddressBook ToDSO(this AddressModel model, AddressBook targetDSO)
        {
            targetDSO.Id = model.Id.HasValue ? model.Id.Value : 0;
            targetDSO.FirstName = model.FirstName;
            targetDSO.MiddleName = model.MiddleName;
            targetDSO.LastName = model.LastName;
            targetDSO.MobilePhone = model.MobilePhone;
            targetDSO.Sex = model.Sex;
            targetDSO.Description = model.Description;
            targetDSO.BirthDay = model.BirthDay;
            targetDSO.City = model.City;
            targetDSO.TimeZone = model.TimeZone;
            return targetDSO;
        }

        public static UserProfile ToDSO(this UserProfileModel model, UserProfile targetDSO)
        {
            targetDSO.UserId = model.UserId;
            targetDSO.TimeZone = model.TimeZone;
            targetDSO.Sex = model.Sex;
            targetDSO.MobilePhone = model.MobilePhone;
            targetDSO.MiddleName = model.MiddleName;
            targetDSO.LastName = model.LastName;
            targetDSO.FirstName = model.FirstName;
            targetDSO.City = model.City;
            return targetDSO;
        }

        public static Client ToDSO(this ClientModel model, Client targetDSO, Client operationalUser)
        {
            targetDSO.Id = model.Id.HasValue ? model.Id.Value : 0;
            targetDSO.OrganizationName = model.OrganizationName;
            targetDSO.Status = model.Status ?? ClientStatus.NotActive;
            targetDSO.DeallerOfClientId = model.IsDealler.HasValue && model.IsDealler.Value && model.Id.HasValue ?
                    operationalUser.Id : (long?)null;
            if (operationalUser != null && !model.Id.HasValue)
            {
                //Создаем пользователя, но у нас есть инициализрованный OperationalUser
                targetDSO.CreatedByClientId = operationalUser.Id;
            }
            return targetDSO;
        }

        public static CostRangeModel ToModel(this CostRange costRange)
        {
            return CostRangeModelSelectExpression.Invoke(costRange);
        }
    }
}

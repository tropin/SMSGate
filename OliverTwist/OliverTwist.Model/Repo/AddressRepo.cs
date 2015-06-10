using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;
using OliverTwist.Converters;
using Csharper.OliverTwist.Model.Extensions;
using System.Linq.Expressions;
using LinqKit;
using System.Web.Mvc;
using System.IO;
using System.Reflection;

namespace Csharper.OliverTwist.Repo
{
    public class AddressRepo: RepoBase, IAddressRepo
    {
        protected AddressRepo(string loginedUserName, long loginedClientId, long operationalClientId)
            : base(loginedUserName, loginedClientId, operationalClientId)
        {
        }

        #region IAddressRepo Members

        public IQueryable<AddressBook> GetAdresses(long? groupId = null)
        {
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                return from address in DataContext.AddressBooks
                       let groups = from grp in DataContext.AddressGroups
                                    where grp.Id == groupId || grp.MasterAssigments.Any(assigment => assigment.GroupId == groupId && !assigment.IsDeleted)
                                    select grp.Id
                       where address.ClientId == OperationalClientId && !address.IsDeleted&&
                             (!groupId.HasValue || address.Assigments.Any(assigment => !assigment.IsDeleted && groups.Contains(assigment.GroupId)))
                       select address;
            }
        }

        public IQueryable<AddressModel> GetAdressesProjected(long? groupId = null)
        {
            return GetAdresses(groupId).Select(ModelExtensions.AddressModelSelectExpression);
        }

        public AddressModel GetAdressProjected(long addressId)
        {
            AddressModel result = null;
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                AddressBook ab = DataContext.AddressBooks.FirstOrDefault(address => address.Id == addressId && address.ClientId == OperationalClientId && !address.IsDeleted);
                if (ab != null)
                {
                    result = ab.ToModel();
                }
                return result;
            }
        }

        public void SaveAddress(AddressModel address)
        {
            AddressBook modifying = null;
            if (OperationalClientId.HasValue)
            {
                if (address.Id.HasValue)
                {
                    modifying = DataContext.AddressBooks.FirstOrDefault(ab => ab.Id == address.Id && ab.ClientId == OperationalClientId && !ab.IsDeleted);
                }
                else
                {
                    modifying = new AddressBook();
                    modifying.ClientId = OperationalClientId.Value;
                    DataContext.AddressBooks.InsertOnSubmit(modifying);
                }
                if (modifying != null)
                {
                    address.ToDSO(modifying);
                    DataContext.SubmitChanges();
                }
            }
        }

        public void DeleteAddress(long addressId)
        {
            AddressBook deleting = null;
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                deleting = DataContext.AddressBooks.FirstOrDefault(ab => ab.Id == addressId && ab.ClientId == OperationalClientId && !ab.IsDeleted);
            }
            if (deleting != null)
            {
                deleting.IsDeleted = true;
                DataContext.SubmitChanges();
            }   
        }

        public List<AddressGroupModel> GetGroups(long id)
        {
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                List<AddressGroupModel> groups;
                if (id == 0) //Корневая группа
                {
                    groups = DataContext.AddressGroups.Where(
                                            group => group.ClientId == OperationalClientId && !group.IsDeleted
                                                && group.MasterAssigments.Count == 0).Select(AddressGroupToModel).ToList();
                }
                else
                {
                   groups = 
                       (from itemGroup in DataContext.AddressGroups
                        join assigment in DataContext.Groups2Addresses
                        on itemGroup.Id equals assigment.SlaveGroupId
                        where itemGroup.ClientId == OperationalClientId && !itemGroup.IsDeleted &&
                        assigment.GroupId == id
                        select itemGroup
                        ).Select(AddressGroupToModel).ToList();                                                                      
                }
                return groups;
            }
        }

        private static Func<AddressGroup, AddressGroupModel> AddressGroupToModel =
                              group => new AddressGroupModel()
                                {
                                    data = group.Name,
                                    state = group.Assigments.Where(assigment => assigment.SlaveGroupId.HasValue).Count()>0?                                  
                                        GroupState.closed.ToString(): string.Empty,
                                    attr = new { Id = group.Id }
                                };

        
        public long SaveGroup(long? groupId, string groupName, long? parentGroupId)
        {
            AddressGroup modifying = null;
            long id = -1;
            if (OperationalClientId.HasValue)
            {
                if (groupId.HasValue)
                {
                    modifying = DataContext.AddressGroups.FirstOrDefault(group => group.Id == groupId && group.ClientId == OperationalClientId && !group.IsDeleted);
                }
                else
                {
                    modifying = new AddressGroup();
                    modifying.ClientId = OperationalClientId.Value;
                    DataContext.AddressGroups.InsertOnSubmit(modifying);
                }
                if (modifying != null)
                {
                    if (!string.IsNullOrEmpty(groupName))
                        modifying.Name = groupName;
                    DataContext.SubmitChanges();
                    if (parentGroupId.HasValue)
                    {
                        modifying.MasterAssigments.Where(assigment => !assigment.IsDeleted)
                            .ToList().ForEach(assigment => assigment.IsDeleted = true);
                        modifying.MasterAssigments.Add(
                            new Groups2Address()
                            {
                                SlaveGroupId = modifying.Id,
                                GroupId = parentGroupId.Value
                            }
                           );
                        DataContext.SubmitChanges();
                    }
                    id = modifying.Id;
                }
            }
            return id; 
        }

        public void DeleteGroup(long groupId, bool IsWithChildren = false)
        {
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                AddressGroup deleting = DataContext.AddressGroups.FirstOrDefault(group => group.Id == groupId && group.ClientId == OperationalClientId && !group.IsDeleted);
                if (deleting != null)
                {
                    var innerGroups = deleting.Assigments.Where(assigment => !assigment.IsDeleted).Select(assigment => assigment.SlaveGroup);
                    foreach (var group in innerGroups)
                    {
                        var disconnect = group.MasterAssigments.Where(assigment => assigment.GroupId == deleting.Id);
                        foreach (var link in disconnect)
                            link.IsDeleted = true;
                        foreach (var parentLink in deleting.MasterAssigments)
                        {
                            group.MasterAssigments.Add(new Groups2Address()
                            {
                                GroupId = parentLink.GroupId,
                                SlaveGroupId = group.Id,
                            });
                        }
                    }
                    deleting.IsDeleted = true;
                    DataContext.SubmitChanges();
                    if (IsWithChildren)
                    {
                        foreach(var group in innerGroups)
                        {
                            DeleteGroup(group.Id, IsWithChildren);
                        }
                    }
                }
            }
        }

        public long AddAddressToNewGroup(IEnumerable<long> addresses, string groupName, long parentGroupId)
        {
            long result = SaveGroup(null, groupName, parentGroupId);
            AddToExistingGroup(addresses, result);
            return result;
        }

        public void AddToExistingGroup(IEnumerable<long> addresses, long groupId)
        {
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                var addressesToInclude = DataContext.AddressBooks.Where(ab => ab.ClientId == OperationalClientId && !ab.IsDeleted && addresses.Contains(ab.Id));
                var group = DataContext.AddressGroups.FirstOrDefault(grp => !grp.IsDeleted && grp.ClientId == OperationalClientId && grp.Id == groupId);
                if (group != null && addressesToInclude.Count()>0)
                {
                    group.Assigments.AddRange
                        (
                            addressesToInclude.Select(addr => new Groups2Address()
                            {
                                AddressId = addr.Id
                            }
                        )
                    );
                    DataContext.SubmitChanges();
                }
            }
        }

        public void RemoveFromGroup(IEnumerable<long> addresses, long groupId)
        {
            using (var tran = DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                DataContext.Transaction = tran;
                var addressesToRemove = DataContext.AddressBooks.Where(ab => !ab.IsDeleted && ab.ClientId == OperationalClientId && addresses.Contains(ab.Id));
                var group = DataContext.AddressGroups.FirstOrDefault(grp => !grp.IsDeleted && grp.ClientId == OperationalClientId && grp.Id == groupId);
                if (group != null && addressesToRemove.Count()>0)
                {
                    group.Assigments.Where(ass => !ass.IsDeleted && addressesToRemove.Contains(ass.Address))
                        .ToList().ForEach(
                        ass => ass.IsDeleted = true
                        );
                    DataContext.SubmitChanges();
                }
            }
        }

        private static ModelMetadata _addressMetadata;
        
        private static ModelMetadata AddressMetadata
        {
            get
            {
                if (_addressMetadata == null)
                {
                    AddressModel model = new AddressModel();
                    _addressMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
                }
                return _addressMetadata;
            }
        }

        public FileColumnsMap GetMappings(byte[] importFileBytes, string encodingName, string[] separators)
        {
            FileColumnsMap result = new FileColumnsMap();
            if (importFileBytes.LongCount() <= ImportPolicy.MaxFileSize)
            {
                //Заполняем доступные колонки
                foreach (ModelMetadata propertyMetadata in AddressMetadata.Properties)
                    result.AvailableColumns.Add(propertyMetadata.PropertyName);
                //Теперь разбираем байты
                Encoding decoder = Encoding.GetEncoding(encodingName);
                string importingFile = decoder.GetString(importFileBytes);
                StringReader sr = new StringReader(importingFile);
                string importLine;
                int i = 5;
                while ((importLine = sr.ReadLine()) != null && i>0)
                {
                    result.Items.Add(
                    importLine.Split(separators, StringSplitOptions.None).ToList()
                    );
                    i--;
                }
            }
            else
            {
                throw new OverflowException("Размер импортируемого файл слишком велик");
            }
            return result;
        }

        public void ImportFile(Dictionary<string, int> columnMappings, byte[] importFileBytes, string encodingName, string[] separators)
        {
            if (importFileBytes.LongCount() <= ImportPolicy.MaxFileSize)
            {
                //Теперь разбираем байты
                Encoding decoder = Encoding.GetEncoding(encodingName);
                string importingFile = decoder.GetString(importFileBytes);
                StringReader sr = new StringReader(importingFile);
                string importLine;
                while ((importLine = sr.ReadLine()) != null)
                {
                    AddressModel model = new AddressModel();
                    List<string> columns = importLine.Split(separators, StringSplitOptions.None).ToList();
                    foreach (ModelMetadata propertyMetadata in AddressMetadata.Properties)
                    {
                        PropertyInfo settingProperty = model.GetType().GetProperty(propertyMetadata.PropertyName);
                        //Значение которое собираемся записывать
                        string possibleValue = string.Empty;
                        if (columnMappings.ContainsKey(propertyMetadata.PropertyName) && columns.Count < columnMappings[propertyMetadata.PropertyName])
                        {
                            possibleValue = columns[columnMappings[propertyMetadata.PropertyName]];
                        }
                        if (string.IsNullOrEmpty(possibleValue))
                        {
                            object realValue = null;
                            //Метод парсинга для данного типа
                            if (propertyMetadata.ContainerType != typeof(string))
                            {
                                MethodInfo mi = propertyMetadata.ContainerType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
                                if (mi != null)
                                {
                                    try
                                    {
                                        realValue = mi.Invoke(null, new[] { possibleValue });
                                    }
                                    catch {/*TODO: Кривой формат строки, едем дальше, пока*/ }
                                }
                            }
                            else
                            {
                                realValue = possibleValue;
                            }
                            //Если значение не null, то надо выставлять
                            if (realValue != null)
                            {
                                settingProperty.SetValue(model, realValue, null);
                            }
                        }
                    }
                    SaveAddress(model);
                }
            }
            else
            {
                throw new OverflowException("Размер импортируемого файл слишком велик");
            }
        }

        public byte[] ExportAdresses(long? groupId, Dictionary<string, int> columnMappings)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

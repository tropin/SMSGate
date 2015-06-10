using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csharper.OliverTwist.Model;

namespace Csharper.OliverTwist.Repo
{
    public interface IAddressRepo
    {
        AddressModel GetAdressProjected(long addressId);
        IQueryable<AddressBook> GetAdresses(long? groupId = null);
        IQueryable<AddressModel> GetAdressesProjected(long? groupId = null);
        void SaveAddress(AddressModel adress);
        void DeleteAddress(long addressId);
        long AddAddressToNewGroup(IEnumerable<long> addresses, string groupName, long parentGroupId);
        void AddToExistingGroup(IEnumerable<long> addresses, long groupId);
        void RemoveFromGroup(IEnumerable<long> addresses, long groupId);
        long SaveGroup(long? groupId, string groupName, long? parentGroupId);
        void DeleteGroup(long groupId, bool IsWithChildren = false);
        List<AddressGroupModel> GetGroups(long id);
        FileColumnsMap GetMappings(byte[] importFileBytes, string encodingName, string[] separators);
        void ImportFile(Dictionary<string, int> columnMappings, byte[] importFileBytes, string encodingName, string[] separators);
        byte[] ExportAdresses(long? groupId, Dictionary<string, int> columnMappings);
    }
}

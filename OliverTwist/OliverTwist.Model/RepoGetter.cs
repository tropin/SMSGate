using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Csharper.OliverTwist.Repo
{
    /// <summary>
    /// Класс получения репозитарев
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepoGetter<T>
        where T: RepoBase
    {
        private static Dictionary<string, T> instances = new Dictionary<string, T>();
        public static T Get(string loginedUserName, long? loginedClientId, long? operationalClientId)
        {
            T result = null;
            if (instances.ContainsKey(loginedUserName))
            {
                result = instances[loginedUserName];
                typeof(RepoBase).GetMethod("ReinitInstance", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(result, new object[] { loginedClientId, operationalClientId });
            }
            else
            {
                result = (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {loginedUserName, loginedClientId, operationalClientId}, null);
                instances.Add(loginedUserName, result);
            }
            return result; 
        }
    }    
}

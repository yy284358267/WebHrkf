using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo_RealTime.Hubs
{
    //public class UserOnline
    //{
    //        public string UserName { get; set; }

    //        public List<string> ConnectionIDs { get; set; }
    //}

    //public static class UserManager
    //{
    //    //用户集合
    //    public static List<UserOnline> listUsers = new List<UserOnline>();

    //    //添加用户或添加用户的connenctionID
    //    public static void AddUser(string domain, string connecationID)
    //    {
    //        if (IsExistUser(domain))
    //        {
    //            if (!IsExistUser(domain, connecationID))
    //            {
    //                var user = listUsers.FirstOrDefault(u => u.UserName == domain);
    //                if (user != null)
    //                    user.ConnectionIDs.Add(connecationID);
    //            }
    //        }
    //        else
    //           listUsers.Add(new UserOnline { UserName = domain, ConnectionIDs = new List<string>(new string[] { connecationID }) });
    //    }

    //    //移除用户或移除用户的connectionID
    //    public static void RemoveUser(string domain, string connectionID)
    //    {
    //        var user = listUsers.FirstOrDefault(u => u.UserName == domain);
    //        if (user != null)
    //        {
    //            if (user.ConnectionIDs.Skip(1).Any() && user.ConnectionIDs.FirstOrDefault() == connectionID)
    //                listUsers.Remove(user);
    //            else
    //                user.ConnectionIDs.Remove(connectionID);
    //        }
    //    }

    //    //判断用户的domain是否存在
    //    private static bool IsExistUser(string domain)
    //    {
    //        return listUsers.Any(u => u.UserName == domain);
    //    }

    //    //判断是否存在domain和connectionID
    //    private static bool IsExistUser(string domain, string connectionID)
    //    {
    //        return listUsers.Any(u => u.UserName == domain && u.ConnectionIDs.Contains(connectionID));
    //    }
    //}
}
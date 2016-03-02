using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Security.Principal;

using Demo_RealTime.Models;
using System.Data.SqlClient;
namespace Demo_RealTime.Hubs
{
    [HubName("messageHub")]
    public class MessageHub : Hub
    {
        public void send(string fromUser, string toUser, string message)
        {
            string User = string.Empty;
            string Visitorse = string.Empty;
            int count = 0;
            //var ClientIP = HttpContext.Current.Request.UserHostAddress;
            Users users = new Users();
            Visitor visitor = new Visitor();
            string card = string.Empty;
            string selec = "select card,visitorid from Visitor where Visitor='" + fromUser + "' and DateDiff(dd,convert(datetime,StartTime,105),getdate())=0 ";
            SqlDataReader drd = SqlHelper.GetReader(selec, System.Data.CommandType.Text);
            while (drd.Read())
            {
                visitor.visitorid = drd["visitorid"].ToString();
                card = drd["card"].ToString();
            }
            drd.Close();
            //判断发消息的用户是否是游客
            if (card == "游客")
            {
                string sql = "select * from users where username=@a";
                SqlParameter[] pa = { new SqlParameter("@a", toUser) };
                SqlDataReader dr = SqlHelper.GetReader(sql, System.Data.CommandType.Text, pa);
                while (dr.Read())
                {
                    users.username = dr["username"].ToString();
                    users.usernameid = dr["userid"].ToString();
                }
                dr.Close();
                //判断有没有符合条件的客服
                string sqlInsertKF = "update visitor set KFname='"+toUser+"'";
                SqlHelper.ExeSql(sqlInsertKF,System.Data.CommandType.Text);
                 if (!string.IsNullOrEmpty(users.usernameid))
                 {
                    if (service())
                    {
                        //判断客服是否接待了这个客户，如果没有，需要把客户信息追加数据库
                        string sqlFromUser = "select FromUser,count from users where FromUser like '%" + fromUser + "%'";
                        SqlDataReader drFromUser = SqlHelper.GetReader(sqlFromUser, System.Data.CommandType.Text);
                        while (drFromUser.Read())
                        {
                            User = drFromUser["FromUser"].ToString();
                            count = (int)drFromUser["count"];
                        }
                        if (!User.Contains(fromUser))
                        {
                            //加载客服端的客户树
                            string selectVisitor = "select * from users where username='" + toUser + "'";
                            SqlDataReader drVisitor = SqlHelper.GetReader(selectVisitor, System.Data.CommandType.Text);
                            while (drVisitor.Read())
                            {
                                Visitorse = drVisitor["FromUser"].ToString();
                            }
                            drVisitor.Close();
                            string[] str = Visitorse.Split(',');
                            if (!str.Contains(fromUser))
                            {

                                Clients.Client(users.usernameid).AA(fromUser);
                                Clients.Client(visitor.visitorid).AAA(fromUser, "您好，客服" + fromUser + "为您服务");
                            }

                            User += fromUser + ",";
                            count++;
                            string sqlcount = "update users set state='接客' ,FromUser='" + User + "', count='" + count + "' where  username=@a ";
                            SqlParameter[] pacount = {
                                        new SqlParameter("@a",toUser),

                         };
                            SqlHelper.ExeSql(sqlcount, System.Data.CommandType.Text, pacount);
                        }
                        //改变客服的登陆状态
                        else
                        {
                            string sql1 = "update users set state='接客' where  username=@a ";
                            SqlParameter[] pa1 = {
                              new SqlParameter("@a",toUser),
                              };
                            SqlHelper.ExeSql(sql1, System.Data.CommandType.Text, pa1);
                        }                                        
                            Clients.Client(users.usernameid).AAA(fromUser, message);
                    }
                }           
                else
                {
                    Clients.Client(visitor.visitorid).AAA("系统", "目前没有客服在线");
                    
                }

            }
          
            else
            {
                //判断有没有符合条件的客户
                string sql = "select * from visitor where visitor=@a and state='在线'";
                SqlParameter[] pa = { new SqlParameter("@a", toUser) };
                SqlDataReader dr = SqlHelper.GetReader(sql, System.Data.CommandType.Text, pa);
                while (dr.Read())
                {
                    visitor.visitor = dr["visitor"].ToString();
                    visitor.visitorid = dr["visitorid"].ToString();
                }
                dr.Close();
                if (!string.IsNullOrEmpty(visitor.visitorid))
                {                 
                        Clients.Client(visitor.visitorid).AAA(fromUser, message);                                          
                   
                }
                else
                {
                    string sqlVisit = "select * from users where username=@a";
                    SqlParameter[] paVisit = { new SqlParameter("@a", fromUser) };
                    SqlDataReader drVisit = SqlHelper.GetReader(sqlVisit, System.Data.CommandType.Text, paVisit);
                    while (drVisit.Read())
                    {
                        users.username = drVisit["username"].ToString();
                        users.usernameid = drVisit["userid"].ToString();
                    }
                    drVisit.Close();
                    Clients.Client(users.usernameid).AAA("系统", "目前没有客户访问");
                }
            }         
            
            // var connection = UserManager.listUsers.FirstOrDefault(u => u.UserName == toUser);
           
            //TODO:测试全部广播
            //Clients.All.AAA(name, message);
        }
        public bool service()
        {
            string username = string.Empty;
            string select = "select Top 1 username from users where state='在线' and card='客服'  order by ID";
            SqlDataReader dr = SqlHelper.GetReader(select, System.Data.CommandType.Text);
            while (dr.Read())
            {
                username = dr["username"].ToString();
            }
            dr.Close();
            if (string.IsNullOrEmpty(username))
            {
                string selectorther = "select Top 1 username from users where state='接客' and card='客服'  order by count";

                SqlDataReader dorther = SqlHelper.GetReader(selectorther, System.Data.CommandType.Text);
                while (dorther.Read())
                {
                    username = dorther["username"].ToString();
                }
                dorther.Close();
                if (string.IsNullOrEmpty(username))
                {
                    return false;
                }
               
            }
            return true;
        }
        public static string GetClientIP()
        {
            string UserIp = string.Empty;
            try
            {
                //得到用户ip		
                if (HttpContext.Current != null)
                {
                    string strIp = string.Empty;
                    if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                    {
                        strIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                    }
                    else
                    {
                        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                        {
                            strIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                        }
                    }
                    UserIp = strIp;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return UserIp;
        }
        //浏览器打开 开启链接
        public override Task OnConnected()
        {
            string visitor = string.Empty;
            var connectionID = Context.ConnectionId;
            var ClientIP = HttpContext.Current.Request.UserHostAddress;
            Random rad = new Random();
            int i = rad.Next(0, 100);
            //var userName = (Context.User as Principal).uName;//TODO:处理成当前用户名或其他ID  
            //TODO:模拟测试
            if (HttpContext.Current != null)
            {
                var url = HttpContext.Current.Request.UrlReferrer;
                if (url != null && url.Query.Contains("&") == true)
                {

                    string userName = "aaa";
                    if (string.IsNullOrEmpty(userName) == false)
                    {   
                        //判断用户是否登陆过
                        string selectUser = "select * from Visitor where visitor='aaa' and DateDiff(dd,convert(datetime,StartTime,105),getdate())=0";
                        SqlDataReader dr=SqlHelper.GetReader(selectUser,System.Data.CommandType.Text);
                        while (dr.Read())
                        {
                            visitor = dr["visitor"].ToString();
                        }
                        dr.Close();
                        if (string.IsNullOrEmpty(visitor))
                        {
                            string sql = "insert Visitor select Visitor=@a,VisitorID=@b,card='游客',state='在线',StartTime='"+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+ "',EndTime='',KFname=''";
                            SqlParameter[] pa = {
                                                 new SqlParameter("@a","aaa"),
                                                  new SqlParameter("@b",connectionID)
                              };
                            SqlHelper.ExeSql(sql, System.Data.CommandType.Text, pa);
                        }
                        else
                        {
                            string sql = "update Visitor set VisitorID=@b,state='在线' where  visitor='aaa' and DateDiff(dd,convert(datetime,StartTime,105),getdate())=0";
                            SqlParameter[] pa = {                                          
                                              new SqlParameter("@b",connectionID)
                              };
                            SqlHelper.ExeSql(sql, System.Data.CommandType.Text, pa);
                        }
                    }
                       

                }
                else
                {
                    string username = string.Empty;
                    string select = "select Top 1 username from users where state='在线' and card='客服'  order by count asc";
                    SqlDataReader dr=SqlHelper.GetReader(select,System.Data.CommandType.Text);
                    while (dr.Read())
                    {
                        username = dr["username"].ToString();
                    }
                    dr.Close();
                    string sql = "update users set userid=@b,state='在线' where  username=@a ";
                    SqlParameter[] pa = {
                         new SqlParameter("@a",username),
                          new SqlParameter("@b",connectionID)
                    };
                    SqlHelper.ExeSql(sql, System.Data.CommandType.Text, pa);
                    
                }
                //TODO:
                //1、获取客服账号
                //2、用户MAC或公网IP
                //3、保存 connectionID、设置客服忙碌状态
            }

            return Clients.Client(Context.ConnectionId).Connected();
        }

        //浏览器关闭 关掉链接 
        public override Task OnDisconnected(bool stopCalled)
       {
            string KF = string.Empty;
            string userid = string.Empty;
            var connectionID = Context.ConnectionId;
            if (stopCalled == true)
            {
                Visitor visitor = new Visitor();
                //var userName = (Context.User as Principal).uName;//TODO:处理成当前用户名或其他ID
                //TODO:模拟测试
                //if (HttpContext.Current != null)
                //{
                //    var url = HttpContext.Current.Request.UrlReferrer;
                //    if (url != null && url.Query.Contains("&") == true)
                //    {
                //        string bbb = HttpContext.Current.Request.UrlReferrer.Query.Substring(1).Split('&')[0];
                //        string userName = bbb.Split('=')[1];
                //        if (string.IsNullOrEmpty(userName) == false)
                //            //UserManager.RemoveUser(userName, connectionID);
                //        {
                //string SelectVisitor = "select card,visitorid from Visitor where Visitor='aaa' and DateDiff(dd,convert(datetime,StartTime,105),getdate())=0 ";
                //SqlDataReader drd = SqlHelper.GetReader(SelectVisitor, System.Data.CommandType.Text);
                //while (drd.Read())
                //{
                //    visitor.visitorid = drd["visitorid"].ToString();

                //}
                //drd.Close();
                //Clients.Client(visitor.visitorid).AAA("系统", "客服已下线");
                string RemoveVisitor = "select * from visitor where visitor='aaa' and DateDiff(dd,convert(datetime,StartTime,105),getdate())=0";
                SqlDataReader drd = SqlHelper.GetReader(RemoveVisitor, System.Data.CommandType.Text);
                while (drd.Read())
                {
                    visitor.visitorid = drd["visitorid"].ToString();
                    KF = drd["KFname"].ToString();
                }
                drd.Close();
                string selectKF = "select * from users where username='"+KF+"'";
                SqlDataReader drKF=SqlHelper.GetReader(selectKF,System.Data.CommandType.Text);
                while (drKF.Read())
                {
                    userid=drKF["userid"].ToString();
                }
                drKF.Close();
                Clients.Client(userid).AAA("系统", "游客已离开");
                string sql = "update visitor set State='离线' ,EndTime='"+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+ "',KFname=''  where  Visitor=@a ";
                            SqlParameter[] pa = {
                                   new SqlParameter("@a","aaa"),
                                   new SqlParameter("@b",connectionID)
                             };
                            SqlHelper.ExeSql(sql, System.Data.CommandType.Text, pa);
                            
                        //}
                    //}

                    //TODO:
                    //1、清除ConnectionId、设置客服闲置状态
                //}

            }
            return Clients.AllExcept(new[] { Context.ConnectionId }).disconnected();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Models;


namespace DAL
{
   public class LoginService
    {
        public object checkUser(User user)
        {
            string sql = "SELECT PAA002 FROM DSCSYS_SLM.dbo.POWPAA WHERE PAA001='{0}' AND PAA003='{1}'";

            object o= SqlHelper.GetSingleResult(string.Format(sql,user.ID,Convert.ToBase64String(Encoding.Default.GetBytes(user.PassWord))));
            return o;
        }
    }
}

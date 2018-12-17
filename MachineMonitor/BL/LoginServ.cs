using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
namespace BLL
{
   public class LoginServ
    {
        LoginService loginService = new LoginService();
        public object checkUser(User user)
        {
            return loginService.checkUser(user);
        }
    }
}

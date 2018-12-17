using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using Models;

namespace MachineMonitor
{
    public partial class FrmLogin : Form
    {
        #region 窗体初始化及变量定义
        LoginServ loginServ = new LoginServ();
        public FrmLogin()
        {
            InitializeComponent();
            txtLoginID.Focus();
        }
        #endregion

        #region 登录
        private void btnLogin_Click(object sender, EventArgs e)
        {
            User user = new User()
            {
                ID = txtLoginID.Text,
                PassWord = txtPwd.Text
            };
            object o = loginServ.checkUser(user);
            if (o != null)
            {
                FrmMain frmmain = new FrmMain();
                frmmain.Text = "设备倾向性监测系统---欢迎您：" + o.ToString();
                frmmain.Show();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("用户名不存在或密码错误！", "提示", MessageBoxButtons.OKCancel,MessageBoxIcon.Asterisk);
                txtPwd.Focus();
            }
        }
        #endregion

        #region 控制事件实现
        private void txtLoginID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)//回车时密码文本框获取焦点
            {
                txtPwd.Focus();
            }
        }

        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void txtLoginID_Leave(object sender, EventArgs e)
        {
            if (txtLoginID.Text.Length == 0)
            {
                MessageBox.Show("用户ID不能为空！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLoginID.Focus();
                return;
            }
        }

        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnLogin_Click(null, null);
            }
        }
        #endregion
    }
}

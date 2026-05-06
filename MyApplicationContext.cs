using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cargo_tracker
{
    internal class MyApplicationContext : ApplicationContext
    {
        public MyApplicationContext()
        {
            ShowLogin();
        }

        public void ShowLogin()
        {
            if (Program.AdminAcik)
                return;

            LoginForm login = new LoginForm();
            login.FormClosed += (s, e) =>
            {
                if (!Program.AdminAcik)
                    ExitThread();
            };

            login.Show();
        }

        public void ShowAdmin()
        {
            Program.AdminAcik = true;

            AdminForm admin = new AdminForm();
            admin.FormClosed += (s, e) =>
            {
                Program.AdminAcik = false;
                ShowLogin();
            };

            admin.Show();
        }
    }
}

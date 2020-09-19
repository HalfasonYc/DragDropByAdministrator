/* Description: Win7/Win10 Administrator DragDrop（在Win7/Win10等系统下以管理员身份运行下，不受UAC限制，进行文件名的拖拽操作）
 *          By: HalfasonYc
 *        Time: 2020-09-19
 *       Email: Endless_yangc@foxmail.com
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragDropByAdministrator
{
    public partial class TestForm : Form
    {
        DragDropHelper dragDropHelper;

        public TestForm()
        {
            InitializeComponent();

            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            MessageBox.Show("Is Administrator? " + isElevated.ToString());

            this.dragDropHelper = new DragDropHelper();
        }


        private void DragDropCallBack(string fileName)
        {
            MessageBox.Show(fileName);//换行符分隔 Environment.NewLine
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateControl(); //此处推荐CreateControl而非CreateHandle
            }
            //handle 必须有效
            this.dragDropHelper.Resigter(this.panel1.Handle, this.DragDropCallBack);
        }

    }
}

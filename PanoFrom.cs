using System;
using System.Collections;
using System.Collections.Generic;
 
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NetposaTest
{
   
    [ComVisible(true)]
    public partial class PanoFrom : Form
    {
      
        public PanoFrom()
        {
            InitializeComponent();
            this.Load += (object sender, EventArgs e) =>
            {
                listView1_SelectedIndexChanged(this.百度全景ToolStripMenuItem);
            };
        }

        private void listView1_SelectedIndexChanged(ToolStripItem selectedControl)
        {
            for (int i = 0; i < this.panel1.Controls.Count; i++)
            {
                this.panel1.Controls[i].Visible = false;
            }

            Control control;
            if (selectedControl.Tag == null)
            {
                control = GetMainControl(selectedControl.Text);
                control.Dock = DockStyle.Fill;
                selectedControl.Tag = control;
                panel1.Controls.Add(control);
            }
            else
            {
                control = (Control) selectedControl.Tag;
            }
            control.Visible = true;

        }

        private Control GetMainControl(string controlName)
        {
            if (controlName == "百度全景")
            {
                return  new BaiduUserControl();
            }
            else if (controlName == "720云")
            {
                return new Yun720Control();
            }
            return null;
        }

        private void 百度全景ToolStripMenuItem_Click(object sender, EventArgs e)
        {
             
            listView1_SelectedIndexChanged(this.百度全景ToolStripMenuItem);
        }

        private void 云ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1_SelectedIndexChanged(云ToolStripMenuItem);
        }
    }
}

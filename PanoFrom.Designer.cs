namespace NetposaTest
{
    partial class PanoFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanoFrom));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.百度全景ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.云ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "streetview_no_b14ce20.png");
            this.imageList1.Images.SetKeyName(1, "QQ图片20170606110843.png");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.百度全景ToolStripMenuItem,
            this.云ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1043, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 百度全景ToolStripMenuItem
            // 
            this.百度全景ToolStripMenuItem.Image = global::NetposaTest.Properties.Resources.streetview_no_b14ce20;
            this.百度全景ToolStripMenuItem.Name = "百度全景ToolStripMenuItem";
            this.百度全景ToolStripMenuItem.Size = new System.Drawing.Size(84, 21);
            this.百度全景ToolStripMenuItem.Text = "百度全景";
            this.百度全景ToolStripMenuItem.Click += new System.EventHandler(this.百度全景ToolStripMenuItem_Click);
            // 
            // 云ToolStripMenuItem
            // 
            this.云ToolStripMenuItem.Image = global::NetposaTest.Properties.Resources.QQ图片20170606110843;
            this.云ToolStripMenuItem.Name = "云ToolStripMenuItem";
            this.云ToolStripMenuItem.Size = new System.Drawing.Size(69, 21);
            this.云ToolStripMenuItem.Text = "720云";
            this.云ToolStripMenuItem.Click += new System.EventHandler(this.云ToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1043, 624);
            this.panel1.TabIndex = 1;
            // 
            // PanoFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 649);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PanoFrom";
            this.Text = "PanoFrom";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 百度全景ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 云ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
    }
}
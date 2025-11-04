// We must manually change the type of rtbFile1 and rtbFile2 in this file
// from System.Windows.Forms.RichTextBox to 
// GitContributionApp.DiffUtility.DiffForm.RichTextBoxSync

namespace GitContributionApp.DiffUtility
{
    partial class DiffForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiffForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLoadFile1 = new System.Windows.Forms.ToolStripButton();
            this.btnLoadFile2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompare = new System.Windows.Forms.ToolStripButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panelFile1 = new System.Windows.Forms.Panel();
            this.rtbFile1 = new GitContributionApp.DiffUtility.DiffForm.RichTextBoxSync();
            this.panelHeader1 = new System.Windows.Forms.Panel();
            this.lblFile1 = new System.Windows.Forms.Label();
            this.panelFile2 = new System.Windows.Forms.Panel();
            this.rtbFile2 = new GitContributionApp.DiffUtility.DiffForm.RichTextBoxSync();
            this.panelHeader2 = new System.Windows.Forms.Panel();
            this.lblFile2 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelFile1.SuspendLayout();
            this.panelHeader1.SuspendLayout();
            this.panelFile2.SuspendLayout();
            this.panelHeader2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadFile1,
            this.btnLoadFile2,
            this.toolStripSeparator1,
            this.btnCompare});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1182, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLoadFile1
            // 
            this.btnLoadFile1.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadFile1.Image")));
            this.btnLoadFile1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadFile1.Name = "btnLoadFile1";
            this.btnLoadFile1.Size = new System.Drawing.Size(107, 24);
            this.btnLoadFile1.Text = "Load File 1";
            this.btnLoadFile1.Click += new System.EventHandler(this.btnLoadFile1_Click);
            // 
            // btnLoadFile2
            // 
            this.btnLoadFile2.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadFile2.Image")));
            this.btnLoadFile2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadFile2.Name = "btnLoadFile2";
            this.btnLoadFile2.Size = new System.Drawing.Size(107, 24);
            this.btnLoadFile2.Text = "Load File 2";
            this.btnLoadFile2.Click += new System.EventHandler(this.btnLoadFile2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnCompare
            // 
            this.btnCompare.Image = ((System.Drawing.Image)(resources.GetObject("btnCompare.Image")));
            this.btnCompare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(92, 24);
            this.btnCompare.Text = "Compare";
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 27);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panelFile1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelFile2);
            this.splitContainer.Size = new System.Drawing.Size(1182, 600);
            this.splitContainer.SplitterDistance = 591;
            this.splitContainer.TabIndex = 1;
            // 
            // panelFile1
            // 
            this.panelFile1.Controls.Add(this.rtbFile1);
            this.panelFile1.Controls.Add(this.panelHeader1);
            this.panelFile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFile1.Location = new System.Drawing.Point(0, 0);
            this.panelFile1.Name = "panelFile1";
            this.panelFile1.Size = new System.Drawing.Size(591, 600);
            this.panelFile1.TabIndex = 0;
            // 
            // rtbFile1
            // 
            this.rtbFile1.DetectUrls = false;
            this.rtbFile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbFile1.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbFile1.Location = new System.Drawing.Point(0, 30);
            this.rtbFile1.Name = "rtbFile1";
            this.rtbFile1.Size = new System.Drawing.Size(591, 570);
            this.rtbFile1.TabIndex = 1;
            this.rtbFile1.Text = "";
            this.rtbFile1.WordWrap = false;
            this.rtbFile1.Scroll += new GitContributionApp.DiffUtility.DiffForm.RichTextBoxSync.MessageEventHandler(this.rtb_Scroll);
            // 
            // panelHeader1
            // 
            this.panelHeader1.Controls.Add(this.lblFile1);
            this.panelHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader1.Location = new System.Drawing.Point(0, 0);
            this.panelHeader1.Name = "panelHeader1";
            this.panelHeader1.Size = new System.Drawing.Size(591, 30);
            this.panelHeader1.TabIndex = 0;
            // 
            // lblFile1
            // 
            this.lblFile1.AutoSize = true;
            this.lblFile1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFile1.Location = new System.Drawing.Point(12, 6);
            this.lblFile1.Name = "lblFile1";
            this.lblFile1.Size = new System.Drawing.Size(115, 18);
            this.lblFile1.TabIndex = 0;
            this.lblFile1.Text = "File 1 (Original)";
            // 
            // panelFile2
            // 
            this.panelFile2.Controls.Add(this.rtbFile2);
            this.panelFile2.Controls.Add(this.panelHeader2);
            this.panelFile2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFile2.Location = new System.Drawing.Point(0, 0);
            this.panelFile2.Name = "panelFile2";
            this.panelFile2.Size = new System.Drawing.Size(587, 600);
            this.panelFile2.TabIndex = 1;
            // 
            // rtbFile2
            // 
            this.rtbFile2.DetectUrls = false;
            this.rtbFile2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbFile2.Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbFile2.Location = new System.Drawing.Point(0, 30);
            this.rtbFile2.Name = "rtbFile2";
            this.rtbFile2.Size = new System.Drawing.Size(587, 570);
            this.rtbFile2.TabIndex = 2;
            this.rtbFile2.Text = "";
            this.rtbFile2.WordWrap = false;
            this.rtbFile2.Scroll += new GitContributionApp.DiffUtility.DiffForm.RichTextBoxSync.MessageEventHandler(this.rtb_Scroll);
            // 
            // panelHeader2
            // 
            this.panelHeader2.Controls.Add(this.lblFile2);
            this.panelHeader2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader2.Location = new System.Drawing.Point(0, 0);
            this.panelHeader2.Name = "panelHeader2";
            this.panelHeader2.Size = new System.Drawing.Size(587, 30);
            this.panelHeader2.TabIndex = 1;
            // 
            // lblFile2
            // 
            this.lblFile2.AutoSize = true;
            this.lblFile2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFile2.Location = new System.Drawing.Point(12, 6);
            this.lblFile2.Name = "lblFile2";
            this.lblFile2.Size = new System.Drawing.Size(125, 18);
            this.lblFile2.TabIndex = 1;
            this.lblFile2.Text = "File 2 (Modified)";
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 627);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1182, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(179, 20);
            this.toolStripStatusLabel1.Text = "Ready to compare files...";
            // 
            // DiffForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 653);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "DiffForm";
            this.Text = "Text Diff Utility";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelFile1.ResumeLayout(false);
            this.panelHeader1.ResumeLayout(false);
            this.panelHeader1.PerformLayout();
            this.panelFile2.ResumeLayout(false);
            this.panelHeader2.ResumeLayout(false);
            this.panelHeader2.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoadFile1;
        private System.Windows.Forms.ToolStripButton btnLoadFile2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCompare;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panelFile1;
        private System.Windows.Forms.Panel panelHeader1;
        private System.Windows.Forms.Label lblFile1;
        private System.Windows.Forms.Panel panelFile2;
        private System.Windows.Forms.Panel panelHeader2;
        private System.Windows.Forms.Label lblFile2;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        
        // Custom controls
        private RichTextBoxSync rtbFile1;
        private RichTextBoxSync rtbFile2;
    }
}

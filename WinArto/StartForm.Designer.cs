namespace WinArto
{
    partial class StartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            this.AppLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLoadTestSet = new System.Windows.Forms.Button();
            this.dlgTestSetDialog = new System.Windows.Forms.OpenFileDialog();
            this.AppLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AppLayoutPanel
            // 
            this.AppLayoutPanel.ColumnCount = 1;
            this.AppLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.AppLayoutPanel.Controls.Add(this.btnLoadTestSet, 0, 1);
            this.AppLayoutPanel.Controls.Add(this.panel1, 0, 0);
            this.AppLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.AppLayoutPanel.Name = "AppLayoutPanel";
            this.AppLayoutPanel.RowCount = 3;
            this.AppLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.AppLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.95652F));
            this.AppLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.04348F));
            this.AppLayoutPanel.Size = new System.Drawing.Size(393, 300);
            this.AppLayoutPanel.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(387, 58);
            this.panel1.TabIndex = 1;
            // 
            // btnLoadTestSet
            // 
            this.btnLoadTestSet.Enabled = false;
            this.btnLoadTestSet.Location = new System.Drawing.Point(3, 67);
            this.btnLoadTestSet.Name = "btnLoadTestSet";
            this.btnLoadTestSet.Size = new System.Drawing.Size(161, 46);
            this.btnLoadTestSet.TabIndex = 0;
            this.btnLoadTestSet.Text = "Load Test Set";
            this.btnLoadTestSet.UseVisualStyleBackColor = true;
            this.btnLoadTestSet.Click += new System.EventHandler(this.btnLoadTestSet_Click);
            // 
            // dlgTestSetDialog
            // 
            this.dlgTestSetDialog.Filter = "Test Set Files|*.json";
            this.dlgTestSetDialog.ReadOnlyChecked = true;
            this.dlgTestSetDialog.RestoreDirectory = true;
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 300);
            this.Controls.Add(this.AppLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WinArto";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.AppLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel AppLayoutPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLoadTestSet;
        private System.Windows.Forms.OpenFileDialog dlgTestSetDialog;
    }
}


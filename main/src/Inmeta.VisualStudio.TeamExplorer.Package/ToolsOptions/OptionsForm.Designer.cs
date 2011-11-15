using System.IO;
using System.Xml.Serialization;

namespace Inmeta.VisualStudio.TeamExplorer.ToolsOptions
{
    partial class OptionsForm
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
            this.tbSeparator = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbDelay = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbUseTimerRefresh = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbQDBDontAllow = new System.Windows.Forms.RadioButton();
            this.rbQDBDontAllowFolders = new System.Windows.Forms.RadioButton();
            this.rbQDBAllow = new System.Windows.Forms.RadioButton();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSeparator
            // 
            this.tbSeparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSeparator.Location = new System.Drawing.Point(100, 19);
            this.tbSeparator.MaxLength = 1;
            this.tbSeparator.Name = "tbSeparator";
            this.tbSeparator.Size = new System.Drawing.Size(49, 22);
            this.tbSeparator.TabIndex = 0;
            this.tbSeparator.Text = ".";
            this.tbSeparator.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Separator:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(209, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Save);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbSeparator);
            this.groupBox1.Location = new System.Drawing.Point(15, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 171);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings per Team Project";
            // 
            // tbDelay
            // 
            this.tbDelay.Location = new System.Drawing.Point(77, 63);
            this.tbDelay.Mask = "00";
            this.tbDelay.Name = "tbDelay";
            this.tbDelay.Size = new System.Drawing.Size(30, 20);
            this.tbDelay.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Delay [s]";
            // 
            // cbUseTimerRefresh
            // 
            this.cbUseTimerRefresh.AutoSize = true;
            this.cbUseTimerRefresh.Location = new System.Drawing.Point(26, 36);
            this.cbUseTimerRefresh.Name = "cbUseTimerRefresh";
            this.cbUseTimerRefresh.Size = new System.Drawing.Size(166, 17);
            this.cbUseTimerRefresh.TabIndex = 5;
            this.cbUseTimerRefresh.Text = "Use Timed Refresh at Startup";
            this.cbUseTimerRefresh.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbQDBDontAllow);
            this.groupBox2.Controls.Add(this.rbQDBDontAllowFolders);
            this.groupBox2.Controls.Add(this.rbQDBAllow);
            this.groupBox2.Location = new System.Drawing.Point(26, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(136, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Queue Default builds";
            // 
            // rbQDBDontAllow
            // 
            this.rbQDBDontAllow.AutoSize = true;
            this.rbQDBDontAllow.Location = new System.Drawing.Point(7, 68);
            this.rbQDBDontAllow.Name = "rbQDBDontAllow";
            this.rbQDBDontAllow.Size = new System.Drawing.Size(77, 17);
            this.rbQDBDontAllow.TabIndex = 2;
            this.rbQDBDontAllow.TabStop = true;
            this.rbQDBDontAllow.Text = "Don\'t allow";
            this.rbQDBDontAllow.UseVisualStyleBackColor = true;
            // 
            // rbQDBDontAllowFolders
            // 
            this.rbQDBDontAllowFolders.AutoSize = true;
            this.rbQDBDontAllowFolders.Location = new System.Drawing.Point(7, 44);
            this.rbQDBDontAllowFolders.Name = "rbQDBDontAllowFolders";
            this.rbQDBDontAllowFolders.Size = new System.Drawing.Size(126, 17);
            this.rbQDBDontAllowFolders.TabIndex = 1;
            this.rbQDBDontAllowFolders.TabStop = true;
            this.rbQDBDontAllowFolders.Text = "Don\'t allow on folders";
            this.rbQDBDontAllowFolders.UseVisualStyleBackColor = true;
            // 
            // rbQDBAllow
            // 
            this.rbQDBAllow.AutoSize = true;
            this.rbQDBAllow.Location = new System.Drawing.Point(7, 20);
            this.rbQDBAllow.Name = "rbQDBAllow";
            this.rbQDBAllow.Size = new System.Drawing.Size(50, 17);
            this.rbQDBAllow.TabIndex = 0;
            this.rbQDBAllow.TabStop = true;
            this.rbQDBAllow.Text = "Allow";
            this.rbQDBAllow.UseVisualStyleBackColor = true;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(58, 304);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(52, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "lblVersion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 304);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Version";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbDelay);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cbUseTimerRefresh);
            this.groupBox3.Location = new System.Drawing.Point(15, 190);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 100);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Local Settings per user";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 341);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.groupBox1);
            this.Name = "OptionsForm";
            this.Text = "Inmeta Settings";
            this.Load += new System.EventHandler(this.OnLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSeparator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbQDBDontAllow;
        private System.Windows.Forms.RadioButton rbQDBDontAllowFolders;
        private System.Windows.Forms.RadioButton rbQDBAllow;
        private System.Windows.Forms.MaskedTextBox tbDelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbUseTimerRefresh;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
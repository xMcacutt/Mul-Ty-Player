namespace RKV_Patcher
{
    partial class MainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.SelectTyFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectFolder = new System.Windows.Forms.Button();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.info = new System.Windows.Forms.Label();
            this.message = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SelectFolder
            // 
            this.SelectFolder.AccessibleName = "selectFolderButton";
            this.SelectFolder.AllowDrop = true;
            this.SelectFolder.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SelectFolder.Font = new System.Drawing.Font("Impact", 9F);
            this.SelectFolder.Location = new System.Drawing.Point(50, 10);
            this.SelectFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SelectFolder.Name = "SelectFolder";
            this.SelectFolder.Size = new System.Drawing.Size(146, 25);
            this.SelectFolder.TabIndex = 1;
            this.SelectFolder.Text = "Select Original Ty Folder";
            this.SelectFolder.UseVisualStyleBackColor = true;
            this.SelectFolder.Click += new System.EventHandler(this.SelectFolder_Click);
            // 
            // DownloadButton
            // 
            this.DownloadButton.AccessibleName = "downloadButton";
            this.DownloadButton.AllowDrop = true;
            this.DownloadButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DownloadButton.Enabled = false;
            this.DownloadButton.Font = new System.Drawing.Font("Impact", 9F);
            this.DownloadButton.Location = new System.Drawing.Point(50, 58);
            this.DownloadButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(146, 25);
            this.DownloadButton.TabIndex = 2;
            this.DownloadButton.Text = "Download and Install";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.AccessibleName = "progressBar";
            this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.progressBar.Location = new System.Drawing.Point(20, 116);
            this.progressBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(206, 22);
            this.progressBar.TabIndex = 3;
            // 
            // info
            // 
            this.info.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.info.Location = new System.Drawing.Point(19, 93);
            this.info.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(206, 13);
            this.info.TabIndex = 4;
            this.info.Text = "Installing makes a new folder next to Ty folder";
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.Location = new System.Drawing.Point(9, 41);
            this.message.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(0, 13);
            this.message.TabIndex = 5;
            // 
            // MainMenu
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 148);
            this.Controls.Add(this.message);
            this.Controls.Add(this.info);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.SelectFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "MainMenu";
            this.Text = "RKV Patcher";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog SelectTyFolder;
        private System.Windows.Forms.Button SelectFolder;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label info;
        private System.Windows.Forms.Label message;
    }
}


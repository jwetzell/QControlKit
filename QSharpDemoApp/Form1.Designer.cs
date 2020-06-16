namespace QSharpDemoApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.workspaceListView = new System.Windows.Forms.ListView();
            this.cueNumberHeader = new System.Windows.Forms.ColumnHeader();
            this.cueNameHeader = new System.Windows.Forms.ColumnHeader();
            this.goButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.serverListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // workspaceListView
            // 
            this.workspaceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cueNumberHeader,
            this.cueNameHeader});
            this.workspaceListView.FullRowSelect = true;
            this.workspaceListView.GridLines = true;
            this.workspaceListView.HideSelection = false;
            this.workspaceListView.Location = new System.Drawing.Point(246, 122);
            this.workspaceListView.MultiSelect = false;
            this.workspaceListView.Name = "workspaceListView";
            this.workspaceListView.Size = new System.Drawing.Size(542, 316);
            this.workspaceListView.TabIndex = 0;
            this.workspaceListView.UseCompatibleStateImageBehavior = false;
            this.workspaceListView.View = System.Windows.Forms.View.Details;
            // 
            // cueNumberHeader
            // 
            this.cueNumberHeader.Name = "cueNumberHeader";
            this.cueNumberHeader.Text = "Number";
            // 
            // cueNameHeader
            // 
            this.cueNameHeader.Name = "cueNameHeader";
            this.cueNameHeader.Text = "Name";
            this.cueNameHeader.Width = 300;
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(246, 47);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(104, 69);
            this.goButton.TabIndex = 1;
            this.goButton.Text = "GO";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Servers";
            // 
            // serverListView
            // 
            this.serverListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.serverListView.FullRowSelect = true;
            this.serverListView.GridLines = true;
            this.serverListView.HideSelection = false;
            this.serverListView.Location = new System.Drawing.Point(13, 47);
            this.serverListView.Name = "serverListView";
            this.serverListView.Size = new System.Drawing.Size(227, 391);
            this.serverListView.TabIndex = 4;
            this.serverListView.UseCompatibleStateImageBehavior = false;
            this.serverListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Version";
            this.columnHeader4.Width = 50;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(72, 21);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point(153, 21);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(75, 23);
            this.disconnectButton.TabIndex = 5;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 480);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.serverListView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.workspaceListView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView workspaceListView;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView serverListView;
        private System.Windows.Forms.ColumnHeader cueNumberHeader;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader cueNameHeader;
        private System.Windows.Forms.Button disconnectButton;
    }
}


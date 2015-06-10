namespace Csharper.SMS.PerformanceCounterManagment
{
	partial class PerformanceCounterManagerForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbGroupName = new System.Windows.Forms.TextBox();
            this.bCreate = new System.Windows.Forms.Button();
            this.bDelete = new System.Windows.Forms.Button();
            this.gbExists = new System.Windows.Forms.GroupBox();
            this.bDeleteSelected = new System.Windows.Forms.Button();
            this.lbExists = new System.Windows.Forms.ListBox();
            this.gbAddNew = new System.Windows.Forms.GroupBox();
            this.bAddCounter = new System.Windows.Forms.Button();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbExists.SuspendLayout();
            this.gbAddNew.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Performance counter group name";
            // 
            // tbGroupName
            // 
            this.tbGroupName.Location = new System.Drawing.Point(130, 12);
            this.tbGroupName.Name = "tbGroupName";
            this.tbGroupName.Size = new System.Drawing.Size(140, 20);
            this.tbGroupName.TabIndex = 1;
            this.tbGroupName.Text = "SMSCRelay";
            // 
            // bCreate
            // 
            this.bCreate.Enabled = false;
            this.bCreate.Location = new System.Drawing.Point(276, 10);
            this.bCreate.Name = "bCreate";
            this.bCreate.Size = new System.Drawing.Size(75, 23);
            this.bCreate.TabIndex = 2;
            this.bCreate.Text = "Create";
            this.bCreate.UseVisualStyleBackColor = true;
            this.bCreate.Click += new System.EventHandler(this.BCreateClick);
            // 
            // bDelete
            // 
            this.bDelete.Enabled = false;
            this.bDelete.Location = new System.Drawing.Point(357, 10);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(75, 23);
            this.bDelete.TabIndex = 3;
            this.bDelete.Text = "Delete";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.BDeleteClick);
            // 
            // gbExists
            // 
            this.gbExists.Controls.Add(this.bDeleteSelected);
            this.gbExists.Controls.Add(this.lbExists);
            this.gbExists.Enabled = false;
            this.gbExists.Location = new System.Drawing.Point(12, 44);
            this.gbExists.Name = "gbExists";
            this.gbExists.Size = new System.Drawing.Size(186, 130);
            this.gbExists.TabIndex = 6;
            this.gbExists.TabStop = false;
            this.gbExists.Text = "Exists counters";
            // 
            // bDeleteSelected
            // 
            this.bDeleteSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bDeleteSelected.Enabled = false;
            this.bDeleteSelected.Location = new System.Drawing.Point(6, 101);
            this.bDeleteSelected.Name = "bDeleteSelected";
            this.bDeleteSelected.Size = new System.Drawing.Size(174, 23);
            this.bDeleteSelected.TabIndex = 7;
            this.bDeleteSelected.Text = "Delete selected";
            this.bDeleteSelected.UseVisualStyleBackColor = true;
            this.bDeleteSelected.Click += new System.EventHandler(this.BDeleteSelectedClick);
            // 
            // lbExists
            // 
            this.lbExists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbExists.FormattingEnabled = true;
            this.lbExists.IntegralHeight = false;
            this.lbExists.Location = new System.Drawing.Point(6, 19);
            this.lbExists.Name = "lbExists";
            this.lbExists.Size = new System.Drawing.Size(174, 76);
            this.lbExists.TabIndex = 6;
            this.lbExists.SelectedIndexChanged += new System.EventHandler(this.LbExistsSelectedIndexChanged);
            // 
            // gbAddNew
            // 
            this.gbAddNew.Controls.Add(this.bAddCounter);
            this.gbAddNew.Controls.Add(this.cbType);
            this.gbAddNew.Controls.Add(this.label4);
            this.gbAddNew.Controls.Add(this.tbDescription);
            this.gbAddNew.Controls.Add(this.label3);
            this.gbAddNew.Controls.Add(this.tbName);
            this.gbAddNew.Controls.Add(this.label2);
            this.gbAddNew.Enabled = false;
            this.gbAddNew.Location = new System.Drawing.Point(204, 44);
            this.gbAddNew.Name = "gbAddNew";
            this.gbAddNew.Size = new System.Drawing.Size(228, 130);
            this.gbAddNew.TabIndex = 7;
            this.gbAddNew.TabStop = false;
            this.gbAddNew.Text = "Add new counter";
            // 
            // bAddCounter
            // 
            this.bAddCounter.Location = new System.Drawing.Point(9, 102);
            this.bAddCounter.Name = "bAddCounter";
            this.bAddCounter.Size = new System.Drawing.Size(213, 23);
            this.bAddCounter.TabIndex = 6;
            this.bAddCounter.Text = "Add new counter";
            this.bAddCounter.UseVisualStyleBackColor = true;
            this.bAddCounter.Click += new System.EventHandler(this.BAddCounterClick);
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(72, 71);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(150, 21);
            this.cbType.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Type";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(72, 45);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(150, 20);
            this.tbDescription.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Description";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(72, 19);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(150, 20);
            this.tbName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name";
            // 
            // PerformanceCounterManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 181);
            this.Controls.Add(this.gbAddNew);
            this.Controls.Add(this.gbExists);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bCreate);
            this.Controls.Add(this.tbGroupName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PerformanceCounterManagerForm";
            this.Text = "Performance Counter Manager";
            this.Load += new System.EventHandler(this.PerformanceCounterManagerForm_Load);
            this.gbExists.ResumeLayout(false);
            this.gbAddNew.ResumeLayout(false);
            this.gbAddNew.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbGroupName;
		private System.Windows.Forms.Button bCreate;
		private System.Windows.Forms.Button bDelete;
		private System.Windows.Forms.GroupBox gbExists;
		private System.Windows.Forms.Button bDeleteSelected;
		private System.Windows.Forms.ListBox lbExists;
		private System.Windows.Forms.GroupBox gbAddNew;
		private System.Windows.Forms.Button bAddCounter;
		private System.Windows.Forms.ComboBox cbType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label label2;
	}
}
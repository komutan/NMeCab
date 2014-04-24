namespace WindowsFormsSample
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TargetTextBox = new System.Windows.Forms.TextBox();
            this.DoAnalyzeButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.LatticeLevelGroupBox = new System.Windows.Forms.GroupBox();
            this.LatticeLevelFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.LatticeLevel0RadioButton = new System.Windows.Forms.RadioButton();
            this.LatticeLevel1RadioButton = new System.Windows.Forms.RadioButton();
            this.LatticeLevel2RadioButton = new System.Windows.Forms.RadioButton();
            this.OutputFormatGroupBox = new System.Windows.Forms.GroupBox();
            this.OutputFormatflowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.OutputFormatLatticeRadioButton = new System.Windows.Forms.RadioButton();
            this.OutputFormatWakatiRadioButton = new System.Windows.Forms.RadioButton();
            this.OutputFormatDumpRadioButton = new System.Windows.Forms.RadioButton();
            this.NBestGroupBox = new System.Windows.Forms.GroupBox();
            this.NBestFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.NBestCheckBox = new System.Windows.Forms.CheckBox();
            this.NBestNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.AllMorphsCheckBox = new System.Windows.Forms.CheckBox();
            this.PartialCheckBox = new System.Windows.Forms.CheckBox();
            this.ResultTextBox = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.LatticeLevelGroupBox.SuspendLayout();
            this.LatticeLevelFlowLayoutPanel.SuspendLayout();
            this.OutputFormatGroupBox.SuspendLayout();
            this.OutputFormatflowLayoutPanel.SuspendLayout();
            this.NBestGroupBox.SuspendLayout();
            this.NBestFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NBestNumericUpDown)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ResultTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(624, 442);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TargetTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.DoAnalyzeButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 200);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // TargetTextBox
            // 
            this.TargetTextBox.AcceptsReturn = true;
            this.TargetTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TargetTextBox.Location = new System.Drawing.Point(3, 76);
            this.TargetTextBox.Multiline = true;
            this.TargetTextBox.Name = "TargetTextBox";
            this.TargetTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TargetTextBox.Size = new System.Drawing.Size(618, 92);
            this.TargetTextBox.TabIndex = 0;
            // 
            // DoAnalyzeButton
            // 
            this.DoAnalyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DoAnalyzeButton.Location = new System.Drawing.Point(3, 174);
            this.DoAnalyzeButton.Name = "DoAnalyzeButton";
            this.DoAnalyzeButton.Size = new System.Drawing.Size(75, 23);
            this.DoAnalyzeButton.TabIndex = 1;
            this.DoAnalyzeButton.Text = "Analyze";
            this.DoAnalyzeButton.UseVisualStyleBackColor = true;
            this.DoAnalyzeButton.Click += new System.EventHandler(this.DoAnalyzeButton_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.LatticeLevelGroupBox);
            this.flowLayoutPanel3.Controls.Add(this.OutputFormatGroupBox);
            this.flowLayoutPanel3.Controls.Add(this.NBestGroupBox);
            this.flowLayoutPanel3.Controls.Add(this.AllMorphsCheckBox);
            this.flowLayoutPanel3.Controls.Add(this.PartialCheckBox);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(600, 67);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // LatticeLevelGroupBox
            // 
            this.LatticeLevelGroupBox.Controls.Add(this.LatticeLevelFlowLayoutPanel);
            this.LatticeLevelGroupBox.Location = new System.Drawing.Point(3, 3);
            this.LatticeLevelGroupBox.Name = "LatticeLevelGroupBox";
            this.LatticeLevelGroupBox.Size = new System.Drawing.Size(166, 58);
            this.LatticeLevelGroupBox.TabIndex = 0;
            this.LatticeLevelGroupBox.TabStop = false;
            this.LatticeLevelGroupBox.Text = "LatticeLevel";
            // 
            // LatticeLevelFlowLayoutPanel
            // 
            this.LatticeLevelFlowLayoutPanel.Controls.Add(this.LatticeLevel0RadioButton);
            this.LatticeLevelFlowLayoutPanel.Controls.Add(this.LatticeLevel1RadioButton);
            this.LatticeLevelFlowLayoutPanel.Controls.Add(this.LatticeLevel2RadioButton);
            this.LatticeLevelFlowLayoutPanel.Location = new System.Drawing.Point(9, 18);
            this.LatticeLevelFlowLayoutPanel.Name = "LatticeLevelFlowLayoutPanel";
            this.LatticeLevelFlowLayoutPanel.Size = new System.Drawing.Size(151, 22);
            this.LatticeLevelFlowLayoutPanel.TabIndex = 0;
            // 
            // LatticeLevel0RadioButton
            // 
            this.LatticeLevel0RadioButton.AutoSize = true;
            this.LatticeLevel0RadioButton.Checked = true;
            this.LatticeLevel0RadioButton.Location = new System.Drawing.Point(3, 3);
            this.LatticeLevel0RadioButton.Name = "LatticeLevel0RadioButton";
            this.LatticeLevel0RadioButton.Size = new System.Drawing.Size(46, 16);
            this.LatticeLevel0RadioButton.TabIndex = 0;
            this.LatticeLevel0RadioButton.TabStop = true;
            this.LatticeLevel0RadioButton.Text = "Zero";
            this.LatticeLevel0RadioButton.UseVisualStyleBackColor = true;
            // 
            // LatticeLevel1RadioButton
            // 
            this.LatticeLevel1RadioButton.AutoSize = true;
            this.LatticeLevel1RadioButton.Location = new System.Drawing.Point(55, 3);
            this.LatticeLevel1RadioButton.Name = "LatticeLevel1RadioButton";
            this.LatticeLevel1RadioButton.Size = new System.Drawing.Size(43, 16);
            this.LatticeLevel1RadioButton.TabIndex = 1;
            this.LatticeLevel1RadioButton.TabStop = true;
            this.LatticeLevel1RadioButton.Text = "One";
            this.LatticeLevel1RadioButton.UseVisualStyleBackColor = true;
            // 
            // LatticeLevel2RadioButton
            // 
            this.LatticeLevel2RadioButton.AutoSize = true;
            this.LatticeLevel2RadioButton.Location = new System.Drawing.Point(104, 3);
            this.LatticeLevel2RadioButton.Name = "LatticeLevel2RadioButton";
            this.LatticeLevel2RadioButton.Size = new System.Drawing.Size(44, 16);
            this.LatticeLevel2RadioButton.TabIndex = 2;
            this.LatticeLevel2RadioButton.TabStop = true;
            this.LatticeLevel2RadioButton.Text = "Two";
            this.LatticeLevel2RadioButton.UseVisualStyleBackColor = true;
            // 
            // OutputFormatGroupBox
            // 
            this.OutputFormatGroupBox.Controls.Add(this.OutputFormatflowLayoutPanel);
            this.OutputFormatGroupBox.Location = new System.Drawing.Point(175, 3);
            this.OutputFormatGroupBox.Name = "OutputFormatGroupBox";
            this.OutputFormatGroupBox.Size = new System.Drawing.Size(194, 58);
            this.OutputFormatGroupBox.TabIndex = 1;
            this.OutputFormatGroupBox.TabStop = false;
            this.OutputFormatGroupBox.Text = "OutputFormat";
            // 
            // OutputFormatflowLayoutPanel
            // 
            this.OutputFormatflowLayoutPanel.Controls.Add(this.OutputFormatLatticeRadioButton);
            this.OutputFormatflowLayoutPanel.Controls.Add(this.OutputFormatWakatiRadioButton);
            this.OutputFormatflowLayoutPanel.Controls.Add(this.OutputFormatDumpRadioButton);
            this.OutputFormatflowLayoutPanel.Location = new System.Drawing.Point(9, 18);
            this.OutputFormatflowLayoutPanel.Name = "OutputFormatflowLayoutPanel";
            this.OutputFormatflowLayoutPanel.Size = new System.Drawing.Size(179, 22);
            this.OutputFormatflowLayoutPanel.TabIndex = 0;
            // 
            // OutputFormatLatticeRadioButton
            // 
            this.OutputFormatLatticeRadioButton.AutoSize = true;
            this.OutputFormatLatticeRadioButton.Checked = true;
            this.OutputFormatLatticeRadioButton.Location = new System.Drawing.Point(3, 3);
            this.OutputFormatLatticeRadioButton.Name = "OutputFormatLatticeRadioButton";
            this.OutputFormatLatticeRadioButton.Size = new System.Drawing.Size(55, 16);
            this.OutputFormatLatticeRadioButton.TabIndex = 0;
            this.OutputFormatLatticeRadioButton.TabStop = true;
            this.OutputFormatLatticeRadioButton.Text = "lattice";
            this.OutputFormatLatticeRadioButton.UseVisualStyleBackColor = true;
            // 
            // OutputFormatWakatiRadioButton
            // 
            this.OutputFormatWakatiRadioButton.AutoSize = true;
            this.OutputFormatWakatiRadioButton.Location = new System.Drawing.Point(64, 3);
            this.OutputFormatWakatiRadioButton.Name = "OutputFormatWakatiRadioButton";
            this.OutputFormatWakatiRadioButton.Size = new System.Drawing.Size(56, 16);
            this.OutputFormatWakatiRadioButton.TabIndex = 1;
            this.OutputFormatWakatiRadioButton.TabStop = true;
            this.OutputFormatWakatiRadioButton.Text = "wakati";
            this.OutputFormatWakatiRadioButton.UseVisualStyleBackColor = true;
            // 
            // OutputFormatDumpRadioButton
            // 
            this.OutputFormatDumpRadioButton.AutoSize = true;
            this.OutputFormatDumpRadioButton.Location = new System.Drawing.Point(126, 3);
            this.OutputFormatDumpRadioButton.Name = "OutputFormatDumpRadioButton";
            this.OutputFormatDumpRadioButton.Size = new System.Drawing.Size(50, 16);
            this.OutputFormatDumpRadioButton.TabIndex = 2;
            this.OutputFormatDumpRadioButton.TabStop = true;
            this.OutputFormatDumpRadioButton.Text = "dump";
            this.OutputFormatDumpRadioButton.UseVisualStyleBackColor = true;
            // 
            // NBestGroupBox
            // 
            this.NBestGroupBox.AutoSize = true;
            this.NBestGroupBox.Controls.Add(this.NBestFlowLayoutPanel);
            this.NBestGroupBox.Location = new System.Drawing.Point(375, 3);
            this.NBestGroupBox.Name = "NBestGroupBox";
            this.NBestGroupBox.Size = new System.Drawing.Size(78, 61);
            this.NBestGroupBox.TabIndex = 2;
            this.NBestGroupBox.TabStop = false;
            this.NBestGroupBox.Text = "NBest";
            // 
            // NBestFlowLayoutPanel
            // 
            this.NBestFlowLayoutPanel.AutoSize = true;
            this.NBestFlowLayoutPanel.Controls.Add(this.NBestCheckBox);
            this.NBestFlowLayoutPanel.Controls.Add(this.NBestNumericUpDown);
            this.NBestFlowLayoutPanel.Location = new System.Drawing.Point(6, 18);
            this.NBestFlowLayoutPanel.Name = "NBestFlowLayoutPanel";
            this.NBestFlowLayoutPanel.Size = new System.Drawing.Size(66, 25);
            this.NBestFlowLayoutPanel.TabIndex = 0;
            // 
            // NBestCheckBox
            // 
            this.NBestCheckBox.AutoSize = true;
            this.NBestCheckBox.Location = new System.Drawing.Point(3, 3);
            this.NBestCheckBox.Name = "NBestCheckBox";
            this.NBestCheckBox.Size = new System.Drawing.Size(15, 14);
            this.NBestCheckBox.TabIndex = 0;
            this.NBestCheckBox.UseVisualStyleBackColor = true;
            this.NBestCheckBox.CheckedChanged += new System.EventHandler(this.NBestCheckBox_CheckedChanged);
            // 
            // NBestNumericUpDown
            // 
            this.NBestNumericUpDown.AutoSize = true;
            this.NBestNumericUpDown.Enabled = false;
            this.NBestNumericUpDown.Location = new System.Drawing.Point(24, 3);
            this.NBestNumericUpDown.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.NBestNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NBestNumericUpDown.Name = "NBestNumericUpDown";
            this.NBestNumericUpDown.Size = new System.Drawing.Size(39, 19);
            this.NBestNumericUpDown.TabIndex = 1;
            this.NBestNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // AllMorphsCheckBox
            // 
            this.AllMorphsCheckBox.AutoSize = true;
            this.AllMorphsCheckBox.Location = new System.Drawing.Point(459, 3);
            this.AllMorphsCheckBox.Name = "AllMorphsCheckBox";
            this.AllMorphsCheckBox.Size = new System.Drawing.Size(75, 16);
            this.AllMorphsCheckBox.TabIndex = 3;
            this.AllMorphsCheckBox.Text = "AllMorphs";
            this.AllMorphsCheckBox.UseVisualStyleBackColor = true;
            // 
            // PartialCheckBox
            // 
            this.PartialCheckBox.AutoSize = true;
            this.PartialCheckBox.Location = new System.Drawing.Point(540, 3);
            this.PartialCheckBox.Name = "PartialCheckBox";
            this.PartialCheckBox.Size = new System.Drawing.Size(57, 16);
            this.PartialCheckBox.TabIndex = 4;
            this.PartialCheckBox.Text = "Partial";
            this.PartialCheckBox.UseVisualStyleBackColor = true;
            this.PartialCheckBox.CheckedChanged += new System.EventHandler(this.PartialCheckBox_CheckedChanged);
            // 
            // ResultTextBox
            // 
            this.ResultTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultTextBox.Location = new System.Drawing.Point(0, 0);
            this.ResultTextBox.Multiline = true;
            this.ResultTextBox.Name = "ResultTextBox";
            this.ResultTextBox.ReadOnly = true;
            this.ResultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ResultTextBox.Size = new System.Drawing.Size(624, 215);
            this.ResultTextBox.TabIndex = 0;
            this.ResultTextBox.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 215);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 23);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 18);
            this.toolStripStatusLabel1.Text = "Startup ...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.LatticeLevelGroupBox.ResumeLayout(false);
            this.LatticeLevelFlowLayoutPanel.ResumeLayout(false);
            this.LatticeLevelFlowLayoutPanel.PerformLayout();
            this.OutputFormatGroupBox.ResumeLayout(false);
            this.OutputFormatflowLayoutPanel.ResumeLayout(false);
            this.OutputFormatflowLayoutPanel.PerformLayout();
            this.NBestGroupBox.ResumeLayout(false);
            this.NBestGroupBox.PerformLayout();
            this.NBestFlowLayoutPanel.ResumeLayout(false);
            this.NBestFlowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NBestNumericUpDown)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button DoAnalyzeButton;
        private System.Windows.Forms.TextBox ResultTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TargetTextBox;
        private System.Windows.Forms.GroupBox LatticeLevelGroupBox;
        private System.Windows.Forms.FlowLayoutPanel LatticeLevelFlowLayoutPanel;
        private System.Windows.Forms.RadioButton LatticeLevel0RadioButton;
        private System.Windows.Forms.RadioButton LatticeLevel1RadioButton;
        private System.Windows.Forms.RadioButton LatticeLevel2RadioButton;
        private System.Windows.Forms.GroupBox OutputFormatGroupBox;
        private System.Windows.Forms.FlowLayoutPanel OutputFormatflowLayoutPanel;
        private System.Windows.Forms.RadioButton OutputFormatLatticeRadioButton;
        private System.Windows.Forms.RadioButton OutputFormatWakatiRadioButton;
        private System.Windows.Forms.RadioButton OutputFormatDumpRadioButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox AllMorphsCheckBox;
        private System.Windows.Forms.CheckBox PartialCheckBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox NBestGroupBox;
        private System.Windows.Forms.FlowLayoutPanel NBestFlowLayoutPanel;
        private System.Windows.Forms.NumericUpDown NBestNumericUpDown;
        private System.Windows.Forms.CheckBox NBestCheckBox;

    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NMeCab;
using System.Diagnostics;

namespace WindowsFormsSample
{
    public partial class Form1 : Form
    {
        MeCabTagger tagger;

        private MeCabLatticeLevel LatticeLevel
        {
            get
            {
                if (this.LatticeLevel0RadioButton.Checked)
                    return MeCabLatticeLevel.Zero;
                else if (this.LatticeLevel1RadioButton.Checked)
                    return MeCabLatticeLevel.One;
                else if (this.LatticeLevel2RadioButton.Checked)
                    return MeCabLatticeLevel.Two;
                else
                    return MeCabLatticeLevel.Two;
            }
            set
            {
                switch (value)
                {
                    case MeCabLatticeLevel.Zero:
                        this.LatticeLevel0RadioButton.Checked = true;
                        break;
                    case MeCabLatticeLevel.One:
                        this.LatticeLevel1RadioButton.Checked = true;
                        break;
                    case MeCabLatticeLevel.Two:
                        this.LatticeLevel2RadioButton.Checked = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string OutputFormat
        {
            get
            {
                if (this.OutputFormatLatticeRadioButton.Checked)
                    return this.OutputFormatLatticeRadioButton.Text;
                else if (this.OutputFormatWakatiRadioButton.Checked)
                    return this.OutputFormatWakatiRadioButton.Text;
                else if (this.OutputFormatDumpRadioButton.Checked)
                    return this.OutputFormatDumpRadioButton.Text;
                else
                    return this.OutputFormatDumpRadioButton.Text;
            }
            set
            {
                switch (value)
                {
                    case "lattice":
                        this.OutputFormatLatticeRadioButton.Checked = true;
                        break;
                    case "wakati":
                        this.OutputFormatWakatiRadioButton.Checked = true;
                        break;
                    case "dump":
                        this.OutputFormatDumpRadioButton.Checked = true;
                        break;
                    default:
                        throw new NotImplementedException();
                };
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.Text = Application.ProductName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                this.tagger = MeCabTagger.Create();

                sw.Stop();
                this.toolStripStatusLabel1.Text = string.Format("startup end ({0:0.000}sec)",
                                                                sw.Elapsed.TotalSeconds);

                this.LatticeLevel = this.tagger.LatticeLevel;
                this.OutputFormat = this.tagger.OutPutFormatType;
                this.AllMorphsCheckBox.Checked = this.tagger.AllMorphs;
                this.PartialCheckBox.Checked = this.tagger.Partial;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Startup ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void NBestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.NBestCheckBox.Checked)
            {
                this.NBestNumericUpDown.Enabled = true;
                if (this.LatticeLevel == MeCabLatticeLevel.Zero) this.LatticeLevel = MeCabLatticeLevel.One;
                this.LatticeLevel0RadioButton.Enabled = false;
                this.AllMorphsCheckBox.Checked = false;
                this.AllMorphsCheckBox.Enabled = false;
            }
            else
            {
                this.NBestNumericUpDown.Enabled = false;
                this.LatticeLevel0RadioButton.Enabled = true;
                this.AllMorphsCheckBox.Enabled = true;
            }
        }

        private void PartialCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PartialCheckBox.Checked)
            {
                this.TargetTextBox.AcceptsTab = true;
                this.toolStripStatusLabel1.Text = "Accepts Tab for Partial mode.";
            }
            else
            {
                this.TargetTextBox.AcceptsTab = false;
                this.toolStripStatusLabel1.Text = "";
            }
        }

        private void DoAnalyzeButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.tagger.LatticeLevel = this.LatticeLevel;
                this.tagger.OutPutFormatType = this.OutputFormat;
                this.tagger.AllMorphs = this.AllMorphsCheckBox.Checked;
                this.tagger.Partial = this.PartialCheckBox.Checked;

                this.toolStripStatusLabel1.Text = "Analyzing ...";
                Stopwatch sw = Stopwatch.StartNew();

                if (NBestCheckBox.Checked)
                {
                    this.ResultTextBox.Text = this.tagger.ParseNBest((int)NBestNumericUpDown.Value,
                                                                     this.TargetTextBox.Text);
                }
                else
                {
                    this.ResultTextBox.Text = this.tagger.Parse(this.TargetTextBox.Text);
                }

                sw.Stop();
                this.toolStripStatusLabel1.Text = string.Format("Finish ({0:0.000}sec)",
                                                                sw.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel1.Text = "ERROR";
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.tagger != null) this.tagger.Dispose();
        }
    }
}


namespace ChartQADoc
{
    partial class GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.Prompt1Label = new System.Windows.Forms.Label();
            this.Prompt2Label = new System.Windows.Forms.Label();
            this.YesBut = new System.Windows.Forms.Button();
            this.NoBut = new System.Windows.Forms.Button();
            this.ExecBut = new System.Windows.Forms.Button();
            this.PlanListBox = new System.Windows.Forms.ListBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.OutBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Prompt1Label
            // 
            this.Prompt1Label.AutoSize = true;
            this.Prompt1Label.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Prompt1Label.Location = new System.Drawing.Point(28, 9);
            this.Prompt1Label.Name = "Prompt1Label";
            this.Prompt1Label.Size = new System.Drawing.Size(739, 19);
            this.Prompt1Label.TabIndex = 0;
            this.Prompt1Label.Text = "This program will generate a PDF with a table containing all Chart QA information" +
    " for a plan or course.\r\n";
            // 
            // Prompt2Label
            // 
            this.Prompt2Label.AutoSize = true;
            this.Prompt2Label.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Prompt2Label.Location = new System.Drawing.Point(30, 52);
            this.Prompt2Label.Name = "Prompt2Label";
            this.Prompt2Label.Size = new System.Drawing.Size(508, 19);
            this.Prompt2Label.TabIndex = 1;
            this.Prompt2Label.Text = "Do you want to run ChartQADoc on the entire course or a single plan?";
            // 
            // YesBut
            // 
            this.YesBut.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YesBut.Location = new System.Drawing.Point(811, 52);
            this.YesBut.Name = "YesBut";
            this.YesBut.Size = new System.Drawing.Size(177, 81);
            this.YesBut.TabIndex = 2;
            this.YesBut.Text = "Yes (run on entire course)";
            this.YesBut.UseVisualStyleBackColor = true;
            this.YesBut.Visible = false;
            // 
            // NoBut
            // 
            this.NoBut.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoBut.Location = new System.Drawing.Point(1033, 52);
            this.NoBut.Name = "NoBut";
            this.NoBut.Size = new System.Drawing.Size(214, 81);
            this.NoBut.TabIndex = 3;
            this.NoBut.Text = "No (run on single plan)\r\n";
            this.NoBut.UseVisualStyleBackColor = true;
            this.NoBut.Visible = false;
            // 
            // ExecBut
            // 
            this.ExecBut.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExecBut.Location = new System.Drawing.Point(1088, 184);
            this.ExecBut.Name = "ExecBut";
            this.ExecBut.Size = new System.Drawing.Size(159, 48);
            this.ExecBut.TabIndex = 4;
            this.ExecBut.Text = "Execute";
            this.ExecBut.UseVisualStyleBackColor = true;
            this.ExecBut.Visible = false;
            // 
            // PlanListBox
            // 
            this.PlanListBox.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlanListBox.FormattingEnabled = true;
            this.PlanListBox.ItemHeight = 19;
            this.PlanListBox.Location = new System.Drawing.Point(33, 108);
            this.PlanListBox.Name = "PlanListBox";
            this.PlanListBox.Size = new System.Drawing.Size(353, 194);
            this.PlanListBox.TabIndex = 5;
            this.PlanListBox.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(39, 561);
            this.progressBar1.MarqueeAnimationSpeed = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1214, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 6;
            this.progressBar1.Visible = false;
            // 
            // OutBox
            // 
            this.OutBox.Location = new System.Drawing.Point(419, 146);
            this.OutBox.Multiline = true;
            this.OutBox.Name = "OutBox";
            this.OutBox.ReadOnly = true;
            this.OutBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.OutBox.Size = new System.Drawing.Size(643, 389);
            this.OutBox.TabIndex = 7;
            this.OutBox.Visible = false;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 597);
            this.Controls.Add(this.OutBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.PlanListBox);
            this.Controls.Add(this.ExecBut);
            this.Controls.Add(this.NoBut);
            this.Controls.Add(this.YesBut);
            this.Controls.Add(this.Prompt2Label);
            this.Controls.Add(this.Prompt1Label);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "GUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChartQADoc";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Prompt1Label;
        private System.Windows.Forms.Label Prompt2Label;
        private System.Windows.Forms.Button YesBut;
        private System.Windows.Forms.Button NoBut;
        private System.Windows.Forms.Button ExecBut;
        private System.Windows.Forms.ListBox PlanListBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox OutBox;
    }
}

namespace My_GOL
{
    partial class options
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
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.intervalnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.timeinterval = new System.Windows.Forms.Label();
            this.widthnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.heightnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.width = new System.Windows.Forms.Label();
            this.height = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.intervalnumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthnumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightnumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(94, 151);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(237, 150);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // intervalnumericUpDown
            // 
            this.intervalnumericUpDown.Location = new System.Drawing.Point(224, 38);
            this.intervalnumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.intervalnumericUpDown.Name = "intervalnumericUpDown";
            this.intervalnumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.intervalnumericUpDown.TabIndex = 2;
            this.intervalnumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // timeinterval
            // 
            this.timeinterval.AutoSize = true;
            this.timeinterval.Location = new System.Drawing.Point(63, 40);
            this.timeinterval.Name = "timeinterval";
            this.timeinterval.Size = new System.Drawing.Size(137, 13);
            this.timeinterval.TabIndex = 3;
            this.timeinterval.Text = "Time interval in milliseconds";
            // 
            // widthnumericUpDown
            // 
            this.widthnumericUpDown.Location = new System.Drawing.Point(224, 78);
            this.widthnumericUpDown.Name = "widthnumericUpDown";
            this.widthnumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.widthnumericUpDown.TabIndex = 4;
            // 
            // heightnumericUpDown
            // 
            this.heightnumericUpDown.Location = new System.Drawing.Point(224, 104);
            this.heightnumericUpDown.Name = "heightnumericUpDown";
            this.heightnumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.heightnumericUpDown.TabIndex = 5;
            // 
            // width
            // 
            this.width.AutoSize = true;
            this.width.Location = new System.Drawing.Point(78, 80);
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(87, 13);
            this.width.TabIndex = 6;
            this.width.Text = "width of universe";
            // 
            // height
            // 
            this.height.AutoSize = true;
            this.height.Location = new System.Drawing.Point(78, 106);
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(91, 13);
            this.height.TabIndex = 7;
            this.height.Text = "height of universe";
            // 
            // options
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(422, 195);
            this.Controls.Add(this.height);
            this.Controls.Add(this.width);
            this.Controls.Add(this.heightnumericUpDown);
            this.Controls.Add(this.widthnumericUpDown);
            this.Controls.Add(this.timeinterval);
            this.Controls.Add(this.intervalnumericUpDown);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Name = "options";
            this.Text = "options";
            ((System.ComponentModel.ISupportInitialize)(this.intervalnumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthnumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heightnumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.NumericUpDown intervalnumericUpDown;
        private System.Windows.Forms.Label timeinterval;
        private System.Windows.Forms.NumericUpDown widthnumericUpDown;
        private System.Windows.Forms.NumericUpDown heightnumericUpDown;
        private System.Windows.Forms.Label width;
        private System.Windows.Forms.Label height;
    }
}
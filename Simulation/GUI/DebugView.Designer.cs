namespace Mesh.GUI
{
    partial class DebugView
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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.VBO_Consumption = new System.Windows.Forms.CheckBox();
            this.outputMessageControl1 = new Mesh.GUI.Custom_Forms.OutputMessageControl();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(142, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Do not restrict frame rate";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // VBO_Consumption
            // 
            this.VBO_Consumption.AutoSize = true;
            this.VBO_Consumption.Location = new System.Drawing.Point(12, 35);
            this.VBO_Consumption.Name = "VBO_Consumption";
            this.VBO_Consumption.Size = new System.Drawing.Size(141, 17);
            this.VBO_Consumption.TabIndex = 2;
            this.VBO_Consumption.Text = "Show VBO consumption";
            this.VBO_Consumption.UseVisualStyleBackColor = true;
            // 
            // outputMessageControl1
            // 
            this.outputMessageControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputMessageControl1.Location = new System.Drawing.Point(13, 58);
            this.outputMessageControl1.Name = "outputMessageControl1";
            this.outputMessageControl1.Size = new System.Drawing.Size(259, 178);
            this.outputMessageControl1.TabIndex = 1;
            // 
            // DebugView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 284);
            this.Controls.Add(this.VBO_Consumption);
            this.Controls.Add(this.outputMessageControl1);
            this.Controls.Add(this.checkBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "DebugView";
            this.Text = "DebugView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private Custom_Forms.OutputMessageControl outputMessageControl1;
        private System.Windows.Forms.CheckBox VBO_Consumption;
    }
}
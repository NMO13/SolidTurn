namespace Simulation.GUI
{
    partial class OutputWindow
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
            this.outputMessageControl1 = new Mesh.GUI.Custom_Forms.OutputMessageControl();
            this.SuspendLayout();
            // 
            // outputMessageControl1
            // 
            this.outputMessageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputMessageControl1.Location = new System.Drawing.Point(0, 0);
            this.outputMessageControl1.Name = "outputMessageControl1";
            this.outputMessageControl1.Size = new System.Drawing.Size(284, 262);
            this.outputMessageControl1.TabIndex = 0;
            // 
            // OutputWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.outputMessageControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OutputWindow";
            this.Text = "OutputWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private Mesh.GUI.Custom_Forms.OutputMessageControl outputMessageControl1;

    }
}
namespace ImageRecognition {
    partial class Form1 {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.picBoxOriginal = new System.Windows.Forms.PictureBox();
            this.picBoxRecognized = new System.Windows.Forms.PictureBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtClusters = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRecognized)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxOriginal
            // 
            this.picBoxOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxOriginal.Location = new System.Drawing.Point(12, 12);
            this.picBoxOriginal.Name = "picBoxOriginal";
            this.picBoxOriginal.Size = new System.Drawing.Size(371, 327);
            this.picBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBoxOriginal.TabIndex = 0;
            this.picBoxOriginal.TabStop = false;
            // 
            // picBoxRecognized
            // 
            this.picBoxRecognized.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBoxRecognized.Location = new System.Drawing.Point(389, 12);
            this.picBoxRecognized.Name = "picBoxRecognized";
            this.picBoxRecognized.Size = new System.Drawing.Size(371, 327);
            this.picBoxRecognized.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBoxRecognized.TabIndex = 3;
            this.picBoxRecognized.TabStop = false;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(290, 352);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(100, 31);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run Again";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(184, 358);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(100, 20);
            this.txtThreshold.TabIndex = 5;
            this.txtThreshold.Text = "150";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 361);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Threshold:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 360);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(99, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "checkNegative";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtClusters
            // 
            this.txtClusters.Location = new System.Drawing.Point(184, 385);
            this.txtClusters.Name = "txtClusters";
            this.txtClusters.Size = new System.Drawing.Size(100, 20);
            this.txtClusters.TabIndex = 8;
            this.txtClusters.Text = "3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 388);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Clusters:";
            // 
            // lblTimes
            // 
            this.lblTimes.AutoSize = true;
            this.lblTimes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTimes.Location = new System.Drawing.Point(396, 360);
            this.lblTimes.Name = "lblTimes";
            this.lblTimes.Size = new System.Drawing.Size(16, 16);
            this.lblTimes.TabIndex = 10;
            this.lblTimes.Text = "0";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(290, 383);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 415);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblTimes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtClusters);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtThreshold);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.picBoxRecognized);
            this.Controls.Add(this.picBoxOriginal);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.picBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxRecognized)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxOriginal;
        private System.Windows.Forms.PictureBox picBoxRecognized;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtClusters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTimes;
        private System.Windows.Forms.Button btnClear;
    }
}


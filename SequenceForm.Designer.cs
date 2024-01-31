namespace Seq2Sheet
{
    partial class SeqForm
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
            fileList = new ListBox();
            loadButton = new Button();
            imagePreview = new PictureBox();
            generateButton = new Button();
            cropButton = new Button();
            panel1 = new Panel();
            finalCrop = new Label();
            averageCrop = new Label();
            label1 = new Label();
            alphaThreshold = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)imagePreview).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)alphaThreshold).BeginInit();
            SuspendLayout();
            // 
            // fileList
            // 
            fileList.FormattingEnabled = true;
            fileList.Location = new Point(12, 12);
            fileList.Name = "fileList";
            fileList.Size = new Size(220, 364);
            fileList.TabIndex = 0;
            // 
            // loadButton
            // 
            loadButton.Location = new Point(12, 386);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(220, 52);
            loadButton.TabIndex = 1;
            loadButton.Text = "Load Files";
            loadButton.UseVisualStyleBackColor = true;
            // 
            // imagePreview
            // 
            imagePreview.BackgroundImageLayout = ImageLayout.Zoom;
            imagePreview.BorderStyle = BorderStyle.Fixed3D;
            imagePreview.Image = Properties.Resources.PreviewGrid;
            imagePreview.InitialImage = Properties.Resources.PreviewGrid;
            imagePreview.Location = new Point(238, 12);
            imagePreview.Name = "imagePreview";
            imagePreview.Size = new Size(364, 364);
            imagePreview.SizeMode = PictureBoxSizeMode.Zoom;
            imagePreview.TabIndex = 2;
            imagePreview.TabStop = false;
            // 
            // generateButton
            // 
            generateButton.Location = new Point(418, 386);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(183, 52);
            generateButton.TabIndex = 3;
            generateButton.Text = "Generate";
            generateButton.UseVisualStyleBackColor = true;
            // 
            // cropButton
            // 
            cropButton.Location = new Point(238, 386);
            cropButton.Name = "cropButton";
            cropButton.Size = new Size(174, 52);
            cropButton.TabIndex = 4;
            cropButton.Text = "Crop Images";
            cropButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(finalCrop);
            panel1.Controls.Add(averageCrop);
            panel1.Location = new Point(621, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 364);
            panel1.TabIndex = 5;
            // 
            // finalCrop
            // 
            finalCrop.AutoSize = true;
            finalCrop.Location = new Point(-2, 20);
            finalCrop.Name = "finalCrop";
            finalCrop.Size = new Size(119, 20);
            finalCrop.TabIndex = 1;
            finalCrop.Text = "Final Crop: None";
            // 
            // averageCrop
            // 
            averageCrop.AutoSize = true;
            averageCrop.Location = new Point(-2, 0);
            averageCrop.Name = "averageCrop";
            averageCrop.Size = new Size(143, 20);
            averageCrop.TabIndex = 0;
            averageCrop.Text = "Average Crop: None";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(621, 385);
            label1.Name = "label1";
            label1.Size = new Size(172, 20);
            label1.TabIndex = 2;
            label1.Text = "Alpha Threshold (0-255):";
            // 
            // alphaThreshold
            // 
            alphaThreshold.Location = new Point(799, 382);
            alphaThreshold.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            alphaThreshold.Name = "alphaThreshold";
            alphaThreshold.Size = new Size(72, 27);
            alphaThreshold.TabIndex = 6;
            alphaThreshold.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // SeqForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(883, 450);
            Controls.Add(alphaThreshold);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(cropButton);
            Controls.Add(generateButton);
            Controls.Add(imagePreview);
            Controls.Add(loadButton);
            Controls.Add(fileList);
            MaximizeBox = false;
            Name = "SeqForm";
            Text = "Seq2Sheet";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)imagePreview).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)alphaThreshold).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Button loadButton;
        public ListBox fileList;
        public PictureBox imagePreview;
        public Button generateButton;
        public Button cropButton;
        private Panel panel1;
        public Label averageCrop;
        public Label finalCrop;
        public Label label1;
        public NumericUpDown alphaThreshold;
    }
}

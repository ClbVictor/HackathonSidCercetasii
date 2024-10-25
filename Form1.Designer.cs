namespace FluxNoteV2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnBrowseImage;
        private System.Windows.Forms.Button btnProcessImage;
        private System.Windows.Forms.Button btnSaveText;
        private System.Windows.Forms.RichTextBox rtbExtractedText;
        private System.Windows.Forms.Button btnGenerateQuiz;
        private System.Windows.Forms.Button btnAskQuestion;
        private System.Windows.Forms.ComboBox cmbFiles;
        private System.Windows.Forms.ComboBox cmbCollaborators;
        private System.Windows.Forms.Button btnAddCollaborator;
        private System.Windows.Forms.Button btnEditSelectedFile;
        private System.Windows.Forms.Button btnUploadFile;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnBrowseImage = new System.Windows.Forms.Button();
            this.btnProcessImage = new System.Windows.Forms.Button();
            this.btnSaveText = new System.Windows.Forms.Button();
            this.rtbExtractedText = new System.Windows.Forms.RichTextBox();
            this.btnGenerateQuiz = new System.Windows.Forms.Button();
            this.btnAskQuestion = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbFiles = new System.Windows.Forms.ComboBox();
            this.cmbCollaborators = new System.Windows.Forms.ComboBox();
            this.btnAddCollaborator = new System.Windows.Forms.Button();
            this.btnEditSelectedFile = new System.Windows.Forms.Button();

            this.SuspendLayout();
            // 
            // btnBrowseImage
            // 
            this.btnBrowseImage.Location = new System.Drawing.Point(15, 16);
            this.btnBrowseImage.Name = "btnBrowseImage";
            this.btnBrowseImage.Size = new System.Drawing.Size(90, 24);
            this.btnBrowseImage.TabIndex = 0;
            this.btnBrowseImage.Text = "Browse Image";
            this.btnBrowseImage.UseVisualStyleBackColor = true;
            this.btnBrowseImage.Click += new System.EventHandler(this.btnBrowseImage_Click);
            // 
            // btnProcessImage
            // 
            this.btnProcessImage.Location = new System.Drawing.Point(112, 16);
            this.btnProcessImage.Name = "btnProcessImage";
            this.btnProcessImage.Size = new System.Drawing.Size(90, 24);
            this.btnProcessImage.TabIndex = 1;
            this.btnProcessImage.Text = "Process Image";
            this.btnProcessImage.UseVisualStyleBackColor = true;
            this.btnProcessImage.Click += new System.EventHandler(this.btnProcessImage_Click);
            // 
            // btnSaveText
            // 
            this.btnSaveText.Location = new System.Drawing.Point(210, 16);
            this.btnSaveText.Name = "btnSaveText";
            this.btnSaveText.Size = new System.Drawing.Size(90, 24);
            this.btnSaveText.TabIndex = 2;
            this.btnSaveText.Text = "Save Text";
            this.btnSaveText.UseVisualStyleBackColor = true;
            this.btnSaveText.Click += new System.EventHandler(this.btnSaveText_Click);
            // 
            // rtbExtractedText
            // 
            this.rtbExtractedText.Location = new System.Drawing.Point(15, 49);
            this.rtbExtractedText.Name = "rtbExtractedText";
            this.rtbExtractedText.Size = new System.Drawing.Size(480, 244);
            this.rtbExtractedText.TabIndex = 3;
            this.rtbExtractedText.Text = "";
            this.rtbExtractedText.MouseHover += new System.EventHandler(this.rtbExtractedText_MouseHover);
            // 
            // btnGenerateQuiz
            // 
            this.btnGenerateQuiz.Location = new System.Drawing.Point(308, 16);
            this.btnGenerateQuiz.Name = "btnGenerateQuiz";
            this.btnGenerateQuiz.Size = new System.Drawing.Size(90, 24);
            this.btnGenerateQuiz.TabIndex = 4;
            this.btnGenerateQuiz.Text = "Generate Quiz";
            this.btnGenerateQuiz.UseVisualStyleBackColor = true;
            this.btnGenerateQuiz.Click += new System.EventHandler(this.btnGenerateQuiz_Click);
            // 
            // btnAskQuestion
            // 
            this.btnAskQuestion.Location = new System.Drawing.Point(405, 16);
            this.btnAskQuestion.Name = "btnAskQuestion";
            this.btnAskQuestion.Size = new System.Drawing.Size(90, 24);
            this.btnAskQuestion.TabIndex = 5;
            this.btnAskQuestion.Text = "Ask Question";
            this.btnAskQuestion.UseVisualStyleBackColor = true;
            this.btnAskQuestion.Click += new System.EventHandler(this.btnAskQuestion_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(549, 325);
            this.Controls.Add(this.rtbExtractedText);
            this.Controls.Add(this.btnSaveText);
            this.Controls.Add(this.btnProcessImage);
            this.Controls.Add(this.btnBrowseImage);
            this.Controls.Add(this.btnGenerateQuiz);
            this.Controls.Add(this.btnAskQuestion);
            this.Name = "Form1";
            this.Text = "FluxNoteV2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

            

            // 
            // cmbFiles
            // 
            this.cmbFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiles.FormattingEnabled = true;
            this.cmbFiles.Location = new System.Drawing.Point(20, 300);
            this.cmbFiles.Name = "cmbFiles";
            this.cmbFiles.Size = new System.Drawing.Size(200, 24);
            this.cmbFiles.TabIndex = 6;

            // 
            // cmbCollaborators
            // 
            this.cmbCollaborators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCollaborators.FormattingEnabled = true;
            this.cmbCollaborators.Location = new System.Drawing.Point(250, 300);
            this.cmbCollaborators.Name = "cmbCollaborators";
            this.cmbCollaborators.Size = new System.Drawing.Size(150, 24);
            this.cmbCollaborators.TabIndex = 7;

            // 
            // btnAddCollaborator
            // 
            this.btnAddCollaborator.Location = new System.Drawing.Point(420, 300);
            this.btnAddCollaborator.Name = "btnAddCollaborator";
            this.btnAddCollaborator.Size = new System.Drawing.Size(120, 24);
            this.btnAddCollaborator.TabIndex = 8;
            this.btnAddCollaborator.Text = "Add Collaborator";
            this.btnAddCollaborator.UseVisualStyleBackColor = true;
            this.btnAddCollaborator.Click += new System.EventHandler(this.btnAddCollaborator_Click);

            // 
            // btnEditSelectedFile
            // 
            this.btnEditSelectedFile.Location = new System.Drawing.Point(420, 330);
            this.btnEditSelectedFile.Name = "btnEditSelectedFile";
            this.btnEditSelectedFile.Size = new System.Drawing.Size(120, 24);
            this.btnEditSelectedFile.TabIndex = 9;
            this.btnEditSelectedFile.Text = "Edit Selected File";
            this.btnEditSelectedFile.UseVisualStyleBackColor = true;
            this.btnEditSelectedFile.Click += new System.EventHandler(this.btnEditSelectedFile_Click);

            // Add components to the form
            this.Controls.Add(this.cmbFiles);
            this.Controls.Add(this.cmbCollaborators);
            this.Controls.Add(this.btnAddCollaborator);
            this.Controls.Add(this.btnEditSelectedFile);

            this.btnUploadFile = new System.Windows.Forms.Button();
            this.btnUploadFile.Location = new System.Drawing.Point(420, 360);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(120, 24);
            this.btnUploadFile.TabIndex = 10;
            this.btnUploadFile.Text = "Upload File";
            this.btnUploadFile.UseVisualStyleBackColor = true;
            this.btnUploadFile.Click += new System.EventHandler(this.btnUploadFile_Click);

            // Add to Form Controls
            this.Controls.Add(this.btnUploadFile);

        }
    }
}

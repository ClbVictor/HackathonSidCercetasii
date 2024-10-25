using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Vision.V1; // For Google Vision API
using Newtonsoft.Json; // For JSON handling
using Google.Cloud.Firestore; // For Firebase integration

namespace FluxNoteV2
{
    public partial class Form1 : Form
    {
        private string selectedImagePath = string.Empty;
        private string selectedFilePath = string.Empty;
        private Dictionary<string, string> annotations = new Dictionary<string, string>();
        private ToolTip toolTip1 = new ToolTip();
        private FirestoreDb firestoreDb;
        private string currentUsername = "LoggedUser"; // Replace with actual logged-in user
        


        public Form1()
        {
            InitializeComponent();
            InitializeFirebase();
            InitializeContextMenu();
        }

        private async void Form1_Load(object sender, EventArgs e) 
        {
            currentUsername = FirebaseHelper.LoggedInUsername; // Set the current user
            await PopulateFilesDropdown();
            PopulateCollaboratorsDropdown();
        }

        private void InitializeFirebase()
        {
            firestoreDb = FirestoreDb.Create("your-project-id"); // Replace with your Firebase project ID
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Add Annotation", null, AddAnnotation_Click);
            contextMenu.Items.Add("Edit Annotation", null, EditAnnotation_Click);
            contextMenu.Items.Add("Delete Annotation", null, DeleteAnnotation_Click);

            rtbExtractedText.ContextMenuStrip = contextMenu;
        }

        private bool CanEditFile(string fileOwner, List<string> collaborators)
        {
            return fileOwner == FirebaseHelper.LoggedInUsername || collaborators.Contains(FirebaseHelper.LoggedInUsername);
        }

        private async Task EditFile(string fileId)
        {
            DocumentReference fileRef = FirebaseHelper.firestoreDb.Collection("files").Document(fileId);
            DocumentSnapshot snapshot = await fileRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var fileData = snapshot.ToDictionary();
                string owner = fileData["owner"].ToString();
                var collaborators = fileData["collaborators"] as List<string>;

                if (CanEditFile(owner, collaborators))
                {
                    // Allow editing: Load the file contents into the RichTextBox
                    string filePath = fileData["filePath"].ToString();
                    rtbExtractedText.Text = System.IO.File.ReadAllText(filePath);
                    MessageBox.Show("You are now editing the selected file.");
                }
                else
                {
                    MessageBox.Show("You do not have permission to edit this file.");
                }
            }
            else
            {
                MessageBox.Show("File not found.");
            }
        }

        private async Task PopulateFilesDropdown()
        {
            var userFiles = await FirebaseHelper.GetUserFiles(currentUsername);
            cmbFiles.Items.Clear();

            foreach (var file in userFiles)
            {
                //if (file.ContainsKey("fileName") && file.ContainsKey("fileId")) // Ensure the keys exist
                //{
                    string fileName = (string)file.FileName;
                    string jsonString = JsonConvert.SerializeObject(file);

                // Add the file to the ComboBox
                cmbFiles.Items.Add(new KeyValuePair<string, string>(fileName, jsonString));
                //}
                
            }

            // Set the ComboBox display and value members
            cmbFiles.DisplayMember = "Key";
            cmbFiles.ValueMember = "Value";
        }

         

        private async void btnAddCollaborator_Click(object sender, EventArgs e)
        {
            if (cmbFiles.SelectedItem == null || cmbCollaborators.SelectedItem == null)
            {
                MessageBox.Show("Please select a file and a collaborator.");
                return;
            }

            // Extract the selected file ID
            var selectedFile = (KeyValuePair<string, string>)cmbFiles.SelectedItem;
            
            FirebaseHelper.FileMetadata fileMetadata = JsonConvert.DeserializeObject<FirebaseHelper.FileMetadata>(selectedFile.Value.ToString()); // Ensure that this is a valid document ID or URL
            

            // Extract the selected collaborator's username
            string collaborator = cmbCollaborators.SelectedItem.ToString();

            // Check if fileId is null or empty
            if (string.IsNullOrEmpty(fileMetadata.FileId))
            {
                MessageBox.Show("Invalid file selected. The file ID is empty.");
                return;
            }

            // Call the method to add the collaborator
            await FirebaseHelper.AddCollaboratorToFile(fileMetadata, collaborator);


        }

        private async void btnEditSelectedFile_Click(object sender, EventArgs e)
        {
            if (cmbFiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a file to edit.");
                return;
            }

            string fileId = ((KeyValuePair<string, string>)cmbFiles.SelectedItem).Value;
            await EditFile(fileId);
        }

        private async void PopulateCollaboratorsDropdown()
        {
            // Example: Fetch a list of registered users from Firestore and populate the dropdown
            var registeredUsers = await FirebaseHelper.GetAllUsers();
            cmbCollaborators.Items.Clear();

            foreach (var user in registeredUsers)
            {
                if (user != currentUsername) // Exclude the current user from collaborator list
                {
                    cmbCollaborators.Items.Add(user);
                }
            }
        }


        private void AddAnnotation_Click(object sender, EventArgs e)
        {
            if (rtbExtractedText.SelectedText.Length == 0)
            {
                MessageBox.Show("Please select text to annotate.");
                return;
            }

            using (Form annotationForm = new Form())
            {
                annotationForm.Text = "Add Annotation";
                annotationForm.Width = 400;
                annotationForm.Height = 200;

                TextBox annotationTextBox = new TextBox()
                {
                    Location = new System.Drawing.Point(10, 10),
                    Width = 350
                };

                Button submitButton = new Button()
                {
                    Text = "Submit",
                    Location = new System.Drawing.Point(10, 50),
                    Width = 80
                };

                submitButton.Click += (s, args) =>
                {
                    string selectedText = rtbExtractedText.SelectedText;
                    string annotation = annotationTextBox.Text;

                    if (!string.IsNullOrEmpty(annotation))
                    {
                        annotations[selectedText] = annotation;
                        HighlightText(selectedText);
                        annotationForm.Close();
                    }
                    else
                    {
                        MessageBox.Show("Annotation cannot be empty.");
                    }
                };

                annotationForm.Controls.Add(annotationTextBox);
                annotationForm.Controls.Add(submitButton);
                annotationForm.ShowDialog();
            }
        }

        private void HighlightText(string text)
        {
            int startIndex = rtbExtractedText.Text.IndexOf(text);
            while (startIndex != -1)
            {
                rtbExtractedText.Select(startIndex, text.Length);
                rtbExtractedText.SelectionBackColor = System.Drawing.Color.Yellow;
                startIndex = rtbExtractedText.Text.IndexOf(text, startIndex + text.Length);
            }
        }

        private void EditAnnotation_Click(object sender, EventArgs e)
        {
            if (rtbExtractedText.SelectedText.Length == 0 || !annotations.ContainsKey(rtbExtractedText.SelectedText))
            {
                MessageBox.Show("Please select annotated text to edit.");
                return;
            }

            string selectedText = rtbExtractedText.SelectedText;
            string existingAnnotation = annotations[selectedText];

            using (Form annotationForm = new Form())
            {
                annotationForm.Text = "Edit Annotation";
                annotationForm.Width = 400;
                annotationForm.Height = 200;

                TextBox annotationTextBox = new TextBox()
                {
                    Location = new System.Drawing.Point(10, 10),
                    Width = 350,
                    Text = existingAnnotation
                };

                Button submitButton = new Button()
                {
                    Text = "Submit",
                    Location = new System.Drawing.Point(10, 50),
                    Width = 80
                };

                submitButton.Click += (s, args) =>
                {
                    string updatedAnnotation = annotationTextBox.Text;
                    if (!string.IsNullOrEmpty(updatedAnnotation))
                    {
                        annotations[selectedText] = updatedAnnotation;
                        annotationForm.Close();
                    }
                    else
                    {
                        MessageBox.Show("Annotation cannot be empty.");
                    }
                };

                annotationForm.Controls.Add(annotationTextBox);
                annotationForm.Controls.Add(submitButton);
                annotationForm.ShowDialog();
            }
        }

        private void DeleteAnnotation_Click(object sender, EventArgs e)
        {
            if (rtbExtractedText.SelectedText.Length == 0 || !annotations.ContainsKey(rtbExtractedText.SelectedText))
            {
                MessageBox.Show("Please select annotated text to delete.");
                return;
            }

            string selectedText = rtbExtractedText.SelectedText;
            annotations.Remove(selectedText);
            ClearHighlight();
            HighlightAllAnnotations();
        }

        private void ClearHighlight()
        {
            rtbExtractedText.SelectAll();
            rtbExtractedText.SelectionBackColor = System.Drawing.Color.White;
        }

        private void HighlightAllAnnotations()
        {
            foreach (var annotation in annotations)
            {
                HighlightText(annotation.Key);
            }
        }

        private void rtbExtractedText_MouseHover(object sender, EventArgs e)
        {
            int cursorPosition = rtbExtractedText.SelectionStart;
            string wordUnderCursor = rtbExtractedText.SelectedText;

            if (annotations.ContainsKey(wordUnderCursor))
            {
                toolTip1.SetToolTip(rtbExtractedText, annotations[wordUnderCursor]);
            }
            else
            {
                toolTip1.SetToolTip(rtbExtractedText, string.Empty);
            }
        }

        private async void btnUploadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All Files (*.*)|*.*";
                openFileDialog.Title = "Select a File to Upload";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(selectedFilePath);

                    // Upload file to Firebase Storage
                    string fileUrl = await FirebaseHelper.UploadFileToStorage(selectedFilePath, fileName);
                    if (!string.IsNullOrEmpty(fileUrl))
                    {
                        // Save metadata to Firestore
                        await FirebaseHelper.UploadFileMetadata(fileName, selectedFilePath, fileUrl);
                    }
                    else
                    {
                        MessageBox.Show("File upload failed.");
                    }
                }
            }
        }



        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;
                    MessageBox.Show($"Image selected: {selectedImagePath}");
                }
            }
        }

        private async Task<bool> UploadFileAndSaveMetadataAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileUrl = await FirebaseHelper.UploadFileToStorage(filePath, fileName);

            if (!string.IsNullOrEmpty(fileUrl))
            {
                // Check for duplicates and upload metadata if needed
                await FirebaseHelper.UploadFileMetadataAsync(fileName, filePath, fileUrl, currentUsername);
                await PopulateFilesDropdown();
                return true;
            }
            else
            {
                MessageBox.Show("File upload failed.");
                return false;
            }
        }

        // Process Image button click event
        private async void btnProcessImage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            try
            {
                string extractedText = ExtractTextFromImage(selectedImagePath);
                string correctedText = await ProcessWithChatGPTAsync(extractedText);
                rtbExtractedText.Text = correctedText;

                // Upload the file and save metadata
                await UploadFileAndSaveMetadataAsync(selectedImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Save Text button click event
        private void btnSaveText_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbExtractedText.Text))
            {
                MessageBox.Show("No text available to save.");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.Title = "Save Extracted Text";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, rtbExtractedText.Text);
                    MessageBox.Show("Text saved successfully.");
                }
            }
        }

        // Process and Save button click event
        private async void btnProcessAndSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Please select an image first.");
                return;
            }

            // Process and save the file
            await UploadFileAndSaveMetadataAsync(selectedImagePath);
        }
    

        private string ExtractTextFromImage(string imagePath)
        {
            var client = new ImageAnnotatorClientBuilder
            {
                CredentialsPath = @"C:\Users\Lenovo\source\repos\FluxNoteV2\GoogleCredentials\subtle-anthem-439509-f1-58c8828b3602.json"
            }.Build();

            var image = Google.Cloud.Vision.V1.Image.FromFile(imagePath);
            var response = client.DetectText(image);
            StringBuilder sb = new StringBuilder();

            foreach (var annotation in response)
            {
                sb.AppendLine(annotation.Description);
            }

            return sb.ToString();
        }

        private async Task<string> ProcessWithChatGPTAsync(string extractedText)
        {
            using (var client = new HttpClient())
            {
                string apiKey = "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";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestData = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<dynamic>
                    {
                        new { role = "system", content = "You are an AI specializing in correcting Romanian text for spelling, grammar, and formatting issues." },
                        new { role = "user", content = $"The following text needs correction. Please provide the corrected version of the entire text without leaving out any details. Write the whole text:\n\n{extractedText}" }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: API returned status code {response.StatusCode} - {response.ReasonPhrase}";
                }

                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                if (responseJson == null || responseJson.choices == null || responseJson.choices.Count == 0)
                {
                    return "Error: No choices were returned from the API.";
                }

                var messageContent = responseJson.choices[0]?.message?.content?.ToString();
                if (string.IsNullOrEmpty(messageContent))
                {
                    return "Error: The API response did not contain a valid message content.";
                }

                return messageContent;
            }
        }

        private async void btnGenerateQuiz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbExtractedText.Text))
            {
                MessageBox.Show("No text available to generate a quiz.");
                return;
            }

            try
            {
                string extractedText = rtbExtractedText.Text;
                string quizQuestions = await GenerateQuizFromNotesAsync(extractedText);
                MessageBox.Show(quizQuestions, "Generated Quiz");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> GenerateQuizFromNotesAsync(string notesText)
        {
            using (var client = new HttpClient())
            {
                string apiKey = "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";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestData = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<dynamic>
                    {
                        new { role = "system", content = "You are an AI that specializes in generating quizzes based on educational notes." },
                        new { role = "user", content = $"Generate a 5-question quiz based on these notes:\n{notesText}" }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: API returned status code {response.StatusCode} - {response.ReasonPhrase}";
                }

                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                if (responseJson == null || responseJson.choices == null || responseJson.choices.Count == 0)
                {
                    return "Error: No choices were returned from the API.";
                }

                var messageContent = responseJson.choices[0]?.message?.content?.ToString();
                if (string.IsNullOrEmpty(messageContent))
                {
                    return "Error: The API response did not contain a valid message content.";
                }

                return messageContent;
            }
        }

        private async void btnAskQuestion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbExtractedText.Text))
            {
                MessageBox.Show("No text available to ask questions about.");
                return;
            }

            using (Form askQuestionForm = new Form())
            {
                askQuestionForm.Text = "Ask a Question";
                askQuestionForm.Width = 400;
                askQuestionForm.Height = 200;

                TextBox questionTextBox = new TextBox()
                {
                    Location = new System.Drawing.Point(10, 10),
                    Width = 350
                };

                Button submitButton = new Button()
                {
                    Text = "Submit",
                    Location = new System.Drawing.Point(10, 50),
                    Width = 80
                };

                submitButton.Click += async (s, args) =>
                {
                    string question = questionTextBox.Text;
                    string answer = await GetAnswerFromNotesAsync(rtbExtractedText.Text, question);
                    MessageBox.Show(answer, "Answer to Your Question");
                };

                askQuestionForm.Controls.Add(questionTextBox);
                askQuestionForm.Controls.Add(submitButton);
                askQuestionForm.ShowDialog();
            }
        }

        private async Task<string> GetAnswerFromNotesAsync(string notesText, string userQuestion)
        {
            using (var client = new HttpClient())
            {
                string apiKey = "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";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestData = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new List<dynamic>
                    {
                        new { role = "system", content = "You are an AI that specializes in answering questions based on educational notes." },
                        new { role = "user", content = $"Based on the following notes, answer this question:\n\n{notesText}\n\nQuestion: {userQuestion}" }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                string jsonRequest = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: API returned status code {response.StatusCode} - {response.ReasonPhrase}";
                }

                dynamic responseJson = JsonConvert.DeserializeObject(responseString);
                if (responseJson == null || responseJson.choices == null || responseJson.choices.Count == 0)
                {
                    return "Error: No choices were returned from the API.";
                }

                var messageContent = responseJson.choices[0]?.message?.content?.ToString();
                if (string.IsNullOrEmpty(messageContent))
                {
                    return "Error: The API response did not contain a valid message content.";
                }

                return messageContent;
            }
        }
    }
}

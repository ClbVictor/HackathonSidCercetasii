using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using Firebase.Storage;
using System.IO;

public static class FirebaseHelper
{
    public static FirestoreDb firestoreDb;
    public static string LoggedInUsername; // To track the current logged-in user

    public static void InitializeFirebase()
    {
        firestoreDb = FirestoreDb.Create("fluxnotev2-a8cc4");
        MessageBox.Show("Created Cloud Firestore client with project ID: {0}", "fluxnotev2-a8cc4");
    }

    public static async Task<bool> AuthenticateUser(string username, string password)
    {
        try
        {
            DocumentReference docRef = firestoreDb.Collection("users").Document(username);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<Dictionary<string, string>>();
                if (user.ContainsKey("username") && user.ContainsKey("password") &&
                    user["username"] == username && user["password"] == password)
                {
                    LoggedInUsername = username; // Set the logged-in user
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error authenticating user: {ex.Message}");
            return false;
        }
    }

    public static async Task UploadFileMetadata(string fileName, string filePath, string fileUrl)
    {
        try
        {
            DocumentReference docRef = firestoreDb.Collection("files").Document();
            Dictionary<string, object> fileData = new Dictionary<string, object>
        {
            { "fileName", fileName },
            { "filePath", filePath },
            { "fileUrl", fileUrl },
            { "owner", LoggedInUsername }, // Assuming you are setting this in Login
            { "collaborators", new List<string>() } // Default empty collaborators list
        };
            await docRef.SetAsync(fileData);
            MessageBox.Show("File metadata uploaded successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error uploading metadata: {ex.Message}");
        }
    }

    public static async Task<string> UploadFileToStorage(string localFilePath, string fileName)
    {

        //project id: fluxnotev2-a8cc4

        try
        {
            var stream = File.Open(localFilePath, FileMode.Open);

            var storage = new FirebaseStorage("fluxnotev2-a8cc4.appspot.com"); 

            var uploadTask = storage
                .Child("files") 
                .Child(fileName) 
                .PutAsync(stream);

            string fileUrl = await uploadTask;
            return fileUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file to Firebase Storage: {ex.Message}");
            return null;
        }
    }

    public static async Task AddCollaboratorToFile(FileMetadata fileMetadata, string collaboratorUsername)
    {
        string fileId = fileMetadata.FileId;

        if (string.IsNullOrEmpty(fileId))
        {
            throw new ArgumentException("File ID cannot be null or empty", nameof(fileId));
        }

        // Ensure that you use a valid collection and document path
        DocumentReference fileRef = firestoreDb.Collection("files").Document(fileId);

        // Get the document snapshot
        DocumentSnapshot snapshot = await fileRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            // Proceed with adding the collaborator
            Dictionary<string, object> fileData = snapshot.ToDictionary();
            if (fileData.ContainsKey("collaborators"))
            {
                List<string> collaborators = (List<string>)fileMetadata.Collaborators;
                if (!collaborators.Contains(collaboratorUsername))
                {
                    collaborators.Add(collaboratorUsername);
                    await fileRef.UpdateAsync("collaborators", collaborators);
                    MessageBox.Show($"Collaborator {collaboratorUsername} added successfully.");
                }
                else
                {
                    MessageBox.Show("Collaborator already exists.");
                }
            }
            else
            {
                MessageBox.Show("Collaborator field not found in file document.");
            }
        }
        else
        {
            MessageBox.Show("File not found.");
        }
    }

    public static async Task<List<FileMetadata>> GetUserFiles(string username)
    {
        List<FileMetadata> userFiles = new List<FileMetadata>() ;

        // Fetch files where the user is the owner
        Query ownerQuery = firestoreDb.Collection("files").WhereEqualTo("owner", username);
        QuerySnapshot ownerFiles = await ownerQuery.GetSnapshotAsync();
        Console.WriteLine($"Owner files fetched: {ownerFiles.Count}");

        foreach (DocumentSnapshot doc in ownerFiles.Documents)
        {
            Console.WriteLine($"Document ID: {doc.Id}");
            userFiles.Add(new FileMetadata {
                FileId = doc.Id,
                FileName = doc.GetValue<string>("fileName"),
                Collaborators = doc.GetValue<List<string>>("collaborators")
            });
        }

        // Fetch files where the user is a collaborator
        Query collaboratorQuery = firestoreDb.Collection("files").WhereArrayContains("collaborators", username);
        QuerySnapshot collaboratorFiles = await collaboratorQuery.GetSnapshotAsync();
        Console.WriteLine($"Collaborator files fetched: {collaboratorFiles.Count}");

        foreach (DocumentSnapshot doc in collaboratorFiles.Documents)
        {
            Console.WriteLine($"Document ID: {doc.Id}");
            userFiles.Add(new FileMetadata
            {
                FileId = doc.Id,
                FileName = doc.GetValue<string>("fileName"),
                Collaborators = doc.GetValue<List<string>>("collaborators")
            });
        }

        return userFiles;
    }

    public struct FileMetadata
    {
        
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public string FileId { get; set; }
        public List<string> Collaborators { get; set; }
    }

    public static async Task<List<string>> GetAllUsers()
    {
        try
        {
            List<string> usersList = new List<string>();
            CollectionReference usersRef = firestoreDb.Collection("users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    Dictionary<string, object> userData = doc.ToDictionary();
                    if (userData.ContainsKey("username"))
                    {
                        usersList.Add(userData["username"].ToString());
                    }
                }
            }

            return usersList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching users: {ex.Message}");
            return new List<string>();
        }
    }


    public static async Task<bool> FileExistsAsync(string fileName, string fileUrl, string owner)
    {
        // Query Firestore to check if a file with the same name, URL, and owner already exists
        Query query = firestoreDb.Collection("files")
            .WhereEqualTo("fileName", fileName)
            .WhereEqualTo("fileUrl", fileUrl)
            .WhereEqualTo("owner", owner);

        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Documents.Count > 0;
    }

    public static async Task UploadFileMetadataAsync(string fileName, string filePath, string fileUrl, string currentUsername)
    {
        string owner = currentUsername; // Ensure this is the logged-in user's username

        // Check if the file already exists in Firestore
        bool fileExists = await FileExistsAsync(fileName, fileUrl, owner);

        if (fileExists)
        {
            MessageBox.Show("This file already exists in the database.");
            return;
        }

        // If the file does not exist, proceed to add it
        DocumentReference docRef = firestoreDb.Collection("files").Document();
        Dictionary<string, object> fileData = new Dictionary<string, object>
    {
        { "fileName", fileName },
        { "filePath", filePath },
        { "fileUrl", fileUrl },
        { "owner", owner },
        { "collaborators", new List<string>() } // Default empty list for collaborators
    };

        await docRef.SetAsync(fileData);
        MessageBox.Show("File metadata added successfully.");
    }





}

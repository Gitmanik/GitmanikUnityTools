using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Gitmanik.BaseCode.Editor
{
    public class FTPXML
    {
        public string address = "";
        public string username = "", password = "";
        public string filename = "";
    }

    public class FTPUploader : EditorWindow
    {
        private const string AssetLocation = "FTPUploader.xml";

        private static FTPXML data;

        private static XmlSerializer XmlSerializer;

        [MenuItem("Gitmanik/FTP Uploader")]
        private static void Init()
        {
            FTPUploader window = (FTPUploader)GetWindow(typeof(FTPUploader));
            window.Show();
        }

        private static void LoadData()
        {
            XmlSerializer = new XmlSerializer(typeof(FTPXML));

            if (File.Exists(AssetLocation))
            {
                FileStream fs = new FileStream(AssetLocation, FileMode.Open);
                data = (FTPXML)XmlSerializer.Deserialize(fs);
                fs.Close();
            }
            else
                data = new FTPXML();
        }

        private void OnGUI()
        {
            if (data == null)
                LoadData();

            string targetFilename = data.filename.Replace("{date}", DateTime.Today.ToString("dd-MM-yyyy"));
            EditorGUILayout.LabelField("Gitmanik FTP Uploader", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent($"Address:", "Remote address ending with '/'"));
            data.address = EditorGUILayout.TextField(data.address);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Username:");
            data.username = EditorGUILayout.TextField(data.username);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Password:");
            data.password = EditorGUILayout.PasswordField(data.password);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField($"Filename: {targetFilename}", GUILayout.ExpandWidth(false));
            data.filename = EditorGUILayout.TextField(data.filename);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save"))
            {
                FileStream fs = new FileStream(AssetLocation, FileMode.OpenOrCreate);
                XmlSerializer.Serialize(fs, data);
                fs.Close();
            }

            if (GUILayout.Button("Build and Upload"))
            {
                if (!data.address.EndsWith("/"))
                {
                    if (!EditorUtility.DisplayDialog("Gitmanik FTP Uploader", $"Address does not end with slash!\n{data.address}", "Continue", "Cancel"))
                        return;
                }

                BuildPlayerOptions b = GetBuildPlayerOptions();

                string title = $"Gitmanik FTP Uploader: {targetFilename}";
                EditorUtility.DisplayProgressBar(title, $"Building Project", 0.1f);
                BuildPipeline.BuildPlayer(b);

                EditorUtility.DisplayProgressBar(title, $"Compressing to Zip", 0.5f);

                string zipFile = Path.Combine(Application.dataPath, "../" + targetFilename);
                File.Delete(zipFile);
                ZipFile.CreateFromDirectory(Path.GetDirectoryName(b.locationPathName), zipFile);

                EditorUtility.DisplayProgressBar(title, $"Uploading to FTP", 0.9f);

                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(data.username, data.password);
                    client.UploadFile("ftp://" + data.address + targetFilename, WebRequestMethods.Ftp.UploadFile, zipFile);
                }
                File.Delete(zipFile);

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Gitmanik FTP Uploader", $"Finished uploading: {targetFilename}", "OK");
            }            
        }

        private BuildPlayerOptions GetBuildPlayerOptions(bool askForLocation = false, BuildPlayerOptions defaultOptions = new BuildPlayerOptions())
        {
            MethodInfo method = typeof(BuildPlayerWindow.DefaultBuildMethods).GetMethod(
                "GetBuildPlayerOptionsInternal",
                BindingFlags.NonPublic | BindingFlags.Static);

            return (BuildPlayerOptions)method.Invoke(
                null,
                new object[] { askForLocation, defaultOptions });
        }
    }
}
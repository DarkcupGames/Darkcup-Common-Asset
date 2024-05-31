using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace DarkcupGames
{
    /// <summary>
    /// Helper class to deal with every task related to files and folder
    /// </summary>
    public class FileUtilities
    {
        //private static System.Security.Cryptography.RijndaelManaged rijndael = new System.Security.Cryptography.RijndaelManaged();

        /// <summary>
        /// Read a text file from local storage and decrypt it as needed
        /// </summary>
        /// <param name="filePath">Where the file is saved</param>
        /// <param name="password">If not null, will be used to decrypt the file</param>
        /// <param name="isAbsolutePath">Is the file path an absolute one?</param>
        /// <returns></returns>
        public static string LoadFileWithPassword(string filePath, string password = null, bool isAbsolutePath = false)
        {
            var bytes = LoadFile(filePath, isAbsolutePath);
            if (bytes != null)
            {
                string text = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                if (!string.IsNullOrEmpty(password))
                {
                    string decrypt = EncryptionHelper.Decrypt(Convert.FromBase64String(text), password);
                    if (string.IsNullOrEmpty(decrypt))
                    {
                        return null;
                    }
                    else
                    {
                        return decrypt;
                    }
                }
                else
                {
                    return text;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Read a file at specified path
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="isAbsolutePath">Is this path an absolute one?</param>
        /// <returns>Data of the file, in byte[] format</returns>
        public static byte[] LoadFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }

            string absolutePath = filePath;
            if (!isAbsolutePath) { absolutePath = GetWritablePath(filePath); }

            if (System.IO.File.Exists(absolutePath))
            {
                return System.IO.File.ReadAllBytes(absolutePath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check if a file is existed or not
        /// </summary>
        public static bool IsFileExist(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return false;
            }

            string absolutePath = filePath;
            if (!isAbsolutePath) { absolutePath = GetWritablePath(filePath); }

            return (System.IO.File.Exists(absolutePath));
        }

        public static string SaveFileWithPassword(string content, string filePath, string password = null, bool isAbsolutePath = false)
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(password))
            {
                bytes = EncryptionHelper.Encrypt(content, password);
            }
            else
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(content);
            }
            return SaveFile(bytes, filePath, isAbsolutePath);
        }



        /// <summary>
        /// Save a byte array to storage at specified path and return the absolute path of the saved file
        /// </summary>
        /// <param name="bytes">Data to write</param>
        /// <param name="filePath">Where to save file</param>
        /// <param name="isAbsolutePath">Is this path an absolute one or relative</param>
        /// <returns>Absolute path of the file</returns>
        public static string SaveFile(byte[] bytes, string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
            {
                return null;
            }

            string path = filePath;
            if (!isAbsolutePath)
            {
                path = GetWritablePath(filePath);
            }

            string folderName = Path.GetDirectoryName(path);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            File.WriteAllBytes(path, bytes);

            return path;
        }

        /// <summary>
        /// Return a path to a writable folder on a supported platform
        /// </summary>
        /// <param name="relativeFilePath">A relative path to the file, from the out most writable folder</param>
        /// <returns></returns>
        public static string GetWritablePath(string filename, string folder = "")
        {
            string path = "";

#if UNITY_EDITOR
            //result = Application.dataPath.Replace("Assets", "DownloadedData") + Path.DirectorySeparatorChar + relativeFilePath;
            path = System.IO.Directory.GetCurrentDirectory() + "\\DownloadedData";
#elif UNITY_ANDROID
		path = Application.persistentDataPath ;
#elif UNITY_IPHONE
		path = Application.persistentDataPath ;
#elif UNITY_WP8 || NETFX_CORE || UNITY_WSA
		path = Application.persistentDataPath ;
#endif
            if (folder != "")
            {
                path += Path.DirectorySeparatorChar + folder;
                if (!Directory.Exists(path))
                {
                    Debug.Log("Folder not exist: " + path);
                    Debug.Log("Create folder: " + path);
                    Directory.CreateDirectory(path);
                }
            }
            path += Path.DirectorySeparatorChar + filename;
            return path;
        }

        /// <summary>
        /// Delete a file from storage using default setting
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="isAbsolutePath">Is this file path an absolute path or relative one?</param>
        public static void DeleteFile(string filePath, bool isAbsolutePath = false)
        {
            if (filePath == null || filePath.Length == 0)
                return;

            if (isAbsolutePath)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            else
            {
                string file = GetWritablePath(filePath);
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Get a "safe" file name for current platform so that it can be access without problem
        /// </summary>
        /// <param name="fileName">File name to sanitize</param>
        /// <returns></returns>
        public static string SanitizeFileName(string fileName)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return StripVietnameseAccent(System.Text.RegularExpressions.Regex.Replace(fileName, invalidRegStr, "_"));
        }


        //string to be replaced to sanitize Vietnamese strings
        private static readonly string[] VietnameseSigns = new string[]{
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
    };
        /// <summary>
        /// Remove all accent in Vietnamese and convert to normal text
        /// </summary>
        public static string StripVietnameseAccent(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return str;
        }


        /// <summary>
        /// Serialize an object into JSON string and write it into file
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="data">Object to serialize</param>
        /// <param name="fileName">Filename to write to</param>
        public static void SerializeObjectToFile<T>(T data, string fileName, string password = null, bool isAbsoluteFilePath = false)
        {
            if (data != null)
            {
                string json = JsonConvert.SerializeObject(data);
                byte[] bytes;
                if (!string.IsNullOrEmpty(password))
                {
                    bytes = EncryptionHelper.Encrypt(json, password);
                }
                else
                {
                    bytes = System.Text.Encoding.UTF8.GetBytes(json);
                }
                SaveFile(bytes, fileName, isAbsoluteFilePath);
            }
        }

        public static T DeserializeObjectFromText<T>(string text)
        {
            T obj = default(T);
            if (!string.IsNullOrEmpty(text))
            {
                obj = JsonConvert.DeserializeObject<T>(text);
                return obj;
            }
            else
            {
                Debug.LogWarning("Trying to de-serialize object from empty text T_T, wtf man?");
                return obj;
            }
        }

        /// <summary>
        /// Deserialize an object from JSON file
        /// </summary>
        /// <typeparam name="T">Type of result object</typeparam>
        /// <param name="fileName">Json file content the serialized object</param>
        /// <returns>Object serialized in json file, if the file is not existed or invalid, the result will be default(T)</returns>
        public static T DeserializeObjectFromFile<T>(string fileName, string password = null, bool isAbsolutePath = false)
        {
            T data = default(T);
            byte[] localSaved = LoadFile(fileName, isAbsolutePath);
            if (localSaved == null)
            {
                Debug.Log(fileName + " not exist, returning null");
            }
            else
            {
                string json = System.Text.Encoding.UTF8.GetString(localSaved, 0, localSaved.Length);
                if (!string.IsNullOrEmpty(password))
                {
                    string decrypt = EncryptionHelper.Decrypt(Convert.FromBase64String(json), password);
                    if (string.IsNullOrEmpty(decrypt))
                    {
                        Debug.LogWarning("Can't decrypt file " + fileName);
                        return data;
                    }
                    else
                    {
                        json = decrypt;
                    }
                }
                data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
            return data;
        }
        public static T DeserializeObjectFromFile<T>(byte[] bytes, string password = null, bool isAbsolutePath = false)
        {
            T data = default(T);
            byte[] localSaved = bytes;
            if (localSaved == null)
            {
                Debug.Log("Bytes not exist, returning null");
            }
            else
            {
                string json = System.Text.Encoding.UTF8.GetString(localSaved, 0, localSaved.Length);
                if (!string.IsNullOrEmpty(password))
                {
                    string decrypt = EncryptionHelper.Decrypt(Convert.FromBase64String(json), password);
                    if (string.IsNullOrEmpty(decrypt))
                    {
                        Debug.LogWarning("Can't decrypt file bytes ");
                        return data;
                    }
                    else
                    {
                        json = decrypt;
                    }
                }
                data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
            return data;
        }
    }
}
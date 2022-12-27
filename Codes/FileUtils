using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace YIYI.LocalData
{
	public static class FileUtils
	{
#if UNITY_STANDALONE
	private static void OpenInMacFileBrowser(string path)
	{
		bool openInsidesOfFolder = false;
		// try mac
		string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

		if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}

		//Debug.Log("macPath: " + macPath);
		//Debug.Log("openInsidesOfFolder: " + openInsidesOfFolder);

		if (!macPath.StartsWith("\""))
		{
			macPath = "\"" + macPath;
		}

		if (!macPath.EndsWith("\""))
		{
			macPath = macPath + "\"";
		}

		string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
		Debug.Log("arguments: " + arguments);

		try
		{
			System.Diagnostics.Process.Start("open", arguments);
		}
		catch (System.ComponentModel.Win32Exception e)
		{
			// tried to open mac finder in windows
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	private static void OpenInWinFileBrowser(string path)
	{
		bool openInsidesOfFolder = false;
		// try windows
		string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

		if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
		{
			openInsidesOfFolder = true;
		}

		try
		{
			System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
		}
		catch (System.ComponentModel.Win32Exception e)
		{
			// tried to open win explorer in mac
			// just silently skip error
			// we currently have no platform define for the current OS we are in, so we resort to this
			e.HelpLink = ""; // do anything with this variable to silence warning about not using it
		}
	}

	public static void OpenInFileBrowser(string path)
	{
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			OpenInMacFileBrowser(path);
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			OpenInWinFileBrowser(path);
		}
	}
#endif

		public const string PATH_SEPARATOR = "/";

		/// <summary>
		/// 路径拼接 '/'
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		public static string CombinePath(string parent, string child)
		{
			if (null == parent) { parent = string.Empty; }
			if (null == child) { child = string.Empty; }

			int maximumLength = parent.Length + child.Length + 1;
			System.Text.StringBuilder builder = new System.Text.StringBuilder(maximumLength);
			builder.Append(parent);
			if (!parent.EndsWith(PATH_SEPARATOR) && !child.StartsWith(PATH_SEPARATOR))
			{
				builder.Append(PATH_SEPARATOR);
			}
			builder.Append(child);

			return builder.ToString();
		}

		/// <summary>
		/// 解密JSON数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static T DecryptJSONDataFromFile<T>(string filePath,bool needDecrypt = true, bool useConstantKey = false) where T : class
		{
			T data = null;
			if (System.IO.File.Exists(filePath))
			{
				try
				{
                    string jsonText = System.IO.File.ReadAllText(filePath);
                    if (needDecrypt)
                    {
						byte[] encryptArr = Convert.FromBase64String(jsonText);
                        string encryptStr = LocalCryptoGraphy.Decrypt(encryptArr, useConstantKey);
                        //data = LitJson.JsonMapper.ToObject<T>(encryptStr);
                        data = JsonConvert.DeserializeObject<T>(encryptStr);
                    }
                    else
                    {
						//data = LitJson.JsonMapper.ToObject<T>(jsonText);
                        data = JsonConvert.DeserializeObject<T>(jsonText);
                    }
                }
				catch (System.Exception ex)
				{
					Debug.LogError("Failed reading user data from: " + filePath + "\nerror: " + ex.ToString());
				}
			}
			return data;
		}

		/// <summary>
		/// 加密JSON数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="filePath"></param>
		public static void EncryptJSONDataInFile<T>(T data, string filePath,bool needEncrypt = true, bool useConstantKey = false)
		{
			try
			{
                //string userDataText = LitJson.JsonMapper.ToJson(data);
                string userDataText = JsonConvert.SerializeObject(data);

//#if UNITY_EDITOR || DEVELOPMENT
//			    string ext = Path.GetExtension(filePath);
//			    string debugPath = filePath.Substring(0, filePath.Length - ext.Length) + ".clear" + ext;
//			    File.WriteAllText(debugPath, userDataText);
//#endif
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				}
				FileUtils.DeleteFileIfExists(filePath);

                if (needEncrypt)
	            {
                    byte[] encryptArr = LocalCryptoGraphy.Encrypt(userDataText, useConstantKey);
                    string encryptStr = Convert.ToBase64String(encryptArr);
                    File.WriteAllText(filePath, encryptStr);
				}
                else
                {
                    File.WriteAllText(filePath, userDataText);
				}
            }
			catch (System.Exception ex)
			{
				Debug.LogError("Failed writing user data to: " + filePath + "\nerror: " + ex.ToString());
			}
		}

		public static string EnsureToGetPersistentDataDirectory(string directoryName)
		{
			string directoryDataPath = Path.Combine(Application.persistentDataPath, directoryName);
			if (CreateDirectoryIfNotExists(directoryDataPath))
			{
#if UNITY_IPHONE && !UNITY_EDITOR
			Debugx.Log("Calling IPhone.SetNoBackupFlag @: " + directoryDataPath);
#if UNITY_5
			UnityEngine.iOS.Device.SetNoBackupFlag(directoryDataPath);
#else
			iPhone.SetNoBackupFlag(directoryDataPath);
#endif
#endif
			}
			return directoryDataPath;
		}

		/// <summary>
		/// 创建指定路径
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public static bool CreateDirectoryIfNotExists(string directoryPath)
		{
			bool result = !Directory.Exists(directoryPath);
			if (result)
			{
				try
				{
					Directory.CreateDirectory(directoryPath);
				}
				catch (IOException ex)
				{
					Debug.LogError("Failed creating directory @ " + directoryPath + ":\n" + ex.ToString());
				}
			}
			return result;
		}

		/// <summary>
		/// 删除文件
		/// </summary>
		/// <param name="filePath"></param>
		public static void DeleteFileIfExists(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		/// <summary>
		/// 写入文本 System.IO
		/// </summary>
		/// <param name="path"></param>
		/// <param name="contents"></param>
		/// <param name="encoding"></param>
		public static void WriteAllText(string path, string contents, System.Text.Encoding encoding)
		{
			using (var streamWriter = new System.IO.StreamWriter(path, false, encoding))
			{
				streamWriter.Write(contents);
			}
		}

		/// <summary>
		/// 获取路径下所有文件名
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static string[] GetFileNameListInPath(string path, string pattern = "")
		{
			if (!Directory.Exists(path))
			{
				return null;
			}

			return Directory.GetFiles(path, pattern);
		}

		/// <summary>
		/// 拷贝文件到指定路径
		/// </summary>
		/// <param name="srcPath">源路径</param>
		/// <param name="tarPath">目标路径</param>
		public static void CopyFolder(string srcPath, string tarPath)
		{
			srcPath = srcPath.Replace("\\", "/");
			tarPath = tarPath.Replace("\\", "/");

			if (!Directory.Exists(srcPath))
			{
				Directory.CreateDirectory(srcPath);
			}
			if (!Directory.Exists(tarPath))
			{
				Directory.CreateDirectory(tarPath);
			}
			CopyFile(srcPath, tarPath);
			string[] directionName = Directory.GetDirectories(srcPath);
			foreach (string dirPath in directionName)
			{
				string directionPathTemp = tarPath + "/" + dirPath.Substring(srcPath.Length + 1);
				CopyFolder(dirPath, directionPathTemp);
			}
		}
		/// <summary>
		/// 拷贝文件到指定路径
		/// </summary>
		/// <param name="srcPath">源路径</param>
		/// <param name="tarPath">目标路径</param>
		public static void CopyFile(string srcPath, string tarPath)
		{
			string[] filesList = Directory.GetFiles(srcPath);
			foreach (string f in filesList)
			{
				string fTarPath = tarPath + "/" + f.Substring(srcPath.Length + 1);
				if (File.Exists(fTarPath))
				{
					File.Copy(f, fTarPath, true);
				}
				else
				{
					File.Copy(f, fTarPath);
				}
			}
		}
	}
}

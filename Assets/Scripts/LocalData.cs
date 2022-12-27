using System;
using System.IO;
using UnityEngine;

namespace YIYI.LocalData
{

    [Serializable]
    public class LocalData<T> : ILocalData where T : DataModelBase
    {
      
        public bool NeedEncrypt { get; private set; }

        public bool Initialized { get; private set; }

        protected string FilePath { get; private set; }      
    
        public T Data { get; private set; }

        public LocalData()
        {
            Data = new object() as T;
        }

        public virtual void Initialize(string filePath, bool needEncrypt = false, bool useConstantKey = false)
        {
            Debug.Assert(!Initialized, "Invalid " + GetType().Name + " control flow");
            Debug.Assert(!string.IsNullOrEmpty(filePath) && Directory.Exists(Path.GetDirectoryName(filePath)), "Directory doesn't exist: " + filePath);
            NeedEncrypt = needEncrypt;
            FilePath = filePath;
            InitializeData();
            LoadDataFromFile(useConstantKey);
            Initialized = true;
        }

        public virtual void DeInitialize(bool useConstantKey = false)
        {
            if (Initialized)
            {
                SaveDataToFile(useConstantKey);
                ClearData();
                FilePath = null;
                Initialized = false;
            }
        }

        public virtual void InitializeData()
        {
        }

        public virtual void ClearData()
        {
        }

        public virtual void LoadDataFromFile(bool useConstantKey = false)
        {
            var dataFromFile = FileUtils.DecryptJSONDataFromFile<T>(FilePath, NeedEncrypt, useConstantKey);
            if (null != dataFromFile)
            {
                Data = dataFromFile;
            }
        }

        public virtual void SaveDataToFile(bool useConstantKey = false)
        {
            Debug.Log($"SaveDataToFile data:{Data.ToString()} path:{FilePath}");
            FileUtils.EncryptJSONDataInFile<T>(Data, FilePath, NeedEncrypt, useConstantKey);
        }

        public virtual void DeleteJsonFile()
        {
            var dataFromFile = FileUtils.DecryptJSONDataFromFile<T>(FilePath, NeedEncrypt);
            if (null != dataFromFile)
            {
                File.Delete(FilePath);
            }
        }

        public virtual bool GetFileExist()
        {
            var dataFromFile = FileUtils.DecryptJSONDataFromFile<T>(FilePath, NeedEncrypt);
            return dataFromFile != null ? true : false;
        }

    }
}

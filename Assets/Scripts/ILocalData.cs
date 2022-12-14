using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YIYI.LocalData
{
    public interface ILocalData
    {
        void Initialize(string filePath, bool needEncrypt = false, bool useConstantKey = false);
        void DeInitialize(bool useConstantKey = false);
        void InitializeData();
        void ClearData();
        void LoadDataFromFile(bool useConstantKey = false);
        void SaveDataToFile(bool useConstantKey = false);
    }
}

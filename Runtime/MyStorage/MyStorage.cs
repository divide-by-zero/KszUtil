using System.Collections.Generic;

namespace KszUtil.MyStorage
{
    /// <summary>
    /// ストレージ管理クラス
    /// </summary>
    public class MyStorage : IMyStorage
    {
        private IMyStorage myStorage;

        /// <summary>
        /// コンストラクタで指定した時点のフォルダ名しか扱わない。　それよりも下位または上位は対象としない
        /// </summary>
        /// <param name="path"></param>
        public MyStorage(string path)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
        myStorage = new PlayerPrefsMyStorage(path);
#else
            myStorage = new EncryptMyStorage(path);
#endif
        }

        public void SaveBytes(byte[] data, DatName dataName)
        {
            myStorage.SaveBytes(data, dataName);
        }

        public byte[] LoadBytes(DatName dataName)
        {
            return myStorage.LoadBytes(dataName);
        }

        public void Serialize<T>(T data, DatName dataName)
        {
            myStorage.Serialize(data, dataName);
        }

        public T Deserialize<T>(DatName dataName)
        {
            return myStorage.Deserialize<T>(dataName);
        }

        public bool Exists(DatName datName)
        {
            return myStorage.Exists(datName);
        }

        public void DeleteData(DatName datName)
        {
            myStorage.DeleteData(datName);
        }

        public IEnumerable<string> GetFiles() => myStorage.GetFiles();

        public void SaveText(string text, DatName dataName)
        {
            myStorage.SaveBytes(System.Text.Encoding.UTF8.GetBytes(text), dataName);
        }
    }
}
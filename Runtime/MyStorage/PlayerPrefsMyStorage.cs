using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KszUtil.MyStorage
{
    public class PlayerPrefsMyStorage : IMyStorage
    {
        private string BasePath;
        private string FullPath(DatName datName) => Path.Combine(BasePath, datName.FlattenName);

        public PlayerPrefsMyStorage(string path)
        {
            BasePath = path;
        }

        public void SaveBytes(byte[] data, DatName dataName)
        {
            PlayerPrefs.SetString(FullPath(dataName), Convert.ToBase64String(data));
        }

        public byte[] LoadBytes(DatName dataName)
        {
            if (PlayerPrefs.HasKey(FullPath(dataName)) == false) return new byte[0];
            return Convert.FromBase64String(PlayerPrefs.GetString(FullPath(dataName), ""));
        }

        public void Serialize<T>(T data, DatName dataName)
        {
            PlayerPrefs.SetString(FullPath(dataName), JsonUtility.ToJson(data));
        }

        public T Deserialize<T>(DatName dataName)
        {
            if (PlayerPrefs.HasKey(FullPath(dataName)) == false) return default;
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(FullPath(dataName), ""));
        }

        public bool Exists(DatName datName)
        {
            return PlayerPrefs.HasKey(FullPath(datName));
        }

        public void DeleteData(DatName datName)
        {
            PlayerPrefs.DeleteKey(FullPath(datName));
        }

        public IEnumerable<string> GetFiles()
        {
            return new string[0]; //PlayerPrefsは入っている中身が見れないので
        }
    }
}

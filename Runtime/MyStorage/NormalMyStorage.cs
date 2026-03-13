using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KszUtil.MyStorage
{
    /// <summary>
    /// ストレージ管理クラス
    /// </summary>
    public class NormalMyStorage : IMyStorage
    {
        /// <summary>
        /// コンストラクタで指定した時点のフォルダ名しか扱わない。　それよりも下位または上位は対象としない
        /// </summary>
        /// <param name="path"></param>
        public NormalMyStorage(string path)
        {
            BasePath = Path.Combine(Application.persistentDataPath, path);
            Directory.CreateDirectory(BasePath);

#if !UNITY_EDITOR && UNITY_IOS
        if (System.IO.Directory.Exists(BasePath))
        {
            UnityEngine.iOS.Device.SetNoBackupFlag(BasePath);
        }
#endif
        }

        private string BasePath { set; get; }

        private Stream OpenRead(DatName datName)
        {
            var fullPath = FullPath(datName);
            if (File.Exists(fullPath))
            {
                return File.OpenRead(fullPath);
            }

            return Stream.Null;
        }

        /// <summary>
        /// フルパス取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string FullPath(DatName datName) => Path.Combine(BasePath, datName.FlattenName);

        private Stream OpenWrite(DatName datName)
        {
            return File.Open(FullPath(datName), FileMode.Create);
        }

        public void SaveBytes(byte[] data, DatName dataName)
        {
            using (var stream = OpenWrite(dataName))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        public async UniTask SaveBytesAsync(byte[] data, DatName dataName, CancellationToken ct)
        {
            using (var stream = OpenWrite(dataName))
            {
                await stream.WriteAsync(data, 0, data.Length, ct);
            }
        }

        public byte[] LoadBytes(DatName dataName)
        {
            using (var stream = OpenRead(dataName))
            {
                if (stream == Stream.Null)
                {
                    return new byte[0];
                }
                else
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    return data;
                }
            }
        }

        public void Serialize<T>(T data, DatName dataName)
        {
            using (var stream = OpenWrite(dataName))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(JsonUtility.ToJson(data));
                }
            }
        }

        public T Deserialize<T>(DatName dataName)
        {
            using (var stream = OpenRead(dataName))
            {
                if (stream == Stream.Null)
                {
                    return default;
                }

                using (var reader = new StreamReader(stream))
                {
                    return JsonUtility.FromJson<T>(reader.ReadToEnd());
                }
            }
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(this.BasePath).Select(s => new FileInfo(s).Name);
        }

        public bool Exists(DatName datName)
        {
            var path = FullPath(datName);
            return Directory.Exists(path) || File.Exists(path);
        }

        public void DeleteData(DatName datName)
        {
            File.Delete(FullPath(datName));
        }

        public async UniTask<byte[]> LoadBytesAsync(DatName dataName, CancellationToken ct)
        {
            using (var stream = OpenRead(dataName))
            {
                if (stream == Stream.Null)
                {
                    return new byte[0];
                }
                else
                {
                    var data = new byte[stream.Length];
                    await stream.ReadAsync(data, 0, data.Length, ct);
                    return data;
                }
            }
        }
    }
}
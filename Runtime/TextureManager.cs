using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KszUtil.MyStorage;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KszUtil
{
    public static class TextureManager
    {
        /// <summary>
        /// オンメモリキャッシュ メモリひっ迫するようなら、過去10個のような制限を付ける
        /// </summary>
        private static Dictionary<string, Texture2D> volatileTextureCache = new Dictionary<string, Texture2D>();

        public static void ClearVolatileCache()
        {
            volatileTextureCache = new Dictionary<string, Texture2D>();
        }

        public static async UniTask Load(this Image _img, string imagePath)
        {
            try
            {
                var tex = await LoadTextureAsync(imagePath);
                _img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }
            catch
            {
                _img.sprite = null;
            }
        }

        public static async UniTask Load(this RawImage _img, string imagePath)
        {
            try
            {
                _img.texture = await LoadTextureAsync(imagePath);
            }
            catch
            {
                _img.texture = null;
            }
        }

        public static void SaveTexture(Texture2D tex, string saveName = null)
        {
            var datName = new DatName(saveName);

            var myStorage = new EncryptMyStorage("Cache");

            var data = tex.EncodeToJPG();
            myStorage.SaveBytes(data, datName);
            // Debug.Log($"saveName:{saveName}で{data.Length}byteのデータを保存しました");

            volatileTextureCache[saveName] = tex;
        }

        public static async UniTask<Texture2D> LoadTextureAsync(string imagePath, bool isForceUpdate = false, string saveName = null, CancellationToken ct = default)
        {
            var datName = new DatName(saveName ?? imagePath);

            //オンメモリキャッシュがあるならそちらを使う
            if (isForceUpdate == false && volatileTextureCache.TryGetValue(saveName, out var volatileTexture))
            {
                // Debug.Log(datName.FlattenName + "はすでにオンメモリなのでそっちを使います");
                return volatileTexture;
            }

            var myStorage = new EncryptMyStorage("Cache");

            //ほとんどMock専用
            var resource = Resources.Load<Texture2D>(imagePath);
            if (resource != null)
            {
                if (myStorage.Exists(datName) == false)
                {
                    myStorage.SaveBytes(resource.EncodeToPNG(), datName);
                }
            }

            //ローカルファイルキャッシュが在る場合はキャッシュのbyteを取得して、byte[]→Textureに変換して使う。　キャッシュが無い場合はロードしてbyte[]として保存しておく。
            bool hasCache = myStorage.Exists(datName);
            Texture2D tex = null;
            if (hasCache && isForceUpdate == false)
            {
                tex = new Texture2D(2, 2, TextureFormat.ARGB4444, false);
                var bytes = await myStorage.LoadBytesAsync(datName, ct);
                // var bytes = myStorage.LoadBytes(datName);
                tex.LoadImage(bytes);
                tex.filterMode = FilterMode.Point;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Compress(false);
            }
            else
            {
                if (imagePath.IndexOf("http://", StringComparison.CurrentCultureIgnoreCase) < 0 &&
                    imagePath.IndexOf("https://", StringComparison.CurrentCultureIgnoreCase) < 0 &&
                    imagePath.IndexOf("file://", StringComparison.CurrentCultureIgnoreCase) < 0) return null;
                var www = UnityWebRequestTexture.GetTexture(imagePath);
                await www.SendWebRequest();
                if (ct != default)
                {
                    ct.ThrowIfCancellationRequested();
                }

                if (www.isNetworkError)
                {
                    //                    throw new Exception(www.error);
                    return null;
                }

                using (var handler = www.downloadHandler as DownloadHandlerTexture)
                {
                    tex = handler?.texture;
                    var data = handler?.data;
                    if (data != null)
                    {
                        // await myStorage.SaveBytesAsync(data, datName, ct);
                        myStorage.SaveBytes(data, datName);
                        // Debug.Log($"saveName:{saveName}で{data.Length}byteのデータを保存しました");
                    }
                }
            }

            volatileTextureCache[saveName] = tex;
            return tex;
        }

        public static void RemoveTexture(string path)
        {
            var myStorage = new EncryptMyStorage("Cache");
            var datName = new DatName(path);
            if (myStorage.Exists(datName))
            {
                myStorage.DeleteData(datName);
            }

            if (volatileTextureCache.ContainsKey(path))
            {
                volatileTextureCache.Remove(path);
            }
        }
    }
}
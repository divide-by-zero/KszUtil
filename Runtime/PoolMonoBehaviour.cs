using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KszUtil
{
    public class PoolMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static List<T> activeList = new List<T>();
        private static List<T> inactiveList = new List<T>();
        private static GameObject _poolContainer;

        private static GameObject PoolContainer
        {
            get
            {
                if (!_poolContainer) _poolContainer = new GameObject(typeof(T).Name);
                return _poolContainer;
            }
        }

        public static T PoolInstantiate(T prefab)
        {
            return PoolInstantiate(prefab.gameObject, Vector3.zero, Quaternion.identity);
        }

        public static T PoolInstantiate(T prefab, Vector3 position, Quaternion rotation)
        {
            return PoolInstantiate(prefab.gameObject, position, rotation);
        }

        public static T PoolInstantiate(GameObject prefab)
        {
            return PoolInstantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        public static T PoolInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            activeList.RemoveAll(obj => !obj);
            inactiveList.RemoveAll(obj => !obj);

            //inactiveListが空なら作る。　じゃないならそこから返す
            if (inactiveList.Any())
            {
                var obj = inactiveList.First();
                inactiveList.Remove(obj);
                activeList.Add(obj);
                obj.transform.SetParent(null, true);
                obj.gameObject.SetActive(true);
                obj.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
                obj.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                obj.transform.localPosition = position;
                obj.transform.rotation = rotation;
                return obj;
            }
            var ret = GameObject.Instantiate(prefab).GetComponent<T>();
            if (ret != null)
            {
                activeList.Add(ret);
                ret.transform.localPosition = position;
                ret.transform.rotation = rotation;
            }
            return ret;
        }

        public static void PoolDestroy(T obj)
        {
            //非表示にして、active→inactiveへ移動させる
            activeList.Remove(obj);
            obj.transform.SetParent(PoolContainer.transform, false);
            inactiveList.Add(obj);
            obj.gameObject.SetActive(false);
        }

        //あまり使うことはなさそうだけれど、完璧後始末
        public static void DestoryAll()
        {
            foreach (var obj in inactiveList)
            {
                GameObject.Destroy(obj.gameObject);
            }
            foreach (var obj in activeList)
            {
                GameObject.Destroy(obj.gameObject);
            }
            if (_poolContainer)
            {
                GameObject.Destroy(_poolContainer);
            }
            inactiveList.Clear();
            activeList.Clear();
            _poolContainer = null;
        }
    }
}

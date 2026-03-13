using UnityEngine;

namespace KszUtil
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        static SingletonMonoBehaviour()
        {
            StaticFieldResetter.Register(() => sInstance = null);
        }

        private static T sInstance;

        public static T Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType(typeof(T)) as T;
                    if (sInstance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        sInstance = obj.AddComponent<T>();
                        obj.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
                    }

                    DontDestroyOnLoad(sInstance);
                }

                return sInstance;
            }
        }

        protected virtual void Awake()
        {
            if (this == Instance)
            {
                DontDestroyOnLoad(Instance);
                Instance.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
                return;
            }

            Destroy(gameObject); //２つは作らない//
        }
    }

    public class SingletonMonoBehaviourWithoutDontDestroy<T> : MonoBehaviour where T : SingletonMonoBehaviourWithoutDontDestroy<T>
    {
        static SingletonMonoBehaviourWithoutDontDestroy()
        {
            StaticFieldResetter.Register(() => sInstance = null);
        }

        private static T sInstance;

        public static T Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType(typeof(T)) as T;
                    if (sInstance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        obj.AddComponent<T>();
                        obj.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
                    }
                }

                return sInstance;
            }
        }

        protected virtual void Awake()
        {
            if (this == Instance)
            {
                Instance.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
                return;
            }

            Destroy(gameObject); //２つは作らない//
        }
    }
}

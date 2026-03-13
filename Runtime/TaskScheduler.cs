using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KszUtil
{
    public class TaskScheduler : MonoBehaviour
    {
        private static TaskScheduler instance;

        private readonly Dictionary<Guid, IEnumerator> iteratorList = new Dictionary<Guid, IEnumerator>();
        public float targetTime = 0.01f;

        public static TaskScheduler Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = FindObjectOfType(typeof(TaskScheduler)) as TaskScheduler; //既にシーンに存在する場合はそれを使用
                if (!instance)
                {
                    instance = new GameObject("TaskScheduler").AddComponent<TaskScheduler>(); //さらにない場合は作成
                }
                return instance;
            }
        }

        public Guid AddIterator(IEnumerator iterator)
        {
            var guid = Guid.NewGuid();
            iteratorList.Add(guid, iterator);
            return guid;
        }

        public void RemoveIterator(Guid guid)
        {
            if (iteratorList.ContainsKey(guid))
            {
                iteratorList.Remove(guid);
            }
        }

        public void StopAllIterator()
        {
            iteratorList.Clear();
        }

        public Guid AddAction(Action act)
        {
            return AddIterator(ActionIterator(act));
        }

        public IEnumerator ActionIterator(Action act)
        {
            if (act != null) act();
            yield return null;
        }

        public void Start()
        {
            StartCoroutine(Iterator());
        }

        private IEnumerator Iterator()
        {
            while (true)
            {
                var ts = Time.realtimeSinceStartup;
                do
                {
                    if (iteratorList.Count <= 0) break;
                    foreach (var itr in iteratorList.ToList())
                    {
                        if (itr.Value.MoveNext() == false)
                        {
                            iteratorList.Remove(itr.Key);
                        }
                        if (Time.realtimeSinceStartup - ts > targetTime) yield return null;
                    }
                }
                while (Time.realtimeSinceStartup - ts <= targetTime);
                yield return null;
            }
        }
    }
}

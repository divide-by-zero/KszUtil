using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace KszUtil
{
    public class CollisionHolder : MonoBehaviour
    {
        private HashSet<GameObject> collisionGameObjectSet = new HashSet<GameObject>();
        [SerializeField] private LayerMask targetLayer;

        private Subject<GameObject> _addSubject = new Subject<GameObject>();
        public IObservable<GameObject> OnAdd => _addSubject;
        private Subject<GameObject> _removeSubject = new Subject<GameObject>();
        public IObservable<GameObject> OnRemove => _removeSubject;
        public IObservable<Unit> OnChanged => Observable.Merge(OnAdd, OnRemove).AsUnitObservable();

        /// <summary>
        /// 当たっているGameObject
        /// </summary>
        public IEnumerable<GameObject> CollisionGameObjects => collisionGameObjectSet;
        public bool AnyHit => collisionGameObjectSet.Any();

        private void Add(GameObject go)
        {
            if (IsMask(targetLayer, go.layer)) return;
            if (collisionGameObjectSet.Add(go)) { _addSubject.OnNext(go); }
        }
        private void OnCollisionEnter(Collision other) => Add(other.gameObject);
        private void OnCollisionStay(Collision other) => Add(other.gameObject);
        private void OnTriggerEnter(Collider other) => Add(other.gameObject);
        private void OnTriggerStay(Collider other) => Add(other.gameObject);

        private void Remove(GameObject go)
        {
            if (IsMask(targetLayer, go.layer)) return;
            if (collisionGameObjectSet.Remove(go)) { _removeSubject.OnNext(go); }
        }

        private void OnCollisionExit(Collision other) => Remove(other.gameObject);
        private void OnTriggerExit(Collider other) => Remove(other.gameObject);

        private void Update()
        {
            foreach (var o in collisionGameObjectSet.Where(o => o == null || o.activeSelf == false).ToArray())
            {
                if (collisionGameObjectSet.Remove(o)) { _removeSubject.OnNext(o); }
            }
        }

        private bool IsMask(LayerMask mask, int layerId)
        {
            return (mask & LayerMask.GetMask(LayerMask.LayerToName(layerId))) == 0;
        }
    }
}

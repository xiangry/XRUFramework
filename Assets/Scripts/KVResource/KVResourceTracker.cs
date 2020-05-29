using UnityEngine;

namespace KVResource
{
    public class KVResourceTracker : MonoBehaviour
    {
        public delegate void DelegateDestroyed(KVResourceTracker tracker);
        public event DelegateDestroyed OnDestroyed;

        public string key { get; set; }

        void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}
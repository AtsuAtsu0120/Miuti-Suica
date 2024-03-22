using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Foundation;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace Core.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingObject : MonoBehaviour
    {
        public int SizeIndex { get; set; }
        public bool UnionObj { get; set; }
        public bool IsSimulated => _rigid.simulated;

        [SerializeField] private Rigidbody2D _rigid;
        private IPublisher<UnionInfo> _unionPublisher;
        

        [Inject]
        public void Constructor(IPublisher<UnionInfo> unionPublisher)
        {
            _unionPublisher = unionPublisher;
        }
        public void Start()
        {
            this.GetAsyncCollisionEnter2DTrigger().Subscribe(x =>
            {
                if (x.gameObject.TryGetComponent<FallingObject>(out var otherObj))
                {
                    if (SizeIndex == otherObj.SizeIndex)
                    {
                        _unionPublisher.Publish(new UnionInfo(this, otherObj));
                    }
                }
            }).AddTo(destroyCancellationToken);
            
            if (!UnionObj)
            {
                _rigid.simulated = false;   
            }
        }

        public void StartFail()
        {
            _rigid.simulated = true;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _rigid = GetComponent<Rigidbody2D>();
        }
        #endif
    }
}

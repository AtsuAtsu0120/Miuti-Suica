using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Foundation;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = System.Random;

namespace Core.Components
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private MiuchiAsset _asset;
        [SerializeField] private Transform _spawnPositionTransform;
        [SerializeField] private OutArea _outArea;
        [SerializeField] private Input _input;
        [SerializeField] private GameOverUI _gameOverUI;
        [SerializeField] private InGameUI _inGameUI;
 
        private ISubscriber<UnionInfo> _unionSubscriber;
        private IObjectResolver _container;
        
        private Random _random;
        private readonly HashSet<FallingObject> _collisionObjects = new();

        private bool _isGameOver;
        private int _score;

        private const int ScorePower = 3;
        
        [Inject]
        public void Constructor(IObjectResolver container, ISubscriber<UnionInfo> unionSubscriber)
        {
            _container = container;
            _unionSubscriber = unionSubscriber;
        }

        public void Start()
        {
            _gameOverUI.SetUIActive(false);
            _random = new();
            _unionSubscriber.Subscribe(info =>
            {
                OnCollision(info);
            });
            _outArea.GetAsyncTriggerStay2DTrigger()
                .Take(1).Subscribe(_ =>
                {
                    _isGameOver = true;
                    
                    _gameOverUI.SetUIActive(true);
                });
            
            SpawnFallingObject();
        }

        private void SpawnFallingObject()
        {
            if (_isGameOver)
            {
                return;
            }
            
            var randomValue = _random.Next(0, _asset.FallingObjects.Length);
            var prefab = _asset.FallingObjects[randomValue];
            prefab.transform.position = _spawnPositionTransform.position;
            
            var obj = _container.Instantiate(prefab);
            obj.SizeIndex = randomValue;
            
            obj.GetAsyncCollisionEnter2DTrigger().Take(1).Subscribe(x =>
            {
                SpawnFallingObject();
                obj.gameObject.layer = 3;
            }).AddTo(obj.destroyCancellationToken);

            _input.FallingObject = obj;
        }

        private void OnCollision(in UnionInfo info)
        {
            if (info.Self.SizeIndex != info.Other.SizeIndex)
            {
                return;
            }
            
            // 削除できれば登録されているので、合体させる。
            if (_collisionObjects.Remove(info.Self))
            {
                Union(info);
            }
            else
            {
                _collisionObjects.Add(info.Other);
            }
        }
        private void Union(in UnionInfo info)
        {
            if (info.Self.SizeIndex + 1 >= _asset.FallingObjects.Length)
            {
                return;
            }
            
            var newObjSize = info.Self.SizeIndex + 1;
            var prefab = _asset.FallingObjects[newObjSize];
            prefab.transform.position = info.Self.transform.position;
            var newObj = _container.Instantiate(prefab);
            
            newObj.SizeIndex = newObjSize;
            newObj.UnionObj = true;

            _score += newObjSize * ScorePower;
            _inGameUI.SetScore(_score);
            
            Destroy(info.Self.gameObject);
            Destroy(info.Other.gameObject);
        }
    }
}

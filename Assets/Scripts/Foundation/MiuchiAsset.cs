using Core.Components;
using UnityEngine;

namespace Foundation
{
    [CreateAssetMenu]
    public class MiuchiAsset : ScriptableObject
    {
        public FallingObject[] FallingObjects => _fallingObjects;
        [SerializeField] private FallingObject[] _fallingObjects;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Core.Components
{
    public class Input : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public FallingObject FallingObject { get; set; }
        
        private Vector2 _previousPoint;

        private const float MaxPosition = 1.5f;
        private const float MinPosition = -1.5f;
        private const float Sensi = 0.005f;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(FallingObject.IsSimulated) return;
            
            _previousPoint = eventData.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if(FallingObject.IsSimulated) return;
            
            var result = eventData.position.x - _previousPoint.x;
            var objPosition = FallingObject.transform.position;
            var newPositionX = 
                Mathf.Clamp(objPosition.x + result * Sensi, MinPosition, MaxPosition);
            FallingObject.transform.position = new (newPositionX, objPosition.y);
            
            _previousPoint = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(FallingObject.IsSimulated) return;
            
            FallingObject.StartFail();
            _previousPoint = default;
        }

        public void Retry()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
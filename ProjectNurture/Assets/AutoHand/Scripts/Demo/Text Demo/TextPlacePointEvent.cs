using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

namespace Autohand.Demo{
    public class TextPlacePointEvent : MonoBehaviour{
        public TextChanger changer;
        public PlacePoint point;
        public float changeTime = 0.2f;
        public float fadeTime = 5;
        [TextArea]
        public string placeMessage;
        [TextArea]
        public string highlightMessage;

        private void Start() {
            if(point == null && GetComponent<PlacePoint>() != null)
                point = GetComponent<PlacePoint>();
            point.OnPlaceEvent += OnGrab;
            point.OnRemoveEvent += OnRelease;
            point.OnHighlightEvent += OnHighlight;
            point.OnStopHighlightEvent += OnRelease;
        }
        
        void OnGrab(PlacePoint hand, Grabbable grab) {
            changer.UpdateText(gameObject, placeMessage, changeTime);
        }

        void OnHighlight(PlacePoint hand, Grabbable grab) {
            changer.UpdateText(gameObject, highlightMessage, changeTime);
        }
        
        void OnRelease(PlacePoint hand, Grabbable grab) {
            changer.HideText(changeTime, fadeTime);
        }

    }
}

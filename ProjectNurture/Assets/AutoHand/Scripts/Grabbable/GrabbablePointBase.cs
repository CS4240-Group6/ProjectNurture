using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    [RequireComponent(typeof(Grabbable))]
    public class GrabbablePointBase : MonoBehaviour{
        public virtual bool Align(Hand hand) {
            return true;
        }
    }
}

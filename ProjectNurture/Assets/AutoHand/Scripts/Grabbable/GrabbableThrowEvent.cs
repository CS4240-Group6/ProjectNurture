using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    [RequireComponent(typeof(Rigidbody), typeof(Grabbable))]
    public class GrabbableThrowEvent : MonoBehaviour{
        [Tooltip("The velocity required on collision to cause the break event")]
        public float breakVelocity = 1;
        [Tooltip("The layers that will cause this grabbale to break")]
        public LayerMask collisionLayers = ~0;
        public UnityEvent OnBreak;
        Rigidbody rb;
        Grabbable grab;

        void Start(){
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<Grabbable>();
        }

        private void OnCollisionEnter(Collision collision) {
            if(grab == null)
                return;

            if(!grab.IsThrowing())
                return;

            if(((1 << collision.collider.gameObject.layer) & collisionLayers) == 0)
                return;
        
            if(rb.velocity.magnitude >= breakVelocity) {
                Invoke("Break", Time.fixedDeltaTime);
            }
        }

        void Break() {
            OnBreak.Invoke();
        }
}
}

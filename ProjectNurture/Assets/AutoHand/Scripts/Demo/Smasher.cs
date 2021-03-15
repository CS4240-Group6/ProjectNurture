using UnityEngine;

namespace Autohand.Demo{
    [RequireComponent(typeof(Rigidbody))]
    public class Smasher : MonoBehaviour{
        Rigidbody rb;
        public float forceMulti = 1;

        private void Start() {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision) {
            var smash = collision.transform.GetComponent<Smash>();
            if(smash != null) {
                if(rb.velocity.magnitude*forceMulti >= smash.smashForce)
                    smash.DoSmash();
            }
        }
    }
}

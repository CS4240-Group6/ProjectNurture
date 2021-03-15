using UnityEngine;

namespace Autohand.Demo{
    public class CubeBreak : MonoBehaviour{
        public float force = 10f;
        Vector3[] offsets = { new Vector3(0.25f, 0.25f, 0.25f), new Vector3(-0.25f, 0.25f, 0.25f), new Vector3(0.25f, 0.25f, -0.25f), new Vector3(-0.25f, 0.25f, -0.25f),
                            new Vector3(0.25f, -0.25f, 0.25f), new Vector3(-0.25f, -0.25f, 0.25f), new Vector3(0.25f, -0.25f, -0.25f), new Vector3(-0.25f, -0.25f, -0.25f),};
        [ContextMenu("Break")]
        public void Break() {
            for(int i = 0; i < 8; i++) {
                var smallerCopy = Instantiate(gameObject, transform.position, transform.rotation);
                foreach(var joint in smallerCopy.GetComponents<FixedJoint>()) {
                    Destroy(joint);
                }
                smallerCopy.transform.parent = transform;
                smallerCopy.transform.localPosition += offsets[i];
                smallerCopy.transform.parent = null;
                smallerCopy.transform.localScale = transform.localScale/2f;
                smallerCopy.layer = LayerMask.NameToLayer(Hand.grabbableLayerNameDefault);
                smallerCopy.GetComponent<Rigidbody>().ResetCenterOfMass();
                smallerCopy.GetComponent<Rigidbody>().ResetInertiaTensor();
                smallerCopy.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
                smallerCopy.GetComponent<Rigidbody>().AddRelativeForce(transform.rotation*(offsets[i]*force), ForceMode.Impulse);
                smallerCopy.GetComponent<Rigidbody>().AddRelativeTorque(transform.rotation*(offsets[i]*force + Vector3.one*(Random.value/3f)), ForceMode.Impulse);
                smallerCopy.GetComponent<Rigidbody>().mass /= 2;
                smallerCopy.GetComponent<Grabbable>().jointBreakForce /= 2;
                if(smallerCopy.transform.localScale.x < 0.03f)
                    smallerCopy.GetComponent<Grabbable>().singleHandOnly = true;
            }
            Destroy(gameObject);
        }

    }
}

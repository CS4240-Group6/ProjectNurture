using UnityEngine;
using UnityEngine.SceneManagement;

namespace Autohand.Demo{
    public class ButtonDemoRespawn : MonoBehaviour{
        public GameObject[] respawns;
        Vector3[] startPos;
        Quaternion[] startRot;


        void Start(){
            startPos = new Vector3[respawns.Length];
            startRot = new Quaternion[respawns.Length];
            for(int i = 0; i < respawns.Length; i++) {
                startPos[i] = respawns[i].transform.position;
                startRot[i] = respawns[i].transform.rotation;
            }
        }

        public void Respawn() {
            for(int i = 0; i < respawns.Length; i++) {
                if(respawns[i].GetComponent<Rigidbody>() != null){
                    respawns[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    respawns[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    respawns[i].GetComponent<Rigidbody>().ResetInertiaTensor();
                }
                respawns[i].transform.position = startPos[i];
                respawns[i].transform.rotation = startRot[i];
            }
        }

        public void ReloadScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
}
}
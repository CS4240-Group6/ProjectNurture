using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
    public class Spinner : MonoBehaviour{
        public Vector3 rotationSpeed;
    
        void Update(){
            transform.Rotate(rotationSpeed*Time.deltaTime);
        }
    }
}

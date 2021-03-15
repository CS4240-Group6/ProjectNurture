using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
public class PlayerFallCatcher : MonoBehaviour{
    Vector3 startPos;

    void Awake(){
        startPos = transform.position;
    }
        
    void Update(){
        if(transform.position.y < -10f)
            transform.position = startPos;
    }
}
}

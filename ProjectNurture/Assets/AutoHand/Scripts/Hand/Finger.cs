using UnityEngine;

namespace Autohand{
    public class Finger : MonoBehaviour{
        [Header("Tips")]
        [Tooltip("This transfrom will represent the tip/stopper of the finger")]
        public Transform tip;
        [Tooltip("This determines the radius of the spherecast check when bending fingers")]
        public float tipRadius = 0.01f;
        [Tooltip("This will offset the fingers bend (0 is no bend, 1 is full bend)")]
        [Range(0, 1f)]
        public float bendOffset;
        public float fingerSmoothSpeed = 1;
        float currBendOffset = 0;
        
        float bend = 0;

        [SerializeField]
        [HideInInspector]
        Quaternion[] minGripRotPose;

        [SerializeField]
        [HideInInspector]
        Vector3[] minGripPosPose;

        [SerializeField]
        [HideInInspector]
        Quaternion[] maxGripRotPose;

        [SerializeField]
        [HideInInspector]
        Vector3[] maxGripPosPose;
    
        [SerializeField]
        [HideInInspector]
        Transform[] fingerJoints;
        
        float lastHitBend;

        void Update() {
            SlowBend();
        }

        /// <summary>Forces the finger to a bend until it hits something on the given physics layer</summary>
        /// <param name="steps">The number of steps and physics checks it will make lerping from 0 to 1</param>
        public bool BendFingerUntilHit(int steps, int layermask) {
            ResetBend();
            for(int i = 0; i < steps; i++) {
                var results = new Collider[]{ null };
                var hits = Physics.OverlapSphereNonAlloc(tip.transform.position, tipRadius, results, layermask, QueryTriggerInteraction.Ignore);
                if(results[0] == null){
                    for(int j = 0; j < fingerJoints.Length; j++){
                        fingerJoints[j].localPosition = Vector3.Lerp(minGripPosPose[j], maxGripPosPose[j], (float)i/steps);
                        fingerJoints[j].localRotation = Quaternion.Lerp(minGripRotPose[j], maxGripRotPose[j], (float)i/steps);
                    }
                }
                else{
                    lastHitBend = (float)i/steps;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Forces the finger to a bend until it hits something on the given physics layer</summary>
        /// <param name="steps">The number of steps and physics checks it will make lerping from 0 to 1</param>
        public bool BendFingerUntilHit(int steps, RaycastHit target) {
            ResetBend();
            if(target.transform.transform == null){
                SetFingerBend(0);
                return false;
            }

            for(int i = 0; i < steps; i++){
                var hits = Physics.OverlapSphere(tip.transform.position, tipRadius, 1 << target.collider.transform.gameObject.layer, QueryTriggerInteraction.UseGlobal);
                
                bool didHit = false;
                foreach(var hit in hits) {
                    if(ReferenceEquals(hit.transform.gameObject, target.collider.transform.gameObject))
                        didHit = true;
                }

                if(!didHit){
                    for(int j = 0; j < fingerJoints.Length; j++){
                        fingerJoints[j].localPosition = Vector3.Lerp(minGripPosPose[j], maxGripPosPose[j], (float)i/steps);
                        fingerJoints[j].localRotation = Quaternion.Lerp(minGripRotPose[j], maxGripRotPose[j], (float)i/steps);
                    }
                }
                else{
                    return true;
                }
            }

            return false;
        }
    
        /// <summary>Bends the finger unless its hitting something</summary>
        /// <param name="bend">0 is no bend / 1 is full bend</param>
        public bool UpdateFingerBend(float bend, int layermask) {
            var results = new Collider[]{ null };
            var hits = Physics.OverlapSphereNonAlloc(tip.transform.position, tipRadius, results, layermask, QueryTriggerInteraction.Ignore);
            if(this.bend > bend || results[0] == null){
                this.bend = bend;
                for(int i = 0; i < fingerJoints.Length; i++) {
                    fingerJoints[i].localPosition = Vector3.Lerp(minGripPosPose[i], maxGripPosPose[i], bend+currBendOffset);
                    fingerJoints[i].localRotation = Quaternion.Lerp(minGripRotPose[i], maxGripRotPose[i], bend+currBendOffset);
                }
                return true;
            }
            return false;
        }

        public void UpdateFinger() {
            for(int i = 0; i < fingerJoints.Length; i++) {
                fingerJoints[i].localPosition = Vector3.Lerp(minGripPosPose[i], maxGripPosPose[i], bend+currBendOffset);
                fingerJoints[i].localRotation = Quaternion.Lerp(minGripRotPose[i], maxGripRotPose[i], bend+currBendOffset);
            }
        }

        public void UpdateFinger(float bend) {
            for(int i = 0; i < fingerJoints.Length; i++) {
                fingerJoints[i].localPosition = Vector3.Lerp(minGripPosPose[i], maxGripPosPose[i], bend+currBendOffset);
                fingerJoints[i].localRotation = Quaternion.Lerp(minGripRotPose[i], maxGripRotPose[i], bend+currBendOffset);
            }
        }
    
        /// <summary>Forces the finger to a bend ignoring physics and offset</summary>
        /// <param name="bend">0 is no bend / 1 is full bend</param>
        public void SetFingerBend(float bend) {
            for(int i = 0; i < fingerJoints.Length; i++) {
                fingerJoints[i].localPosition = Vector3.Lerp(minGripPosPose[i], maxGripPosPose[i], bend);
                fingerJoints[i].localRotation = Quaternion.Lerp(minGripRotPose[i], maxGripRotPose[i], bend);
            }
        }

        //This function smooths the finger bend so you can change the grip over a frame and wont be a jump
        void SlowBend(){
            if(currBendOffset != bendOffset) {
                bool less = (currBendOffset < bendOffset) ? true : false;
                currBendOffset += ((currBendOffset < bendOffset) ? Time.deltaTime : -Time.deltaTime) * (Mathf.Abs(currBendOffset - bendOffset)*20*fingerSmoothSpeed);
                if(less && currBendOffset > bendOffset)
                    currBendOffset = bendOffset;
                else if(!less && currBendOffset < bendOffset)
                    currBendOffset = bendOffset;
            }
        }
    



        [ContextMenu("ResetBend")]
        public void ResetBend() {
            for(int i = 0; i < fingerJoints.Length; i++) {
                fingerJoints[i].localPosition = minGripPosPose[i];
                fingerJoints[i].localRotation = minGripRotPose[i];
            }
        }

        [ContextMenu("Grip")]
        public void Grip() {
            for(int i = 0; i < fingerJoints.Length; i++) {
                fingerJoints[i].localPosition = maxGripPosPose[i];
                fingerJoints[i].localRotation = maxGripRotPose[i];
            }
        }


        /// <summary>Returns the bend the finger ended with from the last BendFingerUntilHit() call</summary>
        public float GetLastHitBend() {
            return lastHitBend;
        }
    

        [ContextMenu("Set Closed Finger Pose")]
        public void SetMinPose(){
            int GetKidsCount(Transform obj, ref int count) {
                if(obj != tip){
                    count++;
                    for(int k = 0; k < obj.childCount; k++) {
                        GetKidsCount(obj.GetChild(k), ref count);
                    }
                }
                return count;

            }

            int points = 0;
            GetKidsCount(transform, ref points);
            minGripPosPose = new Vector3[points];
            minGripRotPose = new Quaternion[points];
            fingerJoints = new Transform[points];
            
            int i = 0;
            AssignChildrenPose(transform, ref i);
            void AssignChildrenPose(Transform obj, ref int index) {
                if(obj != tip){
                    AssignPoint(index, obj.localPosition, obj.localRotation, obj);
                    index++;
                    for(int j = 0; j < obj.childCount; j++) {
                        AssignChildrenPose(obj.GetChild(j), ref index);
                    }
                }
            }

            void AssignPoint(int point, Vector3 pos, Quaternion rot, Transform joint) {
                minGripPosPose[point] = pos;
                minGripRotPose[point] = rot;
                fingerJoints[point] = joint;
            }
        }


    
        [ContextMenu("Set Open Finger Pose")]
        public void SetMaxPose(){
            int GetKidsCount(Transform obj, ref int count) {
                if(obj != tip){
                    count++;
                    for(int k = 0; k < obj.childCount; k++) {
                        GetKidsCount(obj.GetChild(k), ref count);
                    }
                }
                return count;
            }

            int points = 0;
            GetKidsCount(transform, ref points);
            maxGripPosPose = new Vector3[points];
            maxGripRotPose = new Quaternion[points];
            fingerJoints = new Transform[points];

            int i = 0;
            AssignChildrenPose(transform, ref i);
            void AssignChildrenPose(Transform obj, ref int index){
                if(obj != tip){
                    AssignPoint(index, obj.localPosition, obj.localRotation, obj);
                    index++;
                    for(int j = 0; j < obj.childCount; j++) {
                        AssignChildrenPose(obj.GetChild(j), ref index);
                    }
                }
            }

            void AssignPoint(int point, Vector3 pos, Quaternion rot, Transform joint) {
                maxGripPosPose[point] = pos;
                maxGripRotPose[point] = rot;
                fingerJoints[point] = joint;
            }
        }


        public float GetCurrentBend() {
            return bend;
        }
    

        private void OnDrawGizmos() {
            if(tip == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(tip.transform.position, tipRadius);
        }
    }
}

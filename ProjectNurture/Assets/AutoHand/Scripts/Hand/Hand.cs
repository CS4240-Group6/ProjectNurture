using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Autohand {
    public delegate void HandGrabEvent(Hand hand, Grabbable grabbable);

    public enum HandType {
        both,
        right,
        left,
        none
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Hand : MonoBehaviour {

        [Header("Fingers")]
        public Finger[] fingers;
        
        [Header("Follow Settings"), Space]
        [Tooltip("Follow target, the hand will always try to match this transforms rotation and position with rigidbody movements")]
        public Transform follow;
        
        [Tooltip("This will offset the position without offsetting the rotation pivot")]
        public Vector3 followPositionOffset;

        [Tooltip("Follow target speed (Can cause jittering if turned too high - recommend increasing drag with speed)"), Min(0)]
        public float followPositionStrength = 30;

        [Tooltip("Follow target rotation speed (Can cause jittering if turned too high - recommend increasing angular drag with speed)"), Min(0)]
        public float followRotationStrength = 30;

        [Tooltip("The maximum allowed velocity of the hand"), Min(0)]
        public float maxVelocity = 4;
        

        [Tooltip("Returns hand to the target after this distance"), Min(0)]
        public float maxFollowDistance = 0.75f;


        
        [Header("Hand"), Space]
        public bool left = false;

        [Tooltip("An empty GameObject that should be placed on the surface of the center of the palm")]
        public Transform palmTransform;

        [Tooltip("Amplifier for applied velocity on released object"), Min(0)]
        public float throwPower = 1.1f;

        [Tooltip("Maximum distance for pickup"), Min(0)]
        public float reachDistance = 0.3f;
        
        
        [Header("Pose"), Space]
        [Tooltip("Turn this on when you want to animate the hand or use other IK Drivers")]
        public bool disableIK = false;

        [Tooltip("How much the fingers sway from the velocity")]
        public float swayStrength = 0.7f;

        [Tooltip("This will offset each fingers bend (0 is no bend, 1 is full bend)")]
        public float gripOffset = 0.1f;

        
        [Tooltip("Increase for closer finger results / Decrease for less physics checks - The number of steps the fingers take when bending to grab something")]
        public int fingerBendSteps = 50;
        
        [Tooltip("Makes grab smoother also based on range and reach distance - a very near grab is instant and a max distance grab is [X] frames - RECOMMEND VALUES ~0.1"), Min(0)]
        public float grabTime = 0f;

        [Tooltip("The animation curve based on the grab time 0-1"), Min(0)]
        public AnimationCurve grabCurve;

        [Tooltip("Makes grab smoother also based on range and reach distance - a very near grab is instant and a max distance grab is [X] frames - RECOMMEND VALUES ~0.1"), Min(0)]
        public float grabReturnTime = 0f;




        [Header("Advanced"), Space]
        [Tooltip("The layers to grab from")]
        public LayerMask grabLayers;

        [Tooltip("The layers to highlight and use look assist on")]
        public LayerMask highlightLayers;

        public float lookAssistSpeed = 1;
        
        [Tooltip("Offsets the width of the grab area -- Turn this down or negative for less weird grabs but requires better aim to grab")]
        public float grabSpreadOffset = 0;
        

#if UNITY_EDITOR
        [Header("Editor"), Space, Space]
        public bool showGizmos = false;
        [Tooltip("Turn this on to enable autograbbing for editor rigging")]
        public bool editorAutoGrab = false;
        [Tooltip("By default procedural grab uses the collider instead of the layer to grab - this will use layer instead")]
        public bool useLayerBasedAutoGrab = false;
#endif
        

        bool freezePos = false;
        bool freezeRot = false;

        internal GameObject lookingAtObj = null;
        internal Grabbable holdingObj = null;

        float idealGrip = 1f;
        float currGrip = 1f;

        internal Transform moveTo;
        protected Rigidbody body;
        protected FixedJoint joint1;
        protected FixedJoint joint2;
        protected float triggerPoint;
        protected Transform grabPoint;
        protected Vector3 startGrabPos;
        protected Quaternion startGrabRot;
        protected Vector3 palmOffset;

        protected Vector3[] handRays;
        protected Quaternion rotationOffset;
        protected internal Vector3 grabPositionOffset;
        protected Vector3 lastMoveToPos; 
        protected Vector3 preRenderPos;
        protected Quaternion preRenderRot;
        protected HandPoseData preRenderHandPose;

        protected int handLayers;
        protected int triggerCount = 0;

        protected bool grabbing = false;
        protected bool squeezing = false;
        protected bool grabLocked;
        protected bool grabbingFrame = false;

        protected GrabbablePose grabPose;
        protected HandPoseData pose;

        protected Coroutine handAnimateRoutine;

        protected bool usingPoseAreas;
        protected HandPoseArea handPoseArea;
        protected HandPoseData preHandPoseAreaPose;

        protected Vector3[] velocityOverTime = new Vector3[5];
        
        protected List<GameObject> collisions;

        protected HandPoseData preGrabPose;

        protected float startDrag;
        




        //Adjust grabbable layers for custom setup.

        //The layer is used and applied to all grabbables in if the hands layermask is not set
        public const string grabbableLayerNameDefault = "Grabbable";

        //This helps the auto grab distinguish between what item is being grabbaed and the items around it
        public const string grabbingLayerName = "Grabbing";

        //This helps prevent conflict when releasing
        public const string releasingLayerName = "Releasing";

        //This was added by request just in case you want to add different layers for left/right hand
        public const string rightHandLayerName = "Hand";
        public const string leftHandLayerName = "Hand";
        public const string rightHandHoldingLayerName = "HandHolding";
        public const string leftHandHoldingLayerName = "HandHolding";




        ///Events for all my programmers out there :)
        public event HandGrabEvent OnBeforeGrabbed;
	    public event HandGrabEvent OnGrabbed;
        
	    public event HandGrabEvent OnBeforeReleased;
        public event HandGrabEvent OnReleased;
        
        public event HandGrabEvent OnForcedRelease;
        
        public event HandGrabEvent OnSqueezed;
        public event HandGrabEvent OnUnsqueezed;

        public event HandGrabEvent OnHighlight;
        public event HandGrabEvent OnStopHighlight;

        /// <summary>This is always called after something is done being held, released or forced release</summary>
        public event HandGrabEvent OnHeldConnectionBreak;
        
        public bool editorSelected = false;

        public void Start() {
            bool setLayers = LayerMask.NameToLayer("HandPlayer") != -1 && grabLayers == (grabLayers | (1 << LayerMask.NameToLayer("HandPlayer")));
            if(grabLayers == 0 || setLayers)
                grabLayers = LayerMask.GetMask(grabbableLayerNameDefault, grabbingLayerName, releasingLayerName);

            if(highlightLayers == 0 || setLayers)
                highlightLayers = LayerMask.GetMask(grabbableLayerNameDefault);

            if(left && Time.fixedDeltaTime > 1/60f)
                Debug.LogError("Auto Hand: Strongly Recommended that Fixed Timestep is reduced to AT LEAST 1/60 for smoothness, (1/90 is best)  --- See [Project Settings/Time]");

            body = GetComponent<Rigidbody>();
            startDrag = body.drag;
            body.useGravity = false;

            moveTo = new GameObject().transform;
            moveTo.transform.parent = transform.parent;
            moveTo.name = "HAND FOLLOW POINT";

            usingPoseAreas = FindObjectOfType<HandPoseArea>() != null;
            collisions = new List<GameObject>();

            foreach(var cam in FindObjectsOfType<Camera>()) {
                if(!cam.GetComponent<HandStabilizer>())
                    cam.gameObject.AddComponent<HandStabilizer>();
            }

#if UNITY_EDITOR
            if(Selection.activeGameObject == gameObject){
                Selection.activeGameObject = null;
                Debug.Log("Auto Hand: Selecting the hand can cause positional lag and quality reduction at runtime. Remove this code at any time.");
                editorSelected = true;
            }

            Application.quitting += () => { if(editorSelected) Selection.activeGameObject = gameObject; };
#endif
            Initialize();
        }

        public virtual void Initialize() {
            //This precalculates the rays so it has to do less math in realtime
            List<Vector3> rays = new List<Vector3>();
            for(int i = 0; i < 100; i++) {
                float ampI = Mathf.Pow(i, 1.05f + grabSpreadOffset/10f) / (Mathf.PI * 0.8f);
                rays.Add(Quaternion.Euler(0, Mathf.Cos(i) * ampI + 90, Mathf.Sin(i) * ampI) * -Vector3.right);
            }
            handRays = rays.ToArray();

            //Sets hand to layer "Hand"
            SetLayerRecursive(transform, LayerMask.NameToLayer(left ? leftHandLayerName : rightHandLayerName));
            
            //preretrieve layermask
            handLayers = LayerMask.GetMask(leftHandHoldingLayerName, rightHandHoldingLayerName, rightHandLayerName, leftHandLayerName);
        }



        public void Update() {
            if(grabbing || body.isKinematic)
                return;
            
            UpdateMoveTo();
            
            //Does Look Assist + Highlight
            LookAssist();
            CheckHighlight();
            
            //Does Finger Sway
            DeterminPose();
        }
        
        public void FixedUpdate(){
            if(grabbing || body.isKinematic)
                return;

            UpdateMoveTo();

            //Calls physics movements
            if(!freezePos) MoveTo();
            if(!freezeRot) TorqueTo();
            
            //Strongly stabilizes one handed holding
            if(holdingObj != null && grabLocked && holdingObj.HeldCount() == 1) {
                if(!freezePos) body.position = grabPoint.transform.position;
                if(!freezeRot) body.rotation = grabPoint.transform.rotation;
            }

            for(int i = 1; i < velocityOverTime.Length; i++) {
                velocityOverTime[i] = velocityOverTime[i-1];
            }
            velocityOverTime[0] = body.velocity;
        }
        
        /// <summary>Manages where the hands ideal position should be -> where it will TRY to physics move/torque to</summary>
        protected virtual void UpdateMoveTo() {
            //Sets [Move To] Object
            moveTo.position = follow.position + transform.rotation*followPositionOffset + grabPositionOffset;
            moveTo.rotation = follow.rotation;
            
            //Adjust the [Move To] based on offsets 
            if(holdingObj != null) {
                if(left){
                    var leftRot = -holdingObj.heldRotationOffset;
                    leftRot.x *= -1;
                    moveTo.localRotation *= Quaternion.Euler(leftRot);
                    var moveLeft = holdingObj.heldPositionOffset;
                    moveLeft.x *= -1;
                    moveTo.position += transform.rotation*moveLeft;
                }
                else{
                    moveTo.position += transform.rotation*holdingObj.heldPositionOffset;
                    moveTo.localRotation *= Quaternion.Euler(holdingObj.heldRotationOffset);
                }

                moveTo.Rotate(moveTo.right, holdingObj.heldPositionOffset.x);
                moveTo.Rotate(moveTo.up, holdingObj.heldPositionOffset.y);
                moveTo.Rotate(moveTo.forward, holdingObj.heldPositionOffset.z);
                
            }
        } 
        


        //This is used to force the hand to always look like its where it should be even when physics is being weird
        public void OnPreRender() {
            //Hides weird grab flicker
            if(grabbingFrame) {
                preRenderHandPose = GetHandPose();
                SetHandPose(preGrabPose);
            }
            //Hides fixed joint jitterings
            if(holdingObj != null && !grabbing) {
                preRenderPos = transform.position;
                preRenderRot = transform.rotation;
                transform.position = grabPoint.transform.position;
                transform.rotation = grabPoint.transform.rotation;
            }
        }
        
        //This puts everything where it should be for the physics update
        public void OnPostRender() {
            if(grabbingFrame) {
                SetHandPose(preRenderHandPose);
            }
            //Returns position after hiding for camera
            if(holdingObj != null && !grabbing) {
                transform.position = preRenderPos;
                transform.rotation = preRenderRot;
            }
        }



        /// <summary>Helps keep track of hand collisions, used to help create extra stability</summary>
        protected void OnCollisionEnter(Collision collision) {
            if(!collisions.Contains(collision.gameObject))
                collisions.Add(collision.gameObject);
        }

        /// <summary>Helps keep track of hand collisions, used to help create extra stability</summary>
        protected void OnCollisionExit(Collision collision) {
            if(collisions.Contains(collision.gameObject))
                collisions.Remove(collision.gameObject);
        }
        
        protected void OnTriggerEnter(Collider other){
            CheckEnterPoseArea(other);
        }
        
        protected void OnTriggerExit(Collider other){
            CheckExitPoseArea(other);
        }
        


        //========================================================
        //================== PHYSICS MOVEMENT  ===================
        

        /// <summary>Moves the hand to the controller position using physics movement</summary>
        internal virtual void MoveTo() {
            var movePos = moveTo.position;
            var distance = Vector3.Distance(movePos, transform.position);

            var minDist = Time.fixedDeltaTime;
            if(distance <= minDist)
                body.drag = Mathf.Lerp(startDrag, startDrag*2f, 1-(distance/minDist));
            else
                body.drag = startDrag;

            //Returns if out of distance -> if you aren't holding anything
            if(distance > maxFollowDistance) {
                if(holdingObj != null){
                    SetHandLocation(movePos, transform.rotation);
                }
                else{
                    transform.position = movePos;
                }
            }
            
            var velocityClamp = maxVelocity;

            //This helps prevent that hand from forcing its way through walls
            if(distance > 0.1f && collisions.Count > 0)
                velocityClamp = 2.5f;
            
            //Sets velocity linearly based on distance from hand
            var vel = (movePos - transform.position).normalized * followPositionStrength * distance;
            vel.x = Mathf.Clamp(vel.x, -velocityClamp, velocityClamp);
            vel.y = Mathf.Clamp(vel.y, -velocityClamp, velocityClamp);
            vel.z = Mathf.Clamp(vel.z, -velocityClamp, velocityClamp);
            body.velocity = vel;
        }


        /// <summary>Rotates the hand to the controller rotation using physics movement</summary>
        internal virtual void TorqueTo() {
            var toRot = rotationOffset * moveTo.rotation;
            float angleDist = Quaternion.Angle(body.rotation, toRot);
            Quaternion desiredRotation = Quaternion.Lerp(body.rotation, toRot, Mathf.Clamp(angleDist, 0, 2) / 4f);

            var kp = 90f * followRotationStrength;
            var kd = 60f;
            Vector3 x;
            float xMag;
            Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
            q.ToAngleAxis(out xMag, out x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 pidv = kp * x * xMag - kd * body.angularVelocity;
            Quaternion rotInertia2World = body.inertiaTensorRotation * transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(body.inertiaTensor);
            pidv = rotInertia2World * pidv;
            body.AddTorque(pidv);
        }
        





        //================================================================

        //================== CORE INTERACTION FUNCTIONS ===================
        
            
        /// <summary>Function for controller trigger fully pressed</summary>
        public virtual void Grab() {
            if(!grabbing && holdingObj == null) {
                RaycastHit closestHit;
                if(HandClosestHit(out closestHit, reachDistance, grabLayers) != Vector3.zero)
                    StartCoroutine(GrabObject(closestHit));
            }
            else if(holdingObj != null) {
                if(holdingObj.GetComponent<GrabLock>()){
                    holdingObj.GetComponent<GrabLock>().OnGrabPressed?.Invoke();
                }
            }
        }


        /// <summary>Function for controller trigger unpressed</summary>
        public virtual void Release() {
            //Do the holding object calls and sets
            if(holdingObj != null) {
                if(holdingObj.GetComponent<GrabLock>())
                    return;

                lookingAtObj = null;

                OnBeforeReleased?.Invoke(this, holdingObj);

                if(squeezing)
                    holdingObj?.OnUnsqueeze(this);

                holdingObj?.OnRelease(this, holdingObj.gameObject.layer != LayerMask.NameToLayer(grabbingLayerName));


                OnReleased?.Invoke(this, holdingObj);
                BreakGrabConnection();
            }
            else if(grabLocked || holdingObj == null) {
                BreakGrabConnection();
            }
        }
        

        /// <summary>Function for controller trigger unpressed</summary>
        public virtual void ReleaseGrabLock() {
            //Do the holding object calls and sets
            if(holdingObj != null) {
                OnBeforeReleased?.Invoke(this, holdingObj);

                if(squeezing)
                    holdingObj?.OnUnsqueeze(this);

                holdingObj?.OnRelease(this, true);

                OnReleased?.Invoke(this, holdingObj);
                BreakGrabConnection();
            }
            else if(grabLocked || holdingObj == null) {
                BreakGrabConnection();
            }
        }


        /// <summary>This will force release the hand without throwing or calling OnRelease\n like losing grip on something instead of throwing</summary>
        public virtual void ForceReleaseGrab() {
            //Do the holding object calls and sets
            if(holdingObj != null) {
                if(squeezing)
                    holdingObj.OnUnsqueeze(this);
                OnForcedRelease?.Invoke(this, holdingObj);
                holdingObj.body.WakeUp();
                holdingObj.OnForceReleaseEvent?.Invoke(this, holdingObj);
            }
            BreakGrabConnection();
        }

        
        /// <summary>Event for controller grip</summary>
        public virtual void Squeeze() {
            squeezing = true;
            OnSqueezed?.Invoke(this, holdingObj);
            holdingObj?.OnSqueeze(this);
        }

        
        /// <summary>Event for controller ungrip</summary>
        public virtual void Unsqueeze() {
            squeezing = false;
            OnUnsqueezed?.Invoke(this, holdingObj);
            holdingObj?.OnUnsqueeze(this);
        }

        
        /// <summary>This is used to simulate and trigger pull-apart effect</summary>
        protected virtual void OnJointBreak(float breakForce) {
            if(joint1 != null)
                Destroy(joint1);
            if(joint2 != null)
                Destroy(joint2);
            holdingObj?.OnHandJointBreak(this);
            ForceReleaseGrab();
        }
        

        /// <summary>Sets the hands grip 0 is open 1 is closed</summary>
        public void SetGrip(float grip) {
            triggerPoint = grip;
        }


        /// <summary>Breaks the grab event</summary>
        protected virtual void BreakGrabConnection(){
            SetLayerRecursive(transform, LayerMask.NameToLayer(left ? leftHandLayerName : rightHandLayerName));

            if(grabbing && holdingObj != null){
                holdingObj.body.velocity = Vector3.zero;
                holdingObj.body.angularVelocity = Vector3.zero;
            }
            grabLocked = false;
            grabPose = null;
            grabPositionOffset = Vector3.zero;

            //Destroy Junk
            if(grabPoint != null)
                Destroy(grabPoint.gameObject);
            if(joint1 != null)
                Destroy(joint1);
            if(joint2 != null)
                Destroy(joint2);
            
            OnHeldConnectionBreak?.Invoke(this, holdingObj);
            holdingObj = null;
        }
        
        
        /// <summary>Takes a hit from a grabbable object and moves the hand towards that point, then calculates ideal hand shape</summary>
        protected virtual IEnumerator GrabObject(RaycastHit hit) {

            Grabbable lookingAtGrab;
            if(HasGrabbable(lookingAtObj, out lookingAtGrab))
                lookingAtGrab.Unhighlight(this);
            
            Grabbable tempHoldingObj;
            if(!HasGrabbable(hit.collider.gameObject, out tempHoldingObj))
                 yield break;
            
            //Checks if the grabbable script is enabled
            if(!tempHoldingObj.enabled)
                yield break;

            //If the hand doesn't match the settings
            if(tempHoldingObj.handType == HandType.none || (tempHoldingObj.handType == HandType.left && !left) || (tempHoldingObj.handType == HandType.right && left))
                yield break;
            
            //Hand Swap - One Handed Items
            if(tempHoldingObj.singleHandOnly && tempHoldingObj.HeldCount() > 0){
                tempHoldingObj.ForceHandsRelease();
                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
            }

            //GrabbableBase check - cancels grab if failed
            var grabBase = tempHoldingObj.GetComponent<GrabbablePointBase>();
            if(grabBase != null && !grabBase.Align(this))
                yield break;
            
            CancelPose();
            ClearPoseArea();

            holdingObj = tempHoldingObj;
            startGrabPos = transform.position;
            startGrabRot = transform.rotation;
            
            holdingObj.OnBeforeGrab(this);
            OnBeforeGrabbed?.Invoke(this, holdingObj);



            //Set layers for grabbing
            var originalLayer = holdingObj.gameObject.layer;
            holdingObj.SetLayerRecursive(holdingObj.transform, originalLayer, LayerMask.NameToLayer(grabbingLayerName));
            SetLayerRecursive(transform, LayerMask.NameToLayer(left ? leftHandLayerName : rightHandLayerName));
            
            //SETS GRAB POINT
            grabPoint = new GameObject().transform;
            grabPoint.position = hit.point;
            grabPoint.parent = hit.transform;

            grabbing = true;
            palmOffset = transform.position - palmTransform.position;
            moveTo.position = grabPoint.transform.position + palmOffset + transform.rotation*followPositionOffset;
            moveTo.rotation = grabPoint.transform.rotation;
            freezeRot = true;


            if(holdingObj.maintainGrabOffset)
                grabPositionOffset = grabPoint.transform.position - palmTransform.position;
            
            preGrabPose = new HandPoseData(this, grabPoint);

            float startGrabDist = 0;
            if(grabTime > 0){
                startGrabDist = Vector3.Distance(palmTransform.position, grabPoint.position)/reachDistance;
            }
            

            //Aligns the hand using the closest hit point
            IEnumerator AutoAlign() {
                grabbingFrame = true;
                
                yield return new WaitForEndOfFrame();
                if(holdingObj == null)
                    yield break;

                foreach (var finger in fingers)
                    finger.ResetBend();

                
                var mass = body.mass;
                var dir = HandClosestHit(out _, reachDistance, grabLayers);

                var palmOriginParent = palmTransform.parent;
                var originParent = transform.parent;

                var preGrabbablePos = holdingObj.body.position;
                var preGrabbableRot = holdingObj.body.rotation;

                palmTransform.parent = null;
                transform.parent = palmTransform;
                palmTransform.LookAt(grabPoint.position, transform.forward);

                transform.parent = originParent;
                palmTransform.parent = palmOriginParent;
                Grabbable wasHolding = holdingObj;
                
                if(holdingObj.instantGrab && holdingObj.HeldCount() == 0){
                    holdingObj.transform.position += (transform.position - palmOffset) - grabPoint.position;
                    holdingObj.body.position += (transform.position - palmOffset) - grabPoint.position;
                }
                else{
                    body.transform.position = grabPoint.position + palmOffset;
                    body.position = grabPoint.position + palmOffset;
                }
                
                var grabbingObjBod = holdingObj.body;

                bool shouldFreeze = originalLayer != LayerMask.NameToLayer(grabbingLayerName);
                CollisionDetectionMode holdingObjDetectionMode = holdingObj.body.collisionDetectionMode;
                
                var objectConstraints = holdingObj.body.constraints;

                if(shouldFreeze)
                    holdingObj.body.constraints = RigidbodyConstraints.FreezeAll;
                
                body.WakeUp();
                holdingObj.body.WakeUp();

                body.velocity = Vector3.zero;


                yield return new WaitForFixedUpdate();

                if(holdingObj == null){
                    if(shouldFreeze)
                        holdingObj.body.constraints = objectConstraints;
                }
                else{
                    if(!holdingObj.instantGrab){
                        holdingObj.body.position = preGrabbablePos;
                        holdingObj.body.rotation = preGrabbableRot;
                    }
                
                    if(shouldFreeze)
                        holdingObj.body.constraints = objectConstraints;

                    holdingObj.body.WakeUp();
                }
                body.mass = mass;
                body.WakeUp();
                
                grabbingFrame = false;
                
            }

            //If it's a predetermined Pose
            if (holdingObj.GetComponent<GrabbablePose>() && holdingObj.GetComponent<GrabbablePose>().HasPose(left)){
                grabPose = holdingObj.GetComponent<GrabbablePose>();
            }

            //Allign Position
            else if(grabBase == null){
                yield return StartCoroutine(AutoAlign());
                if(holdingObj == null){ 
                    BreakGrabConnection();
                    freezePos = false;
                    freezeRot = false;
                    grabbing = false;
                    yield break;
                }
            }
            

            //Finger Bend
            if(grabPose == null){
                foreach(var finger in fingers)
                    finger.BendFingerUntilHit(fingerBendSteps, LayerMask.GetMask(grabbingLayerName));
            }

            //Smooth Grabbing
            if(grabTime > 0 && !holdingObj.instantGrab){
                HandPoseData postGrabPose;
                Transform grabTarget;

                if(grabPose == null){
                    grabTarget = grabPoint;
                    postGrabPose = new HandPoseData(this, grabPoint);
                }
                else {
                    grabTarget = holdingObj.transform;
                    postGrabPose = grabPose.GetHandPoseData(left);
                }

                //Lerp between start and end pose over time related to distance
                for(float i = 0; i < grabTime*startGrabDist; i+=Time.deltaTime){
                    if(holdingObj != null){
                        var point = i/(grabTime*startGrabDist);
                        HandPoseData.LerpPose(preGrabPose, postGrabPose, grabCurve.Evaluate(point)).SetPose(this, grabTarget);
                        yield return new WaitForFixedUpdate();
                    }
                }
                postGrabPose.SetPose(this, grabTarget);
            }
            else if(grabPose != null)
                grabPose.GetHandPoseData(left).SetPose(this, holdingObj.transform);
                

            //Create Connection
            if(holdingObj == null){ 
                BreakGrabConnection();
                freezePos = false;
                freezeRot = false;
                grabbing = false;
                yield break;
            }

            if(holdingObj.maintainGrabOffset)
                holdingObj.heldRotationOffset = (transform.rotation * Quaternion.Inverse(startGrabRot)).eulerAngles;

            //Connect Joints
            joint1 = gameObject.AddComponent<FixedJoint>();
            joint1.connectedBody = holdingObj.body;
            joint1.breakForce = float.PositiveInfinity;
            joint1.breakTorque = float.PositiveInfinity;
                
            joint1.connectedMassScale = 1;
            joint1.massScale = 1;
            joint1.enableCollision = false;
            joint1.enablePreprocessing = false;
                
            joint2 = holdingObj.body.gameObject.AddComponent<FixedJoint>();
            joint2.connectedBody = body;
            joint2.breakForce = float.PositiveInfinity;
            joint2.breakTorque = float.PositiveInfinity;
                
            joint2.connectedMassScale = 1;
            joint2.massScale = 1;
            joint2.enableCollision = false;
            joint2.enablePreprocessing = false;
                
            grabPoint.transform.position = transform.position;
            grabPoint.transform.rotation = transform.rotation;
            OnGrabbed?.Invoke(this, holdingObj);
            holdingObj.OnGrab(this);


            yield return new WaitForFixedUpdate();
                
            if(holdingObj == null){ 
                BreakGrabConnection();
                freezePos = false;
                freezeRot = false;
                grabbing = false;
                yield break;
            }


            joint1.breakForce = holdingObj.jointBreakForce;
            joint1.breakTorque = holdingObj.jointBreakForce;

            joint2.breakForce = holdingObj.jointBreakForce;
            joint2.breakTorque = holdingObj.jointBreakForce;
            moveTo.position = follow.position + transform.rotation*followPositionOffset + grabPositionOffset;
            moveTo.rotation = follow.rotation;
            
            grabLocked = true;
            bool objectFree = holdingObj.body.isKinematic != true && holdingObj.body.constraints == RigidbodyConstraints.None;

            if(holdingObj.instantGrab)
                SetHandLocation(moveTo.position, moveTo.rotation);

            else if(objectFree && grabReturnTime > 0 && holdingObj.HeldCount() == 1) {
                for(float i = 0; i < grabReturnTime*startGrabDist; i += Time.deltaTime){
                    if(holdingObj != null){
                        var bodyPos = body.position;
                        body.position = Vector3.MoveTowards(body.position, moveTo.transform.position, (startGrabDist*Time.deltaTime)/grabReturnTime);
                        var deltaBodyPos = body.position-bodyPos;
                        yield return new WaitForFixedUpdate();
                    }
                }
            }

            

            if(holdingObj == null){ 
                BreakGrabConnection();
                freezePos = false;
                freezeRot = false;
                grabbing = false;
                yield break;
            }
            else 
                SetLayerRecursive(transform, LayerMask.NameToLayer(left ? leftHandHoldingLayerName : rightHandHoldingLayerName));
            
            //Reset Values
            freezePos = false;
            freezeRot = false;
            grabbing = false;

            yield return new WaitForEndOfFrame();
            if(grabPose != null || grabBase != null)
                transform.rotation = follow.rotation;
            
        }
        



        


        //=============================================================
        //=============== HIGHLIGHT AND LOOK ASSIST ===================

        
        
        /// <summary>Manages the highlighting for grabbables</summary>
        protected virtual void CheckHighlight() {
            if(holdingObj == null){
                Grabbable lookingAtGrab;
                RaycastHit hit;
                var dir = HandClosestHit(out hit, reachDistance, highlightLayers);
                //Zero means it didn't hit
                if(dir != Vector3.zero){
                    //Changes look target
                    if(hit.collider.transform.gameObject != lookingAtObj){
                        //Unhighlights current target if found
                        if(lookingAtObj != null && HasGrabbable(lookingAtObj, out lookingAtGrab)){
                            OnStopHighlight?.Invoke(this, lookingAtGrab);
                            lookingAtGrab.Unhighlight(this);
                        }

                        //Highlights new target if found
                        lookingAtObj = hit.collider.transform.gameObject;
                        if(HasGrabbable(lookingAtObj, out lookingAtGrab)){
                            OnHighlight?.Invoke(this, lookingAtGrab);
                            lookingAtGrab.Highlight(this);
                        }
                    }

                    rotationOffset = Quaternion.RotateTowards(rotationOffset, Quaternion.FromToRotation(palmTransform.forward, hit.point - transform.position), 50f * Time.deltaTime * lookAssistSpeed);
                }
                //If it was looking at something but now it's not there anymore
                else if(lookingAtObj != null){
                    //Just in case the object your hand is looking at is destroyed
                    if(HasGrabbable(lookingAtObj, out lookingAtGrab)){
                        OnStopHighlight?.Invoke(this, lookingAtGrab);
                        lookingAtGrab.Unhighlight(this);
                    }

                    lookingAtObj = null;
                    rotationOffset = Quaternion.identity;
                }
                //If you're seeing nothing reset offset
                else{
                    rotationOffset = Quaternion.identity;
                }
            }
            //If you're holding something reset offset
            else{
                rotationOffset = Quaternion.identity;
            }
        }


        /// <summary>Rotates the hand towards the object it's aiming to pick up</summary>
        protected virtual void LookAssist(){
            if(holdingObj == null){
                RaycastHit hit;
                var controllerRot = left ? follow.transform.localRotation*Quaternion.Euler(0, 90, 0) : follow.transform.localRotation*Quaternion.Euler(0, -90, 0);
                var dir = HandClosestHit(follow, controllerRot, out hit, reachDistance, highlightLayers);

                //Zero means it didn't hit
                if(dir != Vector3.zero){
                    //Hiding look assist, will probably be returned in a different form with less jitters
                    rotationOffset = Quaternion.RotateTowards(rotationOffset, Quaternion.FromToRotation(palmTransform.forward, hit.point - transform.position), 50f * Time.deltaTime * lookAssistSpeed);
                }
                //If you're seeing nothing reset offset
                else{
                    rotationOffset = Quaternion.identity;
                }
            }
            //If you're holding something reset offset
            else{
                rotationOffset = Quaternion.identity;
            }
        }

        
        /// <summary>Returns the current held object - null if empty</summary>
        public Grabbable GetHeldGrabbable() {
            return holdingObj;
        }
        

        ///<summary>Moves the hand and whatever it might be holding (if teleport allowed) to given pos/rot</summary>
        internal void SetHandLocation(Vector3 pos, Quaternion rot) {
            if(holdingObj != null && holdingObj.releaseOnTeleport)
                ForceReleaseGrab();

            Vector3 fromPos = transform.position;

            if(holdingObj != null){
                var lastParent = holdingObj.body.transform.parent;
                holdingObj.body.transform.parent = transform;
                transform.position = pos;
                body.position = pos;
                transform.rotation = rot;
                body.rotation = rot;
                holdingObj.transform.parent = lastParent;
            }
            else{
                transform.position = pos;
                transform.rotation = rot;
            }
        }

        

        

        //=================================================================

        //========================= GET AND LOAD POSES ======================
        // How to save and load a pose -> GetHeldPose() -> Save the HandPoseData -> SetHandPose(poseData)


        /// <summary>Sets the hand pose and connects the grabbable</summary>
        public virtual void SetHeldPose(HandPoseData pose, Grabbable grabbable) {
            holdingObj = grabbable;
            OnBeforeGrabbed?.Invoke(this, holdingObj);

            holdingObj.transform.position = transform.position;

            //Set Pose
            pose.SetPose(this, grabbable.transform);

            //Connect Joints
            joint1 = gameObject.AddComponent<FixedJoint>();
            joint1.connectedBody = holdingObj.body;
            joint1.breakForce = holdingObj.jointBreakForce;
            joint1.breakTorque = holdingObj.jointBreakForce;
                
            joint1.connectedMassScale = 1;
            joint1.massScale = 1;
            joint1.enableCollision = false;
            joint1.enablePreprocessing = false;
                
            joint2 = holdingObj.body.gameObject.AddComponent<FixedJoint>();
            joint2.connectedBody = body;
            joint2.breakForce = holdingObj.jointBreakForce;
            joint2.breakTorque = holdingObj.jointBreakForce;
                
            joint2.connectedMassScale = 1;
            joint2.massScale = 1;
            joint2.enableCollision = false;
            joint2.enablePreprocessing = false;
                
            grabPoint = new GameObject().transform;
            grabPoint.parent = holdingObj.transform;
            grabPoint.transform.position = transform.position;
            grabPoint.transform.rotation = transform.rotation;
                
            OnGrabbed?.Invoke(this, holdingObj);
            holdingObj.OnGrab(this);

            SetHandLocation(moveTo.position, moveTo.rotation);

            grabLocked = true;
            
            //Reset Values
            grabbing = false;
            
        }
        
        /// <summary>Sets the hand pose</summary>
        public void SetHandPose(HandPoseData pose) {
            pose.SetPose(this, null);
        }

        /// <summary>(IF SAVING A HELD POSE USE GetHeldPose()) Returns the current hand pose, ignoring what is being held</summary>
        public HandPoseData GetHandPose(){
            return new HandPoseData(this);
        }
        
        /// <summary>Returns the hand pose relative to what it's holding</summary>
        public HandPoseData GetHeldPose() {
            if(holdingObj)
                return new HandPoseData(this, holdingObj);
            return new HandPoseData(this);
        }
        
        /// <summary>Takes a new pose and an amount of time and poses the hand</summary>
        public virtual void UpdatePose(HandPoseData pose, float time){
            if (handAnimateRoutine != null)
                StopCoroutine(handAnimateRoutine);
            handAnimateRoutine = StartCoroutine(LerpHandPose(GetHandPose(), pose, time));
        }

        /// <summary>Ensures any pose being made is canceled</summary>
        protected void CancelPose() {
            if (handAnimateRoutine != null)
                StopCoroutine(handAnimateRoutine);
            handAnimateRoutine = null;
        }

        /// <summary>Not exactly lerped, uses non-linear sqrt function because it looked better -- planning animation curves options soon</summary>
        protected virtual IEnumerator LerpHandPose(HandPoseData fromPose, HandPoseData toPose, float totalTime){
            float timePassed = 0;
            while(timePassed < totalTime){
                SetHandPose(HandPoseData.LerpPose(fromPose, toPose, Mathf.Pow(timePassed/totalTime, 0.5f)));
                yield return new WaitForEndOfFrame();
                timePassed += Time.deltaTime;
            }
            SetHandPose(HandPoseData.LerpPose(fromPose, toPose, 1));
            handAnimateRoutine = null;
        }
       
        
        

        //==========================================================

        //=============== FINGER SWAY AND BENDING ==================


        /// <summary>Determines how the hand should look/move based on its flags</summary>
        protected virtual void DeterminPose(){
            if(!grabbing && !squeezing && holdingObj == null){
                idealGrip = triggerPoint;
            }

            //Responsable for movement finger sway
            if(!holdingObj && !disableIK && !grabPose && handPoseArea == null && handAnimateRoutine == null) {
                float vel = -palmTransform.InverseTransformDirection(body.velocity).z;
                float grip = idealGrip + gripOffset + swayStrength * (vel / 8f);

                if(currGrip != grip) {
                    bool less = (currGrip < grip) ? true : false;
                    currGrip += ((currGrip < grip) ? Time.deltaTime : -Time.deltaTime) * (Mathf.Abs(currGrip - grip) * 25);
                    if(less && currGrip > grip)
                        currGrip = grip;

                    else if(!less && currGrip < grip)
                        currGrip = grip;

                    foreach(var finger in fingers){
                        finger.UpdateFinger(currGrip);
                    }
                }
            }
        }

        



        //===============================================================
        //======================== POSE AREAS ===========================
        
        
        /// <summary>Checks and manages if any of the hands colliders enter a pose area</summary>
        protected virtual void CheckEnterPoseArea(Collider other) {
            if(!usingPoseAreas || !other.gameObject.activeInHierarchy)
                return;

            HandPoseArea tempPose;
            if(tempPose = other.GetComponent<HandPoseArea>()){
                if(handPoseArea == null)
                    preHandPoseAreaPose = GetHandPose();

                if (tempPose.HasPose(left) && (handPoseArea == null || handPoseArea != tempPose)){
                    preHandPoseAreaPose = GetHandPose();
                    triggerCount = 0;
                    handPoseArea = tempPose;
                    if(holdingObj == null)
                        UpdatePose(handPoseArea.GetHandPoseData(left), handPoseArea.transitionTime);
                }

                if (tempPose.Equals(handPoseArea))
                    triggerCount++;
            }
        }
        
        /// <summary>Checks if manages any of the hands colliders exit a pose area</summary>
        protected virtual void CheckExitPoseArea(Collider other) {
            if(!usingPoseAreas || !other.gameObject.activeInHierarchy)
                return;

            if(handPoseArea != null && handPoseArea.gameObject.Equals(other.gameObject)){
                triggerCount--;
                if (triggerCount == 0 && holdingObj == null){
                    UpdatePose(preHandPoseAreaPose, handPoseArea.transitionTime);
                    handPoseArea = null;
                }
                else if(triggerCount == 0 && holdingObj != null)
                    handPoseArea = null;
            }
        }

        protected void ClearPoseArea() {
            triggerCount = 0;
            handPoseArea = null;
        }

        
        

        //=================================================================

        //========================= HELPER FUNCTIONS ======================

            
        /// <summary>Default, uses palm forward, finds the closest raycast from a cone of rays -> Returns average direction of all hits</summary>
        public virtual Vector3 HandClosestHit(out RaycastHit closestHit, float dist, int layerMask) {
            return HandClosestHit(palmTransform, Quaternion.identity, out closestHit, dist, layerMask);
        }
        
        /// <summary>Finds the closest raycast from a cone of rays -> Returns average direction of all hits</summary>
        internal virtual Vector3 HandClosestHit(Transform hand, Quaternion offset, out RaycastHit closestHit, float dist, int layerMask) {
            List<RaycastHit> hits = new List<RaycastHit>();
            foreach(var ray in handRays) {
                RaycastHit hit;
                if(Physics.Raycast(hand.position, offset * hand.rotation * ray, out hit, dist, layerMask))
                    hits.Add(hit);
            }
            if(hits.Count > 0) {
                closestHit = hits[0];
                var closestHitIndex = 0;
                //The minmax stuff helps the hand priorites the middle points over the outer points
                var minMax = new Vector2(1f, 1.05f);
                Vector3 dir = Vector3.zero;
                for(int i = 0; i < hits.Count; i++) {
                    var closestMulti = Mathf.Lerp(minMax.x, minMax.y, ((float)closestHitIndex)/hits.Count);
                    var multi = Mathf.Lerp(minMax.x, minMax.y, ((float)i)/hits.Count);
                    if(hits[i].distance*multi < closestHit.distance*closestMulti){
                        closestHit = hits[i];
                        closestHitIndex = i;
                    }
                    dir += hits[i].point - hand.position;
                }

                return dir/hits.Count;
            }

            closestHit = new RaycastHit();
            return Vector3.zero;
        }
        
        /// <summary>Returns the hands velocity times its strength</summary>
        internal virtual Vector3 ThrowVelocity(){
            if(grabbing)
                return Vector3.zero;

            var averageVel = Vector3.zero;
            for(int i = 0; i < velocityOverTime.Length; i++)
                averageVel += velocityOverTime[i];
            averageVel /= velocityOverTime.Length;
            return averageVel * throwPower;
        }
        
        /// <summary>Returns true if there is a grabbable or link, out null if there is none</summary>
        protected bool HasGrabbable(GameObject obj, out Grabbable grabbable) {
            if(obj == null){
                grabbable = null;
                return false;
            }

            if(obj.GetComponent<Grabbable>()){
                grabbable = obj.GetComponent<Grabbable>();
                return true;
            }

            if(obj.GetComponent<GrabbableChild>()){
                grabbable = obj.GetComponent<GrabbableChild>().grabParent;
                return true;
            }

            grabbable = null;
            return false;
        }
        
        /// <summary>Returns true during the grabbing frames</summary>
        public bool IsGrabbing() {
            return grabbing;
        }
        
        
        /// <summary>Breifly disabled the hand collision (used on release to manage collision overlap bugs)</summary>
        protected IEnumerator TimedResetLayer(float seconds, string startLayer, string resetLayer) {
            SetLayerRecursive(transform, LayerMask.NameToLayer(startLayer));
            yield return new WaitForSeconds(seconds);
            SetLayerRecursive(transform, LayerMask.NameToLayer(startLayer), LayerMask.NameToLayer(resetLayer));
        }
        
        public static void SetLayerRecursive(Transform obj, int fromLayer, int toLayer) {
            if(obj.gameObject.layer == fromLayer) {
                obj.gameObject.layer = toLayer;
            }
            for(int i = 0; i < obj.childCount; i++) {
                SetLayerRecursive(obj.GetChild(i), toLayer, fromLayer);
            }
        }

        public static void SetLayerRecursive(Transform obj, int newLayer) {
            obj.gameObject.layer = newLayer;
            for(int i = 0; i < obj.childCount; i++) {
                SetLayerRecursive(obj.GetChild(i), newLayer);
            }
        }


        


        //=================================================================

        //========================= EDITOR FUNCTIONS ======================


        [ContextMenu("Relaxed Hand")]
        public void RelaxHand() {
            foreach(var finger in fingers)
                finger.SetFingerBend(gripOffset);
        }

        [ContextMenu("Opened Hand")]
        public void OpenHand() {
            foreach(var finger in fingers)
                finger.SetFingerBend(0);
        }
        
        [ContextMenu("Closed Hand")]
        public void CloseHand() {
            foreach(var finger in fingers)
                finger.SetFingerBend(1);
        }

        [ContextMenu("Bend Fingers Until Hit")]
        public void ProceduralFingerBend() {
            ProceduralFingerBend(~handLayers);
        }
        
        /// <summary>Bends each finger until they hit</summary>
        public void ProceduralFingerBend(int layermask){
            foreach(var finger in fingers){
                finger.BendFingerUntilHit(fingerBendSteps, layermask);
            }
        }

        /// <summary>Bends each finger until they hit</summary>
        public void ProceduralFingerBend(RaycastHit hit){
            foreach(var finger in fingers){
                finger.BendFingerUntilHit(fingerBendSteps, hit);
            }
        }
        
        public static int GetHandsLayerMask() {
            return LayerMask.GetMask(leftHandHoldingLayerName, rightHandHoldingLayerName, rightHandLayerName, leftHandLayerName);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos() {
            bool setLayers = LayerMask.NameToLayer("HandPlayer") != -1 && grabLayers == (grabLayers | (1 << LayerMask.NameToLayer("HandPlayer")));
                if(grabLayers == 0 || setLayers)
                grabLayers = LayerMask.GetMask(grabbableLayerNameDefault, grabbingLayerName, releasingLayerName);

            if(highlightLayers == 0 || setLayers)
                highlightLayers = LayerMask.GetMask(grabbableLayerNameDefault);

            if(palmTransform == null)
                return;

            Gizmos.DrawLine(transform.position, (transform.position+transform.rotation*palmOffset));
            if(showGizmos) {
                if(handRays == null || handRays.Length == 0) {
                    if(palmTransform != null) {
                        Vector3 handDir = -Vector3.right;
                        for(int i = 0; i < 100; i++) {
                            float ampI = Mathf.Pow(i, 1.05f + grabSpreadOffset/10f) / (Mathf.PI * 0.8f);
                            Gizmos.DrawRay(palmTransform.position, palmTransform.rotation * Quaternion.Euler(0, Mathf.Cos(i) * ampI + 90, Mathf.Sin(i) * ampI) * handDir * reachDistance);
                        }
                    }
                }
                else {
                    foreach(var ray in handRays) {
                        Gizmos.DrawRay(palmTransform.position, palmTransform.rotation * ray * reachDistance);
                    }

                    Gizmos.color = Color.green;
                    RaycastHit hit;
                    var avgDir = HandClosestHit(out hit, reachDistance, grabLayers);
                    if(avgDir != Vector3.zero) {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawRay(palmTransform.position, avgDir);
                    }
                }
                Gizmos.color = Color.green;
            }
        }
        

        private void OnDrawGizmosSelected() {
            if(palmTransform == null)
                return;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(palmTransform.position, palmTransform.forward*reachDistance);
            
        }
#endif
    }
}

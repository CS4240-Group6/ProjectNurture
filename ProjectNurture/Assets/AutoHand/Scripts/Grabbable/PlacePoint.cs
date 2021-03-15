using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    public delegate void PlacePointEvent(PlacePoint point, Grabbable grabbable);

    #if UNITY_EDITOR
        [ExecuteInEditMode]
    #endif

    //You can override this by turning the radius to zero, and using any other trigger collider
    [RequireComponent(typeof(SphereCollider))]
    public class PlacePoint : MonoBehaviour{

        [Header("Allow/Deny")]
        [Tooltip("Will allow placement for any grabbable with a name containing this array of strings, leave blank for any grabbable allowed")]
        public string[] placeNames;
        [Tooltip("Will prevent placement for any name containing this array of strings")]
        public string[] blacklistNames;

        
        [Header("Placement")]
        public float placeRadius = 0.1f;
        [Space][Tooltip("This will make the point place the object as soon as it enters the radius, instead of on release")]
        public bool forcePlace = false;
        [Space][Tooltip("This will allow you to throw an object into an instant force place")]
        public bool onlyPlaceWhileHolding = true;
        [Space][Tooltip("This will make the point place the object as soon as it enters the radius, instead of on release")]
        public bool parentOnPlace = true;

        
        [Header("Placed Settings")]
        [Space][Tooltip("Snaps an object to the point at start, leave blank if empty")]
        public Grabbable startPlaced;

        [Space][Tooltip("Positional Offset of the original point")]
        public Vector3 placedOffset;

        [Space][Tooltip("Makes the object being placedObject kinematic")]
        public bool makePlacedKinematic = false;

        [Space][Tooltip("The rigidbody to attach the placed grabbable to - leave empty means no joint")]
        public Rigidbody placedJointLink;
        public float placedJointBreakForce = 1500;
        public float placedJointBreakTorque = 1500;

        [Space][Tooltip("Whether or not the held objects highlight will be triggered by the entering place point")]
        public bool grabbableHighlight = false;
        

        [Header("Unity Events")]
        public UnityEvent OnPlace;
        public UnityEvent OnRemove;
        public UnityEvent OnHighlight;
        public UnityEvent OnStopHighlight;
        
        //For the programmers
        public PlacePointEvent OnPlaceEvent;
        public PlacePointEvent OnRemoveEvent;
        public PlacePointEvent OnHighlightEvent;
        public PlacePointEvent OnStopHighlightEvent;

        FixedJoint joint = null;

        //How far the placed object has to be moved to count to auto remove from point so something else can take its place
        float removalDistance = 0.05f;

#if UNITY_EDITOR
        [Header("Editor")]
        public Grabbable editorGrabbable;
        Vector3 preEditorPos;
        Grabbable lastEditorGrabbable;
#endif

        Grabbable placedObject;
        Vector3 placePosition;
        bool occupied = false;
        SphereCollider col;
        Transform originParent;
        bool placingFrame;
        CollisionDetectionMode placedObjDetectionMode;

        protected void Start(){
#if UNITY_EDITOR
            lastEditorGrabbable = null;
            if(!EditorApplication.isPlaying)
                return;
#endif

            col = gameObject.GetComponent<SphereCollider>();
            col.radius = placeRadius;
            col.isTrigger = true;
            SetStartPlaced();
        }

        
        protected void Update() {
            if(joint != null && placedObject != null)
                placedObject.transform.position = placePosition;
#if UNITY_EDITOR
            if(EditorApplication.isPlaying)
                return;

            if(editorGrabbable != null){
                if(lastEditorGrabbable != editorGrabbable) {
                    if(lastEditorGrabbable != null){
                        Remove(lastEditorGrabbable);
                        lastEditorGrabbable.transform.position = preEditorPos;
                    }

                    preEditorPos = editorGrabbable.transform.position;
                }
                else{
                    originParent = editorGrabbable.transform.parent;
                    editorGrabbable.transform.parent = transform;
                    editorGrabbable.transform.localPosition = placedOffset;
                    editorGrabbable.transform.localRotation = Quaternion.identity;
                    editorGrabbable.transform.parent = originParent;
                }
            }
            lastEditorGrabbable = editorGrabbable;
#endif
        }



        protected void FixedUpdate() {
#if UNITY_EDITOR
            if(!EditorApplication.isPlaying)
                return;
#endif
            if(!placingFrame && placedObject != null && Vector3.Distance(transform.position+placedOffset, placedObject.transform.position) > removalDistance) {
                Remove(placedObject);
            }
            placingFrame = false;
        }



        public virtual bool CanPlace(Transform placeObj) {
            if(occupied)
                return false;

            if(placeNames.Length == 0 && blacklistNames.Length == 0)
                return true;
            
            if(blacklistNames.Length > 0)
                foreach(var badName in blacklistNames) {
                    if(placeObj.name.Contains(badName)){
                        return false;
                    }
                }
            
            if(placeNames.Length > 0)
                foreach(var placeName in placeNames) {
                    if(placeObj.name.Contains(placeName)){
                        return true;
                    }
                }


            return false;
        }
        

        public virtual void Place(Grabbable placeObj) {
            placingFrame = true;
            originParent = placeObj.transform.parent;
            placeObj.transform.parent = transform;
            placeObj.transform.localPosition = placedOffset;
            placeObj.transform.localRotation = Quaternion.identity;
            placeObj.body.position = placeObj.transform.position;
            placeObj.body.velocity = Vector3.zero;
            placeObj.body.angularVelocity = Vector3.zero;

            placedObjDetectionMode = placeObj.body.collisionDetectionMode;
            if(makePlacedKinematic) {
                placeObj.body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            placeObj.body.isKinematic = makePlacedKinematic;
                
            
            if(placedJointLink != null){
                joint = placedJointLink.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = placeObj.body;
                joint.breakForce = placedJointBreakForce;
                joint.breakTorque = placedJointBreakForce;
                
                joint.connectedMassScale = 1;
                joint.massScale = 1;
                joint.enableCollision = false;
                joint.enablePreprocessing = false;
            }
            
            if(!parentOnPlace)
                placeObj.transform.parent = originParent;

            occupied = true;
            placedObject = placeObj;
            placePosition = placedObject.transform.position;
            OnPlaceEvent?.Invoke(this, placeObj);
            OnPlace?.Invoke();
        }


        public virtual void Remove(Grabbable placeObj) {
            if(makePlacedKinematic)
                placeObj.body.isKinematic = false;
            placeObj.body.collisionDetectionMode = placedObjDetectionMode;

            if(parentOnPlace)
                placeObj.transform.parent = originParent;
            OnRemoveEvent?.Invoke(this, placeObj);
            OnRemove?.Invoke();
            occupied = false;
            placedObject = null;
            if(joint != null){
                Destroy(joint);
                joint = null;
            }
        }


        internal virtual void Highlight(Grabbable from) {
            if(placedObject == null){
                OnHighlightEvent?.Invoke(this, from);
                OnHighlight?.Invoke();
            }
        }


        internal virtual void StopHighlight(Grabbable from) {
            if(placedObject == null){
                OnStopHighlightEvent?.Invoke(this, from);
                OnStopHighlight?.Invoke();
            }
        }



        public void SetStartPlaced() {
            if(startPlaced != null){
                startPlaced.SetPlacePoint(this);
                Place(startPlaced);
            }
        }
        
        public Grabbable GetPlacedObject() {
            return placedObject;
        }

        internal float Distance(Transform from) {
            return Vector3.Distance(from.position, transform.position+placedOffset);
        }

        protected void OnJointBreak(float breakForce) {
            if(placedObject != null)
                Remove(placedObject);
        }

        void OnDrawGizmosSelected() {
            gameObject.GetComponent<SphereCollider>().radius = placeRadius;
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position + placedOffset, 0.0025f);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Autohand{
    public class HandStabilizer : MonoBehaviour{
        //This is the script that hides unstable joints without compromising joint functionality
        Hand[] hands;
        Vector3[] handsDeltaPos;

        void Start(){
            hands = FindObjectsOfType<Hand>();
            handsDeltaPos = new Vector3[hands.Length];
        }

        void OnEnable(){
            if(GraphicsSettings.renderPipelineAsset != null){
                RenderPipelineManager.beginFrameRendering += OnPreRender;
                RenderPipelineManager.endCameraRendering += OnPostRender;
            }
        }

        void OnDisable(){
            if(GraphicsSettings.renderPipelineAsset != null){
                RenderPipelineManager.beginFrameRendering -= OnPreRender;
                RenderPipelineManager.endCameraRendering -= OnPostRender;
            }
        }
        
        private void OnPostRender() {
            foreach(var hand in hands) {
                hand.OnPostRender();
            }
        }


        private void OnPreRender() {

            foreach(var hand in hands) {
                hand.OnPreRender();
            }

        }

        private void OnPreRender(ScriptableRenderContext src, Camera[] cam) {
            foreach(var hand in hands) {
                hand.OnPreRender();
            }
        }

        private void OnPostRender(ScriptableRenderContext src, Camera cam) {
            foreach(var hand in hands) {
                hand.OnPostRender();
            }
        }
        
    }
}

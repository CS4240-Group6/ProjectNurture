/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided ìAS ISÅEWITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class HandedInputSelector : MonoBehaviour
{
    OVRCameraRig m_CameraRig;
    OVRInputModule m_InputModule;

    void Start()
    {
        m_CameraRig = FindObjectOfType<OVRCameraRig>();
        m_InputModule = FindObjectOfType<OVRInputModule>();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            SetActiveController(OVRInput.Controller.RTouch);
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            SetActiveController(OVRInput.Controller.LTouch);
        }

    }

    void SetActiveController(OVRInput.Controller c)
    {
        Transform t;
        if(c == OVRInput.Controller.LTouch)
        {
            t = m_CameraRig.leftHandAnchor;
        }
        else
        {
            t = m_CameraRig.rightHandAnchor;
        }
        m_InputModule.rayTransform = t;
    }
}

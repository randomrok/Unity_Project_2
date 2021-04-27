using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Cameras
{
    public class CameraSwitcher : MonoBehaviour
    {
        private Cameras.FreeLookCam freeLookCam;
        private Cameras.AutoCam autoCam;

    // Initialisation of scripts to switch
    void Start()
        {
            freeLookCam = GetComponent<FreeLookCam>();
            autoCam = GetComponent<AutoCam>();
        }

        // Check if button is pressed to switch scripts
        void Update()
        {
            if (CrossPlatformInputManager.GetButton("Fire2"))
            {
                freeLookCam.enabled = true;
                autoCam.enabled = false;
            }else
            {
                freeLookCam.enabled = false;
                autoCam.enabled = true;
                freeLookCam.ResetCamera();
            }
        }
    }
}

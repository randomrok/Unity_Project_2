using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof (AeroplaneController))]
    public class AeroplaneUserControl2Axis : MonoBehaviour
    {
        // these max angles are only used on mobile, due to the way pitch and roll input are handled
        public float maxRollAngle = 80;
        public float maxPitchAngle = 80;
        public bool airBrakes;

        // reference to the aeroplane that we're controlling.
        private AeroplaneController m_Aeroplane;

        //Sets up a reference to the camera object we're controlling.
        private GameObject FreeLookCameraRig;
        private Cameras.FreeLookCam m_Camera;



        private void Awake()
        {
            // Set up the reference to the aeroplane controller and the FreeLookCam Script.
            m_Aeroplane = GetComponent<AeroplaneController>();

            FreeLookCameraRig = this.gameObject.transform.GetChild(5).gameObject;
            m_Camera = FreeLookCameraRig.GetComponent<Cameras.FreeLookCam>();
        }


        private void FixedUpdate()
        {
            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            float roll = CrossPlatformInputManager.GetAxis("Mouse X");
            float pitch = CrossPlatformInputManager.GetAxis("Mouse Y");
            float throttle = CrossPlatformInputManager.GetAxis("Vertical");
            float yaw = CrossPlatformInputManager.GetAxis("Horizontal");


            //Additional functionality added as airBrakes
            if (throttle > 0) airBrakes = false;
            else airBrakes = true;

            ToggleFreeLookCam();

            LockCursor();


#if MOBILE_INPUT
            AdjustInputForMobileControls(ref roll, ref pitch, ref throttle);
#endif
            // Pass the input to the aeroplane
            m_Aeroplane.Move(roll, pitch, yaw, throttle, airBrakes);
        }


        private void ToggleFreeLookCam()
        {
            //Turns on camera script and resets the rotation if not wanted.
            if (CrossPlatformInputManager.GetButton("Fire2"))
            {
                m_Camera.enabled = true;
            }
            else m_Camera.ResetCam();
        }

        private void LockCursor()
        {
            if (m_Camera.m_LockCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
        {
            // because mobile tilt is used for roll and pitch, we help out by
            // assuming that a centered level device means the user
            // wants to fly straight and level!

            // this means on mobile, the input represents the *desired* roll angle of the aeroplane,
            // and the roll input is calculated to achieve that.
            // whereas on non-mobile, the input directly controls the roll of the aeroplane.

            float intendedRollAngle = roll*maxRollAngle*Mathf.Deg2Rad;
            float intendedPitchAngle = pitch*maxPitchAngle*Mathf.Deg2Rad;
            roll = Mathf.Clamp((intendedRollAngle - m_Aeroplane.RollAngle), -1, 1);
            pitch = Mathf.Clamp((intendedPitchAngle - m_Aeroplane.PitchAngle), -1, 1);

            // similarly, the throttle axis input is considered to be the desired absolute value, not a relative change to current throttle.
            float intendedThrottle = throttle*0.5f + 0.5f;
            throttle = Mathf.Clamp(intendedThrottle - m_Aeroplane.Throttle, -1, 1);
        }
    }
}

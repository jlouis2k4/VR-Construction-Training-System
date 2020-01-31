using UnityEngine;
using Tobii.Valve;

namespace Tobii.XR
{
    public class OpenVRManager
    {
        private readonly TrackedDevicePose_t[] _poseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        private bool _trackingLost;

        public static bool IsAvailable()
        {
            try
            {
                if (!OpenVR.IsHmdPresent())
                    return false;
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        public Matrix4x4 GetHeadPoseFor(float secondsAgo)
        {
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(OpenVR.Compositor.GetTrackingSpace(), -secondsAgo,
                _poseArray);
            if (_poseArray[OpenVR.k_unTrackedDeviceIndex_Hmd].bPoseIsValid)
            {
                _trackingLost = false;
            }
            else
            {
                if (!_trackingLost) Debug.Log("Failed to get historical pose"); // Only log once
                _trackingLost = true;
                return Matrix4x4.identity;
            }

            return ToMatrix4x4(_poseArray[OpenVR.k_unTrackedDeviceIndex_Hmd].mDeviceToAbsoluteTracking);
        }

        private static Matrix4x4 ToMatrix4x4(HmdMatrix34_t pose)
        {
            return new Matrix4x4
            {
                m00 = pose.m0,
                m01 = pose.m1,
                m02 = -pose.m2,
                m03 = pose.m3,

                m10 = pose.m4,
                m11 = pose.m5,
                m12 = -pose.m6,
                m13 = pose.m7,

                m20 = -pose.m8,
                m21 = -pose.m9,
                m22 = pose.m10,
                m23 = -pose.m11,

                m30 = 0,
                m31 = 0,
                m32 = 0,
                m33 = 1,
            };
        }
    }
}

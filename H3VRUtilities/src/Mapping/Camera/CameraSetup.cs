using UnityEngine;

namespace H3VRUtils.Mapping.Camera
{
    /// https://creativecommons.org/publicdomain/zero/1.0/
    /// License: (CC0 1.0 Universal) You 're free to use these game assets in any project, 
    /// personal or commercial. There's no need to ask permission before using these.
    /// Giving attribution is not required, but is greatly appreciated!
    /// The original author is Garret Polk
    /// https://bitbucket.org/GarretPolk/h3vr-performance-utils/src/main/
    /// <summary>
    /// Set the camera view distance to help Unity's occlusion
    /// culling. Unity is very conservative with culling and 
    /// will show a lot more than it needs to. By manually setting
    /// the camera's view distance to the maximum we need in
    /// our scene we can help Unity occlude more of the objects.
    /// </summary>
    public class CameraSetup : MonoBehaviour
    {
        [Tooltip("Set this to the MAXIMUM distance you want the player to see.")]
        public float cameraViewDistance = 1500f;

        private UnityEngine.Camera cam = null;

        // NOTE : performance on this kinda sucks as
        // calling Camera.main is notoriously heavy.
        // I'd like to see a better mechanism exist to
        // message/event scripts when the scene is ready
        // to play.
        // 
        // Wait until we have a valid camera/player
        void Update()
        {
            // Setup camera view distance for this map
            cam = UnityEngine.Camera.main;

            if (cam != null)
            {
                cam.farClipPlane = cameraViewDistance;

                // Done, turn the component (script) off
                // so it's not running for the whole game.
                enabled = false;
            }
        }
    }

}
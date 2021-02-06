using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
    internal class BetterMagReleaseLatch : MonoBehaviour
    {
        [FormerlySerializedAs("FireArm")] public FVRFireArm fireArm;
        [FormerlySerializedAs("Joint")] public HingeJoint joint;

        [Tooltip("Greatly reduce what you think it may be. I recommend 2 for Sensitivity.")]
        public float jointReleaseSensitivity = 2f;

        [HideInInspector] public float jointAngle;

        [FormerlySerializedAs("_jointReleaseSensitivityAbove")] [HideInInspector]
        public float jointReleaseSensitivityAbove;

        [FormerlySerializedAs("_jointReleaseSensitivityBelow")] [HideInInspector]
        public float jointReleaseSensitivityBelow;

        private bool _isMagazineNotNull;
        private float _timeSinceLastCollision = 6f;

        private void Start()
        {
            _isMagazineNotNull = fireArm.Magazine != null;
            var localEulerAngles = transform.localEulerAngles;
            jointReleaseSensitivityAbove = localEulerAngles.x + jointReleaseSensitivity;
            jointReleaseSensitivityBelow = localEulerAngles.x - jointReleaseSensitivity;
        }

        private void FixedUpdate()
        {
            if (_timeSinceLastCollision < 5f) _timeSinceLastCollision += Time.deltaTime;
            if (_isMagazineNotNull && _timeSinceLastCollision < 0.03f)
                if (transform.localEulerAngles.x > jointReleaseSensitivityAbove ||
                    transform.localEulerAngles.x < jointReleaseSensitivityBelow)
                    fireArm.EjectMag();
            jointAngle = joint.angle;
        }

        private void OnCollisionEnter(Collision col)
        {
            if (col.collider.attachedRigidbody != null && col.collider.attachedRigidbody != fireArm.RootRigidbody &&
                col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != null &&
                col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>().IsHeld)
                _timeSinceLastCollision = 0f;
        }
    }
}
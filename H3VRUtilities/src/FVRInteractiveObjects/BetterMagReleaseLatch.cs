using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class BetterMagReleaseLatch : MonoBehaviour
	{
		[FormerlySerializedAs("FireArm")] public FVRFireArm fireArm;
		[FormerlySerializedAs("Joint")] public HingeJoint joint;
		private float _timeSinceLastCollision = 6f;
		[Tooltip("Greatly reduce what you think it may be. I recommend 2 for Sensitivity.")]
		public float jointReleaseSensitivity = 2f;
		[HideInInspector]
		public float jointAngle;
		[FormerlySerializedAs("_jointReleaseSensitivityAbove")] [HideInInspector]
		public float jointReleaseSensitivityAbove;
		[FormerlySerializedAs("_jointReleaseSensitivityBelow")] [HideInInspector]
		public float jointReleaseSensitivityBelow;

		private bool _isMagazineNotNull;

		[HideInInspector]
		public float basex;

		private void Start()
		{
			_isMagazineNotNull = this.fireArm.Magazine != null;
			basex = this.transform.rotation.x;
			jointReleaseSensitivityAbove = basex + jointReleaseSensitivity;
			jointReleaseSensitivityBelow = basex - jointReleaseSensitivity;
		}
		private void FixedUpdate()
		{
			if (this._timeSinceLastCollision < 5f)
			{
				this._timeSinceLastCollision += Time.deltaTime;
			}
			if (_isMagazineNotNull)
			{
				if (transform.rotation.x >= jointReleaseSensitivityAbove || transform.rotation.x <= jointReleaseSensitivityBelow)
				{
					this.fireArm.EjectMag();
				}
			}
			jointAngle = transform.rotation.x;
			//jointAngle = joint.angle;
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.collider.attachedRigidbody != null && col.collider.attachedRigidbody != this.fireArm.RootRigidbody && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != null && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>().IsHeld)
			{
				this._timeSinceLastCollision = 0f;
			}
		}
	}
}

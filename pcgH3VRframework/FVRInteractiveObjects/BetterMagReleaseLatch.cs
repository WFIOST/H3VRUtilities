using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	class BetterMagReleaseLatch : MonoBehaviour
	{
		public FVRFireArm FireArm;
		public HingeJoint Joint;
		private float timeSinceLastCollision = 6f;
		public float jointReleaseSensitivity = -35f;
		public float jointAngle;

		private void Start()
		{
			jointReleaseSensitivity = Joint.angle - jointReleaseSensitivity;
		}
		private void FixedUpdate()
		{
			if (this.timeSinceLastCollision < 5f)
			{
				this.timeSinceLastCollision += Time.deltaTime;
			}
			if (this.FireArm.Magazine != null && this.timeSinceLastCollision < 0.03f && this.Joint.angle < jointReleaseSensitivity)
			{
				this.FireArm.EjectMag();
			}
			jointAngle = Joint.angle;
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.collider.attachedRigidbody != null && col.collider.attachedRigidbody != this.FireArm.RootRigidbody && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>() != null && col.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>().IsHeld)
			{
				this.timeSinceLastCollision = 0f;
			}
		}
	}
}

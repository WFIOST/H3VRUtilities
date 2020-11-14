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
		public float jointReleaseSensitivity = 35f;
		[HideInInspector]
		public float jointAngle;
		[HideInInspector]
		public float _jointReleaseSensitivityAbove;
		[HideInInspector]
		public float _jointReleaseSensitivityBelow;
	}
}

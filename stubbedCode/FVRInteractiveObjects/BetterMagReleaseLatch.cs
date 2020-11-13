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
		public float jointAngleToRelease = -35f;
		public float jointAngle;
	}
}

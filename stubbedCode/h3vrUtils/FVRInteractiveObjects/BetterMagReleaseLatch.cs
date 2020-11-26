using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	class BetterMagReleaseLatch : MonoBehaviour
	{
		[FormerlySerializedAs("FireArm")] public FVRFireArm fireArm;
		[FormerlySerializedAs("Joint")] public HingeJoint joint;
		[Tooltip("Greatly reduce what you think it may be. I recommend 2 for Sensitivity.")]
		public float jointReleaseSensitivity = 2f;
		[HideInInspector]
		public float jointAngle;
		[FormerlySerializedAs("_jointReleaseSensitivityAbove")] [HideInInspector]
		public float jointReleaseSensitivityAbove;
		[FormerlySerializedAs("_jointReleaseSensitivityBelow")] [HideInInspector]
		public float jointReleaseSensitivityBelow;
	}
}

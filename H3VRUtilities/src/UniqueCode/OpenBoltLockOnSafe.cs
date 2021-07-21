using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils.UniqueCode
{
	public class OpenBoltLockOnSafe : MonoBehaviour
	{
		[FormerlySerializedAs("Bolt")] public Collider bolt;
		[FormerlySerializedAs("SafetySwitch")] public GameObject safetySwitch;
		[FormerlySerializedAs("SafetyRotDir")] public CullOnZLoc.DirType safetyRotDir;
		[FormerlySerializedAs("AngleWhenSafe")] public float angleWhenSafe;
		
		public void FixedUpdate()
		{
			if(safetySwitch.transform.localEulerAngles[(int)safetyRotDir] == angleWhenSafe)
			{
				bolt.enabled = false;
			}
			else
			{
				bolt.enabled = true;
			}
		}
	}
}

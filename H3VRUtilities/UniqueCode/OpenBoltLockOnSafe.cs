using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.UniqueCode
{
	class OpenBoltLockOnSafe : MonoBehaviour
	{
		public Collider Bolt;
		public GameObject SafetySwitch;
		public cullOnZLoc.dirType SafetyRotDir;
		public float AngleWhenSafe;
		
		public void FixedUpdate()
		{
			if(SafetySwitch.transform.localEulerAngles[(int)SafetyRotDir] == AngleWhenSafe)
			{
				Bolt.enabled = false;
			}
			else
			{
				Bolt.enabled = true;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class LockGun : MonoBehaviour
	{

		public void Start()
		{
			Firearm.m_isPivotLocked = true;
			Firearm.m_pivotLockedPos = LockPos.transform.position;
			Firearm.m_pivotLockedRot = LockPos.transform.rotation;
		}
		public FVRPhysicalObject Firearm;
		public GameObject LockPos;
	}
}

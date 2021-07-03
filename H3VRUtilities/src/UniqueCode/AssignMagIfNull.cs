using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	class AssignMagIfNull : MonoBehaviour
	{
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;


		public void FixedUpdate()
		{
			if (firearm.Magazine == null) { firearm.Magazine = magazine; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	public class AssignMagIfNull : MonoBehaviour
	{
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;


		public void FixedUpdate()
		{
			if (firearm.Magazine is null) { firearm.Magazine = magazine; }
		}
	}
}

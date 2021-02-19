﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.NonAddedScripts
{
	class RecoilModificationSystem : MonoBehaviour
	{
		public FVRFireArm firearm;
		private FVRFireArmRecoilProfile modifiedRecoil;
		private FVRFireArmRecoilProfile baseRecoil;


		public void Start()
		{
			firearm = GetComponent<FVRFireArm>();
			if (firearm == null) { Console.WriteLine("Cannot find firearm!"); UnityEngine.Object.Destroy(this); }
		}

	}
}

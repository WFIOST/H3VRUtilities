using System;
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
		private FVRFireArmRecoilProfile _modifiedRecoil;
		private FVRFireArmRecoilProfile _baseRecoil;


		public void Start()
		{
			firearm = GetComponent<FVRFireArm>();
			if (firearm is null) { Console.WriteLine("Cannot find firearm!"); UnityEngine.Object.Destroy(this); }
		}

	}
}

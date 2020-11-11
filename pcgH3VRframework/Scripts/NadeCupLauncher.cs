using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.NadeCup
{
	class NadeCupLauncher : MonoBehaviour
	{
		public FVRFireArmChamber mainChamber;
		public FVRFireArmChamber nadeCup;
		public bool alreadyFired;

		void FixedUpdate()
		{
			 if (mainChamber.IsSpent && alreadyFired == false){
				nadeCup.Fire();
				alreadyFired = true;
			}
			else if (mainChamber.IsSpent == false && alreadyFired == true){
				alreadyFired = false;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace pcgH3VRframework
{
	class NadeCupLauncher : MonoBehaviour
	{
		public FVRFireArmChamber mainChamber;
		public FVRFireArmChamber NadeCup;
		public bool alreadyFired;

		void FixedUpdate()
		{
			 if (mainChamber.IsSpent && alreadyFired == false){
				NadeCup.Fire();
				alreadyFired = true;
			}
			else if (mainChamber.IsSpent == false && alreadyFired == true){
				alreadyFired = false;
			}
		}
	}
}

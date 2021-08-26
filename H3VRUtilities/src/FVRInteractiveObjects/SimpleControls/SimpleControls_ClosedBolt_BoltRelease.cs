using System;
using FistVR;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace H3VRUtils
{
	public class SimpleControls_ClosedBolt_BoltRelease : SimpleControls_ClosedBolt
	{
		public void FixedUpdate() { 
			cbw.HasBoltReleaseButton = UtilsBepInExLoader.SimpleControls.Value;
		}
	}
}
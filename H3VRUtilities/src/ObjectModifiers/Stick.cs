using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.ObjectModifiers
{
	public class Stick : MonoBehaviour
	{
		public FVRPhysicalObject physObj;
		[Tooltip("Point Z to direction to check. If an layer object is within 0.05m from the raycastdir, it will lock.")]
		public Transform raycastDir;
		[Tooltip("If velocity is below this, it will allow it to lock.")]
		public float minVel;
		public LayerMask layer;
		
		private bool isLocked;
		
		public void FixedUpdate()
		{
			if (physObj.RootRigidbody.velocity.magnitude <= minVel)
			{
				RaycastHit hit;
				if(Physics.Raycast(raycastDir.position, raycastDir.forward, out hit, 2, layer))
				{
					isLocked = true;
					physObj.SetIsKinematicLocked(true);
				}
			}
			
			if (isLocked && physObj.IsHeld)
			{
				isLocked = false;
				physObj.SetIsKinematicLocked(false);
			}
		}
	}
}
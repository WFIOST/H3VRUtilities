using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.FVRInteractiveObjects
{
	public class AttachableChargingHandle : MultipleChargingHandleClosedBolt
	{
		public FVRFireArmAttachment attachment;
		private Collider col;

		private void Start()
		{
			col = GetComponent<Collider>();
			col.enabled = false;
		}

		private void FixedUpdate()
		{
			if (Weapon == null)
			{
				if (attachment.curMount != null)
				{
					var wep = attachment.curMount.Parent.GetComponent<ClosedBoltWeapon>();
					if (wep != null)
					{
						if (wep.HasHandle)
						{
							attachment.DetachFromMount();
							return;
						}
						Weapon = wep;
						wep.HasHandle = true;
						wep.Handle = this;
						col.enabled = true;
					}
				}
			}
			else
			{
				if (attachment.curMount == null)
				{
					Weapon.HasHandle = false;
					Weapon.Handle = null;
					Weapon = null;
					col.enabled = false;
				}
			}
		}
	}
}
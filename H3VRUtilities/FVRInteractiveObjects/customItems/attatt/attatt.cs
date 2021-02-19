using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.customItems.AA
{
	public class attatt : MonoBehaviour
	{
		public FVRQuickBeltSlot gunslot;
		public FVRQuickBeltSlot attslot;

		public void FixedUpdate()
		{
			if (gunslot.CurObject != null && attslot.CurObject != null)
			{
				if (attslot.CurObject is FVRFireArmAttachment)
				{
					var attacher = attslot.CurObject.GetComponent<FVRFireArmAttachment>();
					for (int i = 0; i < gunslot.CurObject.AttachmentMounts.Count; i++)
					{
						if (gunslot.CurObject.AttachmentMounts[i].Type == attacher.Type)
						{
							attacher.AttachToMount(gunslot.CurObject.AttachmentMounts[i], true);
						}
					}
				}
			}
		}
	}
}

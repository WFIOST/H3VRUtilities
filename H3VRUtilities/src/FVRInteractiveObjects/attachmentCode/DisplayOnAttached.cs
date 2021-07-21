using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class DisplayOnAttached : MonoBehaviour
	{
		public GameObject displayOnAttach;
		[FormerlySerializedAs("AttachmentMount")] public FVRFireArmAttachmentMount attachmentMount;
		public void FixedUpdate()
		{
			if (attachmentMount.HasAttachmentsOnIt() == true)
			{
				displayOnAttach.SetActive(true);
			}
			else
			{
				displayOnAttach.SetActive(false);
			}
		}
	}
}

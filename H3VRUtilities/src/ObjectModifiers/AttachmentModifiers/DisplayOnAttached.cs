using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class DisplayOnAttached : MonoBehaviour
	{
		public GameObject displayOnAttach;
		public FVRFireArmAttachmentMount AttachmentMount;
		public void FixedUpdate()
		{
			if (AttachmentMount.HasAttachmentsOnIt() == true)
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

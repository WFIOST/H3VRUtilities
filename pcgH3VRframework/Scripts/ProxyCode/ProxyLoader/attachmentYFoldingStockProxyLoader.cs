using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace pcgH3VRframework.Scripts.ProxyCode.ProxyLoader
{
	public class attachmentYFoldingStockProxyLoader : MonoBehaviour
	{
		attachmentYFoldingStockProxy transferfrom;
		attachmentYFoldingStock transferto;

		void Start()
		{
			Console.WriteLine("HELLO WORLD! attachmentYFoldingStockProxyLoader doing it's job.");
			transferfrom = gameObject.GetComponent<attachmentYFoldingStockProxy>();
			transferto = gameObject.AddComponent<attachmentYFoldingStock>();

			//attachmentYFoldingStockProxy vars
			transferto.Root = transferfrom.Root;
			transferto.Stock = transferfrom.Stock;
			transferto.MinRot = transferfrom.MinRot;
			transferto.MaxRot = transferfrom.MaxRot;
			transferto.m_curPos = transferfrom.m_curPos;
			transferto.m_lastPos = transferfrom.m_lastPos;
			transferto.isMinClosed = transferfrom.isMinClosed;
			transferto.FireArm = transferfrom.FireArm;
			transferto.attachment = transferfrom.attachment;

			//FVRInteractiveObject vars
			transferto.ControlType = transferfrom.ControlType;
			transferto.IsSimpleInteract = transferfrom.IsSimpleInteract;
			transferto.HandlingGrabSound = transferfrom.HandlingGrabSound;
			transferto.HandlingReleaseSound = transferfrom.HandlingReleaseSound;
			transferto.PoseOverride = transferfrom.PoseOverride;
			transferto.QBPoseOverride = transferfrom.QBPoseOverride;
			transferto.PoseOverride_Touch = transferfrom.PoseOverride_Touch;
			transferto.UseGrabPointChild = transferfrom.UseGrabPointChild;
			transferto.UseGripRotInterp = transferfrom.UseGripRotInterp;
			transferto.PositionInterpSpeed = transferfrom.PositionInterpSpeed;
			transferto.RotationInterpSpeed = transferfrom.RotationInterpSpeed;
			transferto.EndInteractionIfDistant = transferfrom.EndInteractionIfDistant;
			transferto.EndInteractionDistance = transferfrom.EndInteractionDistance;
			transferto.m_hand = transferfrom.m_hand;
			transferto.UXGeo_Hover = transferfrom.UXGeo_Hover;
			transferto.UXGeo_Held = transferfrom.UXGeo_Held;
			transferto.UseFilteredHandTransform = transferfrom.UseFilteredHandTransform;
			transferto.UseFilteredHandPosition = transferfrom.UseFilteredHandPosition;
			transferto.UseFilteredHandRotation = transferfrom.UseFilteredHandRotation;
			transferto.UseSecondStepRotationFiltering = transferfrom.UseSecondStepRotationFiltering;
			transferfrom.byeworld();
			Console.WriteLine("attachmentYFoldingStockProxyLoader done!");
		}

		void Update()
		{
			if (transferto.FireArm == null && transferto.attachment.curMount != null)
			{
				var _firearm = transform.root.GetComponent<FVRFireArm>();
				if (_firearm != null)
				{
					transferto.FireArm = _firearm;
					Console.WriteLine("attachmentYFoldingStock has connected itself to " + transferto.FireArm);
				}
			}
			else if (transferto.FireArm != null && transferto.attachment.curMount == null){ transferto.FireArm = null; }
		}
	}
}

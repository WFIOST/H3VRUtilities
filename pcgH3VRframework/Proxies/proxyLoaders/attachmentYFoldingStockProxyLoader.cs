using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using H3VRUtils;
using H3VRUtils.Proxy;

namespace H3VRUtils.ProxyLoader
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
			transferto.root = transferfrom.Root;
			transferto.stock = transferfrom.Stock;
			transferto.minRot = transferfrom.MinRot;
			transferto.maxRot = transferfrom.MaxRot;
			transferto.mCurPos = transferfrom.m_curPos;
			transferto.mLastPos = transferfrom.m_lastPos;
			transferto.isMinClosed = transferfrom.isMinClosed;
			transferto.fireArm = transferfrom.FireArm;
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
			if (transferto.attachment.curMount != null)
			{
				if (transferto.fireArm == null)
				{
					var _firearm = transferto.attachment.curMount.Parent.GetComponent<FVRFireArm>();
					transferto.fireArm = _firearm;
					Console.WriteLine("attachmentYFoldingStock has connected itself to " + transferto.fireArm);
				}
			}
			else if (transferto.fireArm != null){ transferto.fireArm = null; }
		}
	}
}

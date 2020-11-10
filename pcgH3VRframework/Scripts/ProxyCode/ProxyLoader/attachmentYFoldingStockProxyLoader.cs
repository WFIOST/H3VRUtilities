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
		void Start()
		{
			Console.WriteLine("HELLO WORLD! attachmentYFoldingStockProxyLoader doing it's job.");
			var transferfrom = gameObject.GetComponent<attachmentYFoldingStockProxy>();
			var transferto = gameObject.AddComponent<attachmentYFoldingStock>();

			//attachmentYFoldingStockProxy vars
			transferfrom.Root = transferto.Root;
			transferfrom.Stock = transferto.Stock;
			transferfrom.MinRot = transferto.MinRot;
			transferfrom.MaxRot = transferto.MaxRot;
			transferfrom.m_curPos = transferto.m_curPos;
			transferfrom.m_lastPos = transferto.m_lastPos;
			transferfrom.isMinClosed = transferto.isMinClosed;
			transferfrom.FireArm = transferto.FireArm;
			transferfrom.attachment = transferto.attachment;

			//FVRInteractiveObject vars
			transferfrom.ControlType = transferto.ControlType;
			transferfrom.IsSimpleInteract = transferto.IsSimpleInteract;
			transferfrom.HandlingGrabSound = transferto.HandlingGrabSound;
			transferfrom.HandlingReleaseSound = transferto.HandlingReleaseSound;
			transferfrom.PoseOverride = transferto.PoseOverride;
			transferfrom.QBPoseOverride = transferto.QBPoseOverride;
			transferfrom.PoseOverride_Touch = transferto.PoseOverride_Touch;
			transferfrom.UseGrabPointChild = transferto.UseGrabPointChild;
			transferfrom.UseGripRotInterp = transferto.UseGripRotInterp;
			transferfrom.PositionInterpSpeed = transferto.PositionInterpSpeed;
			transferfrom.RotationInterpSpeed = transferto.RotationInterpSpeed;
			transferfrom.EndInteractionIfDistant = transferto.EndInteractionIfDistant;
			transferfrom.EndInteractionDistance = transferto.EndInteractionDistance;
			transferfrom.m_hand = transferto.m_hand;
			transferfrom.UXGeo_Hover = transferto.UXGeo_Hover;
			transferfrom.UXGeo_Held = transferto.UXGeo_Held;
			transferfrom.UseFilteredHandTransform = transferto.UseFilteredHandTransform;
			transferfrom.UseFilteredHandPosition = transferto.UseFilteredHandPosition;
			transferfrom.UseFilteredHandRotation = transferto.UseFilteredHandRotation;
			transferfrom.UseSecondStepRotationFiltering = transferto.UseSecondStepRotationFiltering;
			Console.WriteLine("attachmentYFoldingStockProxy done!");
		}
	}
}

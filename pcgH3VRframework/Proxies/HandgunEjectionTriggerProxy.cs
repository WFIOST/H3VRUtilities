using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	class HandgunEjectionTriggerProxy : FVRInteractableObjectProxy
	{
		public Handgun proxyhgReceiver;
		void Start()
		{
			Console.WriteLine("HGETProxy here to do it's job!");
			var transferto = gameObject.AddComponent<HandgunEjectionTrigger>();
			transferto.hgReceiver = proxyhgReceiver;
			var transferfrom = this;

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
			Console.WriteLine("HGETProxy done!");
		}
	}
}

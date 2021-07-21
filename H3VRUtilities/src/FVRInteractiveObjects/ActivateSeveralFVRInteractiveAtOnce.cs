using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	class ActivateSeveralFvrInteractiveAtOnce : FVRInteractiveObject
	{
		[FormerlySerializedAs("InteractiveObjects")] public List<FVRInteractiveObject> interactiveObjects;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			foreach (FVRInteractiveObject obj in interactiveObjects)
			{
				obj.BeginInteraction(hand);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			foreach (FVRInteractiveObject obj in interactiveObjects)
			{
				obj.EndInteraction(hand);
			}
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			foreach (FVRInteractiveObject obj in interactiveObjects)
			{
				obj.SimpleInteraction(hand);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			foreach (FVRInteractiveObject obj in interactiveObjects)
			{
				obj.UpdateInteraction(hand);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	class ActivateSeveralFVRInteractiveAtOnce : FVRInteractiveObject
	{
		public List<FVRInteractiveObject> InteractiveObjects;
		
		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			foreach (FVRInteractiveObject obj in InteractiveObjects)
			{
				obj.BeginInteraction(hand);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			foreach (FVRInteractiveObject obj in InteractiveObjects)
			{
				obj.EndInteraction(hand);
			}
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			foreach (FVRInteractiveObject obj in InteractiveObjects)
			{
				obj.SimpleInteraction(hand);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			foreach (FVRInteractiveObject obj in InteractiveObjects)
			{
				obj.UpdateInteraction(hand);
			}
		}
	}
}

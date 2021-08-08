using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	//thanks to CityRobo for fixing this script and actually making it work!
	//this script breaks without publicized assembly
	class OpenBoltBurstFire : MonoBehaviour
	{
		public OpenBoltReceiver Receiver;

		[Tooltip("Selector setting position that will be burst. Remember, selectors go pos: 0, 1 ,2, not 1, 2, 3")]
		public int SelectorSetting;
		
		[Tooltip("Amount of shots per burst.")]
		public  int BurstAmt = 3;
		private int BurstSoFar;

		private bool wasLoaded;

		public void Start()
		{
			Receiver.FireSelector_Modes[SelectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.FullAuto;
		}

		public void Update()
		{
			//if it's not the correct selector, just don't do anything
			if (Receiver.m_hand == null) return;
			if (Receiver.m_fireSelectorMode != SelectorSetting)
			{
				BurstSoFar = 0;
				return;
			}

			//add to burst if chamber is shot
			if(wasLoaded && !Receiver.Chamber.IsFull)
			{
				BurstSoFar++;
			}
			wasLoaded = Receiver.Chamber.IsFull;

			//if burst amount hit
			if (BurstSoFar >= BurstAmt)
			{
				lockUp();
				if(Receiver.m_hand.Input.TriggerFloat < Receiver.TriggerFiringThreshold)
				{
					unLock();
				}
			}

			//reset amt if trigger is let go
			if (Receiver.m_hand.Input.TriggerFloat < Receiver.TriggerFiringThreshold)
			{
				BurstSoFar = 0;
			}
		}

		public void lockUp()
		{
			//put to safe
			Receiver.FireSelector_Modes[SelectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.Safe;
		}

		public void unLock()
		{
			//put to auto; reset
			BurstSoFar = 0;
			Receiver.FireSelector_Modes[SelectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.FullAuto;
		}
	}
}

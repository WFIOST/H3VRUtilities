using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	class OpenBoltBurstFire : MonoBehaviour
	{
		[FormerlySerializedAs("Receiver")] public OpenBoltReceiver receiver;

		[FormerlySerializedAs("SelectorSetting")] [Tooltip("Selector setting position that will be burst. Remember, selectors go pos: 0, 1 ,2, not 1, 2, 3")]
		public int selectorSetting;

		[FormerlySerializedAs("BurstAmt")] public  int burstAmt;
		private int _burstSoFar;

		private bool _wasLoaded;

		public void Start()
		{
			receiver.FireSelector_Modes[selectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.FullAuto;
		}

		public void Update()
		{
			//if it's not the correct selector, just don't do anything
			if (receiver.m_fireSelectorMode != selectorSetting)
			{
				_burstSoFar = 0;
				return;
			}

			//add to burst if chamber is shot
			if(_wasLoaded && !receiver.Chamber.IsFull)
			{
				_burstSoFar++;
			}
			_wasLoaded = receiver.Chamber.IsFull;

			//if burst amount hit
			if (_burstSoFar >= burstAmt)
			{
				LockUp();
				if(receiver.m_triggerFloat <= receiver.TriggerResetThreshold)
				{
					UnLock();
				}
			}

			//reset amt if trigger is let go
			if (receiver.m_triggerFloat <= receiver.TriggerResetThreshold)
			{
				_burstSoFar = 0;
			}
		}

		public void LockUp()
		{
			//put to safe
			receiver.FireSelector_Modes[selectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.Safe;
		}

		public void UnLock()
		{
			//put to auto; reset
			_burstSoFar = 0;
			receiver.FireSelector_Modes[selectorSetting].ModeType = OpenBoltReceiver.FireSelectorModeType.FullAuto;
		}
	}
}

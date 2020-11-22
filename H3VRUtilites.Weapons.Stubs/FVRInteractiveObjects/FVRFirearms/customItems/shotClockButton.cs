using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.customItems.shotClock
{
	class shotClockButton : FVRPhysicalObject
	{
		public shotClock shotclock;

		public bool pressed;
		public enum buttonType
		{
			start,
			stop,
			register,
			delay
		}
		public buttonType button;

		public void Update()
		{
			if (pressed){
				pressed = false;
				SimpleInteraction(null);
			}
		}
		public void SimpleInteraction(FVRViveHand hand)
		{
			switch (button)
			{
				case buttonType.start:
					shotclock.startClockProcess();
					break;
				case buttonType.stop:
					shotclock.StopClock();
					break;
				case buttonType.register:

					break;
				case buttonType.delay:

					break;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.FVRInteractiveObjects
{
	class AutoRackOnMagLoad : MonoBehaviour
	{
		public FVRFireArm weapon;
		private Handgun hg;
		private ClosedBoltWeapon cbw;
		private OpenBoltReceiver obr;
		private bool WasLoaded;

		public void Start()
		{
			if (weapon is Handgun)
			{
				hg = weapon as Handgun;
			}
			if (weapon is ClosedBoltWeapon)
			{
				cbw = weapon as ClosedBoltWeapon;
			}
			if (weapon is OpenBoltReceiver)
			{
				obr = weapon as OpenBoltReceiver;
			}
		}

		public void FixedUpdate()
		{
			if (weapon.Magazine != null)
			{
				if (WasLoaded == false)
				{
					if (hg != null)
					{
						hg.Slide.ImpartFiringImpulse();
					}
					if (cbw != null)
					{
						cbw.Bolt.ImpartFiringImpulse();
					}
					if (obr != null)
					{
						obr.Bolt.ImpartFiringImpulse();
					}
				}
				WasLoaded = true;
			}
			else
			{
				WasLoaded = false;
			}
		}
	}
}

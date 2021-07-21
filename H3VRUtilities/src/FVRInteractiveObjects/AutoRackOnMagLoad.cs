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
		private Handgun _hg;
		private ClosedBoltWeapon _cbw;
		private OpenBoltReceiver _obr;
		private bool _wasLoaded;

		public void Start()
		{
			if (weapon is Handgun)
			{
				_hg = weapon as Handgun;
			}
			if (weapon is ClosedBoltWeapon)
			{
				_cbw = weapon as ClosedBoltWeapon;
			}
			if (weapon is Handgun)
			{
				_obr = weapon as OpenBoltReceiver;
			}
		}

		public void FixedUpdate()
		{
			if (weapon.Magazine != null)
			{
				if (_wasLoaded == false)
				{
					if (_hg != null)
					{
						_hg.Slide.ImpartFiringImpulse();
					}
					if (_cbw != null)
					{
						_cbw.Bolt.ImpartFiringImpulse();
					}
					if (_obr != null)
					{
						_obr.Bolt.ImpartFiringImpulse();
					}
				}
				_wasLoaded = true;
			}
			else
			{
				_wasLoaded = false;
			}
		}
	}
}

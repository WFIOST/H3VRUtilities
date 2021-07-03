using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.FVRInteractiveObjects
{
	class OpenBoltMagSafety : MonoBehaviour
	{
		public OpenBoltReceiver OpenBoltWep;

		private bool wasLoaded = false;

		void Update()
		{
			if (OpenBoltWep.Magazine == null)
			{
				if (wasLoaded)
				{

				}
				wasLoaded = false;
			}
			if (OpenBoltWep.Magazine != null)
			{
				wasLoaded = true;
			}
		}
	}
}

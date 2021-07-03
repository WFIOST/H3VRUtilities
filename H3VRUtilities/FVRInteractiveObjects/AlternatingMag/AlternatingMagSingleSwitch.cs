using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.AlternatingMags
{
	class AlternatingMagSingleSwitch : FVRInteractiveObject
	{
		public GameObject transformOnInactive;
		public GameObject transformOnActive;
		public AlternatingMagsHandler MagHandler;
		[Tooltip("Use the relevant number in the MagHandler's list of MagMounts.")]
		public int ConnectedMagMount;
		public AudioEvent ClickAudio;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (MagHandler.MagMounts[ConnectedMagMount].IsActive)
			{
				return;
			}
			MagHandler.ChangeMag(ConnectedMagMount);

			try
			{
				SM.PlayGenericSound(ClickAudio, transform.position);
			}
			catch
			{
				Console.WriteLine(this.name + " failed to play sound!");
			}
		}



		public void FixedUpdate()
		{
			
		}
	}
}

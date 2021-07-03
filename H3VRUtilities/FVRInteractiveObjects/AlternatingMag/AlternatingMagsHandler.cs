using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.AlternatingMags
{
	class AlternatingMagsHandler
	{
		public bool AlternateOnEachShot;
		public List<AlternatingMagMount> MagMounts;
		public FVRFireArm firearm;
		[HideInInspector]
		public int activeMagMount;

		public void Start()
		{
			ChangeMag(0);
		}

		public void ChangeMag(int OverrideMagToChange = -1)
		{
			if (OverrideMagToChange == -1)
			{
				OverrideMagToChange = activeMagMount += 1;
				if(activeMagMount > MagMounts.Count)
				{
					activeMagMount = 0;
				}
			}
			activeMagMount = OverrideMagToChange;
			for (int i = 0; i < MagMounts.Count; i++)
			{
				if (i == OverrideMagToChange)
				{
					MagMounts[i].SetActivity(true);
				}
				else
					MagMounts[i].SetActivity(false);
			}
		}

		public void Update()
		{

		}
	}
}

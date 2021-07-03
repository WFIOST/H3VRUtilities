using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.AlternatingMags
{
	class AlternatingMagMount
	{
		public GameObject MagMountWhenActive;
		public GameObject MagMountWhenInactive;
		[HideInInspector]
		public FVRFireArmMagazine curmag;
		public FVRFireArm firearm;
		[HideInInspector]
		public bool IsActive;

		private GameObject MagMount
		{
			get
			{
				if (IsActive)
				{
					return MagMountWhenActive;
				}
				else
				{
					return MagMountWhenInactive;
				}
			}
		}

		private void OnTriggerEnter(Collider obj)
		{
			if (curmag != null) return;
			var gc = obj.GetComponent<FVRFireArmMagazine>();
			if(gc != null)
			{
				if(gc.MagazineType == firearm.MagazineType)
				{
					LoadMag(gc);
				}
			}
		}

		public void SetActivity(bool activity)
		{
			if (IsActive && !activity)
			{
				firearm.Magazine = null;
			}
			else if (!IsActive && activity)
			{
				firearm.LoadMag(curmag);
			}

			IsActive = activity;
			curmag.SetParentage(MagMount.transform);
			curmag.transform.rotation = MagMount.transform.rotation;
			curmag.transform.position = MagMount.transform.position;
		}

		public void LoadMag(FVRFireArmMagazine mag)
		{
			curmag = mag;

			mag.State = FVRFireArmMagazine.MagazineState.Locked;
			mag.FireArm = firearm;

			if (!IsActive)
			{
				firearm.Magazine = null;
			}
			else
			{
				firearm.LoadMag(curmag);
			}

			//mag.FireArm.LoadMag(mag);
			mag.IsHeld = false;
			mag.ForceBreakInteraction();
			/*if (mag.UsesVizInterp)
			{
				mag.m_vizLerpStartPos = this.Viz.transform.position;
				this.m_vizLerpStartRot = this.Viz.transform.rotation;
				this.m_vizLerp = 0f;
				this.m_isVizLerpInward = true;
				this.m_isVizLerping = true;
			}*/
			mag.SetParentage(MagMount.transform);
			mag.transform.rotation = MagMount.transform.rotation;
			mag.transform.position = MagMount.transform.position;
			/*if (mag.UsesVizInterp)
			{
				this.Viz.position = this.m_vizLerpStartPos;
				this.Viz.rotation = this.m_vizLerpStartRot;
			}*/
			mag.StoreAndDestroyRigidbody();
			if (mag.FireArm.QuickbeltSlot != null)
			{
				mag.SetAllCollidersToLayer(false, "NoCol");
			}
			else
			{
				mag.SetAllCollidersToLayer(false, "Default");
			}
			if (firearm.ObjectWrapper != null)
			{
				GM.CurrentSceneSettings.OnFireArmReloaded(firearm.ObjectWrapper);
			}
		}
	}
}

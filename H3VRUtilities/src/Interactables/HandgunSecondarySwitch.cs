using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class HandgunSecondarySwitch : FVRInteractiveObject
	{
		public new void Awake()
		{
			this.UpdateBaseGunSelector(this.CurModeIndex);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			this.CycleMode();
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000AA940 File Offset: 0x000A8D40
		private void CycleMode()
		{
			this.CurModeIndex++;
			if (this.CurModeIndex >= this.Modes.Length)
			{
				this.CurModeIndex = 0;
			}
			this.UpdateBaseGunSelector(this.CurModeIndex);
			this.Weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector, 1f);
		}

		private void UpdateBaseGunSelector(int i)
		{
			
			Handgun.FireSelectorMode fireSelectorMode = this.Modes[i];
			this.Weapon.SetAnimatedComponent(this.SelctorSwitch, fireSelectorMode.SelectorPosition, this.InterpStyle, this.Axis);
			Handgun.FireSelectorMode fireSelectorMode2 = this.Weapon.FireSelectorModes[this.ModeIndexToSub];
			fireSelectorMode2.ModeType = fireSelectorMode.ModeType;
			fireSelectorMode2.BurstAmount = fireSelectorMode.BurstAmount;
			this.Weapon.ResetCamBurst();
		}

		// Token: 0x04002EAF RID: 11951
		public Handgun Weapon;

		// Token: 0x04002EB0 RID: 11952
		public int CurModeIndex;

		// Token: 0x04002EB1 RID: 11953
		public int ModeIndexToSub = 1;

		// Token: 0x04002EB2 RID: 11954
		public Transform SelctorSwitch;

		// Token: 0x04002EB3 RID: 11955
		public FVRPhysicalObject.Axis Axis;

		// Token: 0x04002EB4 RID: 11956
		public FVRPhysicalObject.InterpStyle InterpStyle;

		// Token: 0x04002EB5 RID: 11957
		public Handgun.FireSelectorMode[] Modes;
	}
}

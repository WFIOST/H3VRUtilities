using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class HandgunSecondarySwitch : FVRInteractiveObject
	{
		public new void Awake()
		{
			this.UpdateBaseGunSelector(this.curModeIndex);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			this.CycleMode();
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000AA940 File Offset: 0x000A8D40
		private void CycleMode()
		{
			this.curModeIndex++;
			if (this.curModeIndex >= this.modes.Length)
			{
				this.curModeIndex = 0;
			}
			this.UpdateBaseGunSelector(this.curModeIndex);
			this.weapon.PlayAudioEvent(FirearmAudioEventType.FireSelector, 1f);
		}

		private void UpdateBaseGunSelector(int i)
		{
			
			Handgun.FireSelectorMode fireSelectorMode = this.modes[i];
			this.weapon.SetAnimatedComponent(this.selctorSwitch, fireSelectorMode.SelectorPosition, this.interpStyle, this.axis);
			Handgun.FireSelectorMode fireSelectorMode2 = this.weapon.FireSelectorModes[this.modeIndexToSub];
			fireSelectorMode2.ModeType = fireSelectorMode.ModeType;
			fireSelectorMode2.BurstAmount = fireSelectorMode.BurstAmount;
			this.weapon.ResetCamBurst();
		}

		// Token: 0x04002EAF RID: 11951
		[FormerlySerializedAs("Weapon")] public Handgun weapon;

		// Token: 0x04002EB0 RID: 11952
		[FormerlySerializedAs("CurModeIndex")] public int curModeIndex;

		// Token: 0x04002EB1 RID: 11953
		[FormerlySerializedAs("ModeIndexToSub")] public int modeIndexToSub = 1;

		// Token: 0x04002EB2 RID: 11954
		[FormerlySerializedAs("SelctorSwitch")] public Transform selctorSwitch;

		// Token: 0x04002EB3 RID: 11955
		[FormerlySerializedAs("Axis")] public FVRPhysicalObject.Axis axis;

		// Token: 0x04002EB4 RID: 11956
		[FormerlySerializedAs("InterpStyle")] public FVRPhysicalObject.InterpStyle interpStyle;

		// Token: 0x04002EB5 RID: 11957
		[FormerlySerializedAs("Modes")] public Handgun.FireSelectorMode[] modes;
	}
}

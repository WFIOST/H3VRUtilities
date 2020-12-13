using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	//Deprecated. Use H3VRUtilsMagRelease.
	public class HandgunEjectionTrigger : FVRInteractiveObject
	{
		// Token: 0x060033F4 RID: 13300 RVA: 0x0016AC7E File Offset: 0x0016907E
		public override bool IsInteractable()
		{
			return !(this.hgReceiver.Magazine == null);
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x0016AC9C File Offset: 0x0016909C
		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			if (this.hgReceiver.Magazine != null)
			{
				this.EndInteraction(hand);
				FVRFireArmMagazine magazine = this.hgReceiver.Magazine;
				this.hgReceiver.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}

		// Token: 0x060033F6 RID: 13302 RVA: 0x0016ACF4 File Offset: 0x001690F4
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.Input.TouchpadDown && this.hgReceiver.Magazine != null)
			{
				this.EndInteraction(hand);
				FVRFireArmMagazine magazine = this.hgReceiver.Magazine;
				this.hgReceiver.ReleaseMag();
				hand.ForceSetInteractable(magazine);
				magazine.BeginInteraction(hand);
			}
		}

		// Token: 0x040056DA RID: 22234
		public Handgun hgReceiver;
	}
}
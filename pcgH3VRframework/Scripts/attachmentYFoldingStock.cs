using System;
using UnityEngine;

namespace FistVR
{
	// Token: 0x020003CD RID: 973
	public class attachmentYFoldingStock : FVRInteractiveObject
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x00089ED0 File Offset: 0x000882D0
		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (this.attachment.curMount = null)
			{
				this.FireArm = null;
			}
			else if (this.FireArm != null)
			{
				GameObject _firearm = this.attachment.curMount.Parent.GetComponent<GameObject>();
				this.FireArm = _firearm.GetComponent<FVRFireArm>();
			}

			if (this.FireArm != null) {
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - this.Root.position;
			vector = Vector3.ProjectOnPlane(vector, this.Root.up).normalized;
			Vector3 lhs = -this.Root.transform.forward;
			this.rotAngle = Mathf.Atan2(Vector3.Dot(this.Root.up, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
			if (Mathf.Abs(this.rotAngle - this.MinRot) < 5f)
			{
				this.rotAngle = this.MinRot;
			}
			if (Mathf.Abs(this.rotAngle - this.MaxRot) < 5f)
			{
				this.rotAngle = this.MaxRot;
			}
				if (this.rotAngle >= this.MinRot && this.rotAngle <= this.MaxRot)
				{
					this.Stock.localEulerAngles = new Vector3(0f, this.rotAngle, 0f);
					float num = Mathf.InverseLerp(this.MinRot, this.MaxRot, this.rotAngle);
					if (this.isMinClosed)
					{
						if (num < 0.02f)
						{
							this.m_curPos = FVRFoldingStockYAxis.StockPos.Closed;
							this.FireArm.HasActiveShoulderStock = false;
						}
						else if (num > 0.9f)
						{
							this.m_curPos = FVRFoldingStockYAxis.StockPos.Open;
							this.FireArm.HasActiveShoulderStock = true;
						}
						else
						{
							this.m_curPos = FVRFoldingStockYAxis.StockPos.Mid;
							this.FireArm.HasActiveShoulderStock = false;
						}
					}
					else if (num < 0.1f)
					{
						this.m_curPos = FVRFoldingStockYAxis.StockPos.Open;
						this.FireArm.HasActiveShoulderStock = true;
					}
					else if (num > 0.98f)
					{
						this.m_curPos = FVRFoldingStockYAxis.StockPos.Closed;
						this.FireArm.HasActiveShoulderStock = false;
					}
					else
					{
						this.m_curPos = FVRFoldingStockYAxis.StockPos.Mid;
						this.FireArm.HasActiveShoulderStock = false;
					}
					if (this.m_curPos == FVRFoldingStockYAxis.StockPos.Open && this.m_lastPos != FVRFoldingStockYAxis.StockPos.Open)
					{
						this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen, 1f);
					}
					if (this.m_curPos == FVRFoldingStockYAxis.StockPos.Closed && this.m_lastPos != FVRFoldingStockYAxis.StockPos.Closed)
					{
						this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed, 1f);
					}
					this.m_lastPos = this.m_curPos;
					return;
				}
			}
		}

		// Token: 0x0400276C RID: 10092
		public Transform Root;

		// Token: 0x0400276D RID: 10093
		public Transform Stock;

		// Token: 0x0400276E RID: 10094
		private float rotAngle;

		// Token: 0x0400276F RID: 10095
		public float MinRot;

		// Token: 0x04002770 RID: 10096
		public float MaxRot;

		// Token: 0x04002771 RID: 10097
		public FVRFoldingStockYAxis.StockPos m_curPos;

		// Token: 0x04002772 RID: 10098
		public FVRFoldingStockYAxis.StockPos m_lastPos;

		public bool isMinClosed = true;

		public FVRFireArm FireArm;

		public FVRFireArmAttachment attachment;

		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}
	}
}

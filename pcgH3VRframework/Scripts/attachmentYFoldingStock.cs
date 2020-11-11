using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	// Token: 0x020003CD RID: 973
	public class AttachmentYFoldingStock : FVRInteractiveObject
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x00089ED0 File Offset: 0x000882D0
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - this.root.position;
			var up = this.root.up;
			vector = Vector3.ProjectOnPlane(vector, up).normalized;
			Vector3 lhs = -this.root.transform.forward;
			this._rotAngle = Mathf.Atan2(Vector3.Dot(up, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
			if (Mathf.Abs(this._rotAngle - this.minRot) < 5f)
			{
				this._rotAngle = this.minRot;
			}
			if (Mathf.Abs(this._rotAngle - this.maxRot) < 5f)
			{
				this._rotAngle = this.maxRot;
			}
				if (this._rotAngle >= this.minRot && this._rotAngle <= this.maxRot)
				{
					this.stock.localEulerAngles = new Vector3(0f, this._rotAngle, 0f);
					float num = Mathf.InverseLerp(this.minRot, this.maxRot, this._rotAngle);
				if (fireArm != null)
				{
					if (this.isMinClosed)
					{
						if (num < 0.02f)
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
							this.fireArm.HasActiveShoulderStock = false;
						}
						else if (num > 0.9f)
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
							this.fireArm.HasActiveShoulderStock = true;
						}
						else
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
							this.fireArm.HasActiveShoulderStock = false;
						}
					}
					else if (num < 0.1f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
						this.fireArm.HasActiveShoulderStock = true;
					}
					else if (num > 0.98f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
						this.fireArm.HasActiveShoulderStock = false;
					}
					else
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
						this.fireArm.HasActiveShoulderStock = false;
					}
					if (this.mCurPos == FVRFoldingStockYAxis.StockPos.Open && this.mLastPos != FVRFoldingStockYAxis.StockPos.Open)
					{
						this.fireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen, 1f);
					}
					if (this.mCurPos == FVRFoldingStockYAxis.StockPos.Closed && this.mLastPos != FVRFoldingStockYAxis.StockPos.Closed)
					{
						this.fireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed, 1f);
					}
					this.mLastPos = this.mCurPos;
				}
					return;
				}
		}

		// Token: 0x0400276C RID: 10092
		[FormerlySerializedAs("Root")] public Transform root;

		// Token: 0x0400276D RID: 10093
		[FormerlySerializedAs("Stock")] public Transform stock;

		// Token: 0x0400276E RID: 10094
		private float _rotAngle;

		// Token: 0x0400276F RID: 10095
		public float minRot;

		// Token: 0x04002770 RID: 10096
		public float maxRot;

		// Token: 0x04002771 RID: 10097
		[FormerlySerializedAs("m_curPos")] public FVRFoldingStockYAxis.StockPos mCurPos;

		// Token: 0x04002772 RID: 10098
		[FormerlySerializedAs("m_lastPos")] public FVRFoldingStockYAxis.StockPos mLastPos;

		public bool isMinClosed = true;

		[FormerlySerializedAs("FireArm")] public FVRFireArm fireArm;

		public FVRFireArmAttachment attachment;

		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}
	}
}

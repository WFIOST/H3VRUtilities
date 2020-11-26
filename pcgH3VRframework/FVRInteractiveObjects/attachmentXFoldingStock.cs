using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class attachmentXFoldingStock : FVRFoldingStockXAxis
	{
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - this.Root.position;
			vector = Vector3.ProjectOnPlane(vector, this.Root.right).normalized;
			Vector3 lhs = this.Stock.forward;
			if (this.InvertZRoot)
			{
				lhs = -this.Stock.forward;
			}
			float num = Mathf.Atan2(Vector3.Dot(this.Root.right, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
			num = Mathf.Clamp(num, -10f, 10f);
			this.rotAngle += num;
			this.rotAngle = Mathf.Clamp(this.rotAngle, this.MinRot, this.MaxRot);
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
				this.Stock.localEulerAngles = new Vector3(this.rotAngle, 0f, 0f);
				float num2 = Mathf.InverseLerp(this.MinRot, this.MaxRot, this.rotAngle);
				if (this.EndPiece != null)
				{
					this.EndPiece.localEulerAngles = new Vector3(Mathf.Lerp(this.EndPieceMinRot, this.EndPieceMaxRot, num2), 0f, 0f);
				}
				if (FireArm != null) {
					if (this.isMinClosed)
					{
						if (num2 < 0.02f)
						{
							this.m_curPos = FVRFoldingStockXAxis.StockPos.Closed;
							if (this.DoesToggleStockPoint)
							{
								this.FireArm.HasActiveShoulderStock = false;
							}
						}
						else if (num2 > 0.9f)
						{
							this.m_curPos = FVRFoldingStockXAxis.StockPos.Open;
							if (this.DoesToggleStockPoint)
							{
								this.FireArm.HasActiveShoulderStock = true;
							}
						}
						else
						{
							this.m_curPos = FVRFoldingStockXAxis.StockPos.Mid;
							if (this.DoesToggleStockPoint)
							{
								this.FireArm.HasActiveShoulderStock = false;
							}
						}
					}
					else if (num2 < 0.1f)
					{
						this.m_curPos = FVRFoldingStockXAxis.StockPos.Open;
						if (this.DoesToggleStockPoint)
						{
							this.FireArm.HasActiveShoulderStock = true;
						}
					}
					else if (num2 > 0.98f)
					{
						this.m_curPos = FVRFoldingStockXAxis.StockPos.Closed;
						if (this.DoesToggleStockPoint)
						{
							this.FireArm.HasActiveShoulderStock = false;
						}
					}
					else
					{
						this.m_curPos = FVRFoldingStockXAxis.StockPos.Mid;
						if (this.DoesToggleStockPoint)
						{
							this.FireArm.HasActiveShoulderStock = false;
						}
					}
				}
			}
			if (this.m_curPos == FVRFoldingStockXAxis.StockPos.Open && this.m_lastPos != FVRFoldingStockXAxis.StockPos.Open)
			{
				this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen, 1f);
			}
			if (this.m_curPos == FVRFoldingStockXAxis.StockPos.Closed && this.m_lastPos != FVRFoldingStockXAxis.StockPos.Closed)
			{
				this.FireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed, 1f);
			}
			this.m_lastPos = this.m_curPos;
		}

		public void FixedUpdate()
		{
			if (attachment.curMount != null)
			{
				if (FireArm == null)
				{
					var _firearm = attachment.curMount.Parent.GetComponent<FVRFireArm>();
					FireArm = _firearm;
					Console.WriteLine("attachmentYFoldingStock has connected itself to " + FireArm);
				}
			}
			else if (FireArm != null) { FireArm = null; }
		}

		private float rotAngle;

		public FVRFireArmAttachment attachment;
	}
}

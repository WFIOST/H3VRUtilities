using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	// Token: 0x020003CD RID: 973
	public class attachmentYFoldingStock : FVRInteractiveObject
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
							if (isNotAttachment) this.fireArm.HasActiveShoulderStock = false;
						}
						else if (num > 0.9f)
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
							if (isNotAttachment) this.fireArm.HasActiveShoulderStock = true;
						}
						else
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
							if (isNotAttachment) this.fireArm.HasActiveShoulderStock = false;
						}
					}
					else if (num < 0.1f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
						if (isNotAttachment) this.fireArm.HasActiveShoulderStock = true;
					}
					else if (num > 0.98f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
						if (isNotAttachment) this.fireArm.HasActiveShoulderStock = false;
					}
					else
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
						if (isNotAttachment) this.fireArm.HasActiveShoulderStock = false;
					}
					if (this.mCurPos == FVRFoldingStockYAxis.StockPos.Open && this.mLastPos != FVRFoldingStockYAxis.StockPos.Open && !forBreakOpenFlareGun)
					{
						this.fireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen, 1f);
					}
					if (this.mCurPos == FVRFoldingStockYAxis.StockPos.Closed && this.mLastPos != FVRFoldingStockYAxis.StockPos.Closed)
					{
						this.fireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed, 1f);
						if (forBreakOpenFlareGun)
						{
							flareGun.Latch();
						}
					}
					if (this.mCurPos != FVRFoldingStockYAxis.StockPos.Closed && this.mLastPos == FVRFoldingStockYAxis.StockPos.Closed && forBreakOpenFlareGun)
					{
						this.fireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen, 1f);
						if (forBreakOpenFlareGun)
						{
							flareGun.Unlatch();
						}
					}
					this.mLastPos = this.mCurPos;
				}
				return;
			}
		}

		public void FixedUpdate()
		{
			if (isNotAttachment) return;
			if (attachment.curMount != null)
			{
				if (fireArm == null)
				{
					var _firearm = attachment.curMount.Parent.GetComponent<FVRFireArm>();
					fireArm = _firearm;
					Console.WriteLine("attachmentYFoldingStock has connected itself to " + fireArm);
				}
			}
			else if (fireArm != null) { fireArm = null; }
		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
				Gizmos.DrawWireCube(this.transform.position, Vector3.one * 0.025f);
				Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
				Gizmos.DrawCube(this.transform.position, Vector3.one * 0.025f);
				float num = Time.timeSinceLevelLoad;
				Gizmos.color = new Color(0.1f, 0.7f, 0.9f, Mathf.Clamp01((0.5f - num) * 2f));
				Gizmos.DrawWireCube(this.transform.position, Vector3.one * (0.1f * (num + 0.5f)));
			}
			Vector3 center = base.transform.position + base.transform.up;
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
			Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
			Gizmos.DrawCube(center, Vector3.one * 0.02f);
			Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
			Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(this.minRot, base.transform.right) * base.transform.up);
			Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(-this.maxRot, base.transform.right) * base.transform.up);
		}

		[FormerlySerializedAs("Root")] public Transform root;

		[FormerlySerializedAs("Stock")] public Transform stock;

		private float _rotAngle;

		public float minRot;

		public float maxRot;

		[FormerlySerializedAs("m_curPos")] public FVRFoldingStockYAxis.StockPos mCurPos;

		[FormerlySerializedAs("m_lastPos")] public FVRFoldingStockYAxis.StockPos mLastPos;

		public bool isMinClosed = true;

		[FormerlySerializedAs("FireArm")] [Tooltip("Leave as null if attachment.")] public FVRFireArm fireArm;

		public FVRFireArmAttachment attachment;

		[Header("Alternate Use Settings")]
		[Tooltip("If true, it will not affect the stock point.")]
		public bool isNotAttachment;
		public bool forBreakOpenFlareGun;
		public BreakOpenFlareGun flareGun;

		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}
	}
}

using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class attachmentYFoldingStock : FVRInteractiveObject
	{
		public enum UpRightForward
		{
			Up,
			Down,
			Left,
			Right,
			Forward,
			Backwards
		}

		public UpRightForward DirOfRotation = UpRightForward.Up;

		// Token: 0x0600141B RID: 5147 RVA: 0x00089ED0 File Offset: 0x000882D0
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - this.root.position;
			vector = Vector3.ProjectOnPlane(vector, this.root.up).normalized;
			Vector3 lhs = -this.root.transform.forward;
			this._rotAngle = Mathf.Atan2(Vector3.Dot(this.root.up, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f; if (Mathf.Abs(this._rotAngle - this.minRot) < 5f)
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
							if (!isNotAttachment) this.fireArm.HasActiveShoulderStock = false;
						}
						else if (num > 0.9f)
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
							if (!isNotAttachment)
							this.fireArm.HasActiveShoulderStock = true;
						}
						else
						{
							this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
							if (!isNotAttachment)
							this.fireArm.HasActiveShoulderStock = false;
						}
					}
					else if (num < 0.1f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Open;
						if (!isNotAttachment)
						this.fireArm.HasActiveShoulderStock = true;
					}
					else if (num > 0.98f)
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
						if (!isNotAttachment)
						this.fireArm.HasActiveShoulderStock = false;
					}
					else
					{
						this.mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
						if (!isNotAttachment)
							this.fireArm.HasActiveShoulderStock = false;
					}
					this.mLastPos = this.mCurPos;
				}
				return;
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
				Gizmos.DrawWireCube(stock.transform.position, Vector3.one * 0.025f);
				Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
				Gizmos.DrawCube(stock.transform.position, Vector3.one * 0.025f);
				float num = Time.timeSinceLevelLoad;
				Gizmos.color = new Color(0.1f, 0.7f, 0.9f, Mathf.Clamp01((0.5f - num) * 2f));
				Gizmos.DrawWireCube(stock.transform.position, Vector3.one * (0.1f * (num + 0.5f)));
			}
			Vector3 center = stock.transform.position + -stock.transform.forward;
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
			Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
			Gizmos.DrawCube(center, Vector3.one * 0.02f);
			Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
			var up = this.root.transform.up;
			var down = -this.root.transform.forward;
			switch (DirOfRotation)
			{
				case UpRightForward.Down:
					up = -this.root.transform.up;
					down = this.root.transform.forward;
					break;
				case UpRightForward.Left:
					up = -this.root.transform.right;
					down = this.root.transform.up;
					break;
				case UpRightForward.Right:
					up = this.root.transform.right;
					down = -this.root.transform.up;
					break;
				case UpRightForward.Forward:
					up = this.root.transform.forward;
					down = -this.root.transform.right;
					break;
				case UpRightForward.Backwards:
					up = -this.root.transform.forward;
					down = this.root.transform.right;
					break;
			}
			Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(this.minRot, up) * down);
			Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(-this.maxRot, up) * down);
		}

		[FormerlySerializedAs("Root")] public Transform root;

		[FormerlySerializedAs("Stock")] public Transform stock;



		public float minRot;

		public float maxRot;

		[Tooltip("Default 5; this is the angle diff at which point the stock clamps to closed/open. (AKA: if angle is 4, and minRot is 0, the difference is less than 5, so it just clamps to 0.)")]
		public float clampStrength = 5f;

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

		public float _rotAngle;
		[HideInInspector]
		public Vector3 lhs;

		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}
	}
}

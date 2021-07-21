using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class FeedRamp : MonoBehaviour
	{
		[FormerlySerializedAs("Carrier")] public GameObject carrier;
		public FVRFireArm firearm;
		[FormerlySerializedAs("CarrierDetectDistance")] public float carrierDetectDistance;
		[FormerlySerializedAs("CarrierRots")] public Vector2 carrierRots;
		[FormerlySerializedAs("CarrierComparePoint1")] public Transform carrierComparePoint1;
		[FormerlySerializedAs("CarrierComparePoint2")] public Transform carrierComparePoint2;
		private float _mCurCarrierRot;
		private float _mTarCarrierRot;

		public void Update()
		{
				if (firearm.IsHeld)
				{
					if (firearm.m_hand.OtherHand.CurrentInteractable != null)
					{
						if (firearm.m_hand.OtherHand.CurrentInteractable is FVRFireArmRound)
						{
							float num = Vector3.Distance(firearm.m_hand.OtherHand.CurrentInteractable.transform.position, firearm.GetClosestValidPoint(this.carrierComparePoint1.position, this.carrierComparePoint2.position, firearm.m_hand.OtherHand.CurrentInteractable.transform.position));
							if (num < this.carrierDetectDistance)
							{
								this._mTarCarrierRot = this.carrierRots.y;
							}
							else
							{
								this._mTarCarrierRot = this.carrierRots.x;
							}
						}
						else
						{
							this._mTarCarrierRot = this.carrierRots.x;
						}
					}
					else
					{
						this._mTarCarrierRot = this.carrierRots.x;
					}
				}
				else
				{
					this._mTarCarrierRot = this.carrierRots.x;
				}
				if (Mathf.Abs(this._mCurCarrierRot - this._mTarCarrierRot) > 0.001f)
				{
					this._mCurCarrierRot = Mathf.MoveTowards(this._mCurCarrierRot, this._mTarCarrierRot, 270f * Time.deltaTime);
					this.carrier.transform.localEulerAngles = new Vector3(this._mCurCarrierRot, 0f, 0f);
				}
			}
		}
}

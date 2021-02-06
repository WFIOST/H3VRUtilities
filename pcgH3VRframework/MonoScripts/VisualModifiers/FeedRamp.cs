using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class FeedRamp : MonoBehaviour
	{
		public GameObject Carrier;
		public FVRFireArm firearm;
		public float CarrierDetectDistance;
		public Vector2 CarrierRots;
		public Transform CarrierComparePoint1;
		public Transform CarrierComparePoint2;
		private float m_curCarrierRot;
		private float m_tarCarrierRot;

		public void Update()
		{
				if (firearm.IsHeld)
				{
					if (firearm.m_hand.OtherHand.CurrentInteractable != null)
					{
						if (firearm.m_hand.OtherHand.CurrentInteractable is FVRFireArmRound)
						{
							float num = Vector3.Distance(firearm.m_hand.OtherHand.CurrentInteractable.transform.position, firearm.GetClosestValidPoint(this.CarrierComparePoint1.position, this.CarrierComparePoint2.position, firearm.m_hand.OtherHand.CurrentInteractable.transform.position));
							if (num < this.CarrierDetectDistance)
							{
								this.m_tarCarrierRot = this.CarrierRots.y;
							}
							else
							{
								this.m_tarCarrierRot = this.CarrierRots.x;
							}
						}
						else
						{
							this.m_tarCarrierRot = this.CarrierRots.x;
						}
					}
					else
					{
						this.m_tarCarrierRot = this.CarrierRots.x;
					}
				}
				else
				{
					this.m_tarCarrierRot = this.CarrierRots.x;
				}
				if (Mathf.Abs(this.m_curCarrierRot - this.m_tarCarrierRot) > 0.001f)
				{
					this.m_curCarrierRot = Mathf.MoveTowards(this.m_curCarrierRot, this.m_tarCarrierRot, 270f * Time.deltaTime);
					this.Carrier.transform.localEulerAngles = new Vector3(this.m_curCarrierRot, 0f, 0f);
				}
			}
		}
}

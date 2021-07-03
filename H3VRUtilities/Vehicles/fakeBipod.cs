using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	class fakeBipod : ClosedBoltWeapon
	{
		public override void FVRFixedUpdate()
		{
			if (Bipod != null)
			{
				if (AltGrip != null && Bipod.IsBipodActive)
				{
					Bipod.Deactivate();
				}
				else
				{
					Bipod.UpdateBipod();
				}
			}
			MP.FixedUpdate(Time.deltaTime);
			//FU();
		}

		public void FU2()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (m_timeSinceInQuickbelt < 10f)
			{
				m_timeSinceInQuickbelt += fixedDeltaTime;
			}
			if (m_quickbeltSlot != null)
			{
				m_timeSinceInQuickbelt = 0f;
			}
			if (CheckDestroyTick > 0f)
			{
				CheckDestroyTick -= fixedDeltaTime;
			}
			else
			{
				CheckDestroyTick = UnityEngine.Random.Range(1f, 1.5f);
				if (Transform.position.y < -1000f)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			if (CollisionSound.m_hasCollisionSound && CollisionSound.m_colSoundTick > 0f)
			{
				CollisionSound.m_colSoundTick -= fixedDeltaTime;
			}
			if (IsHeld || QuickbeltSlot != null || IsPivotLocked)
			{
				if (this.RootRigidbody == null)
				{
					this.RecoverRigidbody();
				}
				if (this.UseGrabPointChild && this.UseGripRotInterp && !this.IsAltHeld)
				{
					if (this.Bipod != null && this.Bipod.IsBipodActive)
					{
						this.m_pos_interp_tick = 1f;
					}
					else if (this.m_pos_interp_tick < 1f)
					{
						this.m_pos_interp_tick += fixedDeltaTime * this.PositionInterpSpeed;
					}
					else
					{
						this.m_pos_interp_tick = 1f;
					}
					if (this.Bipod != null && this.Bipod.IsBipodActive)
					{
						this.m_rot_interp_tick = 1f;
					}
					else if (this.m_rot_interp_tick < 1f)
					{
						this.m_rot_interp_tick += fixedDeltaTime * this.RotationInterpSpeed;
					}
					else
					{
						this.m_rot_interp_tick = 1f;
					}
				}
				Vector3 vector;
				Quaternion lhs;
				Vector3 vector2;
				Quaternion rotation;
				if (this.IsPivotLocked)
				{
					vector = this.m_pivotLockedPos;
					lhs = this.m_pivotLockedRot;
					vector2 = base.transform.position;
					rotation = base.transform.rotation;
				}
				else
				{
					vector = this.GetPosTarget();
					lhs = this.GetRotTarget();
					vector2 = this.GetGrabPos();
					rotation = this.GetGrabRot();
				}
				Vector3 vector3 = vector - vector2;
				Quaternion b = lhs * Quaternion.Inverse(rotation);
				bool flag = true;
				if (this.IsPivotLocked)
				{
					vector3 = vector - vector2;
					b = lhs * Quaternion.Inverse(rotation);
				}
				else if (((this.AltGrip != null && this.AltGrip.FunctionalityEnabled) || flag) && !GM.Options.ControlOptions.UseGunRigMode2)
				{
					Vector3 position;
					Vector3 vector4;
					if (this.AltGrip != null)
					{
						//get world position of foregrip hand
						position = this.AltGrip.GetPalmPos(this.m_doesDirectParent);
						//get the position of the foregrip's grab position relative to this object
						vector4 = base.transform.InverseTransformPoint(this.AltGrip.PoseOverride.position);
					}
					else //bipod code
					{
						position = this.Bipod.GetOffsetSavedWorldPoint();
						vector4 = base.transform.InverseTransformPoint(this.Bipod.GetBipodRootWorld());
					}
					//get position of foregrip hand relative to this object
					Vector3 vector5 = base.transform.InverseTransformPoint(position);
					//get position of this object's grab position relative to this object
					Vector3 vector6 = base.transform.InverseTransformPoint(this.PoseOverride.position);
					//get the larger value between the local grab position of this gun's z value + 0.05,
					//or the local position of the foregrip hand, store as z

					//effectively, this returns the position of the foregrip hand relative to this object
					//unless the foregrip hand is behind the main hand's position (with some give, being 0.05)

					//reworded again, return what is infront of the other - the foregrip hand or the main hand
					float z = Mathf.Max(this.PoseOverride.localPosition.z + 0.05f, vector5.z);


					Vector3 position2 = new Vector3
					(vector5.x  //x difference between the foregrip hand position
					- vector4.x,//and the foregrip's grab position
					vector5.y   //y difference between the foregrip hand position
					- vector4.y,//and the foregrip's grab position
					z); //return what is more forward - foregrip hand or main hand
						//reworded, this returns the x/y difference between the foregrip hand and foregrip pos; with the
						//z position being the position of the foregrip hand

					//get the world position of position2
					Vector3 vector7 = base.transform.TransformPoint(position2);

					//get the cross product of
					Vector3 vector8 = Vector3.Cross(
					//lhs being
					vector7 //the difference between the foregrip hand pos and foregrip grab position in the world
					- base.transform.position, //minus this object's position
											   //rhs being
					this.m_hand.transform.right); //the X direction of the hand

					if (flag) //bipod shit
					{
						Vector3 from = Vector3.ProjectOnPlane(vector8, base.transform.forward);
						Vector3 vector9 = Vector3.ProjectOnPlane(Vector3.up, base.transform.forward);
						float num = Vector3.Angle(from, vector9);
						float t = Mathf.Clamp(num - 20f, 0f, 30f) * 0.1f;
						vector8 = Vector3.Slerp(vector9, vector8, t);
					}

					//lhs = Quaternion.LookRotation((vector7- base.transform.position).normalized, vector8)* this.PoseOverride.localRotation;
					//create rotation where
					lhs = Quaternion.LookRotation(
					//the forward vector is
					(vector7 //the cross product vector7
					- base.transform.position) //minus this object's location
					.normalized //normalized
								//and the upward vector is
					, vector8) //the cross product
					* this.PoseOverride.localRotation; //fuckery based of the poseoverride rotation, idk

					b = lhs //get that rotation
					* Quaternion.Inverse(rotation); //the inverse of this object's rotation
													//not even sure what b does.

					//virtualstock code to shoulder
					if (!flag && GM.Options.ControlOptions.UseVirtualStock && this is FVRFireArm && (this as FVRFireArm).HasActiveShoulderStock && (this as FVRFireArm).StockPos != null)
					{
						FVRFireArm fvrfireArm = this as FVRFireArm;
						Vector3 vector10 = fvrfireArm.transform.InverseTransformPoint(fvrfireArm.StockPos.position);
						float num2 = Mathf.Abs(vector10.z - position2.z) - (this as FVRFireArm).GetRecoilZ();
						Vector3 vector11 = fvrfireArm.transform.InverseTransformPoint(vector2);
						float d = Mathf.Abs(vector10.z - vector11.z) - (this as FVRFireArm).GetRecoilZ();
						Transform head = GM.CurrentPlayerBody.Head;
						Vector3 vector12 = head.transform.InverseTransformPoint(vector7);
						Vector3 vector13 = head.transform.InverseTransformPoint(vector);
						Vector3 position3 = GM.CurrentPlayerBody.Head.position - GM.CurrentPlayerBody.Head.forward * 0.1f - GM.CurrentPlayerBody.Head.up * 0.05f;
						Vector3 position4 = head.transform.InverseTransformPoint(position3);
						Vector3 vector14 = GM.CurrentPlayerBody.Head.transform.InverseTransformPoint(vector);
						position4.x += vector14.x;
						position4.y += vector14.y + 0.05f;
						Vector3 vector15 = head.TransformPoint(position4);
						Vector3 normalized = (vector7 - vector15).normalized;
						Vector3 a = vector15 + normalized * d;
						Vector3 a2 = a - vector2;
						Quaternion lhs2 = Quaternion.LookRotation((vector7 - vector15).normalized, vector8) * this.PoseOverride.localRotation;
						Quaternion a3 = lhs2 * Quaternion.Inverse(rotation);
						float num3 = Vector3.Distance(head.position, vector);
						num3 = Mathf.Clamp(num3 - 0.1f, 0f, 1f);
						float t2 = num3 * 5f;
						vector3 = Vector3.Lerp(a2, vector3, t2);
						b = Quaternion.Slerp(a3, b, t2);
					}
				}
				float d3 = 1f;
				float num6;
				Vector3 a7;
				b.ToAngleAxis(out num6, out a7);
				if (num6 > 180f)
				{
					num6 -= 360f;
				}
				if (num6 != 0f)
				{
					Vector3 target = fixedDeltaTime * num6 * a7 * this.AttachedRotationMultiplier;
					this.RootRigidbody.angularVelocity = d3 * Vector3.MoveTowards(this.RootRigidbody.angularVelocity, target, this.AttachedRotationFudge * Time.fixedDeltaTime);
					if (this.UseSecondStepRotationFiltering)
					{
						float num7 = Mathf.Clamp(this.RootRigidbody.angularVelocity.magnitude * 0.35f, 0f, 1f);
						num7 *= num7;
						this.RootRigidbody.angularVelocity *= num7;
					}
				}
				Vector3 target2 = vector3 * this.AttachedPositionMultiplier * fixedDeltaTime;
				this.RootRigidbody.velocity = Vector3.MoveTowards(this.RootRigidbody.velocity, target2, this.AttachedPositionFudge * fixedDeltaTime);
			}

		}
	}
}

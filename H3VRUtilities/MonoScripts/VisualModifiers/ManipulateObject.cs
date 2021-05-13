using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class ManipulateObject : MonoBehaviour
	{
		public enum dirtype
		{
			x = 0,
			y = 1,
			z = 2,
			w = 3
		}
		public enum transformtype
		{
			position,
			rotation,
			scale,
			quaternion,
			quaternionPresentedEuler
		}
		[Header("Object Being Observed")]
		public GameObject ObservedObject;
		public dirtype DirectionOfObservation;
		public transformtype TransformationTypeOfObservedObject;
		public float StartOfObservation;
		public float StopOfObservation;

		[Header("Object Being Affected")]
		public GameObject AffectedObject;
		public dirtype DirectionOfAffection;
		public transformtype TransformationTypeOfAffectedObject;
		public float StartOfAffected;
		public float StopOfAffected;

		//[Header("Debug Values")]
		private float observationpoint;
		private float invertlerp;
		private float lerppoint;
		private float wiggleroom = 0.05f;

		[Header("Special Observations")]
		[Tooltip("When the observed object reaches or exceeds the stopofobservation, the affected object will snap back to the startofaffected, and will only reset when the observed object reaches the startofobserved.")]

		[Header("Snap Forwards - Not Working")]
		public bool SnapForwardsOnMax;
		private bool SnappedForwards;
		private bool starttostopincreasesObservation;
		private bool starttostopincreasesAffected;

		[Header("Move On Touch Pad")]
		public bool ReadHandTouchpadMovement;
		public bool IsToggle;
		private bool p_isCurrentlyOn;
		public FVRPhysicalObject ItemToReadFrom;
		public H3VRUtilsMagRelease.TouchpadDirType DirToRead;
		public enum StreamlinedDirType
		{
			AX_Button,
			BY_Button,
			Trigger
		}
		public StreamlinedDirType DirToReadStreamlined;

		[Header("Move On Mag Loaded")]
		public bool ReadIfGunIsLoaded;
		public FVRFireArm FirearmToReadFrom;

		[Header("Move If Bolt Locked - Closed Bolt Only")]
		public bool ReadIfBoltIsLocked;
		public ClosedBolt BoltToReadFrom;

		public void Update()
		{
			invertlerp = 0;
			//define which is farther from the centre
			if (!(ReadHandTouchpadMovement || ReadIfGunIsLoaded || ReadIfBoltIsLocked))
			{
				if (StartOfObservation < StopOfObservation) starttostopincreasesObservation = true;
				if (StartOfAffected < StopOfAffected) starttostopincreasesAffected = true;

				switch (TransformationTypeOfObservedObject)
				{
					case transformtype.position:
						observationpoint = ObservedObject.transform.localPosition[(int)DirectionOfObservation];
						break;
					case transformtype.rotation:
						observationpoint = ObservedObject.transform.localEulerAngles[(int)DirectionOfObservation];
						break;
					case transformtype.scale:
						observationpoint = ObservedObject.transform.localScale[(int)DirectionOfObservation];
						break;
					case transformtype.quaternion:
						observationpoint = ObservedObject.transform.localRotation[(int)DirectionOfObservation];
						break;
					case transformtype.quaternionPresentedEuler:
						observationpoint = ObservedObject.transform.localRotation[(int)DirectionOfObservation] * 180;
						break;
				}

				invertlerp = Mathf.InverseLerp(StartOfObservation, StopOfObservation, observationpoint);
			}

			if (SnapForwardsOnMax)
			{
				//SnapForwardsOnMax test
				if (starttostopincreasesObservation)
				{
					if (observationpoint >= StopOfObservation - wiggleroom)
					{
						SnappedForwards = true;
					}
				}
				else { if (observationpoint <= StopOfObservation + wiggleroom) SnappedForwards = true; }

				if (starttostopincreasesObservation) { if (observationpoint <= StartOfObservation + wiggleroom) SnappedForwards = false; }
				else { if (observationpoint >= StartOfObservation - wiggleroom) SnappedForwards = false; }
				if (SnappedForwards == true) { invertlerp = 0; }
				//end test
			}

			//SpecialFX - TouchpadDir
			if (ReadHandTouchpadMovement)
			{
				Vector2 dir = Vector2.up;
				bool isTrigger = false;
				if (DirToRead == H3VRUtilsMagRelease.TouchpadDirType.Up) dir = Vector2.up;
				if (DirToRead == H3VRUtilsMagRelease.TouchpadDirType.Down) dir = Vector2.down;
				if (DirToRead == H3VRUtilsMagRelease.TouchpadDirType.Left) dir = Vector2.left;
				if (DirToRead == H3VRUtilsMagRelease.TouchpadDirType.Right) dir = Vector2.right;
				if (DirToRead == H3VRUtilsMagRelease.TouchpadDirType.Trigger) isTrigger = true;

				if (ItemToReadFrom.m_hand != null)
				{
					if (!ItemToReadFrom.m_hand.IsInStreamlinedMode)
					{
						bool pressed = ItemToReadFrom.m_hand.Input.TouchpadPressed;
						if (IsToggle) pressed = ItemToReadFrom.m_hand.Input.TouchpadDown;

						if ((Vector2.Angle(ItemToReadFrom.m_hand.Input.TouchpadAxes, dir) <= 45f && ItemToReadFrom.m_hand.Input.TouchpadAxes.magnitude > 0.4f && pressed))
						{
							p_isCurrentlyOn = !p_isCurrentlyOn;
							invertlerp = 1;
						}



						if (isTrigger)
						{
							if (ItemToReadFrom.m_hand.Input.TriggerPressed)
							{
								invertlerp = 1;
							}
							if(ItemToReadFrom.m_hand.Input.TriggerDown && IsToggle)
							{
								p_isCurrentlyOn = !p_isCurrentlyOn;
							}
						}
					}
					else
					{
						switch (DirToReadStreamlined)
						{
							case StreamlinedDirType.AX_Button:
								if (ItemToReadFrom.m_hand.Input.AXButtonPressed)
								{
									p_isCurrentlyOn = !p_isCurrentlyOn;
									invertlerp = 1;
								}
								break;
							case StreamlinedDirType.BY_Button:
								if (ItemToReadFrom.m_hand.Input.BYButtonPressed)
								{
									p_isCurrentlyOn = !p_isCurrentlyOn;
									invertlerp = 1;
								}
								break;
							case StreamlinedDirType.Trigger:
								if (ItemToReadFrom.m_hand.Input.TriggerDown)
								{
									p_isCurrentlyOn = !p_isCurrentlyOn;
									invertlerp = 1;
								}
								break;
						}
					}

					if (IsToggle)
					{ //bool to int
						if (p_isCurrentlyOn)
							invertlerp = 1;
						else
							invertlerp = 0;
					}
				}
			}

			//EndSpecialFX - TouchpadDir
			//SpecialFX - GunLoaded
			if (ReadIfGunIsLoaded)
			{
				if (FirearmToReadFrom.Magazine != null)
				{
					invertlerp = 1;
				}
				else
				{
					invertlerp = 0;
				}
			}
			//EndSpecialFX - GunLoaded
			//SpecialFX - BoltLocked
			if (ReadIfBoltIsLocked)
			{
				if (BoltToReadFrom.CurPos == ClosedBolt.BoltPos.Locked)
				{
					invertlerp = 1;
				}
				else
				{
					invertlerp = 0;
				}
			}
			//EndSpecialFX - BoltLocked

			lerppoint = Mathf.Lerp(StartOfAffected, StopOfAffected, invertlerp);



			Vector3 v3;

			switch (TransformationTypeOfAffectedObject)
			{
				case transformtype.position:
					v3 = AffectedObject.transform.localPosition;
					v3[(int)DirectionOfAffection] = lerppoint;
					AffectedObject.transform.localPosition = v3;
					break;
				case transformtype.rotation:
					v3 = AffectedObject.transform.localEulerAngles;
					v3[(int)DirectionOfAffection] = lerppoint;
					AffectedObject.transform.localEulerAngles = v3;
					break;
				case transformtype.scale:
					v3 = AffectedObject.transform.localScale;
					v3[(int)DirectionOfAffection] = lerppoint;
					AffectedObject.transform.localScale = v3;
					break;
				case transformtype.quaternion:
					Quaternion qt = AffectedObject.transform.rotation;
					qt[(int)DirectionOfAffection] = lerppoint;
					AffectedObject.transform.localRotation = qt;
					break;
				case transformtype.quaternionPresentedEuler:
					v3 = AffectedObject.transform.localEulerAngles;
					v3[(int)DirectionOfAffection] = lerppoint;
					AffectedObject.transform.localEulerAngles = v3;
					break;
			}
		}
	}
}

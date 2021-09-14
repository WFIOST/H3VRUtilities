using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Valve.VR.InteractionSystem;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	public class ManipulateObject : MonoBehaviour
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
		[HideInInspector]
		public float observationpoint;
		[HideInInspector]
		public float invertlerp;
		[HideInInspector]
		public float lerppoint;
		[HideInInspector]
		public float wiggleroom = 0.05f;
		private float rememberLerpPoint = -999f;

		[Header("Special Observations")]
		[Tooltip(
			"When the observed object reaches or exceeds the stopofobservation, the affected object will snap back to the startofaffected, and will only reset when the observed object reaches the startofobserved.")]
		[Header("Snap Forwards")]
		public bool SnapForwardsOnMax;
		public bool SnappedForwards;
		public float SnapBackAt;
		[Tooltip("If off, it will 'unsnap' when under the bounds of Start Of Observation, Stop Of Observation, and Snap Back At. When on, it will unsnap when above all of those.")]
		public bool ResetIfOverBounds;
		[Tooltip("When on, it will lock when at its max, rather than snapping forward.")]
		public bool LockForward;

		[Header("Move On Touch Pad")]
		public bool ReadHandTouchpadMovement;
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

		[Header("Move If Specific Attachment Attached")]
		public bool MoveIfSpecificAttachmentAttached;
		public List<string> AttachmentIDs;
		public FVRFireArmAttachmentMount AttachmentMount;
		private int rememberAttached;
		private float lastDecision;

		[Header("Move If Object Held")]
		public bool MoveIfObjectHeld;
		public FVRPhysicalObject HeldObject;
		private bool _isObservedObjectNotNull;

		[Header("Move If Grenade Armed")]
		public bool MoveIfGrenadeArmed;
		public PinnedGrenade grenade;

		[Header("Move If Disabled")]
		public bool MoveIfDisabled;

		[Header("Special Affected Things")]
		[Header("Move Attached Items")]
		public bool MoveAttachedItems;

		[Header("Disable If Moved")]
		public bool DisableIfMoved;
		[Tooltip("The percentage (from 0-1, not 0-100) at which point it disables")]
		public float percentageCutoff;

		[Tooltip("NOTE: THIS ONLY APPLIES TO THE FIRST ATTACHMENT IN THE MOUNT.")]
		public FVRFireArmAttachmentMount MAImount;

			private void Start()
		{
			_isObservedObjectNotNull = ObservedObject != null;
		}

		public void Update()
		{
			invertlerp = 0;
			//define which is farther from the centre
			if (_isObservedObjectNotNull)
			{
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
				if (observationpoint <= Math.Min(StopOfObservation, Math.Min(StartOfObservation, SnapBackAt)) || observationpoint >= Math.Max(StopOfObservation, Math.Max(StartOfObservation, SnapBackAt)))
				{
					SnappedForwards = true;
				}

				if (SnappedForwards)
				{
					invertlerp = 0;
				}

				if ((observationpoint <= Math.Min(StopOfObservation, Math.Min(StartOfObservation, SnapBackAt)) && !ResetIfOverBounds) ||
					(observationpoint >= Math.Max(StopOfObservation, Math.Max(StartOfObservation, SnapBackAt)) && ResetIfOverBounds))
				{
					SnappedForwards = false;
				}
				if (SnappedForwards)
				{
					if (LockForward) invertlerp = 1;
				}
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

						if ((Vector2.Angle(ItemToReadFrom.m_hand.Input.TouchpadAxes, dir) <= 45f && ItemToReadFrom.m_hand.Input.TouchpadAxes.magnitude > 0.4f && ItemToReadFrom.m_hand.Input.TouchpadPressed))
						{
							invertlerp = 1;
						}
						if (isTrigger)
						{
							if (ItemToReadFrom.m_hand.Input.TriggerDown)
							{
								invertlerp = 1;
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
									invertlerp = 1;
								}
								break;
							case StreamlinedDirType.BY_Button:
								/*if (ItemToReadFrom.m_hand.Input.BYButtonPressed)
								{
									invertlerp = 1;
								}*/
								break;
							case StreamlinedDirType.Trigger:
								if (ItemToReadFrom.m_hand.Input.TriggerDown)
								{
									invertlerp = 1;
								}
								break;
						}
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

			//SpecialFX - MoveIfSpecificAttachmentAttached
			if (MoveIfSpecificAttachmentAttached)
			{
				//is this slow af? yes. do i give a shit? not really. i cant make it fucking work because i'm incompetent
				//if (rememberAttached != AttachmentMount.AttachmentsList.Count)
				//{
					//bool found = false;
					foreach (var attachment in AttachmentMount.AttachmentsList)
					{
						foreach (var attachmentID in AttachmentIDs)
						{
							if (attachment.ObjectWrapper.ItemID == attachmentID)
							{
								invertlerp = 1;
								//found = true;
								break;
							}
						}
					}
					//if (found) { lastDecision = 0; } else { lastDecision = 1; }
				//}
				//else
				//{
				//	invertlerp = lastDecision;
				//}

				//rememberAttached = AttachmentMount.AttachmentsList.Count;
			}
			//EndSpecialFX - MoveIfSpecificAttachmentAttached
			
			//StartSpecialObservations - MoveIfHeld
			if (MoveIfObjectHeld)
			{
				if (HeldObject.m_hand != null)
					invertlerp = 1;
				else
					invertlerp = 0;
			}
			//EndSpecialObservations - MoveIfHeld
			
			//StartSpecialObservations - MoveIfGrenadeArmed
			if (MoveIfGrenadeArmed)
			{
				if (grenade.m_isLeverReleased)
					invertlerp = 1;
				else
					invertlerp = 0;
			}
			
			//Start Special Observations - Move If Disabled
			if (MoveIfDisabled)
			{
				if (!ObservedObject.activeSelf)
				{
					invertlerp = 1;
				}
				else invertlerp = 0;
			}

			lerppoint = Mathf.Lerp(StartOfAffected, StopOfAffected, invertlerp);

			Vector3 v3;

			//make sure lerp isnt same
			if (Math.Abs(rememberLerpPoint - lerppoint) < float.Epsilon) return;
			rememberLerpPoint = lerppoint;
			
			//StartSpecialAffected - MoveAttachedObject
			if (MoveAttachedItems)
			{
				if (MAImount.HasAttachmentsOnIt())
				{
					AffectedObject = MAImount.AttachmentsList[0].gameObject;
				}
				else
				{
					AffectedObject = null;
				}
			}
			//EndSpecialAffect - MoveAttachedObject
			
			//StartSpecialAffected - DisableOnMoved
			if (DisableIfMoved)
			{
				if (invertlerp >= percentageCutoff)
				{
					AffectedObject.SetActive(false);
				} else AffectedObject.SetActive(true);
			}
			//EndSpecialAffect - DisabledOnMoved

			if (AffectedObject != null)
			{
				switch (TransformationTypeOfAffectedObject)
				{
					case transformtype.position:
						v3 = AffectedObject.transform.localPosition;
						v3[(int) DirectionOfAffection] = lerppoint;
						AffectedObject.transform.localPosition = v3;
						break;
					case transformtype.rotation:
						v3 = AffectedObject.transform.localEulerAngles;
						v3[(int) DirectionOfAffection] = lerppoint;
						AffectedObject.transform.localEulerAngles = v3;
						break;
					case transformtype.scale:
						v3 = AffectedObject.transform.localScale;
						v3[(int) DirectionOfAffection] = lerppoint;
						AffectedObject.transform.localScale = v3;
						break;
					case transformtype.quaternion:
						Quaternion qt = AffectedObject.transform.rotation;
						qt[(int) DirectionOfAffection] = lerppoint;
						AffectedObject.transform.localRotation = qt;
						break;
					case transformtype.quaternionPresentedEuler:
						v3 = AffectedObject.transform.localEulerAngles;
						v3[(int) DirectionOfAffection] = lerppoint;
						AffectedObject.transform.localEulerAngles = v3;
						break;
				}
			}
		}
	}
}

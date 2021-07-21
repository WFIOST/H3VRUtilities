using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class ManipulateObject : MonoBehaviour
	{
		public enum Dirtype
		{
			X = 0,
			Y = 1,
			Z = 2,
			W = 3
		}
		public enum Transformtype
		{
			Position,
			Rotation,
			Scale,
			Quaternion,
			QuaternionPresentedEuler
		}
		[FormerlySerializedAs("ObservedObject")] [Header("Object Being Observed")]
		public GameObject observedObject;
		[FormerlySerializedAs("DirectionOfObservation")] public Dirtype directionOfObservation;
		[FormerlySerializedAs("TransformationTypeOfObservedObject")] public Transformtype transformationTypeOfObservedObject;
		[FormerlySerializedAs("StartOfObservation")] public float startOfObservation;
		[FormerlySerializedAs("StopOfObservation")] public float stopOfObservation;

		[FormerlySerializedAs("AffectedObject")] [Header("Object Being Affected")]
		public GameObject affectedObject;
		[FormerlySerializedAs("DirectionOfAffection")] public Dirtype directionOfAffection;
		[FormerlySerializedAs("TransformationTypeOfAffectedObject")] public Transformtype transformationTypeOfAffectedObject;
		[FormerlySerializedAs("StartOfAffected")] public float startOfAffected;
		[FormerlySerializedAs("StopOfAffected")] public float stopOfAffected;

		//[Header("Debug Values")]
		[HideInInspector]
		public float observationpoint;
		[HideInInspector]
		public float invertlerp;
		[HideInInspector]
		public float lerppoint;
		[HideInInspector]
		public float wiggleroom = 0.05f;
		private float _rememberLerpPoint = -999f;

		[FormerlySerializedAs("SnapForwardsOnMax")]
		[Header("Special Observations")]
		[Tooltip("When the observed object reaches or exceeds the stopofobservation, the affected object will snap back to the startofaffected, and will only reset when the observed object reaches the startofobserved.")]

		[Header("Snap Forwards")]
		public bool snapForwardsOnMax;
		[FormerlySerializedAs("SnappedForwards")] public bool snappedForwards;
		[FormerlySerializedAs("SnapBackAt")] public float snapBackAt;
		[FormerlySerializedAs("ResetIfOverBounds")] [Tooltip("If off, it will 'unsnap' when under the bounds of Start Of Observation, Stop Of Observation, and Snap Back At. When on, it will unsnap when above all of those.")]
		public bool resetIfOverBounds;
		[FormerlySerializedAs("LockForward")] [Tooltip("When on, it will lock when at its max, rather than snapping forward.")]
		public bool lockForward;

		[FormerlySerializedAs("ReadHandTouchpadMovement")] [Header("Move On Touch Pad")]
		public bool readHandTouchpadMovement;
		[FormerlySerializedAs("ItemToReadFrom")] public FVRPhysicalObject itemToReadFrom;
		[FormerlySerializedAs("DirToRead")] public H3VRUtilsMagRelease.TouchpadDirType dirToRead;
		public enum StreamlinedDirType
		{
			AxButton,
			ByButton,
			Trigger
		}
		[FormerlySerializedAs("DirToReadStreamlined")] public StreamlinedDirType dirToReadStreamlined;

		[FormerlySerializedAs("ReadIfGunIsLoaded")] [Header("Move On Mag Loaded")]
		public bool readIfGunIsLoaded;
		[FormerlySerializedAs("FirearmToReadFrom")] public FVRFireArm firearmToReadFrom;

		[FormerlySerializedAs("ReadIfBoltIsLocked")] [Header("Move If Bolt Locked - Closed Bolt Only")]
		public bool readIfBoltIsLocked;
		[FormerlySerializedAs("BoltToReadFrom")] public ClosedBolt boltToReadFrom;

		[FormerlySerializedAs("MoveIfSpecificAttachmentAttached")] [Header("Move If Specific Attachment Attached")]
		public bool moveIfSpecificAttachmentAttached;
		[FormerlySerializedAs("AttachmentIDs")] public List<string> attachmentIDs;
		[FormerlySerializedAs("AttachmentMount")] public FVRFireArmAttachmentMount attachmentMount;
		private int _rememberAttached;
		private float _lastDecision;

		public void Update()
		{
			invertlerp = 0;
			//define which is farther from the centre
			if (!(readHandTouchpadMovement || readIfGunIsLoaded || readIfBoltIsLocked || moveIfSpecificAttachmentAttached))
			{

				switch (transformationTypeOfObservedObject)
				{
					case Transformtype.Position:
						observationpoint = observedObject.transform.localPosition[(int)directionOfObservation];
						break;
					case Transformtype.Rotation:
						observationpoint = observedObject.transform.localEulerAngles[(int)directionOfObservation];
						break;
					case Transformtype.Scale:
						observationpoint = observedObject.transform.localScale[(int)directionOfObservation];
						break;
					case Transformtype.Quaternion:
						observationpoint = observedObject.transform.localRotation[(int)directionOfObservation];
						break;
					case Transformtype.QuaternionPresentedEuler:
						observationpoint = observedObject.transform.localRotation[(int)directionOfObservation] * 180;
						break;
				}

				invertlerp = Mathf.InverseLerp(startOfObservation, stopOfObservation, observationpoint);
			}

			if (snapForwardsOnMax)
			{
				if (observationpoint <= Math.Min(stopOfObservation, Math.Min(startOfObservation, snapBackAt)) || observationpoint >= Math.Max(stopOfObservation, Math.Max(startOfObservation, snapBackAt)))
				{
					snappedForwards = true;
				}

				if (snappedForwards)
				{
					invertlerp = 0;
				}

				if ((observationpoint <= Math.Min(stopOfObservation, Math.Min(startOfObservation, snapBackAt)) && !resetIfOverBounds) ||
					(observationpoint >= Math.Max(stopOfObservation, Math.Max(startOfObservation, snapBackAt)) && resetIfOverBounds))
				{
					snappedForwards = false;
				}
				if (snappedForwards)
				{
					if (lockForward) invertlerp = 1;
				}
			}

			//SpecialFX - TouchpadDir
			if (readHandTouchpadMovement)
			{
				Vector2 dir = Vector2.up;
				bool isTrigger = false;
				if (dirToRead == H3VRUtilsMagRelease.TouchpadDirType.Up) dir = Vector2.up;
				if (dirToRead == H3VRUtilsMagRelease.TouchpadDirType.Down) dir = Vector2.down;
				if (dirToRead == H3VRUtilsMagRelease.TouchpadDirType.Left) dir = Vector2.left;
				if (dirToRead == H3VRUtilsMagRelease.TouchpadDirType.Right) dir = Vector2.right;
				if (dirToRead == H3VRUtilsMagRelease.TouchpadDirType.Trigger) isTrigger = true;

				if (itemToReadFrom.m_hand != null)
				{
					if (!itemToReadFrom.m_hand.IsInStreamlinedMode)
					{

						if ((Vector2.Angle(itemToReadFrom.m_hand.Input.TouchpadAxes, dir) <= 45f && itemToReadFrom.m_hand.Input.TouchpadAxes.magnitude > 0.4f && itemToReadFrom.m_hand.Input.TouchpadPressed))
						{
							invertlerp = 1;
						}
						if (isTrigger)
						{
							if (itemToReadFrom.m_hand.Input.TriggerDown)
							{
								invertlerp = 1;
							}
						}
					}
					else
					{
						switch (dirToReadStreamlined)
						{
							case StreamlinedDirType.AxButton:
								if (itemToReadFrom.m_hand.Input.AXButtonPressed)
								{
									invertlerp = 1;
								}
								break;
							case StreamlinedDirType.ByButton:
								/*if (ItemToReadFrom.m_hand.Input.BYButtonPressed)
								{
									invertlerp = 1;
								}*/
								break;
							case StreamlinedDirType.Trigger:
								if (itemToReadFrom.m_hand.Input.TriggerDown)
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
			if (readIfGunIsLoaded)
			{
				if (firearmToReadFrom.Magazine != null)
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
			if (readIfBoltIsLocked)
			{
				if (boltToReadFrom.CurPos == ClosedBolt.BoltPos.Locked)
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
			if (moveIfSpecificAttachmentAttached)
			{
				//is this slow af? yes. do i give a shit? not really. i cant make it fucking work because i'm incompetent
				//if (rememberAttached != AttachmentMount.AttachmentsList.Count)
				//{
					//bool found = false;
					foreach (FVRFireArmAttachment attachment in attachmentMount.AttachmentsList)
					{
						foreach (string attachmentID in attachmentIDs)
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


			lerppoint = Mathf.Lerp(startOfAffected, stopOfAffected, invertlerp);

			Vector3 v3;

			//make sure lerp isnt same
			if (_rememberLerpPoint == lerppoint) return;
			_rememberLerpPoint = lerppoint;

			switch (transformationTypeOfAffectedObject)
			{
				case Transformtype.Position:
					v3 = affectedObject.transform.localPosition;
					v3[(int)directionOfAffection] = lerppoint;
					affectedObject.transform.localPosition = v3;
					break;
				case Transformtype.Rotation:
					v3 = affectedObject.transform.localEulerAngles;
					v3[(int)directionOfAffection] = lerppoint;
					affectedObject.transform.localEulerAngles = v3;
					break;
				case Transformtype.Scale:
					v3 = affectedObject.transform.localScale;
					v3[(int)directionOfAffection] = lerppoint;
					affectedObject.transform.localScale = v3;
					break;
				case Transformtype.Quaternion:
					Quaternion qt = affectedObject.transform.rotation;
					qt[(int)directionOfAffection] = lerppoint;
					affectedObject.transform.localRotation = qt;
					break;
				case Transformtype.QuaternionPresentedEuler:
					v3 = affectedObject.transform.localEulerAngles;
					v3[(int)directionOfAffection] = lerppoint;
					affectedObject.transform.localEulerAngles = v3;
					break;
			}
		}
	}
}

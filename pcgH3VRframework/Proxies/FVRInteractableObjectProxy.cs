using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	public class FVRInteractableObjectProxy : MonoBehaviour
	{
		[NonSerialized]
		public GameObject GameObject;
		[NonSerialized]
		public Transform Transform;
		[NonSerialized]
		private int m_index = -1;
		public static List<FVRInteractiveObject> All = new List<FVRInteractiveObject>();
		[Header("Interactive Object Config")]
		public FVRInteractionControlType ControlType;
		public bool IsSimpleInteract;
		public HandlingGrabType HandlingGrabSound;
		public HandlingReleaseType HandlingReleaseSound;
		public Transform PoseOverride;
		public Transform QBPoseOverride;
		public Transform PoseOverride_Touch;
		public bool UseGrabPointChild;
		public bool UseGripRotInterp;
		public float PositionInterpSpeed = 1f;
		public float RotationInterpSpeed = 1f;
		protected Transform m_grabPointTransform;
		protected float m_pos_interp_tick;
		protected float m_rot_interp_tick;
		public bool EndInteractionIfDistant = true;
		public float EndInteractionDistance = 0.25f;
		[HideInInspector]
		public bool m_hasTriggeredUpSinceBegin;
		protected float triggerCooldown = 0.5f;
		public FVRViveHand m_hand;
		public GameObject UXGeo_Hover;
		public GameObject UXGeo_Held;
		public bool UseFilteredHandTransform;
		public bool UseFilteredHandPosition;
		public bool UseFilteredHandRotation;
		public bool UseSecondStepRotationFiltering;
		protected Quaternion SecondStepFilteredRotation = Quaternion.identity;
		private bool m_isHovered;
		private bool m_isHeld;
		protected Collider[] m_colliders;

		public void byeworld()
		{
			Destroy(this);
		}
	}
}

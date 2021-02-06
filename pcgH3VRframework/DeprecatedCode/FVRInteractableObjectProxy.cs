using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
    //Proxies are bad lmao dont use them
    public class FVRInteractableObjectProxy : MonoBehaviour
    {
        public static List<FVRInteractiveObject> All = new List<FVRInteractiveObject>();

        [Header("Interactive Object Config")] public FVRInteractionControlType ControlType;

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
        public bool EndInteractionIfDistant = true;
        public float EndInteractionDistance = 0.25f;

        [HideInInspector] public bool m_hasTriggeredUpSinceBegin;

        public FVRViveHand m_hand;
        public GameObject UXGeo_Hover;
        public GameObject UXGeo_Held;
        public bool UseFilteredHandTransform;
        public bool UseFilteredHandPosition;
        public bool UseFilteredHandRotation;
        public bool UseSecondStepRotationFiltering;

        [NonSerialized] public GameObject GameObject;

        protected Collider[] m_colliders;
        protected Transform m_grabPointTransform;

        [NonSerialized] private int m_index = -1;

        private bool m_isHeld;
        private bool m_isHovered;
        protected float m_pos_interp_tick;
        protected float m_rot_interp_tick;
        protected Quaternion SecondStepFilteredRotation = Quaternion.identity;

        [NonSerialized] public Transform Transform;

        protected float triggerCooldown = 0.5f;

        public void byeworld()
        {
            Destroy(this);
        }
    }
}
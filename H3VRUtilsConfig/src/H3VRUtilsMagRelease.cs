using System;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
    public class H3VRUtilsMagRelease : FVRInteractiveObject
    {
        public enum TouchpadDirType
        {
            Up,
            Down,
            Left,
            Right,
            Trigger,
            NoDirection
        }

        [FormerlySerializedAs("ClosedBoltReceiver")] public ClosedBoltWeapon closedBoltReceiver;
        [FormerlySerializedAs("OpenBoltWeapon")] public OpenBoltReceiver openBoltWeapon;
        [FormerlySerializedAs("HandgunReceiver")] public Handgun handgunReceiver;
        [FormerlySerializedAs("BoltActionWeapon")] public BoltActionRifle boltActionWeapon;

        [FormerlySerializedAs("WepType")] [HideInInspector] public int wepType;

        [FormerlySerializedAs("DisallowEjection")] [HideInInspector] public bool disallowEjection;

        [FormerlySerializedAs("PressDownToRelease")] public bool pressDownToRelease;
        [FormerlySerializedAs("TouchpadDir")] public TouchpadDirType touchpadDir;

        public Vector2 dir;

        private Collider _col;

        private FVRFireArmMagazine _mag;


        public void Awake()
        {
			Debug.Log("i shouldn't fuckin exist, wtf");
            base.Awake();
            SetWepType();
            _col = GetComponent<Collider>();
        }

        public void SetWepType()
        {
            if (closedBoltReceiver != null) wepType = 1;
            if (openBoltWeapon != null) wepType = 2;
            if (handgunReceiver != null) wepType = 3;
            if (boltActionWeapon != null) wepType = 4;
        }

        public override bool IsInteractable()
        {
            switch (wepType)
            {
                case 1:
                    return !(closedBoltReceiver.Magazine == null);
                case 2:
                    return !(openBoltWeapon.Magazine == null);
                case 3:
                    return !(handgunReceiver.Magazine == null);
                default:
                    return !(boltActionWeapon.Magazine == null);
            }
        }

        public void FvrFixedUpdate()
        {
            base.FVRFixedUpdate();
            dir = Vector2.up;

            //config override
            if (UtilsBepInExLoader.paddleMagReleaseDir.Value != UtilsBepInExLoader.TouchpadDirTypePt.BasedOnWeapon)
                touchpadDir = (TouchpadDirType) (int) UtilsBepInExLoader.paddleMagReleaseDir.Value;

            switch (touchpadDir)
            {
                case TouchpadDirType.Up:
                    dir = Vector2.up;
                    break;
                case TouchpadDirType.Down:
                    dir = Vector2.down;
                    break;
                case TouchpadDirType.Left:
                    dir = Vector2.left;
                    break;
                case TouchpadDirType.Right:
                    dir = Vector2.right;
                    break;
                case TouchpadDirType.Trigger:
                    break;
                case TouchpadDirType.NoDirection:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsSimpleInteract = touchpadDir == TouchpadDirType.Trigger;
            _col.enabled = !disallowEjection;
        }

        public override void BeginInteraction(FVRViveHand hand)
        {
            base.BeginInteraction(hand);
        }

        public override void SimpleInteraction(FVRViveHand hand)
        {
            base.SimpleInteraction(hand);
            if (touchpadDir == TouchpadDirType.Trigger)
                Dropmag(hand);
        }

        public void Dropmag(FVRViveHand hand, bool @override = false)
        {
            if (disallowEjection && !@override) return;
            FVRFireArmMagazine magazine = null;

            switch (wepType)
            {
                case 1:
                    magazine = closedBoltReceiver.Magazine;
                    closedBoltReceiver.ReleaseMag();
                    break;
                case 2:
                    magazine = openBoltWeapon.Magazine;
                    openBoltWeapon.ReleaseMag();
                    break;
                case 3:
                    magazine = handgunReceiver.Magazine;
                    handgunReceiver.ReleaseMag();
                    break;
                case 4:
                    magazine = boltActionWeapon.Magazine;
                    boltActionWeapon.ReleaseMag();
                    break;
            }

            Movemagtohand(hand, magazine);
        }

        public void Movemagtohand(FVRViveHand hand, FVRFireArmMagazine magazine)
        {
            //puts mag in hand
            if (hand != null) hand.ForceSetInteractable(magazine);
            magazine.BeginInteraction(hand);
        }


        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);

            bool flag = false;
            FVRFireArmMagazine prevmag = null;
            if (_mag != null)
            {
                flag = true;
                prevmag = _mag;
            } //check if mag was previously loaded

            switch (wepType)
            {
                case 1:
                    _mag = closedBoltReceiver.Magazine;
                    break;
                case 2:
                    _mag = openBoltWeapon.Magazine;
                    break;
                case 3:
                    _mag = handgunReceiver.Magazine;
                    break;
                case 4:
                    _mag = boltActionWeapon.Magazine;
                    break;
            }

            if (_mag != null)
            {
                bool flag2 = Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown &&
                             hand.Input.TouchpadAxes.magnitude > 0.2f;


                if (
                    !pressDownToRelease //if it's not a paddle release anyway
                    || !UtilsBepInExLoader.paddleMagRelease.Value //if paddle release is disabled
                    || touchpadDir == TouchpadDirType.NoDirection &&
                    !UtilsBepInExLoader.magDropRequiredRelease.Value //if mag drop required and mag drop is disabled
                    || flag2 //if it is enabled, and user is pressing all the right buttons
                    || hand.IsInStreamlinedMode &&
                    hand.Input
                        .AXButtonPressed) //if it is enabled, and user is pressing streamlined button (and is in steamlined mode)
                {
                    if (touchpadDir == TouchpadDirType.NoDirection &&
                        UtilsBepInExLoader.magDropRequiredRelease.Value) return;
                    Dropmag(hand);
                    EndInteraction(hand);
                }
            }
            else
            {
                if (flag) //if mag was previously loaded, but is now not
                    Movemagtohand(hand, prevmag);
                EndInteraction(hand);
            }
        }
    }
}
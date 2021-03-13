using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

		[Header("Special Affects")]
		[Tooltip("When the observed object reaches or exceeds the stopofobservation, the affected object will snap back to the startofaffected, and will only reset when the observed object reaches the startofobserved.")]
		public bool SnapForwardsOnMax;



		private bool SnappedForwards;
		private bool starttostopincreasesObservation;
		private bool starttostopincreasesAffected;

		public void Update()
		{
			//define which is farther from the centre
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

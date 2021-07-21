using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.Weapons
{
	class JungleMag : MonoBehaviour
	{
		//We will break the shackles that are Monarchy.
		public FVRFireArmMagazine masterMag;
		public FVRFireArmMagazine slaveMag;

		public GameObject masterMagGameObject;
		public GameObject slaveMagGameObject;
		[HideInInspector]
		public GameObject parentMag;
		[FormerlySerializedAs("_isMasterMagNotNull")] [HideInInspector]
		public bool isMasterMagNotNull;
		[FormerlySerializedAs("_isSlaveMagNotNull")] [HideInInspector]
		public bool isSlaveMagNotNull;

		private bool _isFireArmNotNull;
		private bool _fireArmNotNull;

		void Start()
		{
			_fireArmNotNull = slaveMag.FireArm != null;
			_isFireArmNotNull = masterMag.FireArm != null;
			parentMag = masterMagGameObject;
		}


		void Update()
		{
			isMasterMagNotNull = _isFireArmNotNull;
			isSlaveMagNotNull = _fireArmNotNull;
			if (parentMag == masterMagGameObject)
			{
				if (isSlaveMagNotNull)
				{
					//prevents master mag from being loaded
					masterMag.DoesDisplayXOscillate = false;
					//unparents slave mag
					
//					slaveMagGameObject.transform.parent = null;

					//sets the parent of master mag to slave mag
//					masterMagGameObject.transform.parent = slaveMagGameObject.transform;
//					parentMag = slaveMagGameObject;
				}
			}
			else
			{
				if (isMasterMagNotNull)
				{
					slaveMag.DoesDisplayXOscillate = false;
//					masterMagGameObject.transform.parent = null;
//					slaveMagGameObject.transform.parent = masterMagGameObject.transform;
//					parentMag = masterMagGameObject;
				}
			}

			if (isMasterMagNotNull || isSlaveMagNotNull) return;
			masterMag.DoesDisplayXOscillate = true;
			slaveMag.DoesDisplayXOscillate = true;
		}
	}
}

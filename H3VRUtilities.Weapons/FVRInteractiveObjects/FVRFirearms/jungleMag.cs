using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Weapons
{
	class jungleMag : MonoBehaviour
	{
		//We will break the shackles that are Monarchy.
		//https://i.redd.it/x5fdxrstkca31.jpg
		public FVRFireArmMagazine masterMag;
		public FVRFireArmMagazine slaveMag;
		[HideInInspector]
		public GameObject parentMag;
		[HideInInspector]
		public bool _isMasterMagNotNull;
		[HideInInspector]
		public bool _isSlaveMagNotNull;
		[HideInInspector]
		public GameObject masterMagGameObject;
		[HideInInspector]
		public GameObject slaveMagGameObject;

		void Start()
		{
			masterMagGameObject = masterMag.GetComponent<GameObject>();
			slaveMagGameObject = slaveMag.GetComponent<GameObject>();
			parentMag = masterMagGameObject;
			_isMasterMagNotNull = masterMag.FireArm != null;
			_isSlaveMagNotNull = slaveMag.FireArm != null;
			
		}
			

		void Update()
		{
			if (parentMag == masterMagGameObject)
			{
				if (_isSlaveMagNotNull)
				{
					masterMag.DoesDisplayXOscillate = false;
					masterMag.transform.SetParent(slaveMag.transform, true);
					parentMag = slaveMagGameObject;
				}
			}
			else
			{
				if (_isMasterMagNotNull)
				{
					slaveMag.DoesDisplayXOscillate = false;
					slaveMag.transform.SetParent(masterMag.transform, true);
					parentMag = masterMagGameObject;
				}
			}

			if (!_isMasterMagNotNull && !_isSlaveMagNotNull)
			{
				masterMag.DoesDisplayXOscillate = true;
				slaveMag.DoesDisplayXOscillate = true;
			}
		}
	}
}

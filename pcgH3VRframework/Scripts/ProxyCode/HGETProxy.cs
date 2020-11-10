using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace pcgH3VRframework
{
	class HGETProxy : MonoBehaviour
	{
		public Handgun proxyhgReceiver;
		void Start()
		{
			var HGET = gameObject.AddComponent<HandgunEjectionTrigger>();
			HGET.hgReceiver = proxyhgReceiver;
		}
	}
}

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
			Console.WriteLine("HGETProxy here to do it's job!");
			var HGET = gameObject.AddComponent<HandgunEjectionTrigger>();
			HGET.hgReceiver = proxyhgReceiver;
			Console.WriteLine("HGETProxy done!");
		}
	}
}

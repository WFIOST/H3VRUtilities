using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using FistVR;

namespace H3VRUtilities.customItems.shotClock
{
	class shotClock : MonoBehaviour
	{
		public string stopclocktextstring;

		public Vector2 startingTimeWindow = new Vector2(2, 5);
		public float stopclock;
		public int shotsfired;

		public GameObject startbutton;
		public GameObject stopbutton;

		public bool isClockOn;
		public bool isInDelayProcess;

		public Text stopclocktext;
		public Text lastshottext;
		public Text shotsfiredtext;
		public Text delaymintext;
		public Text delaymaxtext;

		private float waittime;

		void New()
		{

		}

		void Update()
		{
			if (isClockOn)
			{
				stopclock += Time.deltaTime;
				stopclocktext.text = updateStopClockTextString();
			}
			if (isInDelayProcess)
			{
				stopclock += Time.deltaTime;
				if (stopclock > waittime)
				{
					isInDelayProcess = false;
					StartClock();
				}
			}
			if (isClockOn || isInDelayProcess)
			{
				startbutton.SetActive(false);
				stopbutton.SetActive(true);
			}
			else
			{
				startbutton.SetActive(true);
				stopbutton.SetActive(false);
			}
		}

		public string updateStopClockTextString()
		{
			stopclocktextstring = "";
			var ts = TimeSpan.FromSeconds(stopclock);
			if (ts.Minutes < 10)
			{
				stopclocktextstring += "0";
			}
			stopclocktextstring += ts.Minutes.ToString() + ":";
			if (ts.Seconds < 10)
			{
				stopclocktextstring += "0";
			}
			stopclocktextstring += ts.Seconds.ToString() + ":";
			if (ts.Milliseconds < 100)
			{
				stopclocktextstring += "0";
				if (ts.Milliseconds < 10)
				{
					stopclocktextstring += "0";
				}
			}
			stopclocktextstring += Math.Round((double)ts.Milliseconds, 3).ToString();
			return stopclocktextstring;
		}

		public void startClockProcess()
		{
			waittime = UnityEngine.Random.Range(startingTimeWindow.x, startingTimeWindow.y);
			isInDelayProcess = true;
		}

		public void StartClock()
		{
			isClockOn = true;
			stopclock = 0;
		}

		public void StopClock()
		{
			isClockOn = false;
		}

		public void ShotDetected()
		{
			if (isClockOn)
			{
				var ts = TimeSpan.FromSeconds(stopclock);
				lastshottext.text = updateStopClockTextString();
			}
		}
	}
}

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
		public Vector2 startingTimeWindow = new Vector2(2,5);
		public float stopclock;
		public int shotsfired;

		public int minTime = 2;
		public int maxTime = 5;

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
				var ts = TimeSpan.FromSeconds(stopclock);
				stopclocktext.text = string.Format("{0:00}:{0:00}:{0:00}", ts.TotalMinutes, ts.Seconds, ts.Milliseconds);
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

		public void startClockProcess()
		{
			waittime = UnityEngine.Random.Range((float)minTime, (float)maxTime);
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
				lastshottext.text = string.Format("{0:00}:{0:00}:{0:00}", ts.TotalMinutes, ts.Seconds, ts.Milliseconds);
			}
		}
	}
}

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

		public FVRFireArmChamber[] registery;
		private bool[] registerySpent;

		public Text[] weptext;

		private BoxCollider registertrigger;

		private bool alreadyInRegisteryFlag;

		public enum screen
		{
			shot,
			register,
			delay
		}

		public screen currentScreen;

		public void Awake()
		{
			registery = new FVRFireArmChamber[11];
			registerySpent = new bool[11];
			weptext = new Text[11];
			registertrigger = this.GetComponent<BoxCollider>();
		}

		public void startClockProcess()
		{
			waittime = UnityEngine.Random.Range(startingTimeWindow.x, startingTimeWindow.y);
			isInDelayProcess = true;
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

			for (int i = 0; i <= registery.Length; i++)
			{
				if (registery[i].IsSpent)
				{
					if (!registerySpent[i])
					{
						registerySpent[i] = true;
						ShotDetected();
					}
				}
				else
				{
					registerySpent[i] = false;
				}
			}

			if (currentScreen == screen.register)
			{
				updateRegistery();
			}
		}

		void updateRegistery()
		{
			//get all firearmchambers
			GameObject[] list = GameObject.FindGameObjectsWithTag("FVRFireArmChamber");
			for (int i = 0; i <= list.Length; i++)
			{
				//for every chamber, check if it's inside registertrigger bounds
				if (registertrigger.bounds.Contains(list[i].transform.position))
				{
					//check if the chamber is already loaded into the registery
					for (int io = 0; io <= registery.Length; i++)
					{
						if (registery[io].GameObject = list[i])
						{
							alreadyInRegisteryFlag = true;
						}
					}
					//if it isn't, load it in
					if (!alreadyInRegisteryFlag)
					{
						for (int ip = 0; ip <= registery.Length; i++)
						{
							//get the first null in registery
							if (registery[i] == null)
							{
								registery[i] = list[i].GetComponent<FVRFireArmChamber>();
							}
						}
					}
				}
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

		public void StartClock()
		{
			isClockOn = true;
			stopclock = 0;
		}

		public void StopClock()
		{
			isClockOn = false;
			stopclock = 0;
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

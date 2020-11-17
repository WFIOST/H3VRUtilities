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

		public FVRFireArmChamber test1;
		public FVRFireArmChamber test2;

		private bool[] registerySpent;

		public Text[] weptext;

		public Text weptext0;
		public Text weptext1;
		public Text weptext2;
		public Text weptext3;
		public Text weptext4;
		public Text weptext5;
		public Text weptext6;
		public Text weptext7;
		public Text weptext8;
		public Text weptext9;
		public Text weptext10;
		public Text weptext11;

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

			weptext[0] = weptext0;
			weptext[1] = weptext1;
			weptext[2] = weptext2;
			weptext[3] = weptext3;
			weptext[4] = weptext4;
			weptext[5] = weptext5;
			weptext[6] = weptext6;
			weptext[7] = weptext7;
			weptext[8] = weptext8;
			weptext[9] = weptext9;
			weptext[10] = weptext10;
			weptext[11] = weptext11;
		}

		public void startClockProcess()
		{
			waittime = UnityEngine.Random.Range(startingTimeWindow.x, startingTimeWindow.y);
			isInDelayProcess = true;
		}

		void Update()
		{
			test1 = registery[0];
			test2 = registery[1];

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
						if (registery[io].transform == list[i])
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

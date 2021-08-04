using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


namespace H3VRUtils.UniqueCode
{
	//Bag Of Many Objects
	public class BOMO : FVRPhysicalObject
	{
		public List<GameObject> itemsInTheBag;
		public Text itemsInTheBagText;
		public int maxItems;
		public bool thevoid;
		
		private Random _rnd = new Random();
		private bool _isitemsInTheBagTextNotNull;
		private int deniedObjectTime;

		private void Start()
		{
			_isitemsInTheBagTextNotNull = itemsInTheBagText != null;
		}

		private void OnTriggerStay(Collider other)
		{
			//i can literally hear my cpu burning itself just reading this
			//sorry, oh gods of optimization, for i have made a grave sin
			if (other.gameObject.layer == gameObject.layer && deniedObjectTime >= 50)
			{
				var obj = other.gameObject.GetComponent<FVRPhysicalObject>();
				bool deny = true;
				if (obj.m_hand == null)
				{
					if (obj is FVRFireArmMagazine)
					{
						FVRFireArmMagazine mag = obj as FVRFireArmMagazine;
						if (mag.FireArm == null) deny = false;
					}
					else if (obj is Speedloader)
					{
						deny = false;
					}
					else if (obj is FVRFireArmClip)
					{
						deny = false;
					}
				}

				//if the mag is not spawnlocked nor denied
				if (!obj.m_isSpawnLock && !deny)
				{
					//deny if too many items in the bag
					if (itemsInTheBag.Count < maxItems)
					{
						if (thevoid) //if it's the void, just destroy it
						{
							if (obj.m_hand != null) obj.m_hand.ForceSetInteractable(null);
							Destroy(other.gameObject);
						}
						else //otherwise store it in the bag
						{
							itemsInTheBag.Add(other.gameObject);
							if (obj.m_hand != null) obj.m_hand.ForceSetInteractable(null);
							other.gameObject.SetActive(false);
						}
						SetText();
					}
				}

			}
		}

		private void SetText()
		{
			if (_isitemsInTheBagTextNotNull)
			{
				itemsInTheBagText.text = itemsInTheBag.Count.ToString();
			}
		}

		private void Update()
		{
			if (m_isHardnessed)
			{
				if (m_hand != null)
				{
					if (itemsInTheBag.Count != 0)
					{
						int rand = _rnd.Next(0, itemsInTheBag.Count);
						FVRInteractiveObject obj;
						obj = itemsInTheBag[rand].GetComponent<FVRInteractiveObject>();
						obj.gameObject.SetActive(true);
						m_hand.ForceSetInteractable(obj);
						itemsInTheBag.Remove(itemsInTheBag[rand]);
						SetText();
						deniedObjectTime = 0;
					}
					else m_hand.ForceSetInteractable(null);
				}
			}
		}

		private void FixedUpdate()
		{
			deniedObjectTime++;
		}
	}
}
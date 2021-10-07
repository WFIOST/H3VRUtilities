using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


namespace H3VRUtils.UniqueCode
{
	//Bag Of Many Objects
	public class BOMO : MonoBehaviour
	{
		[Tooltip("If true, will not un-harness when there are no items.")]
		public bool isStatic;
		[Tooltip("For usage as a static object. Puts itself into quickbelt and harnesses.")]
		public FVRQuickBeltSlot qbSlotForStatic;
		[Tooltip("Physical object the BOMO is connected to.")]
		public FVRPhysicalObject physobj;
		[Tooltip("List of all the items in the BOMO.")]
		public List<GameObject> itemsInTheBag;
		[Tooltip("Text that displays the amount of items in the bag. Not necessary.")]
		public Text itemsInTheBagText;
		[Tooltip("Maximum amount of items that can be stored in the BOMO.")]
		public int maxItems = 5;
		[Tooltip("If true, will just nuke any item put into it.")]
		public bool thevoid;
		
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
			deniedObjectTime++;
			if (other.gameObject.layer == gameObject.layer && deniedObjectTime >= 5)
			{
				deniedObjectTime = 0;
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
			if (physobj.m_isHardnessed)
			{
				if (physobj.m_hand != null)
				{
					if (itemsInTheBag.Count != 0)
					{
						int rand = UnityEngine.Random.Range(0, itemsInTheBag.Count);
						FVRInteractiveObject obj;
						obj = itemsInTheBag[rand].GetComponent<FVRInteractiveObject>();
						obj.gameObject.SetActive(true);
						obj.transform.position = physobj.m_hand.transform.position;
						physobj.m_hand.ForceSetInteractable(obj);
						itemsInTheBag.Remove(itemsInTheBag[rand]);
						SetText();
						deniedObjectTime = 0;
					}
					else
					{
						if(isStatic) physobj.m_hand.ForceSetInteractable(null);
					}
				}
			}
		}

		private void FixedUpdate()
		{
			deniedObjectTime++;
		}
	}
}
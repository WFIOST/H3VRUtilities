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
		[Tooltip("If there are no objects in the BOMO, it will act like a regular harnessed object " +
		         "(grab, move it about, let go and it returns to the slot.)" +
		         " If enabled, it will simply force you to let it go, and not let you unharness the object.")]
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

		public AudioEvent dropInToSound;
		public AudioEvent takeOutOfSound;
		
		private bool _isitemsInTheBagTextNotNull;
		private int deniedObjectTime;
		public List<GameObject> notPhysObjects = new List<GameObject>(); //oh this is one HELL of a fucking bodge

		private void Start()
		{
			_isitemsInTheBagTextNotNull = itemsInTheBagText != null;
			if (isStatic)
			{
				HarnessBOMO();
			}
		}

		public void HarnessBOMO()
		{
			physobj.SetQuickBeltSlot(qbSlotForStatic);
			physobj.Harnessable = false;
			physobj.m_isHardnessed = true;
		}

		private void OnTriggerStay(Collider other)
		{
			//i can literally hear my cpu burning itself just reading this
			//sorry, oh gods of optimization, for i have made a grave sin
			deniedObjectTime++;
			if (other.gameObject.layer == gameObject.layer && deniedObjectTime >= 3)
			{
				//saves objects that isn't a physobject in a list so it doesn't waste time getcomponenting them
				if (notPhysObjects.Contains(other.gameObject)) { return; }
				var obj = other.gameObject.GetComponent<FVRPhysicalObject>();
				if(obj == null) { notPhysObjects.Add(other.gameObject); return; }
				//resets denied object time; only does so if the object is a physobject
				deniedObjectTime = 0;
				bool deny = true;
				//ensure its a script we want to BOMO
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
						if ((obj as FVRFireArmClip).FireArm == null) deny = false;
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
						if(dropInToSound.Clips.Count != 0) SM.PlayGenericSound(dropInToSound, transform.position);
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
						if(takeOutOfSound.Clips.Count != 0) SM.PlayGenericSound(takeOutOfSound, transform.position);
					}
					else
					{
						if(isStatic) physobj.m_hand.ForceSetInteractable(null);
					}
				}
			}
			else
			{
				if(isStatic) { HarnessBOMO(); }
			}
		}

		private void FixedUpdate()
		{
			deniedObjectTime++;
		}
	}
}
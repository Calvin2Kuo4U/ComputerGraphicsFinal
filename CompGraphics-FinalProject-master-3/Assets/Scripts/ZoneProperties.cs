using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneProperties : MonoBehaviour
{
	public float ZCenter;
	public float ZCameraMovementLimit;
	public float ZMaxOffset;	//The offset from ZCenter where all objects in the zone should have their hitboxes in 2D mode.

	private bool isActive;
	private PlayerController playerController;

	private void Start()
	{
		this.playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		isActive = true;
		playerController.SetActiveZone(this);
	}

	private void OnTriggerStay(Collider other)
	{
		if(!isActive && other.tag.Equals("Player")){
			isActive = true;
			playerController.SetActiveZone(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		isActive = false;
	}

	public HitboxManager GetHitboxManagerForZone(){
		return this.transform.parent.GetComponentInChildren<HitboxManager>();
	}
}

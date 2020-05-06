using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    // Start is called before the first frame update
	private PlayerController player;
	private ZoneProperties zone;		//The zone that the level geometry governed by this script belongs to.
	private GameObject hitboxes2D;
    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		this.zone = this.transform.parent.GetComponentInChildren<ZoneProperties>();
		//Create a new game object as child that contains all the colliders when in 2D mode.
		BoxCollider[] allCols = this.GetComponentsInChildren<BoxCollider>();
		hitboxes2D = new GameObject("2D Hitboxes");
		hitboxes2D.transform.parent = this.transform;
		foreach(BoxCollider col in allCols){
			BoxCollider newCol = hitboxes2D.AddComponent<BoxCollider>() as BoxCollider;
			newCol.center = new Vector3(col.bounds.center.x, col.bounds.center.y, zone.ZCenter + zone.ZMaxOffset);
			newCol.size = new Vector3(col.bounds.size.x, col.bounds.size.y, 0.2f);
		}
    }

    // Update is called once per frame
	
	public void Toggle2DHitbox(){
		if(player.is2D){
			this.hitboxes2D.SetActive(true);
		}else{
			this.hitboxes2D.SetActive(false);
		}
	}
}

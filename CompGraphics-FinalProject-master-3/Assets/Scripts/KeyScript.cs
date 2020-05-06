using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    // Start is called before the first frame update
	private PlayerController player;
	private BoxCollider keyHitbox;
	private bool isFollowingPlayer;
	private bool isHitbox2D;
	private Vector3 velocity = new Vector3(0,0,0);

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
		this.keyHitbox = this.GetComponent<BoxCollider>();
		this.isFollowingPlayer = false;
		isHitbox2D = true;
		StartCoroutine(Force2DHitbox());		//Need this because hitbox won't start out in 2D mode, and can't directly set in Start() because activeZone might be null (race condition).
    }

	private void Update()
	{
		if(isHitbox2D != player.is2D){
			if(isHitbox2D){		//currently 2D, needs to be in 3D
				keyHitbox.center = Vector3.zero;
			}else{				//currently 3D, needs to be in 2D
				ZoneProperties activeZone = player.GetActiveZone();
				//Doing some conversion from local space of player to world space to local space of key to make sure Z position of hitbox is correct
				Vector3 zPos = player.transform.TransformPoint(0,0,activeZone.ZCenter + activeZone.ZMaxOffset);
				zPos = this.transform.InverseTransformPoint(zPos);
				keyHitbox.center = new Vector3(keyHitbox.center.x, keyHitbox.center.y, zPos.z);
			}
			isHitbox2D = player.is2D;
		}
	}

	void FixedUpdate()
    {
        if(isFollowingPlayer){
			this.transform.position = Vector3.SmoothDamp(this.transform.position, player.transform.position, ref velocity, 1f, 50f);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player")){
			isFollowingPlayer = true;
			player.PlayerGotKey();
		}
	}

	private IEnumerator Force2DHitbox(){
		ZoneProperties activeZone = player.GetActiveZone();
		while(activeZone == null){
			yield return null;
			activeZone = player.GetActiveZone();
		}
		Vector3 zPos = player.transform.TransformPoint(0,0,activeZone.ZCenter + activeZone.ZMaxOffset);
		zPos = this.transform.InverseTransformPoint(zPos);
		keyHitbox.center = new Vector3(keyHitbox.center.x, keyHitbox.center.y, zPos.z);
	}
}

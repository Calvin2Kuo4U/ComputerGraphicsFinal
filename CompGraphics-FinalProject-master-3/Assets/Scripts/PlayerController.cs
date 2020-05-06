using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
	public bool is2D;
	public float playerMaxSpeed;

	private Rigidbody rb;
	private CameraController cameraController;
	private Animator animator;
	private SpriteRenderer spriteRend;
	private ZoneProperties activeZone;
	private BoxCollider hitbox;
	private HitboxManager platformHitbox;
	private bool isRotating;
	private float lerpProgress, lastFrameYPos, groundYPos;
	private Vector3 RotatedEulerAngle = new Vector3(0, 90, 0);

	private bool hasKey;

	// Start is called before the first frame update
	void Start()
	{
		this.is2D = true;
		this.rb = GetComponent<Rigidbody>();
		this.isRotating = false;
		this.cameraController = Camera.main.GetComponent<CameraController>();
		this.hitbox = this.gameObject.GetComponent<BoxCollider>();
		this.animator = this.gameObject.GetComponent<Animator>();
		this.spriteRend = this.gameObject.GetComponent<SpriteRenderer>();
		this.hasKey = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift) && !isRotating) {
			isRotating = true;
			lerpProgress = 0.0f;
			if(is2D){
				//Set player position's Z coordinate to Z coordinate as specified by the active zone (going from ortho -> perspective this should happen at beginning of transition).
				this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, activeZone.ZCenter);
				//Also change hitbox's center
				this.hitbox.center = Vector3.zero;
			}
		}

		if (isRotating) {
			lerpProgress += Time.deltaTime / cameraController.RotationDuration;
			if (lerpProgress < 1.0f) {
				if (is2D) {
					this.transform.eulerAngles = Vector3.Lerp(Vector3.zero, RotatedEulerAngle, Mathf.Clamp(lerpProgress, 0.0f, 1.0f));
				} else {
					this.transform.eulerAngles = Vector3.Lerp(RotatedEulerAngle, Vector3.zero, Mathf.Clamp(lerpProgress, 0.0f, 1.0f));
				}
			} else {
				isRotating = false;
				is2D = !is2D;
				if(is2D){
					//Set player's Z coord to active zone's Z center, happens at end of transition going from perspective -> ortho.
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, activeZone.ZCenter);
					this.hitbox.center = new Vector3(0,0, activeZone.ZCenter + activeZone.ZMaxOffset);	//TODO: Adjust this and the below to handle side switching
				}
				platformHitbox.Toggle2DHitbox();
			}
		}
	}

	void FixedUpdate()
	{
		if (!isRotating) {
			if (this.transform.position.y == lastFrameYPos) {
				if (Input.GetKey(KeyCode.Space)) {
					groundYPos = this.transform.position.y;		//Used to tell if player is about to reach the ground to cancel jump anim
					rb.AddForce(0, 4, 0, ForceMode.Impulse);
					animator.SetBool("IsJumping", true);
				}
			} else if(Mathf.Abs(this.transform.position.y - groundYPos) < 0.01){
				animator.SetBool("IsJumping", false);
			}
			if (is2D) {
				if (Input.GetKey(KeyCode.LeftArrow)) {
					rb.AddForce(-20, 0, 0);
					spriteRend.flipX = false;
					animator.SetBool("IsWalking", true);
				} else if (Input.GetKey(KeyCode.RightArrow)) {
					rb.AddForce(20, 0, 0);
					spriteRend.flipX = true;
					animator.SetBool("IsWalking", true);
				} else{
					animator.SetBool("IsWalking", false);
				}
				if (Mathf.Abs(rb.velocity.x) > playerMaxSpeed) {
					rb.velocity = new Vector3(rb.velocity.x / Mathf.Abs(rb.velocity.x) * playerMaxSpeed, rb.velocity.y, rb.velocity.z);
				}
			} else {
				if (Input.GetKey(KeyCode.DownArrow)) {
					rb.AddForce(-20, 0, 0);
					animator.SetBool("IsWalking", true);
				} else if (Input.GetKey(KeyCode.UpArrow)) {
					rb.AddForce(20, 0, 0);
					animator.SetBool("IsWalking", true);
				}
				if (Input.GetKey(KeyCode.RightArrow)) {
					rb.AddForce(0, 0, -20);
					spriteRend.flipX = true;
					animator.SetBool("IsWalking", true);
				} else if (Input.GetKey(KeyCode.LeftArrow)) {
					rb.AddForce(0, 0, 20);
					spriteRend.flipX = false;
					animator.SetBool("IsWalking", true);
				} else{
					animator.SetBool("IsWalking", false);
				}
				Vector3 velocity_noY = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				if (velocity_noY.magnitude > playerMaxSpeed) {
					rb.velocity = new Vector3(velocity_noY.normalized.x * playerMaxSpeed, rb.velocity.y, velocity_noY.normalized.z * playerMaxSpeed);
				}
			}
		}
		lastFrameYPos = this.transform.position.y;
	}

	public void SetActiveZone(ZoneProperties zone){
		this.activeZone = zone;
		this.platformHitbox = zone.GetHitboxManagerForZone();
		if(is2D){
			this.hitbox.center = new Vector3(this.hitbox.center.x, this.hitbox.center.y, zone.ZCenter + zone.ZMaxOffset);
		}
	}
	public ZoneProperties GetActiveZone(){
		return this.activeZone;
	}

	public void PlayerGotKey(){
		hasKey = true;
	}
}

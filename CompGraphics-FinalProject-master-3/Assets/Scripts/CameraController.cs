using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	public float RotationDuration = 0.5f;
	public float cameraMaxSpeed = 10.0f;
	public float cameraMovementTime = 0.1f;

	private Transform followerTransform;
	private GameObject player;
	private PlayerController playerController;
	private Camera playerCam;
	private bool isRotating = false;
	private float lerpProgress = 0.0f;
	private Vector3 cameraVelocity = new Vector3(0,0,0);
	private Matrix4x4 perspectiveProj;
	private Matrix4x4 orthoProj;

	private Vector3 perspectiveCameraEulerAngle = new Vector3(10, 90, 0);

	private void Start()
	{
		this.playerCam = GetComponent<Camera>();
		this.player = GameObject.FindGameObjectWithTag("Player");
		this.playerController  = player.GetComponent<PlayerController>();
		this.followerTransform = this.transform.parent;
		//The below assumes that the camera initially uses an orthographic projection; initializes perspective and orthographic projection matrices.
		this.orthoProj = playerCam.projectionMatrix;
		playerCam.orthographic = false;
		this.perspectiveProj = playerCam.projectionMatrix;
		playerCam.ResetProjectionMatrix();
		playerCam.orthographic = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift) && !isRotating) {
			isRotating = true;
			lerpProgress = 0.0f;
		}
	}

	private void FixedUpdate()
	{
		//Follow player character (this is in FixedUpdate to match the character position being updated in FixedUpdate, eliminating jitter)
		if(!playerCam.orthographic){
			//In perspective mode, need to restrict camera's movement on Z axis to prevent clipping of background/foreground images.
			ZoneProperties activeZone = playerController.GetActiveZone();
			float restrictedZ = Mathf.Clamp(player.transform.position.z, activeZone.ZCenter - activeZone.ZCameraMovementLimit, activeZone.ZCenter + activeZone.ZCameraMovementLimit);
			Vector3 clampedPosition = new Vector3(player.transform.position.x, player.transform.position.y, restrictedZ);
			followerTransform.position = Vector3.SmoothDamp(followerTransform.position, clampedPosition, ref cameraVelocity, cameraMovementTime, cameraMaxSpeed);
		}else{
			followerTransform.position = Vector3.SmoothDamp(followerTransform.position, player.transform.position, ref cameraVelocity, cameraMovementTime, cameraMaxSpeed);
		}
	}

	private void LateUpdate()
	{
		//Don't perform the rotation if not currently rotating.
		if (!isRotating) {
			return;
		}

		//Set projection matrix to lerp'd matrix between ortho/perspective and rotate camera
		lerpProgress += Time.deltaTime / RotationDuration;
		if (lerpProgress < 1.0f) {
			if (playerCam.orthographic) {
				playerCam.projectionMatrix = MatrixLerp(orthoProj, perspectiveProj, lerpProgress);
				followerTransform.eulerAngles = Vector3.Lerp(Vector3.zero, perspectiveCameraEulerAngle, Mathf.Clamp(lerpProgress, 0.0f, 1.0f));
			} else {
				playerCam.projectionMatrix = MatrixLerp(perspectiveProj, orthoProj, lerpProgress);
				followerTransform.eulerAngles = Vector3.Lerp(perspectiveCameraEulerAngle, Vector3.zero, Mathf.Clamp(lerpProgress, 0.0f, 1.0f));
			}
		} else {
			isRotating = false;
			playerCam.orthographic = !playerCam.orthographic;
			playerCam.ResetProjectionMatrix();
			followerTransform.eulerAngles = playerCam.orthographic ? Vector3.zero : perspectiveCameraEulerAngle;
		}
	}

	private Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);
		Matrix4x4 newMatrix = new Matrix4x4();
		newMatrix.SetRow(0, Vector4.Lerp(from.GetRow(0), to.GetRow(0), t));
		newMatrix.SetRow(1, Vector4.Lerp(from.GetRow(1), to.GetRow(1), t));
		newMatrix.SetRow(2, Vector4.Lerp(from.GetRow(2), to.GetRow(2), t));
		newMatrix.SetRow(3, Vector4.Lerp(from.GetRow(3), to.GetRow(3), t));
		return newMatrix;
	}
}

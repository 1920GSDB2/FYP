using UnityEngine;

// This class corresponds to the 3rd person camera features.
public class ThirdPersonOrbitCamBasic : MonoBehaviour 
{
    public Transform player;                                           // Player's reference.
    Vector3 pivotOffset;                                               // Offset to repoint the camera.
    Vector3 camOffset;                                                 // Offset to relocate the camera related to the player position.
    public float smooth = 10f;                                         // Speed of camera responsiveness.
    public float aimingSpeed = 25f;                                    // Camera turn speed.
    public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
    public float minVerticalAngle = -60f;                              // Camera min clamp angle.
    float zoomSpeed = 10f;                                             // Speed of Zoom in and Zoom out camera
    float maxZoom = 1.5f;                                              // Camera max clamp zooming in
    float minZoom = -5f;

    float angleH = 0;                                                  // Float to store camera horizontal angle related to mouse movement.
    float angleV = 0;                                                  // Float to store camera vertical angle related to mouse movement.
    Transform cam;                                                     // This transform.
    Vector3 relCameraPos;                                              // Current camera position relative to the player.
    float relCameraPosMag;                                             // Current camera distance to the player.
    Vector3 smoothPivotOffset;                                         // Camera current pivot offset on interpolation.
    Vector3 smoothCamOffset;                                           // Camera current offset on interpolation.
    Vector3 targetPivotOffset;                                         // Camera pivot offset target to iterpolate.
    Vector3 targetCamOffset;                                           // Camera offset target to interpolate.
    float targetMaxVerticalAngle;                                      // Custom camera max vertical clamp angle.
    float viewportOffset;                                              // Player and Camera distance offset.
    Quaternion camYRotation;
    Quaternion aimRotation;

    // Get the camera horizontal angle.
    public float GetH { get { return angleH; } }
    // Get the camera vertical angle.
    public float GetV { get { return angleV; } }

    void Awake()
	{
		// Reference to the camera transform.
		cam = transform;

        // Set camera default position.
        camOffset = new Vector3(0, 0, -7.0f);
        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
		cam.rotation = Quaternion.identity;

		// Get camera position relative to the player, used for collision test.
		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;

        // Set up references and default values.
        pivotOffset = new Vector3(0.0f, 0, 0.0f);
        smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;
		angleH = player.eulerAngles.y;

		ResetTargetOffsets ();
		ResetMaxVerticalAngle();
	}

	void Update()
	{
        // Get mouse movement to orbit the camera.
        // Mouse:
        if (Input.GetMouseButton(1))
        {
            // Get mouse movement to orbit the camera.
            // Mouse:
            angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * aimingSpeed;
            angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * aimingSpeed;
            // Joystick:
            //angleH += Mathf.Clamp(Input.GetAxis(XAxis), -1, 1) * 60 * horizontalAimingSpeed * Time.deltaTime;
            //angleV += Mathf.Clamp(Input.GetAxis(YAxis), -1, 1) * 60 * verticalAimingSpeed * Time.deltaTime;

            // Set vertical movement limit.
            angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

            // Set camera orientation.
            camYRotation = Quaternion.Euler(0, angleH, 0);
            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            cam.rotation = aimRotation;
        }

        // Zoom in and Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // Mouse scrolling
        {
            viewportOffset = Mathf.Clamp(viewportOffset + Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minZoom, maxZoom);
        }

        // Test for collision with the environment based on current camera position.
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        for (float zOffset = targetCamOffset.z + viewportOffset; zOffset <= 0; zOffset += 0.1f)
        {
            if (zOffset <= -5)
            {
                noCollisionOffset.z = zOffset;
            }
            else
            {
                noCollisionOffset.y += 0.15f;
            }
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0)
            {
                break;
            }
        }
        // Repostition the camera.
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
    }

	// Set camera offsets to custom values.
	public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
	}

	// Reset camera offsets to default values.
	public void ResetTargetOffsets()
	{
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;
	}

	// Reset the camera vertical offset.
	public void ResetYCamOffset()
	{
		targetCamOffset.y = camOffset.y;
	}

	// Set camera vertical offset.
	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	// Set camera horizontal offset.
	public void SetXCamOffset(float x)
	{
		targetCamOffset.x = x;
	}

	// Set max vertical camera rotation angle.
	public void SetMaxVerticalAngle(float angle)
	{
		targetMaxVerticalAngle = angle;
	}

	// Reset max vertical camera rotation angle to default value.
	public void ResetMaxVerticalAngle()
	{
		targetMaxVerticalAngle = maxVerticalAngle;
	}

	// Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
	bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
	{
		float playerFocusHeight = player.GetComponent<CapsuleCollider> ().height * 0.75f;
		return ViewingPosCheck (checkPos, playerFocusHeight) && ReverseViewingPosCheck (checkPos, playerFocusHeight, offset);
	}

	// Check for collision from camera to player.
	bool ViewingPosCheck (Vector3 checkPos, float deltaPlayerHeight)
	{
		// Cast target.
		Vector3 target = player.position + (Vector3.up * deltaPlayerHeight);
		// If a raycast from the check position to the player hits something...
		if (Physics.SphereCast(checkPos, 0.8f, target - checkPos, out RaycastHit hit, relCameraPosMag))
		{
			// ... if it is not the player...
			if(hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		return true;
	}

	// Check for collision from player to camera.
	bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
	{
		// Cast origin.
		Vector3 origin = player.position + (Vector3.up * deltaPlayerHeight);
		if (Physics.SphereCast(origin, 0.8f, checkPos - origin, out RaycastHit hit, maxDistance))
		{
			if(hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	// Get camera magnitude.
	public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs ((finalPivotOffset - smoothPivotOffset).magnitude);
	}
}

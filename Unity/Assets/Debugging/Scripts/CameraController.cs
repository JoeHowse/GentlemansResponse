using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	
// The object in our scene that our camera is currently tracking.
	private GameObject target;
	public GameObject Target
    {
        set { this.target = value; }
        get { return this.target; }
    }

// How far back should the camera be from the target?
	public float distance = 15.0F;
	public float springiness = 4.0F;

	private bool targetLock = false;

// This is for setting interpolation on our target, but making sure we don't permanently
// alter the target's interpolation setting.  This is used in the SetTarget () function.
	private RigidbodyInterpolation savedInterpolationSetting = RigidbodyInterpolation.None;

	void SetTarget (GameObject newTarget, bool snap)
	{
		// If there was a target, reset its interpolation value if it had a rigidbody.
		// otherwise, set target to the target passed into the function
		if (target) {
			if (target.rigidbody)
				target.rigidbody.interpolation = savedInterpolationSetting;
		}
	
		target = newTarget;
	
		// Now, save the new target's interpolation setting and set it to interpolate for now.
		if (target) {
			Rigidbody targetRigidbody = target.rigidbody;
			if (targetRigidbody) {
				savedInterpolationSetting = targetRigidbody.interpolation;
				targetRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			}
		}
	
		// If we should snap the camera to the target, do so now.
		if (snap) {
			transform.position = GetGoalPosition ();
		}
	}
	
	// Overload
	void SetTarget (GameObject newTarget)
	{
		SetTarget (newTarget, false);
	}
	
	// set up camera for late update so that it follows the character immediately after he moves
	void LateUpdate ()
	{
		Vector3 goalPosition = GetGoalPosition ();
		transform.position = Vector3.Lerp (transform.position, goalPosition, Time.deltaTime * springiness);	
	}

	// Based on the camera attributes and the target's special camera attributes, find out where the
	// camera should move to.
	Vector3 GetGoalPosition ()
	{
		// If there is no target, don't move the camera.  So return the camera's current position as the goal position.
		if (!target)
			return transform.position;
	
		// Our camera script can take attributes from the target.  If there are no attributes attached, we have
		// the following defaults.
		float heightOffset = 0.0F;
		float distanceModifier = 1.0F;
		float velocityLookAhead = 0.0;
		Vector2 maxLookAhead = Vector2 (0.0, 0.0);
	
		// Look for CameraTargetAttributes in our target.
		var cameraTargetAttributes = target.GetComponent(CameraTargetAttributes);
	
		// If our target has special attributes, use these instead of our above defaults.
		if (cameraTargetAttributes) {
			heightOffset = cameraTargetAttributes.heightOffset;
			distanceModifier = cameraTargetAttributes.distanceModifier;
			velocityLookAhead = cameraTargetAttributes.velocityLookAhead;
			maxLookAhead = cameraTargetAttributes.maxLookAhead;
		}
	
		// First do a rough goalPosition that simply follows the target at a certain relative height and distance.
		Vector3 goalPosition = target.position + Vector3 (0, heightOffset, -distance * distanceModifier);
	
		// Determine velocity of the target
		Vector3 targetVelocity = Vector3.zero;
		if (target.GetComponent (Rigidbody)) {
			targetVelocity = target.GetComponent(Rigidbody).velocity;
		}
		else if (target.GetComponent (PlatformerController)) {
			targetVelocity = target.GetComponent(PlatformerController).GetVelocity ();
		}
	
		// Determine where it is belived the target will be
		// Modify the value so that the character doesn't move offscreen 
		Vector3 lookAhead = targetVelocity * velocityLookAhead;
		lookAhead.x = Mathf.Clamp (lookAhead.x, -maxLookAhead.x, maxLookAhead.x);
		lookAhead.y = Mathf.Clamp (lookAhead.y, -maxLookAhead.y, maxLookAhead.y);
		lookAhead.z = 0.0;
	
		// adjust the goal position with the new look ahead value
		goalPosition += lookAhead;
	
		// make it so that anything offscreen is not shown
		Vector3 clampOffset = Vector3.zero;
	
		// Temporarily set the camera to the goal position so we can test positions for clamping.
		Vector3 cameraPositionSave = transform.position;
		transform.position = goalPosition;
	
		// Get the target position in viewport space.  Viewport space is relative to the camera.
		Vector3 targetViewportPosition = camera.WorldToViewportPoint (target.position);
	
		// Clamp top and right
		Vector3 upperRightCameraInWorld = camera.ViewportToWorldPoint (Vector3 (1.0, 1.0, targetViewportPosition.z));
		clampOffset.x = Mathf.Min (levelBounds.xMax - upperRightCameraInWorld.x, 0.0);
		clampOffset.y = Mathf.Min ((levelBounds.yMax - upperRightCameraInWorld.y), 0.0);
		goalPosition += clampOffset;
	
		// Clamp bottom and left
		transform.position = goalPosition;
		Vector3 lowerLeftCameraInWorld = camera.ViewportToWorldPoint (Vector3 (0.0, 0.0, targetViewportPosition.z));
	
		// Find out how far outside the world the camera is right now.
		clampOffset.x = Mathf.Max ((levelBounds.xMin - lowerLeftCameraInWorld.x), 0.0);
		clampOffset.y = Mathf.Max ((levelBounds.yMin - lowerLeftCameraInWorld.y), 0.0);
		goalPosition += clampOffset;
	
		// reset camera to original position
		transform.position = cameraPositionSave;
		
		// return the goal position
		return goalPosition;
	}

}

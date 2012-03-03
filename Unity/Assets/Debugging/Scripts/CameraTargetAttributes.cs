using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour
{
	float heightOffset = 0.0F;
	float distanceModifier = 1.0F;
	float velocityLookAhead = 0.15F;
	Vector2 maxLookAhead = new Vector2 (3.0F, 3.0F);
}

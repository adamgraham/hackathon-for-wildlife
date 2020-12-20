using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour 
{
	public float radius;
	public float speed;
	public float startAngle;

	public RotationDirection direction;
	public bool randomizeDirection = true;

	private float _currentAngle;
	private float _directionMultiplier;
	private Vector3 _position;
	private Vector3 _center;

	private void Start()
	{
		_currentAngle = startAngle;
		_position = Vector3.zero;
		_center = transform.localPosition;
		_directionMultiplier = (randomizeDirection) ? 
			((Random.Range( 0, 2 ) == 1) ? -1.0f : 1.0f) :
			((direction == RotationDirection.Clockwise) ? -1.0f : 1.0f);
	}

	private void Update()
	{
		_currentAngle += speed * _directionMultiplier * Time.deltaTime;

		float radians = _currentAngle * Mathf.Deg2Rad;

		_position.x = _center.x + (Mathf.Cos( radians ) * radius);
		_position.y = _center.y;
		_position.z = _center.z + (Mathf.Sin( radians ) * radius);

		transform.localPosition = _position;
	}

	public void SetCenterPoint( Vector3 center )
	{
		_center = center;
	}

}

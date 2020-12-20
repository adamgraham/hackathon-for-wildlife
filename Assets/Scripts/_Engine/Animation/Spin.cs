using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour 
{
	[Header( "Spin Settings" )]

	public float speed;

	public RotationDirection direction;
	public bool randomizeDirection = true;

	private Vector3 _rotation;
	private float _directionMultiplier;

	private void Start()
	{
		_directionMultiplier = (randomizeDirection) ?
			((Random.Range( 0, 2 ) == 1) ? -1.0f : 1.0f) :
			((direction == RotationDirection.Clockwise) ? -1.0f : 1.0f);
		_rotation = new Vector3( 0.0f, speed * _directionMultiplier, 0.0f );
	}

	private void Update() 
	{
		transform.eulerAngles += _rotation * Time.deltaTime;
	}

}

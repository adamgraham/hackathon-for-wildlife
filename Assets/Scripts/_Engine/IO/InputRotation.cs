using UnityEngine;
using System.Collections;

public class InputRotation : MonoBehaviour 
{
	public string inputAxis = "Mouse X";
	public KeyCode keyDown = KeyCode.Mouse0;
	public enum Axis { X, Y, Z, X_NEG, Y_NEG, Z_NEG };
	public InputRotation.Axis axis = Axis.Y_NEG;
	public bool localRotation = true;
	public float speed = 100.0f;

	private Vector3 _rotation;

	private void Update()
	{
		if ( keyDown != KeyCode.None )
		{
			if ( Input.GetKey( keyDown ) )
				Rotate();
		} 
		else 
		{
			Rotate();
		}
	}

	private void Rotate()
	{
		float axisDelta = Input.GetAxis( inputAxis );

		if ( MathUtils.IsNotZero( axisDelta ) )
		{
			if ( localRotation )
			{
				_rotation.x = transform.localEulerAngles.x;
				_rotation.y = transform.localEulerAngles.y;
				_rotation.z = transform.localEulerAngles.z;
			} 
			else 
			{
				_rotation.x = transform.eulerAngles.x;
				_rotation.y = transform.eulerAngles.y;
				_rotation.z = transform.eulerAngles.z;
			}

			if ( axis == Axis.X_NEG || axis == Axis.Y_NEG || axis == Axis.Z_NEG )
				axisDelta *= -1.0f;

			if ( axis == Axis.X || axis == Axis.X_NEG )
				_rotation.x += axisDelta * speed * Time.deltaTime;
			else if ( axis == Axis.Y || axis == Axis.Y_NEG )
				_rotation.y += axisDelta * speed * Time.deltaTime;
			else if ( axis == Axis.Z || axis == Axis.Z_NEG )
				_rotation.z += axisDelta * speed * Time.deltaTime;

			if ( localRotation )
				transform.localEulerAngles = _rotation;
			else
				transform.eulerAngles = _rotation;
		}
	}

}

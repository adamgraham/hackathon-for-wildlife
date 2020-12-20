using UnityEngine;
using System.Collections;

public class BillboardTexture : MonoBehaviour 
{
	public Vector3 defaultLookRotation;

	public bool constrainX;
	public bool constrainY;
	public bool constrainZ;

	private Vector3 _lookRot;

	private void Awake()
	{
		_lookRot = new Vector3( defaultLookRotation.x, defaultLookRotation.y, defaultLookRotation.z );
	}

	private void LateUpdate()
	{
		transform.LookAt( Camera.main.transform );

		Vector3 look = transform.eulerAngles;

		_lookRot.x = !constrainX ? look.x : defaultLookRotation.x;
		_lookRot.y = !constrainY ? look.y : defaultLookRotation.y;
		_lookRot.z = !constrainZ ? look.z : defaultLookRotation.z;

		transform.eulerAngles = _lookRot;
	}

}

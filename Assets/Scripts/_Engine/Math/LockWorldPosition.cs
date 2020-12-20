using UnityEngine;
using System.Collections;

public class LockWorldPosition : MonoBehaviour 
{
	public Vector3 position;

	public bool lockX;
	public bool lockY;
	public bool lockZ;

	private void LateUpdate()
	{
		Vector3 pos = transform.position;

		if ( lockX )
			pos.x = position.x;

		if ( lockY )
			pos.y = position.y;

		if ( lockZ )
			pos.z = position.z;

		transform.position = pos;
	}

}

using UnityEngine;
using System.Collections;

public class LockWorldRotation : MonoBehaviour 
{
	public Vector3 rotation;
	
	public bool lockX;
	public bool lockY;
	public bool lockZ;
	
	private void LateUpdate()
	{
		Vector3 rot = transform.eulerAngles;
		
		if ( lockX )
			rot.x = rotation.x;
		
		if ( lockY )
			rot.y = rotation.y;
		
		if ( lockZ )
			rot.z = rotation.z;
		
		transform.eulerAngles = rot;
	}
	
}

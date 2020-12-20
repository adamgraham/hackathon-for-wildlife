using UnityEngine;
using System.Collections;

public class ActiveAtDistance : MonoBehaviour 
{
	public GameObject obj;
	public Transform target;
	public float minDistance;

	private void Update()
	{
		if ( obj != null && target != null )
		{
			if ( obj.activeSelf )
			{
				if ( Vector3.Distance( obj.transform.position, target.position ) > minDistance )
					obj.SetActive( false );
			}
			else
			{
				if ( Vector3.Distance( obj.transform.position, target.position ) <= minDistance )
					obj.SetActive( true );
			}
		}
	}

}

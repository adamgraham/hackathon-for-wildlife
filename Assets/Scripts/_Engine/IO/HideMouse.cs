using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour 
{
	private void Awake()
	{
		Cursor.visible = false;
		Destroy( this );
	}

}

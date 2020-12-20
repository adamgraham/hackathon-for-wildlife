using UnityEngine;
using System.Collections;

public class LookAtMouse : MonoBehaviour
{
	public bool hideMouse;

	private void Update()
	{
		Cursor.visible = !hideMouse;

		Vector3 objectPos = Camera.main.WorldToScreenPoint( transform.position );
		Vector3 dir = Input.mousePosition - objectPos;

		transform.rotation = Quaternion.Euler( new Vector3( 0.0f, (-Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg) + 90.0f, 0.0f ) );
	}

}

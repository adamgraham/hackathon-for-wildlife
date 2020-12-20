using UnityEngine;
using System.Collections;

public class ScreenResizeEvent : MonoBehaviour 
{
	public delegate void OnScreenResize( Vector2 newSize );

	static public OnScreenResize onScreenResize;

	static private ScreenResizeEvent _instance;

	static private Vector3 _screenSize;
	static private Vector3 _screenCenter;

	private void Awake()
	{
		if ( _instance == null )
		{
			_instance = this;
			_instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

			StoreScreenSize();
		}
		else
		{
			StoreScreenSize();
			DestroyImmediate( this );
		}
	}

	private void OnDestroy()
	{
		if ( _instance == this )
		{
			_instance = null;
			onScreenResize = null;
		}
	}

	private void Update()
	{
		if ( _screenSize.x != Screen.width || _screenSize.y != Screen.height )
		{
			StoreScreenSize();

			if ( onScreenResize != null )
				onScreenResize( _screenSize );
		}
	}

	static private void StoreScreenSize()
	{
		_screenSize = new Vector3( Screen.width, Screen.height, 0.0f );
		_screenCenter = new Vector3( Screen.width * 0.5f, Screen.height * 0.5f, 0.0f );
	}

	static public Vector3 GetScreenSize()
	{
		return _screenSize;
	}

	static public Vector3 GetScreenCenter()
	{
		return _screenCenter;
	}

}

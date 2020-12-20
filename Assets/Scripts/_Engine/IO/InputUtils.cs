using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputUtils : MonoBehaviour
{
	#region Variables

	static private InputUtils _instance;
	static private MouseSwipe _mouseSwipe;
	static private Hashtable _controlsDoubleTap;

	static private InputUtils instance
	{
		get
		{
			InputUtils script = _instance;

			if ( script == null )
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "InputUtils";

				script = gameObject.AddComponent<InputUtils>();

				_mouseSwipe = new GameObject().AddComponent<MouseSwipe>();
				_controlsDoubleTap = new Hashtable();
			}

			return script;
		}
	}

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( _instance == null )
		{
			_instance = this;
			_instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

			if ( _mouseSwipe == null )
				_mouseSwipe = new GameObject().AddComponent<MouseSwipe>();

			if ( _controlsDoubleTap == null )
				_controlsDoubleTap = new Hashtable();
		}
		else
		{
			DestroyImmediate( this );
		}
	}

	private void OnDestroy()
	{
		if ( _instance == this )
		{
			Destroy( _mouseSwipe );

			_instance = this;
			_controlsDoubleTap = null;
			_mouseSwipe = null;
		}
	}

	#endregion

	#region Double Tapping

	static public bool GetButtonDoubleTap( string control )
	{
		ButtonDoubleTap doubleTap = (ButtonDoubleTap)_controlsDoubleTap[control];
		if ( doubleTap != null ) return doubleTap.IsDoubleTapped();
		return false;
	}

	static public void ListenForButtonDoubleTap( string control, float tapDelay = 0.2f )
	{
		if ( InputUtils.instance != null )
		{
			if ( !IsListeningToButtonDoubleTap( control ) )
			{
				ButtonDoubleTap doubleTap = new GameObject().AddComponent<ButtonDoubleTap>();

				doubleTap.buttonControl = control;
				doubleTap.doubleTapDelay = tapDelay;

				_controlsDoubleTap.Add( control, doubleTap );
			}
		}
	}

	static public void UnlistenForButtonDoubleTap( string control )
	{
		if ( control != null )
		{
			ButtonDoubleTap doubleTap = (ButtonDoubleTap)_controlsDoubleTap[control];
			Destroy( doubleTap.gameObject );

			_controlsDoubleTap.Remove( control );
		}
	}

	static public bool IsListeningToButtonDoubleTap( string control )
	{
		ButtonDoubleTap doubleTap = (ButtonDoubleTap)_controlsDoubleTap[control];
		if ( doubleTap != null ) return true;
		return false;
	}

	#endregion

	#region Mouse Swiping

	static public void ListenForMouseSwipes()
	{
		if ( InputUtils.instance != null )
			if ( _mouseSwipe != null )
				_mouseSwipe.enabled = true;
	}

	static public void UnlistenForMouseSwipes()
	{
		if ( _mouseSwipe != null )
			_mouseSwipe.enabled = false;
	}

	static public bool IsListeningForMouseSwipes()
	{
		return (_mouseSwipe != null) ? _mouseSwipe.enabled : false;
	}

	static public MouseSwipeData GetMouseSwipeData()
	{
		if ( _mouseSwipe != null )
			return _mouseSwipe.GetSwipeData();

		return new MouseSwipeData();
	}

	static public MouseSwipeData GetMouseUnfinishedSwipeData()
	{
		if ( _mouseSwipe != null )
			return _mouseSwipe.GetUnfinishedSwipeData();

		return new MouseSwipeData();
	}

	static public void AddMouseSwipeCallback( MouseSwipe.MouseSwipeCallback callback )
	{
		if ( InputUtils.instance != null )
			if ( _mouseSwipe != null )
				_mouseSwipe.onSwipe += callback;
	}

	static public void RemoveMouseSwipeCallback( MouseSwipe.MouseSwipeCallback callback )
	{
		if ( _mouseSwipe != null )
			_mouseSwipe.onSwipe -= callback;
	}

	#endregion

	#region Keyboard

	static public bool IsAnyKeyDown()
	{
		return Input.anyKeyDown && 
			  !Input.GetMouseButtonDown( 0 ) && 
			  !Input.GetMouseButtonDown( 1 ) && 
			  !Input.GetMouseButtonDown( 2 );
	}

	static public bool IsAnyKey()
	{
		return Input.anyKeyDown &&
			  !Input.GetMouseButton( 0 ) &&
			  !Input.GetMouseButton( 1 ) &&
			  !Input.GetMouseButton( 2 );
	}

	#endregion

}

#region Button Double Tap

public class ButtonDoubleTap : MonoBehaviour
{
	public string buttonControl;
	public float doubleTapDelay;

	private bool doubleTapped;

	private float _lastTapTime;

	private void Awake()
	{
		gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
	}

	private void OnDestroy()
	{
		buttonControl = null;
	}

	private void LateUpdate()
	{
		doubleTapped = false;

		if ( Input.GetButtonDown( buttonControl ) )
		{
			if ( Time.time - _lastTapTime <= doubleTapDelay )
				doubleTapped = true;

			_lastTapTime = Time.time;
		}
	}

	public bool IsDoubleTapped()
	{
		return doubleTapped;
	}

}

#endregion

#region Mouse Swipe

public class MouseSwipe : MonoBehaviour
{
	public MouseSwipeCallback onSwipe;
	public delegate void MouseSwipeCallback( MouseSwipeData swipe );

	private MouseSwipeData _swipeData;

	private void Awake()
	{
		gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
	}

	private void OnDestroy()
	{
		onSwipe = null;
	}

	private void Update()
	{
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			_swipeData.Clear();
			_swipeData.direction = Vector3.zero;
			_swipeData.startPoint = Input.mousePosition;
			_swipeData.startTime = Time.time;
		}
		else if ( Input.GetMouseButtonUp( 0 ) )
		{
			_swipeData.endPoint = Input.mousePosition;
			_swipeData.endTime = Time.time;
			_swipeData.Calculate();

			if ( onSwipe != null )
				onSwipe( GetSwipeData() );
		}
	}

	public Vector3 GetSwipeDirection()
	{
		return _swipeData.direction;
	}

	public Vector3 GetSwipeStartPoint()
	{
		return _swipeData.startPoint;
	}

	public Vector3 GetSwipeEndPoint()
	{
		return _swipeData.endPoint;
	}

	public MouseSwipeData GetSwipeData()
	{
		MouseSwipeData data = new MouseSwipeData();
		data.Calculate( _swipeData );
		return data;
	}

	public MouseSwipeData GetUnfinishedSwipeData()
	{
		MouseSwipeData data = new MouseSwipeData();
		data.Calculate( _swipeData.startPoint, Input.mousePosition, _swipeData.startTime, Time.time );
		return data;
	}

}

public struct MouseSwipeData
{
	public Vector3 startPoint;
	public Vector3 endPoint;

	public float startTime;
	public float endTime;

	public Vector3 direction;
	public float distance;
	public float duration;

	[Range( 0.0f, 1.0f )]
	static public float cardinalAngleDelta = 0.15f;

	internal void Clear()
	{
		startPoint = Vector3.zero;
		endPoint = Vector3.zero;

		startTime = 0.0f;
		endTime = 0.0f;

		direction = Vector3.zero;
		distance = 0.0f;
		duration = 0.0f;
	}

	internal void Calculate( Vector3 _startPoint, Vector3 _endPoint, float _startTime, float _endTime )
	{
		startPoint = _startPoint;
		endPoint = _endPoint;

		startTime = _startTime;
		endTime = _endTime;

		direction = (endPoint - startPoint).normalized;
		distance = Vector3.Distance( startPoint, endPoint );
		duration = endTime - startTime;
	}

	internal void Calculate()
	{
		Calculate( startPoint, endPoint, startTime, endTime );
	}

	internal void Calculate( MouseSwipeData other )
	{
		Calculate( other.startPoint, other.endPoint, other.startTime, other.endTime );
	}

	public bool IsSwipeUp()
	{
		return direction.y > (1.0f - cardinalAngleDelta);
	}

	public bool IsSwipeDown()
	{
		return direction.y < (-1.0f + cardinalAngleDelta);
	}

	public bool IsSwipeRight()
	{
		return direction.x > (1.0f - cardinalAngleDelta);
	}

	public bool IsSwipeLeft()
	{
		return direction.x < (-1.0f + cardinalAngleDelta);
	}

	public bool IsNonSwipe()
	{
		return !IsSwipeUp() && !IsSwipeDown() && !IsSwipeLeft() && !IsSwipeRight();
	}

}

#endregion

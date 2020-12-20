using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cinematic : MonoBehaviour
{
	#region Variables

	public delegate void CinematicCallback();

	static public float barSizePercent = 0.15f;
	static public float barTweenDuration = 1.0f;

	static public Ease easeIn = Ease.OutQuad;
	static public Ease easeOut = Ease.InQuad;

	static private GUITexture _topBar;
	static private GUITexture _bottomBar;

	static private CinematicPan _panIn;
	static private CinematicPan _panOut;

	static private CinematicCallback _onComplete;

	static private bool _isCinematicActive;

	static private Cinematic _instance;
	static private Cinematic instance
	{
		get
		{
			Cinematic script = _instance;

			if ( script == null )
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "Cinematic";

				script = gameObject.AddComponent<Cinematic>();

				CreateTopBar();
				CreateBottomBar();

				ScreenResizeEvent.onScreenResize += OnScreenResize;
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
			_instance.gameObject.AddComponent<ScreenResizeEvent>();

			_isCinematicActive = false;

			if ( _topBar == null )
				CreateTopBar();

			if ( _bottomBar == null )
				CreateBottomBar();
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
			DestroyTopBar();
			DestroyBottomBar();

			_instance = null;
			_topBar = null;
			_bottomBar = null;
			_panIn = null;
			_panOut = null;

			ScreenResizeEvent.onScreenResize -= OnScreenResize;
		}
	}

	static private void OnScreenResize( Vector2 newSize )
	{
		if ( _topBar != null )
		{
			_topBar.pixelInset = new Rect( 0.0f, 0.0f, Screen.width, Screen.height );
			_topBar.transform.position = (!_isCinematicActive) ? 
				new Vector3( 0.0f, 1.5f, 0.0f ) :
				new Vector3( 0.0f, 1.5f - barSizePercent, 0.0f ); ;
		}

		if ( _bottomBar != null )
		{
			_bottomBar.pixelInset = new Rect( 0.0f, 0.0f, Screen.width, Screen.height );
			_bottomBar.transform.position = (!_isCinematicActive) ?
				new Vector3( 0.0f, -1.5f, 0.0f ) :
				new Vector3( 0.0f, -1.5f + barSizePercent, 0.0f );
		}
	}

	#region Creation

	static private GUITexture CreateCinematicBar()
	{
		GameObject newObject = new GameObject();

		newObject.name = "Cinematic Bar";
		newObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

		GUITexture bar = newObject.AddComponent<GUITexture>();

		bar.transform.position = Vector3.zero;
		bar.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		bar.texture = new Texture2D( 2048, 2048 );
		bar.pixelInset = new Rect( 0.0f, 0.0f, Screen.width, Screen.height );
		bar.color = Color.black;
		bar.enabled = false;

		return bar;
	}

	static private void CreateTopBar()
	{
		DestroyTopBar();

		_topBar = CreateCinematicBar();
		_topBar.transform.position = new Vector3( 0.0f, 1.5f, 0.0f );
	}

	static private void CreateBottomBar()
	{
		DestroyBottomBar();

		_bottomBar = CreateCinematicBar();
		_bottomBar.transform.position = new Vector3( 0.0f, -1.5f, 0.0f );
	}

	static private void DestroyTopBar()
	{
		if ( _topBar != null )
			Destroy( _topBar.gameObject );
	}

	static private void DestroyBottomBar()
	{
		if ( _bottomBar != null )
			Destroy( _bottomBar.gameObject );
	}

	#endregion

	#endregion

	#region Start Cinematic

	static public bool StartCinematic( CinematicPan panIn = null, CinematicPan panOut = null, CinematicCallback onComplete = null )
	{
		bool started = false;

		if ( Cinematic.instance != null )
		{
			if ( !_isCinematicActive )
			{
				_isCinematicActive = true;

				_topBar.enabled = true;
				_topBar.transform.position = new Vector3( 0.0f, 1.5f, 0.0f );
				_topBar.transform.DOMoveY( 1.5f - barSizePercent, barTweenDuration )
					.SetEase( easeIn );

				_bottomBar.enabled = true;
				_bottomBar.transform.position = new Vector3( 0.0f, -1.5f, 0.0f );
				_bottomBar.transform.DOMoveY( -1.5f + barSizePercent, barTweenDuration )
					.SetEase( easeIn );

				_panIn = panIn;
				_panOut = panOut;

				if ( _panIn != null )
				{
					_panIn.PanIn();
					_panIn = null;
				}

				_onComplete = onComplete;

				started = true;
				Debug.Log( "Cinematic Started" );
			}
		}

		return started;
	}

	static public void StartCinematic( float duration, CinematicPan panIn = null, CinematicPan panOut = null, CinematicCallback onComplete = null )
	{
		if ( StartCinematic( panIn, panOut, onComplete ) )
		{
			_instance.CancelInvoke( "StopCinematicInstance" );
			_instance.Invoke( "StopCinematicInstance", duration );
		}
	}

	#endregion

	#region Stop Cinematic

	private void StopCinematicInstance()
	{
		Cinematic.StopCinematic();
	}

	static public void StopCinematic()
	{
		if ( Cinematic.instance != null )
		{
			if ( _isCinematicActive )
			{
				_topBar.transform.DOMoveY( 1.5f, barTweenDuration ).
					SetEase( easeOut );

				_bottomBar.transform.DOMoveY( -1.5f, barTweenDuration ).
					SetEase( easeOut ).OnComplete( OnStopCinematicComplete );

				if ( _panOut != null )
				{
					_panOut.PanOut();
					_panOut = null;
				}
			}
		}
	}

	static public void StopCinematicImmediate()
	{
		if ( Cinematic.instance != null )
		{
			if ( _isCinematicActive )
			{
				_topBar.transform.position = new Vector3( _topBar.transform.position.x, 1.5f, _topBar.transform.position.z );
				_topBar.enabled = false;

				_bottomBar.transform.position = new Vector3( _bottomBar.transform.position.x, -1.5f, _bottomBar.transform.position.z );
				_bottomBar.enabled = false;

				if ( _panOut != null )
					_panOut.OnPanOutComplete();

				_isCinematicActive = false;

				if ( _onComplete != null )
					_onComplete();

				Debug.Log( "Cinematic Stopped" );
			}
		}
	}

	static private void OnStopCinematicComplete()
	{
		_topBar.enabled = false;
		_bottomBar.enabled = false;

		_isCinematicActive = false;

		if ( _onComplete != null )
			_onComplete();

		Debug.Log( "Cinematic Stopped" );
	}

	#endregion

}

#region Cinematic Pan

[System.Serializable]
public class CinematicPan
{
	public Transform target;
	public float duration;
	public float delay = 0.0f;
	public Ease ease = Ease.InOutCubic;
	public DG.Tweening.Core.TweenCallback onComplete;

	public void Dispose()
	{
		target = null;
		onComplete = null;
	}

	public void PanIn()
	{
		CameraOperator.PanToFocus( target, duration, delay, ease, onComplete );
	}

	public void PanOut()
	{
		CameraOperator.PanToFocus( target, duration, delay, ease, OnPanOutComplete );
	}

	internal void OnPanOutComplete()
	{
		CameraOperator.FollowFocusObject();

		if ( onComplete != null )
			onComplete();
	}

}

#endregion

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ScreenFader : MonoBehaviour
{
	#region Variables

	public delegate void ScreenFaderCallback();

	static private ScreenFader _instance;
	static private GUITexture _guiTexture;
	static private ScreenFaderCallback _onFadeComplete;
	static private Tweener _currentFade;

	#endregion

	#region Unity Events

	static private ScreenFader instance
	{
		get
		{
			ScreenFader script = _instance;

			if ( script == null )
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "Screen Fader";

				script = gameObject.AddComponent<ScreenFader>();

				_guiTexture = CreateGUITexture( gameObject );

				ScreenResizeEvent.onScreenResize += OnScreenResize;
			}

			return script;
		}
	}

	private void Awake()
	{
		if ( _instance == null )
		{
			_instance = this;
			_instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
			_instance.gameObject.AddComponent<ScreenResizeEvent>();

			if ( _guiTexture == null )
				_guiTexture = CreateGUITexture( gameObject );
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
			_instance = null;
			_guiTexture = null;
			_onFadeComplete = null;
			_currentFade = null;

			ScreenResizeEvent.onScreenResize -= OnScreenResize;
		}
	}

	static private void OnScreenResize( Vector2 newSize )
	{
		if ( _guiTexture != null )
		{
			_guiTexture.pixelInset = new Rect( 0.0f, 0.0f, Screen.width, Screen.height );
			_guiTexture.transform.position = new Vector3( 0.0f, 0.0f, 1.0f );
		}
	}

	static private GUITexture CreateGUITexture( GameObject gameObject )
	{
		GUITexture texture = gameObject.GetComponent<GUITexture>();
		if ( texture == null ) texture = gameObject.AddComponent<GUITexture>();

		texture.transform.position = new Vector3( 0.0f, 0.0f, 1.0f );
		texture.texture = new Texture2D( 2048, 2048 );
		texture.pixelInset = new Rect( 0.0f, 0.0f, Screen.width, Screen.height );
		texture.color = Color.clear;
		texture.enabled = false;

		return texture;
	}

	#endregion

	#region Fade To

	static public void FadeToColor( Color color, float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		if ( ScreenFader.instance != null )
		{
			if ( _guiTexture != null )
			{
				_guiTexture.enabled = true;
				if ( clearColor ) _guiTexture.color = Color.clear;

				_onFadeComplete = onComplete;
				if ( _currentFade != null ) _currentFade.Kill();

				_currentFade = DOTween.To( () => _guiTexture.color, x => _guiTexture.color = x, color, duration ).
					OnComplete( OnFadeToComplete );
			}
		}
	}

	static public void FadeToColorPartial( Color color, float percentFade, float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		Color partial = new Color( color.r, color.g, color.b, color.a * percentFade );
		FadeToColor( partial, duration, onComplete, clearColor );
	}

	static public void FadeToColorHalfway( Color color, float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToColorPartial( color, 0.5f, duration, onComplete, clearColor );
	}

	static public void FadeToWhite( float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToColor( Color.white, duration, onComplete, clearColor );
	}

	static public void FadeToWhitePartial( float percentFade, float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToColorPartial( Color.white, percentFade, duration, onComplete, clearColor );
	}

	static public void FadeToWhiteHalfway( float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToWhitePartial( 0.5f, duration, onComplete, clearColor );
	}

	static public void FadeToBlack( float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false ) 
	{
		FadeToColor( Color.black, duration, onComplete, clearColor );
	}

	static public void FadeToBlackPartial( float percentFade, float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToColorPartial( Color.black, percentFade, duration, onComplete, clearColor );
	}

	static public void FadeToBlackHalfway( float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false )
	{
		FadeToBlackPartial( 0.5f, duration, onComplete, clearColor );
	}

	static public void FadeToClear( float duration = 1.5f, ScreenFaderCallback onComplete = null, bool clearColor = false ) 
	{
		FadeFromColor( _guiTexture.color, duration, onComplete );
	}

	static private void OnFadeToComplete()
	{
		if ( _onFadeComplete != null )
			_onFadeComplete();
	}

	#endregion

	#region Fade From

	static public void FadeFromColor( Color color, float duration = 1.5f, ScreenFaderCallback onComplete = null )
	{
		if ( ScreenFader.instance != null )
		{
			if ( _guiTexture != null )
			{
				_guiTexture.enabled = true;
				_guiTexture.color = color;

				_onFadeComplete = onComplete;
				if ( _currentFade != null ) _currentFade.Kill();

				_currentFade = DOTween.To( () => _guiTexture.color, x => _guiTexture.color = x, Color.clear, duration ).
					OnComplete( OnFadeFromComplete );
			}
		}
	}
	
	static public void FadeFromWhite( float duration = 1.5f, ScreenFaderCallback onComplete = null )
	{
		FadeFromColor( Color.white, duration, onComplete );
	}
	
	static public void FadeFromBlack( float duration = 1.5f, ScreenFaderCallback onComplete = null ) 
	{
		FadeFromColor( Color.black, duration, onComplete );
	}

	static private void OnFadeFromComplete()
	{
		if ( _guiTexture != null )
			_guiTexture.enabled = false;

		if ( _onFadeComplete != null )
			_onFadeComplete();
	}

	#endregion
}

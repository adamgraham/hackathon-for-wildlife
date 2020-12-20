using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SceneTransitioner
{
	public delegate void SceneTransitionCallback();

	static private string _toScene;
	static private SceneTransitionCallback _toCallback;

	public enum FadeColor{ Black, White }

	public const float DEFAULT_DURATION = 2.5f;

	static public void TransitionToScene( string scene, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, DEFAULT_DURATION, FadeColor.Black, callback );
	}

	static public void TransitionToScene( string scene, FadeColor fadeColor, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, DEFAULT_DURATION, fadeColor, callback );
	}

	static public void TransitionToScene( string scene, AudioSource[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, DEFAULT_DURATION, FadeColor.Black, callback );
		AudioUtils.FadeOutAudio( audio, DEFAULT_DURATION );
	}

	static public void TransitionToScene( string scene, AudioSourceExtended[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, DEFAULT_DURATION, FadeColor.Black, callback );
		AudioUtils.FadeOutAudio( audio, DEFAULT_DURATION );
	}

	static public void TransitionToScene( string scene, float duration, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, duration, FadeColor.Black, callback );
	}

	static public void TransitionToScene( string scene, float duration, AudioSource[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, duration, FadeColor.Black, callback );
		AudioUtils.FadeOutAudio( audio, duration );
	}

	static public void TransitionToScene( string scene, float duration, AudioSourceExtended[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, duration, FadeColor.Black, callback );
		AudioUtils.FadeOutAudio( audio, duration );
	}

	static public void TransitionToScene( string scene, float duration, FadeColor fadeColor, SceneTransitionCallback callback = null )
	{
		_toScene = scene;
		_toCallback = callback;

		if ( fadeColor == FadeColor.Black )
			ScreenFader.FadeToBlack( duration, OnFadeOutComplete );
		else if ( fadeColor == FadeColor.White )
			ScreenFader.FadeToWhite( duration, OnFadeOutComplete );
	}

	static public void TransitionToScene( string scene, float duration, FadeColor fadeColor, AudioSource[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, duration, fadeColor, callback );
		AudioUtils.FadeOutAudio( audio, duration );
	}

	static public void TransitionToScene( string scene, float duration, FadeColor fadeColor, AudioSourceExtended[] audio, SceneTransitionCallback callback = null )
	{
		TransitionToScene( scene, duration, fadeColor, callback );
		AudioUtils.FadeOutAudio( audio, duration );
	}

	static private void OnFadeOutComplete()
	{
		if ( _toCallback != null )
			_toCallback();

		Application.LoadLevel( _toScene );
	}

}

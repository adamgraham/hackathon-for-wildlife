using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AudioUtils : MonoBehaviour
{
	static private AudioUtils _instance;
	static private AudioSource _audioSource;

	static private AudioUtils instance
	{
		get
		{
			AudioUtils script = _instance;

			if ( script == null ) 
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "AudioUtils";

				script = gameObject.AddComponent<AudioUtils>();

				_audioSource = gameObject.AddComponent<AudioSource>();
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

			if ( _audioSource == null )
				_audioSource = gameObject.AddComponent<AudioSource>();
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
			_audioSource = null;
		}
	}

	private IEnumerator PlayClipDelayed( AudioClip clip, Vector3 position, float delay, float volume )
	{
		float elapsed = 0.0f;

		while ( elapsed <= delay )
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		AudioSource.PlayClipAtPoint( clip, position, volume );
	}

	static public void PlayClipAtPoint( AudioClipExtended clip, Vector3 position )
	{
		if ( clip != null )
			PlayClipAtPoint( clip.audioClip, position, 0.0f, clip.volume );
	}

	static public void PlayClipAtPoint( AudioClipExtended clip, Vector3 position, float delay )
	{
		if ( clip != null )
			PlayClipAtPoint( clip.audioClip, position, delay, clip.volume );
	}

	static public void PlayClipAtPoint( AudioClipExtended clip, Vector3 position, float delay, float volume )
	{
		if ( clip != null )
			PlayClipAtPoint( clip.audioClip, position, delay, volume );
	}

	static public void PlayClipAtPoint( AudioClip clip, Vector3 position, float delay )
	{
		PlayClipAtPoint( clip, position, delay, 1.0f );
	}

	static public void PlayClipAtPoint( AudioClip clip, Vector3 position, float delay, float volume )
	{
		if ( clip != null )
		{
			if ( delay <= 0.0f )
				AudioSource.PlayClipAtPoint( clip, position, volume );
			else
				instance.StartCoroutine( instance.PlayClipDelayed( clip, position, delay, volume ) );
		}
	}

	static public void PlayClipsAtPoint( AudioClipExtended[] clips, Vector3 position )
	{
		if ( clips != null )
		{
			int len = clips.Length;
			for ( int i = 0; i < len; i++ )
				PlayClipAtPoint( clips[i].audioClip, position, 0.0f, clips[i].volume );
		}
	}

	static public void PlayClipsAtPoint( AudioClipExtended[] clips, Vector3 position, float delay )
	{
		if ( clips != null )
		{
			int len = clips.Length;
			for ( int i = 0; i < len; i++ )
				PlayClipAtPoint( clips[i].audioClip, position, delay, clips[i].volume );
		}
	}

	static public void PlayClipsAtPoint( AudioClipExtended[] clips, Vector3 position, float delay, float volume )
	{
		if ( clips != null )
		{
			int len = clips.Length;
			for ( int i = 0; i < len; i++ )
				PlayClipAtPoint( clips[i].audioClip, position, delay, volume );
		}
	}

	static public void PlayClipsAtPoint( AudioClip[] clips, Vector3 position, float delay )
	{
		if ( clips != null )
		{
			int len = clips.Length;
			for ( int i = 0; i < len; i++ )
				PlayClipAtPoint( clips[i], position, delay, 1.0f );
		}
	}

	static public void PlayClipsAtPoint( AudioClip[] clips, Vector3 position, float delay, float volume )
	{
		if ( clips != null )
		{
			int len = clips.Length;
			for ( int i = 0; i < len; i++ )
				PlayClipAtPoint( clips[i], position, delay, volume );
		}
	}

	static public void FadeOutAudio( AudioSource[] audio, float duration )
	{
		if ( audio != null )
		{
			int len = audio.Length;
			for ( int i = 0; i < len; i++ )
				audio[i].DOFade( 0.0f, duration );
		}
	}

	static public void FadeOutAudio( AudioSourceExtended[] audio, float duration )
	{
		if ( audio != null )
		{
			int len = audio.Length;
			for ( int i = 0; i < len; i++ )
				audio[i].FadeOut( duration );
		}
	}

	static public void FadeOutAudio( AudioSource audio, float duration )
	{
		if ( audio != null )
			audio.DOFade( 0.0f, duration );
	}

	static public void FadeInAudio( AudioSource[] audio, float duration, float volume = 1.0f )
	{
		if ( audio != null )
		{
			int len = audio.Length;
			for ( int i = 0; i < len; i++ )
				audio[i].DOFade( volume, duration );
		}
	}

	static public void FadeInAudio( AudioSource audio, float duration, float volume = 1.0f )
	{
		if ( audio != null )
			audio.DOFade( volume, duration );
	}

}

[System.Serializable]
public class AudioClipExtended
{
	public AudioClip audioClip;
	[Range( 0.0f, 1.0f )]
	public float volume = 1.0f;

	public void Play()
	{
		AudioUtils.PlayClipAtPoint( this, Vector3.zero );
	}

	public void Play( Vector3 position )
	{
		AudioUtils.PlayClipAtPoint( this, position );
	}

}

[System.Serializable]
public class AudioSourceExtended
{
	public AudioSource audioSource;
	[Range( 0.0f, 1.0f )]
	public float defaultVolume = 1.0f;

	public void PlayAndFadeIn( float duration )
	{
		audioSource.Play();
		FadeIn( duration );
	}

	public void FadeIn( float duration )
	{
		audioSource.volume = 0.0f;
		audioSource.DOFade( defaultVolume, duration );
	}

	public void FadeOut( float duration )
	{
		audioSource.DOFade( 0.0f, duration );
	}

}

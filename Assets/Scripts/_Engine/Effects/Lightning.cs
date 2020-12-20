using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent( typeof( Light ) )]
public class Lightning : MonoBehaviour
{
	#region Variables

	[Header( "General" )]

	public bool enableOnAwake = true;
	public bool globalLightning = false;

	[HideInInspector]
	public bool debug = false;
	[HideInInspector]
	public KeyCode debugKey;

	[Range( 0.0f, 1.0f )]
	public float lightningChance = 0.0025f;

	private Light _light;

	private bool _flashing;
	static private bool _globalFlashing;

	static public List<Lightning> lightningList;

	[Header( "Transform" )]

	public bool maintainRotation = true;
	[Range( 0.0f, 360.0f )]
	public float rotationMin = 0.0f;
	[Range( 0.0f, 360.0f )]
	public float rotationMax = 360.0f;

	[Header( "Intensity" )]

	[Range( 0.0f, 8.0f )]
	public float intensityMin = 0.0f;
	[Range( 0.0f, 8.0f )]
	public float intensityMax = 4.0f;
	[Range( 0.0f, 8.0f )]
	public float intensityFluctuationMin = 0.0f;
	[Range( 0.0f, 8.0f )]
	public float intensityFluctuationMax = 1.0f;

	private float _fillLightIntensity;

	[Header( "Timing" )]

	[Range( 0.0f, 1.0f )]
	public float speedInMin = 0.95f;
	[Range( 0.0f, 1.0f )]
	public float speedInMax = 0.95f;

	[Range( 0.0f, 1.0f )]
	public float speedOutMin = 0.95f;
	[Range( 0.0f, 1.0f )]
	public float speedOutMax = 0.95f;

	[Header( "Ripling" )]

	public int ripplesMin = 2;
	public int ripplesMax = 4;

	[Header( "Audio" )]

	public AudioClip[] audioClips;

	public float audioIntervalMin;
	public float audioIntervalMax;

	private bool _audioCooldown;

	#endregion

	#region Unity Events

	private void Awake()
	{
		_light = gameObject.GetComponent<Light>();
		_fillLightIntensity = _light.intensity;

		if ( lightningList == null )
			lightningList = new List<Lightning>();

		lightningList.Add( this );
		enabled = enableOnAwake;
	}

	private void Update()
	{
		if ( _flashing )
			_globalFlashing = true;

		/*
		if ( debug )
		{
			if ( Input.GetKeyDown( debugKey ) )
				Flash();
		}
		 * */
	}

	private void FixedUpdate() 
	{
		//if ( !debug )
		//{
			if ( Random.value < lightningChance )
				Flash();
		//}
	}

	private void OnDestroy()
	{
		if ( lightningList != null )
			lightningList.Remove( this );

		_light = null;

		audioClips = null;
	}

	#endregion

	#region Flash

	public void Flash()
	{
		if ( !_flashing )
		{
			if ( !globalLightning || (globalLightning && !_globalFlashing) )
			{
				_flashing = true;
				_globalFlashing = true;

				if ( !maintainRotation )
					transform.eulerAngles = new Vector3( transform.eulerAngles.x, Random.Range( rotationMin, rotationMax ), transform.eulerAngles.z );

				Sequence thunder = DOTween.Sequence();

				float intensity = Random.Range( intensityMin, intensityMax );
				float durationIn = 1.0f - Random.Range( speedInMin, speedInMax );
				float durationOut = 1.0f - Random.Range( speedOutMin, speedOutMax );

				int ripples = Random.Range( ripplesMin, ripplesMax );
				for ( int i = 0; i < ripples; i++ )
				{
					thunder.Append( _light.DOIntensity( intensity, durationIn ).SetEase( Ease.OutQuad ) );
					thunder.Append( _light.DOIntensity( 1.0f, durationOut ).SetEase( Ease.InQuad ) );

					intensity = Mathf.Clamp( intensity + (Random.Range( intensityFluctuationMin, intensityFluctuationMax ) * ((Random.Range( 0, 2 ) == 1) ? 1.0f : -1.0f)), intensityMin, intensityMax );
				}

				thunder.Append( _light.DOIntensity( intensity, durationIn ).SetEase( Ease.OutQuad ) );
				thunder.Append( _light.DOIntensity( _fillLightIntensity, durationOut ).SetEase( Ease.InQuad ) );
				thunder.Play().OnComplete( OnFlashComplete );

				if ( audioClips.Length > 0 )
				{
					if ( !_audioCooldown )
					{
						AudioSource.PlayClipAtPoint( audioClips[Random.Range( 0, audioClips.Length )], Camera.main.transform.position );

						_audioCooldown = true;

						CancelInvoke( "OnAudioIntervalComplete" );
						Invoke( "OnAudioIntervalComplete", Random.Range( audioIntervalMin, audioIntervalMax ) );
					}
				}
			}
		}
	}

	private void OnFlashComplete()
	{
		_flashing = false;
		_globalFlashing = false;
	}

	private void OnAudioIntervalComplete()
	{
		_audioCooldown = false;
	}

	#endregion

	#region Static Methods

	static public void ToggleAllLightning( bool state )
	{
		if ( Lightning.lightningList != null )
		{
			int len = Lightning.lightningList.Count;
			for ( int i = 0; i < len; i++ )
			{
				Lightning lightning = Lightning.lightningList[i];
				if ( lightning != null ) lightning.enabled = state;
			}
		}
	}

	#endregion

}

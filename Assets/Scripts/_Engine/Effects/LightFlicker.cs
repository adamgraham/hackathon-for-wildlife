using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent( typeof( Light ) )]
public class LightFlicker : MonoBehaviour 
{
	[HideInInspector]
	[SerializeField]
	new Light light;

	[HideInInspector]
	public float baseIntensity;

	public float flickerFluctuation = 1.5f;
	public float flickerDuration = 0.15f;

	private float _startingIntensity;
	private float _startingRange;
	private bool _flickering;

	private void Awake() 
	{
		light = gameObject.GetComponent<Light>();

		_startingIntensity = light.intensity;
		_startingRange = light.range;

		baseIntensity = _startingIntensity;
	}

	private void OnDestroy()
	{
		light = null;
	}

	private void FixedUpdate() 
	{
		if ( !_flickering )
			Flicker();
	}

	private void Flicker()
	{
		light.DOIntensity( baseIntensity + Random.Range( -flickerFluctuation, flickerFluctuation ), flickerDuration )
			.OnComplete( OnFlickerComplete );

		_flickering = true;
	}

	private void OnFlickerComplete()
	{
		_flickering = false;
	}

	public void RevertToStartingValues()
	{
		baseIntensity = _startingIntensity;

		light.DOKill();
		light.range = _startingRange;

		Flicker();
	}

}

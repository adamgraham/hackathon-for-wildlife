using UnityEngine;
using System.Collections;
using DG.Tweening;

public class YoYoScale : MonoBehaviour 
{
	public Vector3 deltaMin;
	public Vector3 deltaMax;

	public Ease ease = Ease.InOutQuad;

	public float durationMin;
	public float durationMax;

	public bool autoStartAndStop = true;

	private Vector3 _startingScale;
	private float _duration;

	private void Start()
	{
		_startingScale = transform.localScale;

		if ( autoStartAndStop )
			StartYoYo();
	}

	private void OnEnable()
	{
		if ( autoStartAndStop )
			StartYoYo();
	}

	private void OnDisable()
	{
		if ( autoStartAndStop )
			StopYoYo();
	}

	private void OnDestroy()
	{
		StopYoYo();
	}

	public void StopYoYo()
	{
		transform.DOKill();
	}

	public void StartYoYo()
	{
		_duration = Random.Range( durationMin, durationMax );

		if ( _duration > 0.0f )
		{
			transform.DOKill();
			transform.DOScale( _startingScale + deltaMin, _duration * 0.5f ).
				SetEase( ease ).OnComplete( OnYoYoComplete );
		}
	}

	private void OnYoYoComplete()
	{
		transform.DOKill();
		transform.DOScale( _startingScale + deltaMax, _duration * 0.5f ).
			SetEase( ease ).OnComplete( StartYoYo );
	}

}

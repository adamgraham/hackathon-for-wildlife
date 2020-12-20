using UnityEngine;
using System.Collections;
using DG.Tweening;

public class YoYoTranslate : MonoBehaviour
{
	public Vector3 deltaMin;
	public Vector3 deltaMax;

	public Ease ease = Ease.InOutQuad;

	public float durationMin;
	public float durationMax;

	public bool autoStartAndStop = true;
	public bool localMovement = true;

	private Vector3 _startingPosition;
	private float _duration;

	private void Start()
	{
		if ( localMovement )
			_startingPosition = transform.localPosition;
		else
			_startingPosition = transform.position;

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

			if ( localMovement )
			{
				transform.DOLocalMove( _startingPosition + deltaMin, _duration * 0.5f ).
					SetEase( ease ).OnComplete( OnYoYoComplete );
			}
			else
			{
				transform.DOMove( _startingPosition + deltaMin, _duration * 0.5f ).
					SetEase( ease ).OnComplete( OnYoYoComplete );
			}
		}
	}

	private void OnYoYoComplete()
	{
		transform.DOKill();

		if ( localMovement )
		{
			transform.DOLocalMove( _startingPosition + deltaMax, _duration * 0.5f ).
				SetEase( ease ).OnComplete( StartYoYo );
		}
		else
		{
			transform.DOMove( _startingPosition + deltaMax, _duration * 0.5f ).
				SetEase( ease ).OnComplete( StartYoYo );
		}
	}

}

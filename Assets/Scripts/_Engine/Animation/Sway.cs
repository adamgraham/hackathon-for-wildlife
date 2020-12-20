using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Sway : MonoBehaviour
{
	public Vector3 swayTo;

	public float durationMin = 5.0f;
	public float durationMax = 10.0f;

	private Vector3 _swayFrom;
	private float _duration;

	private void Start()
	{
		_swayFrom = transform.eulerAngles;
	}

	private void OnEnable()
	{
		StartSway();
	}

	private void OnDisable()
	{
		transform.DOKill();
	}

	private void StartSway()
	{
		_duration = Random.Range( durationMin, durationMax );

		transform.DOKill();
		transform.DORotate( swayTo, _duration * 0.5f ).
			SetEase( Ease.InOutSine ).OnComplete( OnSwayToComplete );
	}

	private void OnSwayToComplete()
	{
		transform.DORotate( _swayFrom, _duration * 0.5f ).
			SetEase( Ease.InOutSine ).OnComplete( StartSway );
	}

}

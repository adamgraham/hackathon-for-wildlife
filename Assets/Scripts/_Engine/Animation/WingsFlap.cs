using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WingsFlap : MonoBehaviour
{
	public Transform wingLeft;
	public Transform wingRight;

	public Vector3 deltaMin = new Vector3( 20.0f, 20.0f, -20.0f );
	public Vector3 deltaMax = new Vector3( -20.0f, -20.0f, 20.0f );

	public Ease easeDown = Ease.InOutSine;
	public Ease easeUp = Ease.InOutSine;

	public float durationDown = 0.6f;
	public float durationUp = 0.5f;

	public float durationFluctuationMin = -0.05f;
	public float durationFluctuationMax = 0.10f;

	public bool autoStartAndStop = true;
	public bool localMovement = true;

	private Vector3 _startingRotationLeft;
	private Vector3 _startingRotationRight;
	private Vector3 _rotationLeft;
	private Vector3 _rotationRight;

	private float _durationFluctuation;

	private void Awake()
	{
		_rotationLeft = new Vector3();
		_rotationRight = new Vector3();
	}

	private void Start()
	{
		if ( wingLeft != null )
			_startingRotationLeft = (localMovement) ? wingLeft.localEulerAngles : wingLeft.eulerAngles;

		if ( wingRight != null )
			_startingRotationRight = (localMovement) ? wingRight.localEulerAngles : wingRight.eulerAngles;

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
		if ( wingLeft != null )
			wingLeft.DOKill();

		if ( wingRight != null )
			wingRight.DOKill();
	}

	public void StartYoYo()
	{
		StopYoYo();

		if ( durationUp > 0.0f )
		{
			_durationFluctuation = Random.Range( durationFluctuationMin, durationFluctuationMax );

			_rotationLeft.x = _startingRotationLeft.x + deltaMin.x;
			_rotationLeft.y = _startingRotationLeft.y + deltaMin.y;
			_rotationLeft.z = _startingRotationLeft.z + deltaMin.z;

			_rotationRight.x = _startingRotationRight.x + deltaMin.x;
			_rotationRight.y = _startingRotationRight.y - deltaMin.y;
			_rotationRight.z = _startingRotationRight.z - deltaMin.z;

			float duration = durationUp + _durationFluctuation;

			if ( localMovement )
			{
				if ( wingLeft != null )
					wingLeft.DOLocalRotate( _rotationLeft, duration ).
						SetEase( easeUp );

				if ( wingRight != null )
					wingRight.DOLocalRotate( _rotationRight, duration ).
						SetEase( easeUp ).OnComplete( OnYoYoComplete );
			}
			else
			{
				if ( wingLeft != null )
					wingLeft.DORotate( _rotationLeft, duration ).
						SetEase( easeUp );

				if ( wingRight != null )
					wingRight.DORotate( _rotationRight, duration ).
						SetEase( easeUp ).OnComplete( OnYoYoComplete );
			}
		}
	}

	private void OnYoYoComplete()
	{
		if ( durationDown > 0.0f )
		{
			_rotationLeft.x = _startingRotationLeft.x + deltaMax.x;
			_rotationLeft.y = _startingRotationLeft.y + deltaMax.y;
			_rotationLeft.z = _startingRotationLeft.z + deltaMax.z;

			_rotationRight.x = _startingRotationRight.x + deltaMax.x;
			_rotationRight.y = _startingRotationRight.y - deltaMax.y;
			_rotationRight.z = _startingRotationRight.z - deltaMax.z;

			float duration = durationDown + _durationFluctuation;

			if ( localMovement )
			{
				if ( wingLeft != null )
					wingLeft.DOLocalRotate( _rotationLeft, duration ).
						SetEase( easeDown );

				if ( wingRight != null )
					wingRight.DOLocalRotate( _rotationRight, duration ).
						SetEase( easeDown ).OnComplete( StartYoYo );
			}
			else
			{
				if ( wingLeft != null )
					wingLeft.DORotate( _rotationLeft, duration ).
						SetEase( easeDown );

				if ( wingRight != null )
					wingRight.DORotate( _rotationRight, duration ).
						SetEase( easeDown ).OnComplete( StartYoYo );
			}
		}
	}

}

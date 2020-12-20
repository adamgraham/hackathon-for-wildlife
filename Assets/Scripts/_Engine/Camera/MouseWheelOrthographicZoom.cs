using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent( typeof( Camera ) )]
public class MouseWheelOrthographicZoom : MonoBehaviour 
{
	public float zoomMin = 4.0f;
	public float zoomMax = 20.0f;
	public float zoomSpeed = 1.0f;
	public float zoomDampTime = 0.5f;
	public Ease zoomEase = Ease.InOutSine;

	private Camera _camera;
	private Tween _tween;
	private float _targetZoom;

	private void Awake()
	{
		_camera = gameObject.GetComponent<Camera>();
		_camera.orthographicSize = Mathf.Clamp( _camera.orthographicSize, zoomMin, zoomMax );
		_targetZoom = _camera.orthographicSize;
	}

	public void Update()
	{
		float scrollDelta = Input.GetAxis( "Mouse ScrollWheel" );

		if ( scrollDelta != 0.0f )
		{
			if ( _tween != null )
				_tween.Kill();

			_targetZoom = Mathf.Clamp( _targetZoom - (scrollDelta * zoomSpeed), zoomMin, zoomMax );
			_tween = DOTween.To( () => _camera.orthographicSize, x => _camera.orthographicSize = x, _targetZoom, zoomDampTime ).
				SetEase( zoomEase );
		}
	}

}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraOperator : MonoBehaviour
{
	#region Variables

	static public float dampTime = 0.5f;

	static public Vector3 cameraOffset = Vector3.zero;

	static public bool clampPosition = false;
	static public Vector3 clampPositionMin = new Vector3( float.MinValue, float.MinValue, float.MinValue );
	static public Vector3 clampPositionMax = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue );

	static public bool constantHeight = true;

	static private Camera _camera;

	static private Transform _focus;
	static private Transform _averagingA;
	static private Transform _averagingB;

	static private Vector3 _position;
	static private Vector3 _velocity;

	static private float _zoomOriginal;

	static private bool _following;
	static private bool _averaging;

	static private CameraOperator _instance;
	static private CameraOperator instance
	{
		get
		{
			CameraOperator script = _instance;

			if ( script == null )
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "Camera Operator";

				script = gameObject.AddComponent<CameraOperator>();

				_camera = Camera.main;
			}

			return script;
		}
	}

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( _instance == null )
		{
			_instance = this;
			_instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
			_instance.gameObject.AddComponent<ScreenResizeEvent>();

			if ( _camera == null )
				SetActiveCamera( Camera.main );
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
			_camera = null;
			_focus = null;
			_averagingA = null;
			_averagingB = null;
		}
	}

	#endregion

	#region General

	static public void SetActiveCamera( Camera camera )
	{
		if ( camera != null && camera != _camera )
		{
			if ( _camera != null )
			{
				_camera.DOKill();
				_camera.orthographicSize = _zoomOriginal;
			}

			_camera = camera;
			StoreZoomOrthographic();
		}
	}

	static public Camera GetActiveCamera()
	{
		if ( CameraOperator.instance != null )
			return _camera;

		return null;
	}

	#endregion

	#region Movement

	private void FixedUpdate()
	{
		if ( _following )
		{
			if ( _focus != null )
				MoveTo( dampTime, _focus.transform.position );
		}
		else if ( _averaging )
		{
			if ( _averagingA != null && _averagingB != null )
			{
				if ( _camera != null )
				{
					Vector3 averagePos = _averagingA.position + ((_averagingB.position - _averagingA.position) * 0.5f);
					Vector3 point = _camera.WorldToViewportPoint( averagePos );
					Vector3 delta = averagePos - _camera.ViewportToWorldPoint( new Vector3( 0.5f, 0.5f, point.z ) );
					Vector3 target = _camera.transform.position + delta + cameraOffset;

					if ( clampPosition )
						target = MathUtils.ClampVector( target, clampPositionMin, clampPositionMax );

					_position = Vector3.SmoothDamp( _camera.transform.position, target, ref _velocity, dampTime );

					if ( constantHeight )
						_position.y = _camera.transform.position.y;

					_camera.transform.position = _position;
				}
			}
		}
	}

	private void MoveTo( float smoothTime )
	{
		MoveTo( smoothTime, _focus.transform.position );
	}

	private void MoveTo( float smoothTime, Vector3 position )
	{
		if ( _camera != null )
		{
			_position = Vector3.SmoothDamp( _camera.transform.position, GetCameraTargetPosition( position ), ref _velocity, smoothTime );

			if ( constantHeight )
				_position.y = _camera.transform.position.y;

			_camera.transform.position = _position;
		}
	}

	private Vector3 GetCameraTargetPosition( Vector3 endPoint )
	{
		Vector3 point = _camera.WorldToViewportPoint( endPoint );
		Vector3 delta = endPoint - _camera.ViewportToWorldPoint( new Vector3( 0.5f, 0.5f, point.z ) );
		Vector3 target = _camera.transform.position + delta + cameraOffset;

		if ( clampPosition )
			target = MathUtils.ClampVector( target, clampPositionMin, clampPositionMax );

		return target;
	}

	#endregion

	#region Zooming

	static public void ZoomOrthographic( float deltaZoom, float duration, Ease ease = Ease.InOutQuad )
	{
		if ( CameraOperator.instance != null )
		{
			if ( _camera != null )
			{
				_camera.DOKill();

				DOTween.To( () => _camera.orthographicSize, x => _camera.orthographicSize = x, _zoomOriginal + deltaZoom, duration ).
					SetId( _camera ).SetEase( ease );
			}
		}
	}

	static public void UnZoomOrthographic( float duration, Ease ease = Ease.InOutQuad )
	{
		if ( CameraOperator.instance != null )
		{
			if ( _camera != null )
			{
				_camera.DOKill();

				DOTween.To( () => _camera.orthographicSize, x => _camera.orthographicSize = x, _zoomOriginal, duration ).
					SetId( _camera ).SetEase( ease );
			}
		}
	}

	static public void StoreZoomOrthographic()
	{
		if ( _camera != null )
			_zoomOriginal = _camera.orthographicSize;
	}

	#endregion

	#region Tracking

	static public void FollowFocusObject( Transform newFocus = null )
	{
		if ( CameraOperator.instance != null )
		{
			if ( newFocus != null )
				SetFocusObject( newFocus );

			UnaverageBetweenObjects();

			_following = true;
		}
	}

	static public void UnfollowFocusObject()
	{
		_following = false;
	}

	static public void UnfollowFocusObject( Transform obj )
	{
		if ( IsFocusingObject( obj ) )
			UnfollowFocusObject();
	}

	static public void PanToFocus( float duration, float delay = 0.0f, Ease ease = Ease.InOutCubic, DG.Tweening.Core.TweenCallback onComplete = null )
	{
		CameraOperator script = instance;

		if ( script != null )
		{
			if ( _focus != null && _camera != null )
			{
				UnfollowFocusObject();
				UnaverageBetweenObjects();

				_camera.transform.DOKill();
				_camera.transform.DOMove( script.GetCameraTargetPosition( _focus.position ), duration ).
					SetDelay( delay ).SetEase( ease ).OnComplete( onComplete );
			}
		}
	}

	static public void PanToFocus( Transform newFocus, float duration, float delay = 0.0f, Ease ease = Ease.InOutCubic, DG.Tweening.Core.TweenCallback onComplete = null )
	{
		SetFocusObject( newFocus );
		PanToFocus( duration, delay, ease, onComplete );
	}

	static public void SnapToFocus( Transform newFocus = null )
	{
		if ( newFocus != null )
			SetFocusObject( newFocus );

		instance.MoveTo( 0.0f );
	}

	static public void LookAtFocus( Transform newFocus = null )
	{
		if ( newFocus != null )
			SetFocusObject( newFocus );

		if ( _focus != null && _camera != null )
			_camera.transform.LookAt( _focus );
	}

	static public void SetFocusObject( Transform obj )
	{
		_focus = obj;
	}

	static public GameObject GetFocusObject()
	{
		return (_focus != null) ? _focus.gameObject : null;
	}

	static public bool IsFocusingObject( Transform obj )
	{
		return _focus == obj;
	}

	#endregion

	#region Averaging

	static public void AverageBetweenObjects( Transform objA, Transform objB )
	{
		if ( CameraOperator.instance != null )
		{
			if ( objA != null && objB != null )
			{
				UnfollowFocusObject();

				_averagingA = objA;
				_averagingB = objB;
				_averaging = true;
			}
		}
	}

	static public void UnaverageBetweenObjects()
	{
		_averagingA = null;
		_averagingB = null;
		_averaging = false;
	}

	static public void UnaverageBetweenObjects( Transform obj )
	{
		if ( IsAveragingObject( obj ) )
			UnaverageBetweenObjects();
	}

	static public GameObject GetAveragingObjectA()
	{
		return (_averagingA != null) ? _averagingA.gameObject : null;
	}

	static public GameObject GetAveragingObjectB()
	{
		return (_averagingB != null) ? _averagingB.gameObject : null;
	}

	static public bool IsAveragingObject( Transform obj )
	{
		return _averagingA == obj || _averagingB == obj;
	}

	#endregion

	#region Fixed Point

	static public void SetFixedPoint( Vector3 position )
	{
		UnfollowFocusObject();
		UnaverageBetweenObjects();

		instance.MoveTo( 0.0f, position );
	}

	#endregion

	#region Attaching

	static public void AttachTo( Transform transform, bool resetLocalPosition = true )
	{
		if ( CameraOperator.instance != null )
		{
			if ( _camera != null )
			{
				_camera.transform.parent = transform;

				if ( resetLocalPosition && _camera != null )
					_camera.transform.localPosition = Vector3.zero;
			}
		}
	}

	#endregion

}

#region Camera Shots

[System.Serializable]
public class CameraTrackingShot
{
	public Transform startPoint;
	public Transform endPoint;
	public float duration;
}

#endregion

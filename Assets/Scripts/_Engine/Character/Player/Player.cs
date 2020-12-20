using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Player : Character
{
	#region Variables

	static public Player instance;

	[Header( "Camera" )]

	public Player.CameraScheme cameraScheme;
	public enum CameraScheme { FirstPerson, ThirdPerson, Overhead };

	public Transform cameraFocus;
	public bool cameraFollow;

	public float cameraHeight = 25.0f;
	public float cameraDampTime = 0.1f;
	[Range( 60.0f, 110.0f )]
	public float cameraFOV = 60.0f;
	[Range( -90.0f + CAMERA_GIMBAL_SAFEGUARD, 90.0f - CAMERA_GIMBAL_SAFEGUARD )]
	public float cameraAngle = 20.0f;
	public float cameraDistance = 5.0f;

	public GameObject crosshair;

	[Range( 0.0f, 90.0f )]
	public const float CAMERA_GIMBAL_SAFEGUARD = 5.0f;

	[Header( "Movement" )]

	public Player.MovementScheme movementScheme;
	public enum MovementScheme { Axis, Cardinal, ForwardOnly, Locked };

	public Player.MovementAxisPriority movementDirectionPriority;
	public enum MovementAxisPriority { Vertical, Horizontal };

	public bool movementDiagonalsAllowed;
	public bool movementDiagonalsOnly;

	public bool canJump;
	public bool canRun;
	public bool canWalk;
	public bool canCrouch;

	public float speedRunMultiplier = 1.0f;
	public float speedWalkMultiplier = 1.0f;
	public float speedCrouchMultiplier = 1.0f;

	public float jumpForce;
	public float doubleJumpForce;
	public int doubleJumps;

	public float crouchLookPositionY;
	public float crouchTweenDuration;

	protected Transform _movementForwardTransform;
	protected Vector3 _movementDirection;

	protected int _doubleJumps;

	private float _crouchLookPositionY;

	[Header( "Look Rotation" )]

	public Player.LookScheme lookScheme;
	public enum LookScheme { Axis, Cardinal, Locked };

	public Transform lookTransform;

	[Range( 1.0f, 10.0f )]
	public float lookSensitivity = 5.0f;
	[Range( 0.0f, 1.0f )]
	public float lookMovementMultiplier;
	public bool lookDiagonalsAllowed;
	public bool lookDiagonalsOnly;

	protected Vector3 _lookDirection;
	protected Vector3 _lookDirectionPrevious;

	protected float _lookSensitivity;

	[Header( "Controls" )]

	public PlayerControls controls;

	private delegate void MovementSchemeMethod();
	private MovementSchemeMethod _movementMethod;

	private delegate void LookSchemeMethod();
	private LookSchemeMethod _lookMethod;

	public bool mouseDisabled;
	private bool _mouseDisabled;

	#endregion

	#region Unity Events

	protected override void OnAwake()
	{
		if ( instance == null )
		{
			instance = this;
			base.OnAwake();
		}
		else
		{
			DestroyImmediate( gameObject );
		}
	}

	protected override void OnDispose()
	{
		base.OnDispose();

		cameraFocus = null;
		crosshair = null;
		lookTransform = null;
		_movementForwardTransform = null;

		if ( instance == this )
			instance = null;
	}

	#endregion

	#region Update

	protected override void OnUpdate()
	{
		_movementMethod();
		_lookMethod();

		CheckForSecondaryMovement();
	}

	protected override void OnFixedUpdate()
	{
		MoveRigidbody( _movementDirection, _movementForwardTransform );
	}

	#endregion

	#region Control Schemes & Input

	public void ApplyControlSchemes()
	{
		_movementForwardTransform = null;

		if ( movementScheme == MovementScheme.Axis )
		{
			_movementMethod = AxisMovementScheme;
			_movementForwardTransform = GetLookTransform();
		}
		else if ( movementScheme == MovementScheme.Cardinal )
		{
			_movementMethod = CardinalMovementScheme;
		}
		else if ( movementScheme == MovementScheme.ForwardOnly )
		{
			_movementMethod = ForwardOnlyMovementScheme;
			_movementForwardTransform = GetLookTransform();
		}
		else if ( movementScheme == MovementScheme.Locked )
		{
			_movementMethod = LockedMovementScheme;
		}

		if ( lookScheme == LookScheme.Axis )
			_lookMethod = AxisLookScheme;
		else if ( lookScheme == LookScheme.Cardinal )
			_lookMethod = CardinalLookScheme;
		else if ( lookScheme == LookScheme.Locked )
			_lookMethod = LockedLookScheme;

		RecalculateLookSensitivity();
	}

	protected void CheckForSecondaryMovement()
	{
		if ( canJump )
			if ( Input.GetButtonDown( controls.jump ) )
				Jump( jumpForce );

		if ( canRun )
		{
			if ( Input.GetButtonDown( controls.run ) )
				Run( speedRunMultiplier );
			else if ( Input.GetButtonUp( controls.run ) )
				StopRunning();
		}

		if ( canWalk )
		{
			if ( Input.GetButtonDown( controls.walk ) )
				Walk( speedWalkMultiplier );
			else if ( Input.GetButtonUp( controls.walk ) )
				StopWalking();
		}

		if ( canCrouch )
		{
			if ( Input.GetButtonDown( controls.crouch ) )
				Crouch( speedCrouchMultiplier );
			else if ( Input.GetButtonUp( controls.crouch ) )
				StopCrouching();
		}
	}

	#region Movement Schemes

	protected void AxisMovementScheme()
	{
		_movementDirection.x = 0.0f;
		_movementDirection.y = 0.0f;
		_movementDirection.z = 0.0f;

		if ( Input.GetButton( controls.moveUp ) ) { _movementDirection.z = 1.0f; }
		else if ( Input.GetButton( controls.moveDown ) ) { _movementDirection.z = -1.0f; }
		else _movementDirection.z = Input.GetAxis( controls.movementAxisVertical );

		if ( Input.GetButton( controls.moveRight ) ) { _movementDirection.x = 1.0f; }
		else if ( Input.GetButton( controls.moveLeft ) ) { _movementDirection.x = -1.0f; }
		else _movementDirection.x = Input.GetAxis( controls.movementAxisHorizontal );
	}

	protected void CardinalMovementScheme()
	{
		_movementDirection.x = 0.0f;
		_movementDirection.y = 0.0f;
		_movementDirection.z = 0.0f;

		if ( movementDiagonalsAllowed )
		{
			if ( Input.GetButton( controls.moveUp ) ) { _movementDirection.z = 1.0f; }
			else if ( Input.GetButton( controls.moveDown ) ) { _movementDirection.z = -1.0f; }

			if ( Input.GetButton( controls.moveRight ) ) { _movementDirection.x = 1.0f; }
			else if ( Input.GetButton( controls.moveLeft ) ) { _movementDirection.x = -1.0f; }

			if ( movementDiagonalsOnly )
			{
				// insure both x and z are not zero
				// if one of them is zero then it is not a diagonal look rotation

				if ( _movementDirection.x == 0.0f || _movementDirection.z == 0.0f )
				{
					_movementDirection.x = 0.0f;
					_movementDirection.z = 0.0f;
				}
			}
		}
		else
		{
			if ( movementDirectionPriority == MovementAxisPriority.Vertical )
			{
				// vertical axis Input first

				if ( Input.GetButton( controls.moveUp ) ) { _movementDirection.z = 1.0f; _movementDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.moveDown ) ) { _movementDirection.z = -1.0f; _movementDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.moveRight ) ) { _movementDirection.x = 1.0f; _movementDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.moveLeft ) ) { _movementDirection.x = -1.0f; _movementDirection.z = 0.0f; }
			}
			else
			{
				// horizontal axis Input first

				if ( Input.GetButton( controls.moveRight ) ) { _movementDirection.x = 1.0f; _movementDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.moveLeft ) ) { _movementDirection.x = -1.0f; _movementDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.moveUp ) ) { _movementDirection.z = 1.0f; _movementDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.moveDown ) ) { _movementDirection.z = -1.0f; _movementDirection.x = 0.0f; }
			}
		}
	}

	protected void ForwardOnlyMovementScheme()
	{
		_movementDirection.x = 0.0f;
		_movementDirection.y = 0.0f;
		_movementDirection.z = 0.0f;

		if ( Input.GetButton( controls.moveUp ) ) { _movementDirection.z = 1.0f; }
		else if ( Input.GetButton( controls.moveDown ) ) { _movementDirection.z = -1.0f; }
		else _movementDirection.z = Input.GetAxis( controls.movementAxisVertical );
	}

	protected void LockedMovementScheme()
	{
		// do nothing
	}

	#endregion

	#region Look Schemes

	protected void AxisLookScheme()
	{
		float axisY = Input.GetAxis( controls.lookAxisVertical );
		float axisX = Input.GetAxis( controls.lookAxisHorizontal );

		if ( !MathUtils.IsZero( axisY ) ) 
			_lookDirection.z = Mathf.Clamp( _lookDirection.z + (axisY * _lookSensitivity), -90.0f + CAMERA_GIMBAL_SAFEGUARD, 90.0f - CAMERA_GIMBAL_SAFEGUARD );

		if ( !MathUtils.IsZero( axisX ) ) 
			_lookDirection.x += (axisX * _lookSensitivity);

		SetFreeLookRotation();
	}

	protected void CardinalLookScheme()
	{
		_lookDirection.x = 0.0f;
		_lookDirection.y = 0.0f;
		_lookDirection.z = 0.0f;

		if ( lookDiagonalsAllowed )
		{
			if ( Input.GetButton( controls.lookUp ) ) { _lookDirection.z = 1.0f; }
			else if ( Input.GetButton( controls.lookDown ) ) { _lookDirection.z = -1.0f; }

			if ( Input.GetButton( controls.lookRight ) ) { _lookDirection.x = 1.0f; }
			else if ( Input.GetButton( controls.lookLeft ) ) { _lookDirection.x = -1.0f; }

			if ( lookDiagonalsOnly )
			{
				// insure both x and z are not zero
				// if one of them is zero then it is not a diagonal look rotation

				if ( _lookDirection.x == 0.0f || _lookDirection.z == 0.0f )
				{
					_lookDirection.x = 0.0f;
					_lookDirection.z = 0.0f;
				}
			}
		}
		else
		{
			if ( movementDirectionPriority == MovementAxisPriority.Vertical )
			{
				// vertical axis Input first

				if ( Input.GetButton( controls.lookUp ) ) { _lookDirection.z = 1.0f; _lookDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.lookDown ) ) { _lookDirection.z = -1.0f; _lookDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.lookRight ) ) { _lookDirection.x = 1.0f; _lookDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.lookLeft ) ) { _lookDirection.x = -1.0f; _lookDirection.z = 0.0f; }
			}
			else
			{
				// horizontal axis Input first

				if ( Input.GetButton( controls.lookRight ) ) { _lookDirection.x = 1.0f; _lookDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.lookLeft ) ) { _lookDirection.x = -1.0f; _lookDirection.z = 0.0f; }
				else if ( Input.GetButton( controls.lookUp ) ) { _lookDirection.z = 1.0f; _lookDirection.x = 0.0f; }
				else if ( Input.GetButton( controls.lookDown ) ) { _lookDirection.z = -1.0f; _lookDirection.x = 0.0f; }
			}
		}

		if ( MathUtils.IsZero( _lookDirection ) )
		{
			_lookDirection.x = _lookDirectionPrevious.x;
			_lookDirection.y = _lookDirectionPrevious.y;
			_lookDirection.z = _lookDirectionPrevious.z;
		}
		else
		{
			_lookDirectionPrevious.x = _lookDirection.x;
			_lookDirectionPrevious.y = _lookDirection.y;
			_lookDirectionPrevious.z = _lookDirection.z;
		}

		SetCardinalLookRotation();
	}

	protected void LockedLookScheme()
	{
		// do nothing
	}

	#endregion

	#endregion

	#region Camera

	public Transform GetCameraFocus()
	{
		return (cameraFocus != null) ? cameraFocus : transform;
	}

	public void CameraFollowFocus()
	{
		CameraOperator.dampTime = cameraDampTime;
		CameraOperator.FollowFocusObject( GetCameraFocus() );
	}

	public void CameraSnapToFocus()
	{
		CameraOperator.SnapToFocus( GetCameraFocus() );
	}

	public void CameraLookAtFocus()
	{
		CameraOperator.LookAtFocus( GetCameraFocus() );
	}

	protected void ApplyCameraSettings()
	{
		Camera playerCamera = Camera.main;
		CameraOperator.SetActiveCamera( playerCamera );

		if ( crosshair != null )
			crosshair.gameObject.SetActive( false );

		if ( cameraScheme == CameraScheme.FirstPerson || cameraScheme == CameraScheme.ThirdPerson )
		{
			playerCamera.fieldOfView = cameraFOV;

			CameraOperator.AttachTo( GetLookTransform() );
			playerCamera.transform.localRotation = Quaternion.identity;

			if ( cameraScheme == CameraScheme.ThirdPerson )
			{
				playerCamera.transform.localEulerAngles = new Vector3( cameraAngle, 0.0f, 0.0f );
				playerCamera.transform.localPosition = playerCamera.transform.forward * cameraDistance;
			}
			else
			{
				if ( crosshair != null )
					crosshair.gameObject.SetActive( true );
			}
		}
		else if ( cameraScheme == CameraScheme.Overhead )
		{
			if ( cameraFollow )
			{
				CameraLookAtFocus();
				CameraFollowFocus();
			}

			playerCamera.transform.position = new Vector3( playerCamera.transform.position.x, cameraHeight, playerCamera.transform.position.z );
		}
	}

	#endregion

	#region Movement

	public override void Jump( float force )
	{
		if ( !_jumping )
		{
			base.Jump( force );
		}
		else if ( _jumping && _doubleJumps < doubleJumps )
		{
			base.Jump( doubleJumpForce );
			_doubleJumps++;
		}
	}

	protected override void FinishJump()
	{
		base.FinishJump();
		_doubleJumps = 0;
	}

	public override bool Crouch( float speedMultiplier = 1.0f )
	{
		if ( base.Crouch( speedMultiplier ) )
		{
			Transform transform = GetLookTransform();
			_crouchLookPositionY = transform.localPosition.y;

			if ( crouchTweenDuration <= 0.0f )
				transform.localPosition = new Vector3( transform.localPosition.x, crouchLookPositionY, transform.localPosition.z );
			else
				transform.DOLocalMoveY( crouchLookPositionY, crouchTweenDuration );

			return true;
		}

		return false;
	}

	public override bool StopCrouching()
	{
		if ( base.StopCrouching() )
		{
			Transform transform = GetLookTransform();

			if ( crouchTweenDuration <= 0.0f )
				transform.localPosition = new Vector3( transform.localPosition.x, _crouchLookPositionY, transform.localPosition.z );
			else
				transform.DOLocalMoveY( _crouchLookPositionY, crouchTweenDuration );

			return true;
		}

		return false;
	}

	#endregion

	#region Look Transform

	public Transform GetLookTransform()
	{
		return (lookTransform != null) ? lookTransform : transform;
	}

	protected void SetFreeLookRotation()
	{
		vector.x = Mathf.Clamp( -_lookDirection.z, -90.0f + CAMERA_GIMBAL_SAFEGUARD, 90.0f - CAMERA_GIMBAL_SAFEGUARD );
		vector.y = _lookDirection.x;
		vector.z = 0.0f;

		GetLookTransform().rotation = Quaternion.Euler( vector );
	}

	protected void SetCardinalLookRotation()
	{
		float degrees = (-Mathf.Atan2( _lookDirection.z + (_movementDirection.z * lookMovementMultiplier),
									   _lookDirection.x + (_movementDirection.x * lookMovementMultiplier) ) * Mathf.Rad2Deg) + 90.0f;

		vector.x = 0.0f;
		vector.y = degrees;
		vector.z = 0.0f;

		GetLookTransform().rotation = Quaternion.Euler( vector );
	}

	public float RecalculateLookSensitivity()
	{
		// f(x) = 3/5x - 0.5
		_lookSensitivity = (0.6f * lookSensitivity) - 0.5f;
		return _lookSensitivity;
	}

	#endregion

	#region Health System

	protected override void OnKilled( int lives )
	{
		Despawn();
	}

	#endregion

	#region Spawning

	protected override void OnSpawn()
	{
		DisableNavMeshMovement();
		ApplyControlSchemes();
		ApplyCameraSettings();

		_mouseDisabled = mouseDisabled;
	}

	protected override void OnDespawn()
	{
		CameraOperator.UnfollowFocusObject();
	}

	#endregion

}

#region Player Controls

[System.Serializable]
public class PlayerControls
{
	[Header( "Movement" )]

	public string moveUp = "Up";
	public string moveDown = "Down";
	public string moveRight = "Right";
	public string moveLeft = "Left";

	public string movementAxisVertical = "Vertical";
	public string movementAxisHorizontal = "Horizontal";

	[Header( "Look Rotation" )]

	public string lookUp = "LookUp";
	public string lookDown = "LookDown";
	public string lookRight = "LookRight";
	public string lookLeft = "LookLeft";

	public string lookAxisVertical = "LookVertical";
	public string lookAxisHorizontal = "LookHorizontal";

	[Header( "Secondary Movement" )]

	public string jump = "Jump";
	public string run = "Run";
	public string walk = "Walk";
	public string crouch = "Crouch";
}

#endregion

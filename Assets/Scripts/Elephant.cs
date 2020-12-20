using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Elephant : MonoBehaviour 
{
	#region Variables

	static public Elephant instance;

	public GameObject damageBloodEffectPrefab;

	private EnvironmentCube _currentCube;
	private EnvironmentCube _previousCube;

	private int _health;
	private bool _canMove;

	static private readonly float MOVEMENT_ANIMATION_DURATION = 0.35f;
	static private readonly float MOVEMENT_ANIMATION_ROTATION_DURATION = 0.5f;
	static private readonly float MOVEMENT_COOLDOWN = 0.15f;
	static private readonly int MOVEMENT_CUBE_DISTANCE = 2;
	static private readonly int MAX_HEALTH = 3;

	#endregion

	#region Unity Events

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if ( instance == this )
			instance = null;
	}

	private void Update() 
	{
		if ( _canMove )
		{
			if ( Input.GetKey( KeyCode.W ) || Input.GetKey( KeyCode.UpArrow ) )
				MoveUp();
			else if ( Input.GetKey( KeyCode.S ) || Input.GetKey( KeyCode.DownArrow ) )
				MoveDown();
			else if ( Input.GetKey( KeyCode.A ) || Input.GetKey( KeyCode.LeftArrow ) )
				MoveLeft();
			else if ( Input.GetKey( KeyCode.D ) || Input.GetKey( KeyCode.RightArrow ) )
				MoveRight();
		}
	}

	#endregion

	#region Spawning

	public void Spawn( EnvironmentCube cube )
	{
		gameObject.SetActive( true );
		transform.position = cube.GetCoordinates().GetWorldPosition();
		Camera.main.transform.position = new Vector3( transform.position.x, Camera.main.transform.position.y, transform.position.z );

		_currentCube = cube;
		_canMove = true;
		_health = MAX_HEALTH;

		CameraOperator.SetFocusObject( transform );
		CameraOperator.SnapToFocus();
		CameraOperator.FollowFocusObject();
	}

	#endregion

	#region Movement

	public GridCoordinates GetCoordinates()
	{
		return _currentCube.GetCoordinates();
	}

	private void MoveUp() 
	{
		MoveToCube( World.instance.GetAdjacentNorth( _currentCube, true, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 180.0f, 0.0f ) );
	}

	private void MoveDown() 
	{
		MoveToCube( World.instance.GetAdjacentSouth( _currentCube, true, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 0.0f, 0.0f ) );
	}

	private void MoveLeft() 
	{
		MoveToCube( World.instance.GetAdjacentWest( _currentCube, true, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 90.0f, 0.0f ) );
	}

	private void MoveRight() 
	{
		MoveToCube( World.instance.GetAdjacentEast( _currentCube, true, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 270.0f, 0.0f ) );
	}

	private void MoveToCube( EnvironmentCube cube, Vector3 rotation )
	{
		if ( cube != null && cube != _currentCube )
		{
			_previousCube = _currentCube;
			_currentCube = cube;
			_canMove = false;

			if ( MathUtils.IsNotEqual( transform.eulerAngles, rotation, 1.0f ) )
			{
				transform.DOKill();
				transform.DORotate( rotation, MOVEMENT_ANIMATION_ROTATION_DURATION ).
					OnComplete( StartMovementAnimation );
			} 
			else 
			{
				StartMovementAnimation();
			}
		}
	}

	private void StartMovementAnimation()
	{
		// x/z movement

		transform.DOKill();
		transform.DOMove( _currentCube.GetCoordinates().GetWorldPosition(), MOVEMENT_ANIMATION_DURATION ).
			OnComplete( OnMovementComplete );

		// y movement (hopping)

		float halfDuration = MOVEMENT_ANIMATION_DURATION * 0.5f;
		float hopHeight = 0.25f;
		if ( _previousCube != null && _previousCube.cubeType == EnvironmentCube.CubeType.Water && _currentCube.cubeType != EnvironmentCube.CubeType.Water )
			hopHeight = 0.75f;
		float yPositionOffset = (_currentCube.cubeType != EnvironmentCube.CubeType.Water) ? 0.0f : 0.5f;

		transform.DOLocalMoveY( transform.localPosition.y + hopHeight, halfDuration );
		transform.DOLocalMoveY( 0.00f - yPositionOffset, halfDuration ).
			SetDelay( halfDuration );
	}

	private void OnMovementComplete()
	{
		CancelInvoke( "OnMovementCooldownComplete" );
		Invoke( "OnMovementCooldownComplete", MOVEMENT_COOLDOWN );
	}

	private void OnMovementCooldownComplete()
	{
		_canMove = true;
	}

	#endregion

	#region Health System

	public void Damage( int damage )
	{
		_health -= damage;

		if ( _health <= 0 )
			GameManager.instance.GameOver();

		GameObject.Instantiate( damageBloodEffectPrefab, transform.position, Quaternion.identity );
	}

	#endregion

}

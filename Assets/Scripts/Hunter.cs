using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Hunter : MonoBehaviour 
{
	#region Variables

	static public Hunter instance;

	public Spear spearPrefab;

	private EnvironmentCube _currentCube;
	private EnvironmentCube _previousCube;
	private bool _movementEnabled;
	private bool _canMove;
	private bool _canAttack;
	private bool _aggro;
	
	static readonly float MOVEMENT_ANIMATION_DURATION = 0.25f;
	static readonly float MOVEMENT_ANIMATION_ROTATION_DURATION = 0.25f;
	static readonly float MOVEMENT_COOLDOWN = 0.15f;
	static readonly int MOVEMENT_CUBE_DISTANCE = 1;

	static readonly float AGGRO_RANGE = 15.0f;
	static readonly float ATTACK_RANGE = 8.0f;
	static readonly float ATTACK_COOLDOWN = 2.0f;

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
		if ( _canAttack ) 
		{
			float absDistance = Mathf.Abs( Vector3.Distance( transform.position, Elephant.instance.transform.position ) );
			if ( absDistance <= ATTACK_RANGE ) 
				Attack();
		}
	}

	#endregion

	#region Spawning

	public void Spawn( EnvironmentCube cube )
	{
		gameObject.SetActive( true );
		transform.position = cube.GetCoordinates().GetWorldPosition();

		_currentCube = cube;
		_canMove = true;
		_canAttack = true;

		BeginMovement();
	}

	#endregion

	#region Movement

	public GridCoordinates GetCoordinates()
	{
		return _currentCube.GetCoordinates();
	}

	public void BeginMovement()
	{
		if ( !_movementEnabled )
		{
			_movementEnabled = true;
			Move();
		}
	}

	public void StopMovement()
	{
		if ( _movementEnabled )
			_movementEnabled = false;
	}

	private void Move()
	{
		if ( _movementEnabled && _canMove )
		{
			if ( Elephant.instance != null )
				_aggro = Vector3.Distance( transform.position, Elephant.instance.transform.position ) < AGGRO_RANGE;

			if ( !_aggro )
			{
				int randomDirection = Random.Range( 0, 4 );

				switch ( randomDirection )
				{
				case 0:
					MoveUp();
					break;
				case 1:
					MoveDown();
					break;
				case 2:
					MoveLeft();
					break;
				case 3:
					MoveRight();
					break;
				}
			} 
			else
			{
				int xDistance = _currentCube.GetCoordinates().x - Elephant.instance.GetCoordinates().x;
				int absXDistance = Mathf.Abs( xDistance );

				int zDistance = _currentCube.GetCoordinates().z - Elephant.instance.GetCoordinates().z;
				int absZDistance = Mathf.Abs( zDistance );

				if ( absXDistance >= absZDistance )
				{
					if ( xDistance >= 0.0f )
						MoveLeft();
					else 
						MoveRight();
				} 
				else 
				{
					if ( zDistance >= 0.0f )
						MoveDown();
					else
						MoveUp();
				}
			}
		}
	}

	private void MoveUp() 
	{
		MoveToCube( World.instance.GetAdjacentNorth( _currentCube, false, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 180.0f, 0.0f ) );
	}
	
	private void MoveDown() 
	{
		MoveToCube( World.instance.GetAdjacentSouth( _currentCube, false, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 0.0f, 0.0f ) );
	}
	
	private void MoveLeft() 
	{
		MoveToCube( World.instance.GetAdjacentWest( _currentCube, false, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 90.0f, 0.0f ) );
	}
	
	private void MoveRight() 
	{
		MoveToCube( World.instance.GetAdjacentEast( _currentCube, false, MOVEMENT_CUBE_DISTANCE ), 
		           new Vector3( 0.0f, 270.0f, 0.0f ) );
	}
	
	private void MoveToCube( EnvironmentCube cube, Vector3 rotation )
	{
		if ( cube != null && cube != _currentCube )
		{
			_previousCube = _currentCube;
			_currentCube = cube;
			_canMove = false;

			if ( transform.eulerAngles != rotation )
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
		else 
		{
			Move();
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
		Move();
	}

	#endregion

	#region Attacking

	private void Attack() 
	{
		if ( _canAttack )
		{
			Spear spear = GameObject.Instantiate<Spear>( spearPrefab );
			spear.transform.position = transform.position;
			spear.direction = Elephant.instance.transform.position - transform.position;
			spear.direction.Normalize();

			_canAttack = false;
			Invoke( "OnAttackCooldownComplete", ATTACK_COOLDOWN );
		}
	}

	private void OnAttackCooldownComplete() 
	{
		_canAttack = true;
	}
	
	#endregion

}

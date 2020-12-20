using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( NavMeshAgent ) )]
[RequireComponent( typeof( HealthSystem ) )]
public class Character : MonoBehaviour, ISpawnable, IKillable, IPauseable
{
	#region Variables

	[Header( "General" )]

	protected bool _initialized;

	static protected Vector3 vector = Vector3.zero;

	protected const float COLLISION_FLOAT_PRECISION = 0.001f;

	[Header( "Spawning" )]

	public bool spawnOnAwake;

	static public Vector3 defaultSpawnPoint = Vector3.zero;

	[Header( "Movement" )]

	public float speed = 1.0f;
	public float speedMultiplier = 1.0f;
	public float speedBackpedalMultplier = 1.0f;

	public bool rigidbodyMovementEnabled;
	public bool navMeshMovementEnabled;

	protected Rigidbody _rigidbody;
	protected NavMeshAgent _navMeshAgent;

	protected bool _jumping;
	protected bool _running;
	protected bool _walking;
	protected bool _crouching;
	protected bool _grounded;

	private float _secondarySpeedMultiplier = 1.0f;
	private bool _useGravity;

	[Header( "Animation" )]

	protected Animator _animator;

	static public string ANIMATOR_BOOL_MOVING = "Moving";

	[Header( "Health System" )]

	protected HealthSystem _healthSystem;

	[Header( "Weapon System" )]

	protected WeaponSystem _weaponSystem;

	[Header( "Spawning" )]

	protected Vector3 _previousSpawnPoint;
	protected bool _spawned;

	public delegate void CharacterSpawnCallback();
	public CharacterSpawnCallback onSpawn;
	public CharacterSpawnCallback onDespawn;

	[Header( "Pausing" )]

	protected bool _paused;

	#endregion

	#region Unity Events

	private void Awake()
	{
		_initialized = true;
		_rigidbody = gameObject.GetComponent<Rigidbody>();
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		_healthSystem = gameObject.GetComponent<HealthSystem>();
		_weaponSystem = gameObject.GetComponentInChildren<WeaponSystem>();
		_animator = gameObject.GetComponentInChildren<Animator>();

		if ( _weaponSystem != null )
			_weaponSystem.character = this;

		OnAwake();

		if ( spawnOnAwake )
			Spawn();
		else
			gameObject.SetActive( false );
	}

	private void Start()
	{
		OnStart();
	}

	private void OnEnable()
	{
		OnEnabled();
	}

	private void OnDisable()
	{
		OnDisabled();
	}

	private void OnDestroy()
	{
		Despawn();
		OnDispose();

		_rigidbody = null;
		_navMeshAgent = null;
		_healthSystem = null;
		_weaponSystem = null;
		_animator = null;

		onSpawn = null;
		onDespawn = null;
	}

	virtual protected void OnAwake()
	{
		// override, if necessary
	}

	virtual protected void OnStart()
	{
		// override, if necessary
	}

	virtual protected void OnEnabled()
	{
		// override, if necessary
	}

	virtual protected void OnDisabled()
	{
		// override, if necessary
	}

	virtual protected void OnDispose()
	{
		// override, if necessary
	}

	protected void StartInvoke( string method, float time )
	{
		CancelInvoke( method );
		Invoke( method, time );
	}

	protected void StartInvokeRepeating( string method, float time, float repeatTime )
	{
		CancelInvoke( method );
		InvokeRepeating( method, time, repeatTime );
	}

	#endregion

	#region Update

	private void Update()
	{
		if ( !_grounded && MathUtils.IsZero( _rigidbody.velocity.y, COLLISION_FLOAT_PRECISION ) )
		{
			if ( _jumping )
				FinishJump();
			else
				_grounded = true;
		}

		OnUpdate();
	}

	private void FixedUpdate()
	{
		OnFixedUpdate();
	}

	private void LateUpdate()
	{
		OnLateUpdate();
	}

	virtual protected void OnUpdate()
	{
		// override, if necessary
	}

	virtual protected void OnFixedUpdate()
	{
		// override, if necessary
	}

	virtual protected void OnLateUpdate()
	{
		// override, if necessary
	}

	#endregion

	#region Movement

	#region Physics Based

	public void MoveRigidbody( Vector3 direction, Transform forwardTransform = null, bool normalizeDiagonalMovement = true )
	{
		if ( rigidbodyMovementEnabled )
		{
			float directionSpeedMultiplier = (normalizeDiagonalMovement) ? MathUtils.CalculateDiagonalMultiplier( direction.x, direction.z ) : 1.0f;
			float mSpeed = CalculateRigidbodyMovementSpeed() * directionSpeedMultiplier;

			if ( direction.z < 0.0f )
				mSpeed *= speedBackpedalMultplier;

			if ( MathUtils.IsNotZero( mSpeed ) )
			{
				if ( forwardTransform != null )
					direction = Quaternion.Euler( 0.0f, forwardTransform.eulerAngles.y, 0.0f ) * direction;

				Vector3 velocity = direction * mSpeed;
				velocity.y = _rigidbody.velocity.y;

				_rigidbody.velocity = velocity;
			}
		}
	}

	public void StopRigidbodyMovement()
	{
		_rigidbody.velocity = Vector3.zero;
	}

	public void EnableRigidbodyMovement()
	{
		_rigidbody.isKinematic = false;
	}

	public void DisableRigidbodyMovement()
	{
		_rigidbody.isKinematic = true;
	}

	virtual public float CalculateRigidbodyMovementSpeed()
	{
		return speed * speedMultiplier * _secondarySpeedMultiplier;
	}

	#endregion

	#region NavMesh Based

	public void WarpToPosition( Vector3 position )
	{
		if ( _navMeshAgent != null )
			if ( _navMeshAgent.isActiveAndEnabled )
				_navMeshAgent.Warp( position );
	}

	public void SetNavMeshDestination( Vector3 destination )
	{
		if ( navMeshMovementEnabled )
		{
			if ( _navMeshAgent != null )
			{
				if ( _navMeshAgent.isActiveAndEnabled )
				{
					_navMeshAgent.speed = CalculateNavMeshMovementSpeed();
					_navMeshAgent.SetDestination( destination );
					_navMeshAgent.Resume();

					SetAnimatorBool( ANIMATOR_BOOL_MOVING, true );
				}
			}
		}
	}

	public void StopNavMeshMovement()
	{
		if ( _navMeshAgent != null )
		{
			if ( _navMeshAgent.isActiveAndEnabled )
				_navMeshAgent.Stop();

			SetAnimatorBool( ANIMATOR_BOOL_MOVING, false );
		}
	}

	public void ResumeNavMeshMovement()
	{
		if ( navMeshMovementEnabled )
		{
			if ( _navMeshAgent != null )
			{
				if ( _navMeshAgent.isActiveAndEnabled )
				{
					_navMeshAgent.speed = CalculateNavMeshMovementSpeed();
					_navMeshAgent.Resume();
				}

				SetAnimatorBool( ANIMATOR_BOOL_MOVING, true );
			}
		}
	}

	public void EnableNavMeshMovement()
	{
		if ( _navMeshAgent != null )
			_navMeshAgent.enabled = true;
	}

	public void DisableNavMeshMovement()
	{
		if ( _navMeshAgent )
			_navMeshAgent.enabled = false;
	}

	virtual public float CalculateNavMeshMovementSpeed()
	{
		return speed * speedMultiplier * _secondarySpeedMultiplier;
	}

	#endregion

	#region Secondary Movement

	#region General

	public void StopAllSecondaryMovement()
	{
		StopRunning();
		StopWalking();
		StopCrouching();

		_secondarySpeedMultiplier = 1.0f;
	}

	#endregion

	#region Jump

	virtual public void Jump( float force )
	{
		_jumping = true;
		_grounded = false;

		_useGravity = _rigidbody.useGravity;

		_rigidbody.useGravity = true;
		_rigidbody.AddForce( Vector3.up * force );
		_rigidbody.velocity = new Vector3( 
			_rigidbody.velocity.x, 
			_rigidbody.velocity.y + (COLLISION_FLOAT_PRECISION * 10.0f), 
			_rigidbody.velocity.z );
	}

	virtual protected void FinishJump()
	{
		if ( _jumping )
		{
			_jumping = false;
			_grounded = true;
			_rigidbody.useGravity = _useGravity;
		}
	}

	virtual public bool IsJumping()
	{
		return _jumping;
	}

	virtual public bool IsGrounded()
	{
		return _grounded;
	}

	#endregion

	#region Run

	virtual public bool Run( float speedMultiplier = 1.0f )
	{
		if ( !_running && !_jumping )
		{
			StopAllSecondaryMovement();

			_running = true;
			_secondarySpeedMultiplier = speedMultiplier;

			return true;
		}

		return false;
	}

	virtual public bool StopRunning()
	{
		if ( _running )
		{
			_running = false;
			_secondarySpeedMultiplier = 1.0f;

			return true;
		}

		return false;
	}

	virtual public bool IsRunning()
	{
		return _running;
	}

	#endregion

	#region Walk

	virtual public bool Walk( float speedMultiplier = 1.0f )
	{
		if ( !_walking && !_jumping )
		{
			StopAllSecondaryMovement();

			_walking = true;
			_secondarySpeedMultiplier = speedMultiplier;

			return true;
		}

		return false;
	}

	virtual public bool StopWalking()
	{
		if ( _walking )
		{
			_walking = false;
			_secondarySpeedMultiplier = 1.0f;

			return true;
		}

		return false;
	}

	virtual public bool IsWalking()
	{
		return _walking;
	}

	#endregion

	#region Crouch

	virtual public bool Crouch( float speedMultiplier = 1.0f )
	{
		if ( !_crouching )
		{
			StopAllSecondaryMovement();

			_crouching = true;
			_secondarySpeedMultiplier = speedMultiplier;

			return true;
		}

		return false;
	}

	virtual public bool StopCrouching()
	{
		if ( _crouching )
		{
			_crouching = false;
			_secondarySpeedMultiplier = 1.0f;

			return true;
		}

		return false;
	}

	virtual public bool IsCrouching()
	{
		return _crouching;
	}

	#endregion

	#endregion

	#endregion

	#region Animation

	public void EnableAnimator()
	{
		if ( _animator != null )
			_animator.enabled = true;
	}

	public void DisableAnimator()
	{
		if ( _animator != null )
			_animator.enabled = false;
	}

	public void SetAnimatorBool( string animatorBool, bool value )
	{
		if ( _animator != null )
			_animator.SetBool( animatorBool, value );
	}

	public void SetAnimatorTrigger( string animatorTrigger )
	{
		if ( _animator != null )
			_animator.SetTrigger( animatorTrigger );
	}

	#endregion

	#region Health System

	public HealthSystem GetHealthSystem()
	{
		return _healthSystem;
	}

	public void OnDamage( float health, float delta )
	{
		OnDamaged( health, delta );
	}

	public void OnHeal( float health, float delta )
	{
		OnHealed( health, delta );
	}

	public void OnKill( int lives )
	{
		Despawn();
		OnKilled( lives );
	}

	public void OnResetHealth( float health )
	{
		OnHealthReset( health );
	}

	public void OnResetLives( int lives )
	{
		OnLivesReset( lives );
	}

	virtual protected void OnDamaged( float health, float delta )
	{
		// override, if necessary
	}

	virtual protected void OnHealed( float health, float delta )
	{
		// override, if necessary
	}

	virtual protected void OnKilled( int lives )
	{
		// override, if necessary
	}

	virtual protected void OnHealthReset( float health )
	{
		// override, if necessary
	}

	virtual protected void OnLivesReset( int lives )
	{
		// override, if necessary
	}

	#endregion

	#region Weapon System

	public WeaponSystem GetWeaponSystem()
	{
		return _weaponSystem;
	}

	#endregion

	#region Spawning

	public void Spawn()
	{
		Spawn( defaultSpawnPoint );
	}

	public void Spawn( Vector3 spawnPoint )
	{
		if ( _initialized )
		{
			if ( !_spawned )
			{
				_spawned = true;
				_previousSpawnPoint = spawnPoint;
				_secondarySpeedMultiplier = 1.0f;

				gameObject.SetActive( true );
				enabled = true;

				EnableNavMeshMovement();
				WarpToPosition( spawnPoint );

				OnSpawn();

				if ( onSpawn != null )
					onSpawn();
			}
		}
		else
		{
			_previousSpawnPoint = spawnPoint;

			CancelInvoke( "Respawn" );
			Invoke( "Respawn", Mathf.Epsilon );
		}
	}

	public void Despawn()
	{
		if ( _spawned )
		{
			DisableNavMeshMovement();
			OnDespawn();

			if ( onDespawn != null )
				onDespawn();

			enabled = false;
			gameObject.SetActive( false );

			_spawned = false;
		}
	}

	public void Respawn()
	{
		Spawn( _previousSpawnPoint );
	}

	public bool IsSpawned()
	{
		return _spawned;
	}

	public bool IsDespawned()
	{
		return !_spawned;
	}

	virtual protected void OnSpawn()
	{
		// override, if necessary
	}

	virtual protected void OnDespawn()
	{
		// override, if necessary
	}

	#endregion

	#region Pausing

	public void Pause()
	{
		if ( !_paused )
		{
			_paused = true;

			StopNavMeshMovement();
			OnPause();
		}
	}

	public void Unpause()
	{
		if ( _paused )
		{
			OnUnPause();
			ResumeNavMeshMovement();

			_paused = false;
		}
	}

	public void TogglePause()
	{
		if ( _paused )
			Unpause();
		else
			Pause();
	}

	public bool IsPaused()
	{
		return _paused;
	}

	public bool IsUnpaused()
	{
		return !_paused;
	}

	virtual protected void OnPause()
	{
		// override, if necessary
	}

	virtual protected void OnUnPause()
	{
		// override, if necessary
	}

	#endregion

}

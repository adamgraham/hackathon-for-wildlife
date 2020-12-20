using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Character
{
	#region Variables

	[Header( "Enemy" )]

	public float logicUpdateDelay;
	public Transform target;

	public enum Type { Aggressive, Passive, Skittish }
	public Enemy.Type enemyType;

	protected Canvas _ui;
	protected ActiveAtDistance _activeAtDistance;

	static private List<Enemy> _enemies;

	[Header( "Movement" )]

	public EnemyAggroSettings aggro;
	public EnemyRoamingSettings roaming;
	public EnemyFleeingSettings fleeing;

	protected bool _aggroed;
	protected Transform _aggroTarget;
	protected Vector3 _aggroOrigin;

	protected bool _roaming;
	protected Vector3 _roamingOrigin;

	protected bool _retreating;
	protected bool _fleeing;

	[Header( "Spawning" )]

	protected EnemySpawner _spawner;

	#endregion

	#region Unity Events

	protected override void OnAwake()
	{
		_ui = gameObject.GetComponentInChildren<Canvas>();
		_activeAtDistance = gameObject.GetComponentInChildren<ActiveAtDistance>();

		AddEnemyToList( this );
	}

	protected override void OnDispose()
	{
		RemoveEnemyFromList( this );
	}

	#endregion

	#region Movement

	public override float CalculateNavMeshMovementSpeed()
	{
		float returnSpeed = base.CalculateNavMeshMovementSpeed();

		if ( _aggroed )
			returnSpeed *= aggro.aggroSpeedMultiplier;
		else if ( _roaming )
			returnSpeed *= roaming.roamingSpeedMultiplier;

		return returnSpeed;
	}

	public void StopAllMovement()
	{
		StopAggro();
		StopRetreating();
		StopFleeing();
		StopRoaming();
		StopNavMeshMovement();
	}

	#endregion

	#region Aggro

	virtual protected void Aggro( Transform target, float maxDuration = 0.0f )
	{
		if ( aggro.canAggro )
		{
			if ( !_aggroed )
			{
				StopRetreating();
				StopRoaming();
				StopFleeing();

				_aggroed = true;
				_aggroTarget = target;
				_aggroOrigin = transform.position;

				SetAggroDestination();
			}

			StartInvokeStopAggro( maxDuration );
		}
	}

	virtual protected void StopAggro()
	{
		if ( _aggroed )
		{
			_aggroed = false;

			StopNavMeshMovement();

			CancelInvoke( "SetAggroDestination" );
			CancelInvoke( "StopAggro" );

			Retreat();
		}
	}

	virtual public bool IsAggroed()
	{
		return _aggroed;
	}

	virtual protected void SetAggroDestination()
	{
		SetNavMeshDestination( _aggroTarget.position );
		StartInvokeSetAggroDestination();
	}

	virtual protected void RecalculateAggroDistance()
	{
		if ( isActiveAndEnabled )
		{
			if ( _aggroed )
			{
				if ( _aggroTarget != null )
				{
					if ( Vector3.Distance( transform.position, _aggroTarget.position ) > aggro.aggroRadius )
						StopAggro();
				}
			} 
			else 
			{
				if ( target != null )
				{
					if ( Vector3.Distance( transform.position, target.position ) <= aggro.aggroRadius )
						Aggro( target, aggro.aggroMaxTime );
				}
			}
		}
	}

	private void StartInvokeSetAggroDestination()
	{
		StartInvoke( "SetAggroDestination", logicUpdateDelay );
	}

	private void StartInvokeStopAggro( float maxDuration )
	{
		CancelInvoke( "StopAggro" );

		if ( maxDuration > 0.0f )
			Invoke( "StopAggro", maxDuration );
	}

	private void StartInvokeRecalculateAggroDistance()
	{
		if ( enemyType == Type.Aggressive )
			StartInvokeRepeating( "RecalculateAggroDistance", logicUpdateDelay, logicUpdateDelay );
	}

	#endregion

	#region Retreating

	virtual protected void Retreat()
	{
		if ( !_retreating )
		{
			_retreating = true;

			SetNavMeshDestination( _aggroOrigin );
			StartCheckIfRetreatedInvoke();
		}
	}

	virtual protected void StopRetreating()
	{
		if ( _retreating )
		{
			_retreating = false;

			StopNavMeshMovement();
			CancelInvoke( "CheckIfRetreated" );
			Roam();
		}
	}

	virtual public bool IsRetreating()
	{
		return _retreating;
	}

	virtual protected void CheckIfRetreated()
	{
		if ( Vector3.Distance( transform.position, _aggroOrigin ) < 1.0f )
			StopRetreating();
	}

	private void StartCheckIfRetreatedInvoke()
	{
		StartInvokeRepeating( "CheckIfRetreated", logicUpdateDelay, logicUpdateDelay );
	}

	#endregion

	#region Fleeing

	virtual protected void Flee( Transform target )
	{
		if ( fleeing.canFlee )
		{
			if ( !_fleeing )
			{
				StopRetreating();
				StopRoaming();
				StopAggro();

				_fleeing = true;
			}

			ContinueFleeing( target );
		}
	}

	virtual protected void ContinueFleeing( Transform target )
	{
		//... TO DO
	}

	virtual protected void StopFleeing()
	{
		if ( _fleeing )
		{
			_fleeing = false;

			StopNavMeshMovement();
		}
	}

	virtual public bool IsFleeing()
	{
		return _fleeing;
	}

	#endregion

	#region Roaming

	virtual public void Roam()
	{
		if ( roaming.canRoam )
		{
			if ( !_roaming )
			{
				StopAllMovement();

				if ( roaming.roamingRadius > 0 )
				{
					_roaming = true;
					_roamingOrigin = transform.position;

					SetRoamingDestination();
					StartInvokeCheckRoamingDestinationReached();
				}
			}
		}
	}

	virtual public void StopRoaming()
	{
		if ( _roaming )
		{
			_roaming = false;

			StopNavMeshMovement();

			CancelInvoke( "SetRoamingDestination" );
			CancelInvoke( "CheckRoamingDestinationReached" );
		}
	}

	virtual public bool IsRoaming()
	{
		return _roaming;
	}

	virtual protected void SetRoamingDestination()
	{
		SetNavMeshDestination( _roamingOrigin + (Random.insideUnitSphere * roaming.roamingRadius) );
		StartInvokeSetRoamingDestination();
	}

	private void StartInvokeSetRoamingDestination()
	{
		StartInvoke( "SetRoamingDestination", Random.Range( roaming.roamingUpdateDelayMin, roaming.roamingUpdateDelayMax ) );
	}

	virtual protected void CheckRoamingDestinationReached()
	{
		if ( PhysicsUtils.HasNavAgentReachedDestination( _navMeshAgent ) )
			SetAnimatorBool( Character.ANIMATOR_BOOL_MOVING, false );
	}

	private void StartInvokeCheckRoamingDestinationReached()
	{
		StartInvokeRepeating( "CheckRoamingDestinationReached", logicUpdateDelay, logicUpdateDelay );
	}

	#endregion

	#region Spawning

	protected override void OnSpawn()
	{
		Roam();
		StartInvokeRecalculateAggroDistance();
		base.OnSpawn();
	}

	protected override void OnDespawn()
	{
		StopAllMovement();
		base.OnDespawn();
	}

	#endregion

	#region Static Methods

	static public int GetAmountEnemies()
	{
		return (_enemies != null) ? _enemies.Count : 0;
	}

	static public List<Enemy> GetEnemyList()
	{
		return new List<Enemy>( _enemies );
	}

	static public void ClearEnemyList()
	{
		if ( _enemies != null )
		{
			int len = _enemies.Count;
			for ( int i = 0; i < len; i++ )
			{
				Enemy enemy = _enemies[i];
				if ( enemy != null )
					Destroy( enemy.gameObject );
			}
		}

		_enemies = new List<Enemy>();
	}

	static private void AddEnemyToList( Enemy enemy )
	{
		if ( _enemies == null )
			_enemies = new List<Enemy>();

		if ( !_enemies.Contains( enemy ) )
			_enemies.Add( enemy );
	}

	static private void RemoveEnemyFromList( Enemy enemy )
	{
		if ( _enemies != null )
			_enemies.Remove( enemy );
	}

	#endregion

}

#region Data Classes

[System.Serializable]
public class EnemyAggroSettings
{
	public bool canAggro = true;
	public float aggroRadius;
	public float aggroMaxTime;
	public float aggroSpeedMultiplier = 1.0f;
}

[System.Serializable]
public class EnemyRoamingSettings
{
	public bool canRoam = true;
	public float roamingRadius;
	public float roamingUpdateDelayMin;
	public float roamingUpdateDelayMax;
	public float roamingSpeedMultiplier = 1.0f;
}

[System.Serializable]
public class EnemyFleeingSettings
{
	public bool canFlee = true;
	public int fleeMaxDistance;
}

#endregion

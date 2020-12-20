using UnityEngine;
using System.Collections;

[RequireComponent( typeof( HealthSystem ) )]
public class EnemySpawnerDestroyable : EnemySpawner, IKillable
{
	#region Variables

	protected HealthSystem _healthSystem;

	#endregion

	#region Unity Events

	protected override void OnAwake()
	{
		_healthSystem = gameObject.GetComponent<HealthSystem>();
	}

	protected override void OnDispose()
	{
		_healthSystem = null;
	}

	#endregion

	#region Health System

	public HealthSystem GetHealthSystem()
	{
		return _healthSystem;
	}

	public void OnDamage( float health, float delta )
	{
	}

	public void OnHeal( float health, float delta )
	{
	}

	public void OnKill( int lives )
	{
		StopSpawner();
	}

	public void OnResetHealth( float health )
	{
	}

	public void OnResetLives( int lives )
	{
	}

	#endregion

}

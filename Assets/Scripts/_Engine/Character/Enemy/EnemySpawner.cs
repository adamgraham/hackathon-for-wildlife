using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
	#region Variables

	[Header( "Enemies" )]

	public Enemy[] enemyPrefabs;

	public Transform target = null;

	private int _enemiesSpawned;

	static public int globalMaxEnemies = 32;

	[Header( "Spawn Settings" )]

	public float spawnIntervalMin;
	public float spawnIntervalMax;
	public float spawnRadius = 2.0f;

	public int enemiesPerSpawn = 1;
	public int maxEnemies = 8;

	public bool spawnOnStart = true;

	[Header( "Spawn Area" )]

	public bool accountForMinMax;
	public Vector3 minSpawn;
	public Vector3 maxSpawn;

	#endregion

	#region Unity Events

	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		if ( spawnOnStart )
			StartSpawner();

		OnStart();
	}

	private void OnEnable()
	{
		OnEnabled();
	}

	private void OnDisable()
	{
		StopSpawner();
		OnDisabled();
	}

	private void OnDestroy()
	{
		OnDispose();
	}

	virtual protected void OnAwake()
	{
		// override, if necesary
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

	#endregion

	#region Spawning

	public void Spawn()
	{
		Spawn( enemiesPerSpawn );
		StartSpawner();
	}

	public void Spawn( int amount )
	{
		if ( Game.instance.IsLevelActive() )
		{
			if ( enemyPrefabs.Length > 0 )
			{
				for ( int i = 0; i < amount; i++ )
				{
					if ( Enemy.GetAmountEnemies() < globalMaxEnemies )
					{
						Enemy enemy = (Instantiate( enemyPrefabs[Random.Range( 0, enemyPrefabs.Length )].gameObject ) as GameObject).GetComponent<Enemy>();

						enemy.onSpawn += OnEnemySpawned;
						enemy.onDespawn += OnEnemyDespawned;
						enemy.target = target;
						enemy.Spawn( GetSpawnPoint() );
					}
				}
			}
		}
	}

	public void StartSpawner()
	{
		CancelInvoke( "Spawn" );
		Invoke( "Spawn", Random.Range( spawnIntervalMin, spawnIntervalMax ) );
	}

	public void StopSpawner()
	{
		CancelInvoke( "Spawn" );
	}

	private void OnEnemySpawned()
	{
		_enemiesSpawned++;
	}

	private void OnEnemyDespawned()
	{
		_enemiesSpawned = Mathf.Clamp( _enemiesSpawned - 1, 0, int.MaxValue );
	}

	private Vector3 GetSpawnPoint()
	{
		Vector3 spawnPoint = transform.position + (Random.insideUnitSphere * spawnRadius);

		if ( accountForMinMax )
		{
			spawnPoint = new Vector3(
				Mathf.Clamp( spawnPoint.x, minSpawn.x, maxSpawn.x ),
				Mathf.Clamp( spawnPoint.y, minSpawn.y, maxSpawn.y ),
				Mathf.Clamp( spawnPoint.z, minSpawn.z, maxSpawn.z ) );
		}

		return spawnPoint;
	}

	#endregion

}

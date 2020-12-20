using UnityEngine;
using System.Collections;

public class ProjectileShooter : Weapon 
{
	[Header( "Projectile Shooter" )]

	public Projectile projectilePrefab;
	public ProjectionPattern customProjectionPattern;

	public enum ProjectionShape { Line, Cone, Ring, Spiral }
	public ProjectionShape projectionShape;

	public int amountProjectilesMin = 1;
	public int amountProjectilesMax = 1;

	public float projectileSpeedMin;
	public float projectileSpeedMax;

	private ProjectionPattern _currentProjectionPattern;
	private ProjectionShape _currentProjectionShape;

	static private Hashtable _projectionPatterns;

	protected override void OnAwake()
	{
		if ( _projectionPatterns == null )
		{
			_projectionPatterns = new Hashtable();
			_projectionPatterns[ProjectionShape.Line] = new LineProjectionPattern();
			_projectionPatterns[ProjectionShape.Cone] = new ConeProjectionPattern();
			_projectionPatterns[ProjectionShape.Ring] = new RingProjectionPattern();
			_projectionPatterns[ProjectionShape.Spiral] = new SpiralProjectionPattern();
		}

		SetCurrentProjectionShape();
	}

	protected override void OnDispose()
	{
		projectilePrefab = null;
		customProjectionPattern = null;
		_currentProjectionPattern = null;
	}

	protected override void OnAttack()
	{
		if ( projectilePrefab != null )
		{
			ProjectionPattern pattern = customProjectionPattern;

			if ( pattern == null )
			{
				if ( _currentProjectionShape != projectionShape )
					SetCurrentProjectionShape();

				pattern = _currentProjectionPattern;
			}

			Vector3 forwardDirection = CalculateAttackDirection();
			float forwardAngle = CalculateAttackAngle();
			int amountProjectiles = GetAmountOfProjectiles();

			pattern.StartProjection( amountProjectiles );

			for ( int i = 0; i < amountProjectiles; i++ )
				pattern.Project( CreateProjectile(), forwardDirection, forwardAngle, i );

			pattern.EndProjection();
		}
	}

	public Projectile CreateProjectile()
	{
		projectilePrefab.transform.position = transform.position;

		Projectile projectile = Instantiate( projectilePrefab ).GetComponent<Projectile>();

		projectile.sender = GetSender();
		projectile.speed = Random.Range( projectileSpeedMin, projectileSpeedMax );
		projectile.transform.position = transform.position;
		projectile.gameObject.SetActive( true );

		return projectile;
	}

	public int GetAmountOfProjectiles()
	{
		return Random.Range( amountProjectilesMin, amountProjectilesMax );
	}

	public GameObject GetSender()
	{
		GameObject sender = (weaponSystem != null) ? weaponSystem.GetCharacterGameObject() : null;
		if ( sender == null ) sender = gameObject;
		return sender;
	}

	private void SetCurrentProjectionShape()
	{
		_currentProjectionShape = projectionShape;
		_currentProjectionPattern = (ProjectionPattern)_projectionPatterns[projectionShape];
	}

}

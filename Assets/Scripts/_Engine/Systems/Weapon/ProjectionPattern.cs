using UnityEngine;
using System.Collections;

abstract public class ProjectionPattern
{
	protected int _amountOfProjectiles;
	protected bool _projecting;

	public void StartProjection( int amountOfProjectiles )
	{
		_amountOfProjectiles = amountOfProjectiles;
		_projecting = true;

		OnStartProjection();
	}

	virtual protected void OnStartProjection()
	{
		// override, if necessary
	}

	public void EndProjection()
	{
		_projecting = false;
		OnEndProjection();
	}

	virtual protected void OnEndProjection()
	{
		// override, if necessary
	}

	public bool IsProjecting()
	{
		return _projecting;
	}

	public bool IsNotProjecting()
	{
		return !_projecting;
	}

	virtual public void Project( Projectile projectile, Vector3 forwardDirection, float forwardAngle, int projectileIndex = 0 )
	{
		// override

		projectile.SetDirection( forwardDirection );
	}

}

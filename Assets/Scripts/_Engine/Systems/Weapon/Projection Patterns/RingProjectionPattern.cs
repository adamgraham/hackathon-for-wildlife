using UnityEngine;
using System.Collections;

public class RingProjectionPattern : ProjectionPattern
{
	static private Transform _calculationTransform;

	public override void Project( Projectile projectile, Vector3 forwardDirection, float forwardAngle, int projectileIndex = 0 )
	{
		float deltaAngle = 360.0f / (float)_amountOfProjectiles;
		float angle = forwardAngle + (projectileIndex * deltaAngle);

		Vector3 direction = new Vector3( 0.0f, angle, 0.0f );

		if ( _calculationTransform == null )
		{
			_calculationTransform = new GameObject().transform;
			_calculationTransform.gameObject.SetActive( false );
			_calculationTransform.gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		_calculationTransform.transform.eulerAngles = direction;

		projectile.SetDirection( _calculationTransform.forward );
	}

}

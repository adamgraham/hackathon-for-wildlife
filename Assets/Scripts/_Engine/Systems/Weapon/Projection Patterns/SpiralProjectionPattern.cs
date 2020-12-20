using UnityEngine;
using System.Collections;

public class SpiralProjectionPattern : ProjectionPattern 
{
	private float _angle;

	static private Transform _calculationTransform;

	static public float rotationSpeed = 15.0f;

	public override void Project( Projectile projectile, Vector3 forwardDirection, float forwardAngle, int projectileIndex = 0 )
	{
		if ( _calculationTransform == null )
		{
			_calculationTransform = new GameObject().transform;
			_calculationTransform.gameObject.SetActive( false );
			_calculationTransform.gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
			_angle = forwardAngle;
		}

		float deltaAngle = 360.0f / (float)_amountOfProjectiles;
		float angle = _angle + (projectileIndex * deltaAngle);

		Vector3 direction = new Vector3( 0.0f, angle, 0.0f );

		_calculationTransform.transform.eulerAngles = direction;

		projectile.SetDirection( _calculationTransform.forward );
	}

	protected override void OnEndProjection()
	{
		_angle += rotationSpeed;
	}

}

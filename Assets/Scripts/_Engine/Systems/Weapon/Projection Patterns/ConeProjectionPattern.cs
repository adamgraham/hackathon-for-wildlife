using UnityEngine;
using System.Collections;

public class ConeProjectionPattern : ProjectionPattern
{
	static public float changeInAngle = 0.15f;

	public override void Project( Projectile projectile, Vector3 forwardDirection, float forwardAngle, int projectileIndex = 0 )
	{
		float midIndex = (float)(_amountOfProjectiles - 1) * 0.5f;
		float angle = ((float)projectileIndex - midIndex) * changeInAngle;

		Vector3 direction = new Vector3(
			forwardDirection.x + (forwardDirection.z * angle), 
			forwardDirection.y,
			forwardDirection.z + (forwardDirection.x * angle) );

		projectile.SetDirection( direction );
	}

}

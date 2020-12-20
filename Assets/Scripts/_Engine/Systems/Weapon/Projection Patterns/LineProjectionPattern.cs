using UnityEngine;
using System.Collections;

public class LineProjectionPattern : ProjectionPattern
{
	static public float projectilePadding = 1.0f;

	public override void Project( Projectile projectile, Vector3 forwardDirection, float forwardAngle, int projectileIndex = 0 )
	{
		float midIndex = (float)(_amountOfProjectiles - 1) * 0.5f;
		float padding = ((float)projectileIndex - midIndex) * projectilePadding;
		
		projectile.transform.position = new Vector3(
			projectile.transform.position.x + (forwardDirection.z * padding), 
			projectile.transform.position.y, 
			projectile.transform.position.z + (forwardDirection.x * padding) );

		base.Project( projectile, forwardDirection, projectileIndex );
	}

}

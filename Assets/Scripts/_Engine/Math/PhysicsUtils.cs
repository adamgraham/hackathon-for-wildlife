using UnityEngine;
using System.Collections;

public class PhysicsUtils
{
	#region Nav Mesh

	static public bool HasNavAgentReachedDestination( NavMeshAgent agent )
	{
		if ( agent != null )
		{
			if ( agent.isActiveAndEnabled )
			{
				if ( !agent.pathPending )
				{
					if ( agent.remainingDistance <= agent.stoppingDistance )
					{
						if ( !agent.hasPath || agent.velocity.sqrMagnitude <= Mathf.Epsilon )
						{
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	static public NavMeshObstacle AddNavObstacleFromCollider( GameObject gameObject, float sizeMultiplier = 1.0f, bool disregardTriggers = true )
	{
		NavMeshObstacle navObstacle = gameObject.GetComponent<NavMeshObstacle>();

		if ( navObstacle == null )
		{
			Collider collider = gameObject.GetComponent<Collider>();

			if ( collider != null )
			{
				if ( !collider.isTrigger || !disregardTriggers )
				{
					navObstacle = gameObject.AddComponent<NavMeshObstacle>();

					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					BoxCollider boxCollider = collider as BoxCollider;
					SphereCollider sphereCollider = collider as SphereCollider;

					if ( capsuleCollider != null )
					{
						navObstacle.shape = NavMeshObstacleShape.Capsule;
						navObstacle.center = capsuleCollider.center;
						navObstacle.radius = capsuleCollider.radius * sizeMultiplier;
						navObstacle.height = capsuleCollider.height * sizeMultiplier;
					}
					else if ( boxCollider != null )
					{
						navObstacle.shape = NavMeshObstacleShape.Box;
						navObstacle.center = boxCollider.center;
						navObstacle.size = boxCollider.size * sizeMultiplier;
					}
					else if ( sphereCollider != null )
					{
						navObstacle.shape = NavMeshObstacleShape.Capsule;
						navObstacle.center = sphereCollider.center;
						navObstacle.radius = sphereCollider.radius * sizeMultiplier;
						navObstacle.height = sphereCollider.radius * sizeMultiplier;
					}
				}
			}
		}

		if ( navObstacle != null )
			navObstacle.carving = true;

		return navObstacle;
	}

	#endregion

	#region Colliders

	static public Collider AddTriggersFromColliders( GameObject gameObject, float sizeMultiplier = 1.5f )
	{
		Collider trigger = null;
		Collider collider = null;
		Collider[] colliders = gameObject.GetComponents<Collider>();

		if ( colliders != null )
		{
			bool hasTrigger = false;

			foreach ( Collider tCollider in colliders )
			{
				if ( tCollider.isTrigger )
				{
					hasTrigger = true;
					trigger = tCollider;
				}
				else
				{
					collider = tCollider;
				}
			}

			if ( !hasTrigger )
			{
				if ( collider != null )
				{
					BoxCollider boxCollider = collider as BoxCollider;
					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					SphereCollider sphereCollider = collider as SphereCollider;

					if ( boxCollider != null )
					{
						BoxCollider triggerBox = gameObject.AddComponent<BoxCollider>();
						triggerBox.center = boxCollider.center;
						triggerBox.size = boxCollider.size * sizeMultiplier;
						trigger = triggerBox;
					}
					else if ( capsuleCollider != null )
					{
						CapsuleCollider triggerCapsule = gameObject.AddComponent<CapsuleCollider>();
						triggerCapsule.center = capsuleCollider.center;
						triggerCapsule.radius = capsuleCollider.radius * sizeMultiplier;
						triggerCapsule.height = capsuleCollider.height * sizeMultiplier;
						trigger = triggerCapsule;
					}
					else if ( sphereCollider != null )
					{
						SphereCollider triggerSphere = gameObject.AddComponent<SphereCollider>();
						triggerSphere.center = sphereCollider.center;
						triggerSphere.radius = sphereCollider.radius * sizeMultiplier;
						trigger = triggerSphere;
					}
				}
			}
		}

		if ( trigger == null )
			trigger = gameObject.AddComponent<BoxCollider>();

		trigger.isTrigger = true;

		return trigger;
	}

	static public void SetCollidersEnabled( GameObject obj, bool enabled )
	{
		Collider[] colliders = obj.GetComponentsInChildren<Collider>();
		int len = colliders.Length;

		for ( int i = 0; i < len; i++ )
			colliders[i].enabled = enabled;
	}

	#endregion

	#region Rigidbodies

	static public void SetKinematicRigidbodies( GameObject obj, bool isKinematic )
	{
		Rigidbody[] rigidBodies = obj.GetComponentsInChildren<Rigidbody>();
		int len = rigidBodies.Length;

		for ( int i = 0; i < len; i++ )
			rigidBodies[i].isKinematic = isKinematic;
	}

	#endregion

}

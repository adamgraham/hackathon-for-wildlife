using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Rigidbody ) )]
public class Projectile : MonoBehaviour 
{
	[HideInInspector]
	public GameObject sender;

	[HideInInspector]
	public float speed = 1.0f;

	public GameObject collisionHitPrefab;
	public float delayedDestroy = 0.0f;
	public float hitForce;

	public float maxLifeTime = 5.0f;

	private Vector3 _direction;
	private Rigidbody _rigidbody;

	private void Awake()
	{
		_rigidbody = gameObject.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		if ( _direction == Vector3.zero )
			SetDirection( transform.forward );

		_rigidbody.velocity = _direction * speed;

		Destroy( gameObject, maxLifeTime );
	}

	private void OnDestroy()
	{
		sender = null;

		_rigidbody = null;
	}

	private void OnCollisionEnter( Collision collision )
	{
		_rigidbody.isKinematic = true;

		if ( collision.collider.gameObject != sender )
			Destroy( gameObject, delayedDestroy );

		if ( collisionHitPrefab != null )
		{
			GameObject hit = Instantiate( collisionHitPrefab );
			hit.transform.position = transform.position;
			Destroy( hit, maxLifeTime );
		}

		if ( collision.rigidbody != null )
			collision.rigidbody.AddExplosionForce( hitForce, transform.position, 1.0f );
	}

	public void SetDirection( Vector3 direction )
	{
		_direction = direction;
	}

}

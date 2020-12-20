using UnityEngine;
using System.Collections;

public class Spear : MonoBehaviour 
{
	static readonly float MAX_LIFE = 4.0f;

	public Vector3 direction;
	public float speed;
	public int damage;

	private void Start() 
	{
		Invoke("DestroySpear", MAX_LIFE);
		transform.forward = direction;
	}

	private void Update() 
	{
		Vector3 currentPosition = transform.position;
		currentPosition += direction * speed * Time.deltaTime;
		transform.position = currentPosition;
	}

	private void OnTriggerEnter( Collider other )
	{
		if ( other.gameObject.GetComponent<Elephant>() != null )
		{
			Elephant.instance.Damage( damage );
			CancelInvoke("DestroySpear");
			transform.parent = other.transform;
			enabled = false;
		}
	}

	private void DestroySpear()
	{
		Destroy(gameObject);
	}

}

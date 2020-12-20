using UnityEngine;
using System.Collections;

public interface ISpawnable
{
	void Spawn( Vector3 position );
	void Respawn();
	void Despawn();
	bool IsSpawned();
}

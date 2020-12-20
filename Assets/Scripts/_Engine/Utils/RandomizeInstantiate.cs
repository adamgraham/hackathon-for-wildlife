using UnityEngine;
using System.Collections;

public class RandomizeInstantiate : MonoBehaviour 
{
	public RandomizeInstantiateObject[] objects;
	public bool destroyScript;

	private void Start()
	{
		int len = objects.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomizeInstantiateObject objRandom = objects[i];

			int amount = 0;
			while ( amount++ < objRandom.spawnAmount )
			{
				if ( Random.value <= objRandom.spawnChance )
				{
					GameObject newObj = Instantiate( objRandom.obj, transform.position, Quaternion.identity ) as GameObject;
					if ( objRandom.parented ) newObj.transform.parent = transform;
				}
			}
		}

		if ( destroyScript )
			Destroy( this );
	}

}

[System.Serializable]
public class RandomizeInstantiateObject
{
	public GameObject obj;
	[Range( 0.0f, 1.0f )]
	public float spawnChance = 0.5f;
	public int spawnAmount = 1;
	public bool parented;
}

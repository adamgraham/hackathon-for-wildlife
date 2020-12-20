using UnityEngine;
using System.Collections;

public class RandomizeColor : MonoBehaviour 
{
	public RandomMaterial[] matRandomizations;
	public RandomTint[] tintRandomizations;
	public bool destroyScript;

	private void Start()
	{
		int len = matRandomizations.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomMaterial matRandomization = matRandomizations[i];
			Renderer renderer = matRandomization.renderer;
			Material[] mats = matRandomization.mats;

			if ( renderer == null ) renderer = gameObject.GetComponent<Renderer>();
			if ( renderer != null && mats.Length > 0 ) renderer.material = mats[Random.Range( 0, mats.Length )];
		}

		len = tintRandomizations.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomTint tint = tintRandomizations[i];
			tint.material.color = tint.colors[Random.Range( 0, tint.colors.Length )];
		}

		if ( destroyScript )
			Destroy( this );
	}
}

[System.Serializable]
public class RandomMaterial
{
	public Renderer renderer;
	public Material[] mats;
}

[System.Serializable]
public class RandomTint
{
	public Material material;
	public Color[] colors;
}

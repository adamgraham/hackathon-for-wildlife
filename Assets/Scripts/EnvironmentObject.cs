using UnityEngine;
using System.Collections;

public class EnvironmentObject : MonoBehaviour 
{
	public GameObject[] models;
	public Texture[] modelTextures;

	private void Awake()
	{
		CreateRandomModel();
	}

	private void CreateRandomModel()
	{
		int randomModelIndex = Random.Range( 0, models.Length );
		GameObject modelPrefab = models[randomModelIndex];
		
		GameObject model = GameObject.Instantiate( modelPrefab ) as GameObject;
		model.transform.parent = transform;

		int randomTextureIndex = Random.Range( 0, modelTextures.Length );
		Texture modelTexture = modelTextures[randomTextureIndex];
		model.GetComponent<MeshRenderer>().material.SetTexture( 0, modelTexture );
	}

}

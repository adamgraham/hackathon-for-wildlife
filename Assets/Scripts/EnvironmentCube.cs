using UnityEngine;
using System.Collections;

public class EnvironmentCube : MonoBehaviour 
{
	internal GridCoordinates _coordinates;
	internal EnvironmentObject _environmentalObject;

	public CubeType cubeType;
	public enum CubeType
	{
		Grass,
		Water,
		Dirt
	}

	public EnvironmentObject CreateEnvironmentalObject( GameObject prefab )
	{
		GameObject gameObject = GameObject.Instantiate( prefab ) as GameObject;

		_environmentalObject = gameObject.GetComponent<EnvironmentObject>();
		if ( _environmentalObject == null )
			_environmentalObject = gameObject.AddComponent<EnvironmentObject>();

		_environmentalObject.transform.parent = transform;
		_environmentalObject.transform.position = transform.position;

		return _environmentalObject;
	}

	public GridCoordinates GetCoordinates()
	{
		return _coordinates;
	}
	
	public bool IsOccupied()
	{
		return _environmentalObject != null;
	}

	public bool IsUnoccupied()
	{
		return _environmentalObject == null;
	}

}

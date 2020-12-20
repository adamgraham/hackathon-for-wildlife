using UnityEngine;
using System.Collections;

public class World : MonoBehaviour 
{
	#region Variables

	static public World instance;

	[Header( "Grid" )]

	public int gridSizeX;
	public int gridSizeZ;

	private EnvironmentCube[,] _grid;

	[Header( "Prefabs" )]

	public EnvironmentCube grassCubePrefab;
	public EnvironmentCube dirtCubePrefab;

	[Header( "Generation" )]

	public ZoneGeneration waterZonesGeneration;
	public ZoneGeneration treeZonesGeneration;

	#endregion

	#region Unity Events

	private void Awake() 
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if ( instance == this )
			instance = null;
	}

	#endregion

	#region Generation

	public void Generate()
	{
		CreateGrid( grassCubePrefab );
		GenerateEnvironmentalZone( waterZonesGeneration );
		GenerateEnvironmentalZone( treeZonesGeneration );
		LineWaterWithDirt( dirtCubePrefab );
	}

	private void CreateGrid( EnvironmentCube basePrefab )
	{
		_grid = new EnvironmentCube[gridSizeX, gridSizeZ];

		for ( int x = 0; x < gridSizeX; x++ )
		{
			for ( int z = 0; z < gridSizeZ; z++ )
			{
				GridCoordinates coordinates = new GridCoordinates( x, z );
				_grid[x, z] = CreateCube( basePrefab.gameObject, coordinates );
			}
		}
	}

	private void LineWaterWithDirt( EnvironmentCube dirtPrefab )
	{
		if ( dirtPrefab == null )
			return;

		for ( int x = 0; x < gridSizeX; x++ )
		{
			for ( int z = 0; z < gridSizeZ; z++ )
			{
				GridCoordinates coordinates = new GridCoordinates( x, z );
				EnvironmentCube cube = _grid[coordinates.x, coordinates.z];
				if ( cube != null )
				{
					if ( cube.cubeType == EnvironmentCube.CubeType.Grass )
					{
						EnvironmentCube adjacentNorth = GetAdjacentNorth( cube );
						EnvironmentCube adjacentSouth = GetAdjacentSouth( cube );
						EnvironmentCube adjacentEast = GetAdjacentEast( cube );
						EnvironmentCube adjacentWest = GetAdjacentWest( cube );
						EnvironmentCube adjacentNorthEast = GetAdjacentNorthEast( cube );
						EnvironmentCube adjacentNorthWest = GetAdjacentNorthWest( cube );
						EnvironmentCube adjacentSouthEast = GetAdjacentSouthEast( cube );
						EnvironmentCube adjacentSouthWest = GetAdjacentSouthWest( cube );

						if ( (adjacentNorth != null && adjacentNorth.cubeType == EnvironmentCube.CubeType.Water) || 
						     (adjacentSouth != null && adjacentSouth.cubeType == EnvironmentCube.CubeType.Water) ||
						     (adjacentEast != null && adjacentEast.cubeType == EnvironmentCube.CubeType.Water) ||
						     (adjacentWest != null && adjacentWest.cubeType == EnvironmentCube.CubeType.Water) || 
						     (adjacentNorthEast != null && adjacentNorthEast.cubeType == EnvironmentCube.CubeType.Water) || 
						     (adjacentNorthWest != null && adjacentNorthWest.cubeType == EnvironmentCube.CubeType.Water) ||
						     (adjacentSouthEast != null && adjacentSouthEast.cubeType == EnvironmentCube.CubeType.Water) ||
						     (adjacentSouthWest != null && adjacentSouthWest.cubeType == EnvironmentCube.CubeType.Water))
						{
							ReplaceCube( cube, dirtPrefab.gameObject );
						}
					}
				}
			}
		}
	}

	private void GenerateEnvironmentalZone( ZoneGeneration generationData )
	{
		int numSources = Random.Range( generationData.minSources, generationData.maxSources + 1 );
		for ( int i = 0; i < numSources; i++ )
		{
			EnvironmentCube randomCube = GetRandomCubeOfType( EnvironmentCube.CubeType.Grass, true );
			EnvironmentCube sourceCube = randomCube;

			generationData.currentPrefab = generationData.prefabs[Random.Range( 0 , generationData.prefabs.Length )];

			if ( generationData.isCubeGeneration )
				sourceCube = ReplaceCube( randomCube, generationData.currentPrefab );
			else 
				randomCube.CreateEnvironmentalObject( generationData.currentPrefab );

			SpreadEnvironmentalZone( generationData, sourceCube );
		}
	}

	private void SpreadEnvironmentalZone( ZoneGeneration generationData, EnvironmentCube source )
	{
		int numSpread = Random.Range( generationData.minSpread, generationData.maxSpread + 1 );
		int spreadCount = 0;

		while ( spreadCount < numSpread )
		{
			EnvironmentCube randomCube = GetRandomAdjacentCube( source, true );

			if ( randomCube.cubeType != EnvironmentCube.CubeType.Water )
			{
				if ( generationData.isCubeGeneration )
					source = ReplaceCube( randomCube, generationData.currentPrefab );
				else
				{
					randomCube.CreateEnvironmentalObject( generationData.currentPrefab );
					source = randomCube;
				}

				spreadCount++;
			}
			else 
			{
				source = randomCube;
			}
		}
	}

	private EnvironmentCube CreateCube( GameObject prefab, GridCoordinates coordinates )
	{
		GameObject gameObject = GameObject.Instantiate( prefab ) as GameObject;
		gameObject.transform.parent = transform;
		gameObject.transform.position = coordinates.GetWorldPosition();
		
		EnvironmentCube environmentCube = gameObject.GetComponent<EnvironmentCube>();
		environmentCube._coordinates = coordinates;
        
        return environmentCube;
	}

	private EnvironmentCube ReplaceCube( EnvironmentCube oldCube, GameObject newCubePrefab )
	{
		GridCoordinates coordinates = oldCube._coordinates;
		Destroy( oldCube.gameObject );

		EnvironmentCube newCube = CreateCube( newCubePrefab, coordinates );
		_grid[coordinates.x, coordinates.z] = newCube;

		return newCube;
	}

	#endregion

	#region Helper Methods

	public bool AreValidCoordinates( GridCoordinates coordinates ) 
	{
		if ( coordinates.x >= 0 && coordinates.x < gridSizeX &&
		    coordinates.z >= 0 && coordinates.z < gridSizeZ )
			return true;
		
		return false;
	}

	public EnvironmentCube GetRandomCube()
	{
		int x = Random.Range( 0, gridSizeX );
		int z = Random.Range( 0, gridSizeZ );
		return _grid[x, z];
	}

	public EnvironmentCube GetRandomCubeOfType( EnvironmentCube.CubeType type, bool unoccupied = false )
	{
		EnvironmentCube randomCube = null;

		while ( randomCube == null )
		{
			randomCube = GetRandomCube();
			bool isUnoccupied = !randomCube.IsOccupied();

			if ( randomCube.cubeType == type && (!unoccupied || (unoccupied && isUnoccupied)) )
				break;
			else
				randomCube = null;
		}

		return randomCube;
	}

	public EnvironmentCube GetRandomAdjacentCube( EnvironmentCube source, bool unoccupied = false )
	{
		EnvironmentCube adjacent = null;
		GridCoordinates coordinates = source._coordinates;
		int option = Random.Range( 0, 4 );

		while ( adjacent == null )
		{
			switch ( option )
			{
			case 0:
				coordinates.z += 1; // north
				break;

			case 1:
				coordinates.z -= 1; // south
				break;

			case 2:
				coordinates.x += 1; // east
				break;

			case 3:
				coordinates.x -= 1; // west
				break;
			}

			if ( AreValidCoordinates( coordinates ) ) 
				adjacent = _grid[coordinates.x, coordinates.z];

			if ( adjacent == null || (unoccupied && adjacent.IsOccupied()) )
			{
				coordinates = source._coordinates;

				if ( ++option > 3 )
					option = 0;
			}
		}

		return adjacent;
	}

	public EnvironmentCube GetAdjacentNorth( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.North, unoccupied, distance );
	}

	public EnvironmentCube GetAdjacentSouth( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.South, unoccupied, distance );
	}

	public EnvironmentCube GetAdjacentEast( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.East, unoccupied, distance );
	}

	public EnvironmentCube GetAdjacentWest( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.West, unoccupied, distance );
	}

	public EnvironmentCube GetAdjacentNorthEast( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.NorthEast, unoccupied, distance );
	}
	
	public EnvironmentCube GetAdjacentNorthWest( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.NorthWest, unoccupied, distance );
	}
	
	public EnvironmentCube GetAdjacentSouthEast( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.SouthEast, unoccupied, distance );
	}
	
	public EnvironmentCube GetAdjacentSouthWest( EnvironmentCube cube, bool unoccupied = false, int distance = 1 ) 
	{
		return GetAdjacentCube( cube, Axis.SouthWest, unoccupied, distance );
	}

	private EnvironmentCube GetAdjacentCube( EnvironmentCube cube, Axis axis, bool unoccupied = false, int distance = 1 )
	{
		EnvironmentCube adjacent = null;

		while ( adjacent == null )
		{
			GridCoordinates coordinates = cube.GetCoordinates();

			if ( axis == Axis.East )
				coordinates.x += distance;
			else if ( axis == Axis.West )
				coordinates.x -= distance;
			else if ( axis == Axis.North )
				coordinates.z += distance;
			else if ( axis == Axis.South )
				coordinates.z -= distance;
			else if ( axis == Axis.NorthEast )
			{
				coordinates.x += distance;
				coordinates.z += distance;
			}
			else if ( axis == Axis.NorthWest )
			{
				coordinates.x -= distance;
				coordinates.z += distance;
			}
			else if ( axis == Axis.SouthEast )
			{
				coordinates.x += distance;
				coordinates.z -= distance;
			}
			else if ( axis == Axis.SouthWest )
			{
				coordinates.x -= distance;
				coordinates.z -= distance;
			}
			
			if ( AreValidCoordinates( coordinates ) ) 
				adjacent = _grid[coordinates.x, coordinates.z];

			if ( adjacent != null && unoccupied && adjacent.IsOccupied() )
				adjacent = null;

			if ( adjacent == null )
				distance--;
		}
		
		return adjacent;
	}

	#endregion

}

#region Data Structures

[System.Serializable]
public struct GridCoordinates
{
	public int x;
	public int z;

	public GridCoordinates( int _x = 0, int _z = 0 )
	{
		x = _x;
		z = _z;
	}

	public Vector3 GetWorldPosition()
	{
		return new Vector3( (float)x, 0.0f, (float)z );
	}
}

[System.Serializable]
public class ZoneGeneration
{
	public GameObject[] prefabs;
	public int minSources;
	public int maxSources;
	public int minSpread;
	public int maxSpread;
	public bool isCubeGeneration;

	internal GameObject currentPrefab;
}

#endregion

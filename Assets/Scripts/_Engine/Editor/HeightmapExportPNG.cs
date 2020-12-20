#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

class HeightmapExportPNG : EditorWindow
{
	static TerrainData terrainData;

	[MenuItem( "Terrain/Export Height Map as PNG" )]
	static public void ExportHeightMapPNG()
	{
		terrainData = null;
		Terrain terrain = null;

		if ( Selection.activeGameObject )
			terrain = Selection.activeGameObject.GetComponent<Terrain>();

		if ( terrain == null )
			terrain = Terrain.activeTerrain;

		if ( terrain != null )
			terrainData = terrain.terrainData;

		if ( terrainData == null )
		{
			EditorUtility.DisplayDialog( "No terrain selected", "Please select a terrain.", "Cancel" );
			return;
		}

		// get the terrain heights into an array and apply them to a texture2D
		int index = 0;
		byte[] bytes;
		float[,] rawHeights;

		Texture2D duplicateHeightMap = new Texture2D( terrainData.heightmapWidth, terrainData.heightmapHeight, TextureFormat.ARGB32, false );
		rawHeights = terrainData.GetHeights( 0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight );

		// run through the array row by row
		for ( int y = 0; y < duplicateHeightMap.height; ++y )
		{
			for ( int x = 0; x < duplicateHeightMap.width; ++x )
			{
				// for wach pixel set RGB to the same so it's gray
				Color color = new Color( rawHeights[x, y], rawHeights[x, y], rawHeights[x, y], 1.0f );
				duplicateHeightMap.SetPixel( x, y, color );
				index++;
			}
		}

		// apply all SetPixel calls
		duplicateHeightMap.Apply();

		// make it a PNG and save it to the Assets folder
		bytes = duplicateHeightMap.EncodeToPNG();
		string fileName = "DupeHeightMap.png";
		File.WriteAllBytes( Application.dataPath + "/" + fileName, bytes );
		EditorUtility.DisplayDialog( "Heightmap Duplicated", "Saved as PNG in Assets/ as: " + fileName, "" );
	}

}
#endif

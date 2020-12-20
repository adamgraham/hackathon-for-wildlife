#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PrefabUtils
{
	[UnityEditor.MenuItem( "Tools/Revert Selected Prefabs" )]
	static private void RevertSelectedPrefabs()
	{
		GameObject[] selection = Selection.gameObjects;

		for ( int i = 0; i < selection.Length; i++ )
			PrefabUtility.RevertPrefabInstance( selection[i] );
	}

	[UnityEditor.MenuItem( "Tools/Replace Selected Prefabs" )]
	static private void ReplaceSelectedPrefabs()
	{
		ScriptableWizard.DisplayWizard( "Replace Selected Prefabs", typeof( ReplacePrefabs ), "Replace" );
	}

}

[System.Serializable]
public class ReplacePrefabs : ScriptableWizard
{
	public bool copyTransform = true;
	public bool keepChildren = false;

	public GameObject newObjectPrefab;

	private void OnWizardCreate()
	{
		foreach ( GameObject selection in Selection.gameObjects )
		{
			GameObject replacement = (GameObject)PrefabUtility.InstantiatePrefab( newObjectPrefab );

			replacement.transform.parent = selection.transform.parent;

			if ( copyTransform )
			{
				replacement.transform.position = selection.transform.position;
				replacement.transform.rotation = selection.transform.rotation;
				replacement.transform.localScale = selection.transform.localScale;
			}

			if ( keepChildren )
			{
				Transform[] children = selection.GetComponentsInChildren<Transform>();
				int len = children.Length;

				for ( int i = 0; i < len; i++ )
				{
					if ( children[i] != selection )
						children[i].parent = replacement.transform;
				}
			}

			DestroyImmediate( selection );
		}
	}

}
#endif

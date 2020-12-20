using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialMatcher : MonoBehaviour 
{
	public MaterialGroup[] materialGroups;
	public bool matchOnAwake = true;

	private void Awake()
	{
		if ( matchOnAwake )
			MatchMaterials();
	}

	public void MatchMaterials()
	{
		if ( materialGroups != null )
		{
			int len = materialGroups.Length;
			for ( int i = 0; i < len; i++ )
			{
				MaterialGroup group = materialGroups[i];
				if ( group != null )
				{
					if ( group.autoFindRenderers )
					{
						group.renderers = null;
						group.renderers = AutoFindRenderers( group.autoFindRoot, group.autoFindSearchString );

						if ( group.autoFindOnlyOnce )
							group.autoFindRenderers = false;
					}

					if ( group.renderers != null )
					{
						int len2 = group.renderers.Count;
						for ( int j = 0; j < len2; j++ )
						{
							Renderer renderer = group.renderers[j];
							if ( renderer != null )
							{
								Texture mainTexture = renderer.material.GetTexture( "_MainTex" );
								renderer.material = group.material;
								renderer.material.SetTexture( "_MainTex", mainTexture );
							}
						}
					}
				}
			}
		}
	}

	private List<Renderer> AutoFindRenderers( Transform root, string searchString )
	{
		List<Renderer> renderers = null;

		if ( root != null && searchString != null )
		{
			Renderer[] childrenRenderers = root.GetComponentsInChildren<Renderer>();

			if ( childrenRenderers != null )
			{
				searchString = searchString.ToLower();
				renderers = new List<Renderer>();

				int len = childrenRenderers.Length;
				for ( int i = 0; i < len; i++ )
				{
					Renderer renderer = childrenRenderers[i];

					if ( renderer != null )
					{
						if ( renderer.name.ToLower().Contains( searchString ) )
							renderers.Add( renderer );
					}
				}
			}
		}

		return renderers;
	}

}

[System.Serializable]
public class MaterialGroup
{
	public List<Renderer> renderers;
	public Material material;

	[Header( "Auto Finding" )]

	public bool autoFindRenderers;
	public bool autoFindOnlyOnce = true;
	public string autoFindSearchString;
	public Transform autoFindRoot;
}

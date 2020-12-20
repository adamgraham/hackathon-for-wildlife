using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class MaterialUtils
{
	static public void SetMatRenderingMode( Material material, MaterialRenderingMode renderingMode )
	{
		if ( material != null )
		{
			int currentMode = GetMatRenderingModeNum( material );

			switch ( renderingMode )
			{
				case MaterialRenderingMode.Opaque:
					if ( currentMode != 0 )
					{
						material.SetFloat( "_Mode", 0 );
						material.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
						material.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );
						material.SetInt( "_ZWrite", 1 );
						material.DisableKeyword( "_ALPHATEST_ON" );
						material.DisableKeyword( "_ALPHABLEND_ON" );
						material.DisableKeyword( "_ALPHAPREMULTIPLY_ON" );
						material.renderQueue = -1;
					}
					break;

				case MaterialRenderingMode.Cutout:
					if ( currentMode != 1 )
					{
						material.SetFloat( "_Mode", 1 );
						material.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
						material.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero );
						material.SetInt( "_ZWrite", 1 );
						material.EnableKeyword( "_ALPHATEST_ON" );
						material.DisableKeyword( "_ALPHABLEND_ON" );
						material.DisableKeyword( "_ALPHAPREMULTIPLY_ON" );
						material.renderQueue = 2450;
					}
					break;

				case MaterialRenderingMode.Fade:
					if ( currentMode != 2 )
					{
						material.SetFloat( "_Mode", 2 );
						material.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha );
						material.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );
						material.SetInt( "_ZWrite", 0 );
						material.DisableKeyword( "_ALPHATEST_ON" );
						material.EnableKeyword( "_ALPHABLEND_ON" );
						material.DisableKeyword( "_ALPHAPREMULTIPLY_ON" );
						material.renderQueue = 3000;
					}
					break;

				case MaterialRenderingMode.Transparent:
					if ( currentMode != 3 )
					{
						material.SetFloat( "_Mode", 3 );
						material.SetInt( "_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One );
						material.SetInt( "_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );
						material.SetInt( "_ZWrite", 0 );
						material.DisableKeyword( "_ALPHATEST_ON" );
						material.DisableKeyword( "_ALPHABLEND_ON" );
						material.EnableKeyword( "_ALPHAPREMULTIPLY_ON" );
						material.renderQueue = 3000;
					}
					break;
			}
		}
	}

	static public void SetMatRenderingMode( Material material, int renderingMode )
	{
		switch ( renderingMode )
		{
			case 0:
				SetMatRenderingMode( material, MaterialRenderingMode.Opaque );
				break;

			case 1:
				SetMatRenderingMode( material, MaterialRenderingMode.Cutout );
				break;

			case 2:
				SetMatRenderingMode( material, MaterialRenderingMode.Fade );
				break;

			case 3:
				SetMatRenderingMode( material, MaterialRenderingMode.Transparent );
				break;
		}
	}

	static public void GetMatRenderingMode( Material material )
	{
		int renderingMode = (int)material.GetFloat( "_Mode" );

		switch ( renderingMode )
		{
			case 0:
				SetMatRenderingMode( material, MaterialRenderingMode.Opaque );
				break;

			case 1:
				SetMatRenderingMode( material, MaterialRenderingMode.Cutout );
				break;

			case 2:
				SetMatRenderingMode( material, MaterialRenderingMode.Fade );
				break;

			case 3:
				SetMatRenderingMode( material, MaterialRenderingMode.Transparent );
				break;
		}
	}

	static public int GetMatRenderingModeNum( Material material )
	{
		return (int)material.GetFloat( "_Mode" );
	}

}

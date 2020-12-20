using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ColorUtils
{
	static public bool Compare( Color a, Color b, bool compareAlpha = false )
	{
		bool output = false;

		if ( (int)(a.r * 1000.0f) == (int)(b.r * 1000.0f) )
		{
			if ( (int)(a.g * 1000.0f) == (int)(b.g * 1000.0f) )
			{
				if ( (int)(a.b * 1000.0f) == (int)(b.b * 1000.0f) )
				{
					if ( !compareAlpha )
						output = true;
					else
					{
						if ( (int)(a.a * 1000.0f) == (int)(b.a * 1000.0f) )
						{
							output = true;
						}
					}
				}
			}
		}

		return output;
	}

}

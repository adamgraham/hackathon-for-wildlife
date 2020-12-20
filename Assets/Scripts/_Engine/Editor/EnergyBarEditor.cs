#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( EnergyBar ) )]
[CanEditMultipleObjects]
[System.Serializable]
public class EnergyBarEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EnergyBar script = (EnergyBar)target;

		if ( script.animationType == EnergyBar.AnimationType.BurnOff )
		{
			script.burnOffColor = EditorGUILayout.ColorField( "Burn Off Color", script.burnOffColor );
			script.burnOnColor = EditorGUILayout.ColorField( "Burn On Color", script.burnOnColor );
			script.burnTimeDelayPercent = EditorGUILayout.Slider( "Burn Time Delay Percent", script.burnTimeDelayPercent, 0.0f, 1.0f );
		}

		// save, apply, and serialize properties

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty( script );
		}
	}

}
#endif

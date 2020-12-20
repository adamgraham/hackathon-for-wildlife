#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( Player ) )]
[CanEditMultipleObjects]
[System.Serializable]
public class PlayerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Player script = (Player)target;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "Spawning", EditorStyles.boldLabel );

		script.spawnOnAwake = EditorGUILayout.Toggle( "Spawn On Awake", script.spawnOnAwake );

		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "Camera", EditorStyles.boldLabel );

		script.cameraScheme = (Player.CameraScheme)EditorGUILayout.EnumPopup( "Camera Scheme", script.cameraScheme );
		script.cameraFocus = (Transform)EditorGUILayout.ObjectField( "Camera Focus", script.cameraFocus, typeof( Transform ), true );

		if ( script.cameraScheme == Player.CameraScheme.FirstPerson || script.cameraScheme == Player.CameraScheme.ThirdPerson )
		{
			script.cameraFOV = EditorGUILayout.Slider( "Camera FOV", script.cameraFOV, 60.0f, 110.0f );

			if ( script.cameraScheme == Player.CameraScheme.ThirdPerson )
			{
				script.cameraAngle = EditorGUILayout.Slider( "Camera Angle", script.cameraAngle, -90.0f + Player.CAMERA_GIMBAL_SAFEGUARD, 90.0f - Player.CAMERA_GIMBAL_SAFEGUARD );
				script.cameraDistance = EditorGUILayout.FloatField( "Camera Distance", script.cameraDistance );
			}
			else
			{
				script.crosshair = (GameObject)EditorGUILayout.ObjectField( "Crosshair", script.crosshair, typeof( GameObject ), true );
			}
		}
		else if ( script.cameraScheme == Player.CameraScheme.Overhead )
		{
			script.cameraHeight = EditorGUILayout.FloatField( "Camera Height", script.cameraHeight );
			script.cameraFollow = EditorGUILayout.Toggle( "Camera Follow", script.cameraFollow );

			if ( script.cameraFollow )
				script.cameraDampTime = EditorGUILayout.FloatField( "Camera Damp Time", script.cameraDampTime );
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "Movement", EditorStyles.boldLabel );

		script.movementScheme = (Player.MovementScheme)EditorGUILayout.EnumPopup( "Movement Scheme", script.movementScheme );

		if ( script.movementScheme != Player.MovementScheme.Locked )
		{
			if ( script.movementScheme == Player.MovementScheme.Cardinal )
			{
				script.movementDirectionPriority = (Player.MovementAxisPriority)EditorGUILayout.EnumPopup( "Movement Direction Priority", script.movementDirectionPriority );
				script.movementDiagonalsAllowed = EditorGUILayout.Toggle( "Movement Diagonals Allowed", script.movementDiagonalsAllowed );

				if ( script.movementDiagonalsAllowed )
					script.movementDiagonalsOnly = EditorGUILayout.Toggle( "Movement Diagonals Only", script.movementDiagonalsOnly );
			}

			EditorGUILayout.Separator();

			script.rigidbodyMovementEnabled = EditorGUILayout.Toggle( "Rigidbody Movement Enabled", script.rigidbodyMovementEnabled );
			script.navMeshMovementEnabled = EditorGUILayout.Toggle( "NavMesh Movement Enabled", script.navMeshMovementEnabled );

			EditorGUILayout.Separator();

			script.canJump = EditorGUILayout.Toggle( "Can Jump", script.canJump );
			script.canRun = EditorGUILayout.Toggle( "Can Run", script.canRun );
			script.canWalk = EditorGUILayout.Toggle( "Can Walk", script.canWalk );
			script.canCrouch = EditorGUILayout.Toggle( "Can Crouch", script.canCrouch );

			EditorGUILayout.Separator();

			script.speed = EditorGUILayout.FloatField( "Speed", script.speed );
			script.speedMultiplier = EditorGUILayout.FloatField( "Speed Multiplier", script.speedMultiplier );
			script.speedBackpedalMultplier = EditorGUILayout.FloatField( "Speed Backpedal Multiplier", script.speedBackpedalMultplier );
			
			if ( script.canRun )
				script.speedRunMultiplier = EditorGUILayout.FloatField( "Speed Run Multiplier", script.speedRunMultiplier );
			
			if ( script.canWalk )
				script.speedWalkMultiplier = EditorGUILayout.FloatField( "Speed Walk Multiplier", script.speedWalkMultiplier );

			if ( script.canCrouch )
				script.speedCrouchMultiplier = EditorGUILayout.FloatField( "Speed Crouch Multiplier", script.speedCrouchMultiplier );

			if ( script.canJump )
			{
				EditorGUILayout.Separator();

				script.jumpForce = EditorGUILayout.FloatField( "Jump Force", script.jumpForce );
				script.doubleJumps = EditorGUILayout.IntField( "Double Jumps", script.doubleJumps );

				if ( script.doubleJumps > 0 )
					script.doubleJumpForce = EditorGUILayout.FloatField( "Double Jump Force", script.doubleJumpForce );
			}

			if ( script.canCrouch )
			{
				EditorGUILayout.Separator();

				script.crouchLookPositionY = EditorGUILayout.FloatField( "Crouch Look Position Y", script.crouchLookPositionY );
				script.crouchTweenDuration = EditorGUILayout.FloatField( "Crouch Tween Duration", script.crouchTweenDuration );
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "Look Rotation", EditorStyles.boldLabel );

		script.lookScheme = (Player.LookScheme)EditorGUILayout.EnumPopup( "Look Scheme", script.lookScheme );

		if ( script.lookScheme != Player.LookScheme.Locked )
		{
			script.lookTransform = (Transform)EditorGUILayout.ObjectField( "Look Transform", script.lookTransform, typeof( Transform ), true );

			if ( script.lookScheme == Player.LookScheme.Axis )
			{
				script.lookSensitivity = EditorGUILayout.Slider( "Look Sensitivity", script.lookSensitivity, 1.0f, 10.0f );
			}
			else if ( script.lookScheme == Player.LookScheme.Cardinal )
			{
				script.lookMovementMultiplier = EditorGUILayout.Slider( "Look Rotation Movement Multiplier", script.lookMovementMultiplier, 0.0f, 1.0f );
				script.lookDiagonalsAllowed = EditorGUILayout.Toggle( "Look Diagonals Allowed", script.lookDiagonalsAllowed );

				if ( script.lookDiagonalsAllowed )
					script.lookDiagonalsOnly = EditorGUILayout.Toggle( "Look Diagonals Only", script.lookDiagonalsOnly );
			}
		}

		SerializedProperty controls = serializedObject.FindProperty( "controls" );
		EditorGUILayout.PropertyField( controls, true );

		// save, apply, and serialize properties

		if ( GUI.changed )
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty( script );
		}
	}

}
#endif

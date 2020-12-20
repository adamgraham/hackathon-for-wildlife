using UnityEngine;
using System.Collections;

public class SpriteDirectionFlipper : MonoBehaviour
{
	#region Variables

	public enum Direction { Forward, Backward, Left, Right, Idle };
	public enum DirectionPriority { Vertical, Horizontal };

	private bool _started;

	[Header( "References" )]

	public Transform movingTransform;

	public GameObject defaultSide;
	public GameObject frontSide;
	public GameObject backSide;
	public GameObject leftSide;
	public GameObject rightSide;

	[Header( "Swithcing" )]

	private Vector3 _previousPosition;
	private GameObject _currentSide;
	private SpriteDirectionFlipper.Direction _direction;

	[Header( "Priority" )]

	public SpriteDirectionFlipper.DirectionPriority priority;

	[Header( "Cooldown" )]

	public float cooldown;

	private bool _cooldown;

	#endregion

	#region Unity Events

	private void Start()
	{
		StoreInitialData();
		_started = true;
	}

	private void OnEnable()
	{
		if ( _started )
			StoreInitialData();
	}

	private void StoreInitialData()
	{
		if ( movingTransform != null )
			_previousPosition = movingTransform.position;

		if ( _currentSide == null )
		{
			DisableAllSides();

			if ( defaultSide != null )
			{
				if ( defaultSide == frontSide )
					Switch( Direction.Backward );
				else if ( _currentSide == backSide )
					Switch( Direction.Forward );
				else if ( _currentSide == leftSide )
					Switch( Direction.Left );
				else if ( _currentSide == rightSide )
					Switch( Direction.Right );
			}
		}
	}

	#endregion

	#region Update

	private void LateUpdate()
	{
		if ( movingTransform != null )
		{
			if ( priority == DirectionPriority.Horizontal )
			{
				if ( movingTransform.position.x < _previousPosition.x )
					Switch( Direction.Left );
				else if ( movingTransform.position.x > _previousPosition.x )
					Switch( Direction.Right );
				else if ( movingTransform.position.z > _previousPosition.z )
					Switch( Direction.Forward );
				else if ( movingTransform.position.z < _previousPosition.z )
					Switch( Direction.Backward );
			}
			else
			{
				if ( movingTransform.position.z > _previousPosition.z )
					Switch( Direction.Forward );
				else if ( movingTransform.position.z < _previousPosition.z )
					Switch( Direction.Backward );
				else if ( movingTransform.position.x < _previousPosition.x )
					Switch( Direction.Left );
				else if ( movingTransform.position.x > _previousPosition.x )
					Switch( Direction.Right );
			}

			_previousPosition = movingTransform.position;
		}
	}

	#endregion

	#region Switching

	public void Switch( Direction direction )
	{
		if ( direction != _direction )
		{
			_direction = direction;

			if ( _currentSide != null )
			{
				_currentSide.gameObject.SetActive( false );
				_currentSide = null;
			}

			if ( _direction == Direction.Forward )
				_currentSide = backSide;
			else if ( _direction == Direction.Backward )
				_currentSide = frontSide;
			else if ( _direction == Direction.Left )
				_currentSide = leftSide;
			else if ( _direction == Direction.Right )
				_currentSide = rightSide;

			if ( _currentSide == null )
				_currentSide = defaultSide;

			if ( _currentSide != null )
				_currentSide.gameObject.SetActive( true );
		}
	}

	public void DisableAllSides()
	{
		if ( frontSide != null ) frontSide.gameObject.SetActive( false );
		if ( backSide != null ) backSide.gameObject.SetActive( false );
		if ( leftSide != null ) leftSide.gameObject.SetActive( false );
		if ( rightSide != null ) rightSide.gameObject.SetActive( false );

		_currentSide = null;
		_direction = Direction.Idle;
	}

	#endregion

	#region Cooldown

	public void StartCooldown()
	{
		_cooldown = true;

		if ( cooldown > 0.0f )
			Invoke( "StopCooldown", cooldown );
		else
			StopCooldown();
	}

	public void StopCooldown()
	{
		_cooldown = false;
	}

	#endregion

}

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	static public GameManager instance;

	public World world;
	public Elephant player;
	public Hunter hunter;

	private bool _gameOver;

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		if ( instance == this )
			instance = null;
	}

	private void Start()
	{
		ScreenFader.FadeFromWhite( 5.0f );
		world.Generate();
		player.Spawn( world.GetRandomCubeOfType( EnvironmentCube.CubeType.Grass ) );
		hunter.Spawn( world.GetRandomCubeOfType( EnvironmentCube.CubeType.Water ) );
	}

	public void GameOver()
	{
		if ( !_gameOver )
		{
			_gameOver = true;
			SceneTransitioner.TransitionToScene( "GameOver", 2.5f, SceneTransitioner.FadeColor.White );
		}
	}

}

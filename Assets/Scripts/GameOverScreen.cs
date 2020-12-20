using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GameOverScreen : MonoBehaviour 
{
	public Text title;
	public Text clickToRestart;
	public Text infoText;
	public Image greenBarTitle;
	public Image greenBarClickToBegin;

	private void Start()
	{
		//ScreenFader.FadeFromWhite();
	}
	
	private void Update()
	{
		if ( Input.GetMouseButtonDown( 0 ) )
		{
			if ( infoText.gameObject.activeSelf )
				StartGame();
			else
				Begin();
		}
	}
	
	private void Begin()
	{
		infoText.gameObject.SetActive( true );
		infoText.color = new Color( infoText.color.r, infoText.color.g, infoText.color.b, 0.0f );
		infoText.DOFade( 1.0f, 1.0f );
	}
	
	private void StartGame()
	{
		enabled = false;
		
		float fadeDuration = 1.5f;
		ScreenFader.FadeToWhite( 2.0f, OnFadeComplete );
		title.DOFade( 0.0f, fadeDuration );
		clickToRestart.DOFade( 0.0f, fadeDuration );
		infoText.DOFade( 0.0f, fadeDuration );
		greenBarTitle.DOFade( 0.0f, fadeDuration );
		greenBarClickToBegin.DOFade( 0.0f, fadeDuration );
	}
	
	private void OnFadeComplete()
	{
		Application.LoadLevel( "TitleScreen" );
	}
	
}

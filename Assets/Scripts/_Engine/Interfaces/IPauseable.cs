using UnityEngine;
using System.Collections;

public interface IPauseable
{
	void Pause();
	void Unpause();
	void TogglePause();
	bool IsPaused();
}

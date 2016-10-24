using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : RJWard.Core.Singleton.SingletonSceneLifetime< UIManager >
{
	public Color buttonNormalColour = new Color( 100f/255f, 100f/255f, 100f/255f );
	public Color buttonPressedColour = new Color( 100f / 255f, 200f / 255f, 100f / 255f );
	public Color buttonHighlightedColour = new Color( 100f / 255f, 100f / 255f, 200f / 255f );
	public Color buttonDisabledColour = new Color( 200f / 255f, 100f / 255f, 100f / 255f );

	public float buttonFadeDuration = 0.1f;

}

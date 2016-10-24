using UnityEngine;
using System.Collections;

[RequireComponent (typeof( UnityEngine.UI.Button ))]
public class ButtonSkinner : Skinner
{
	private void Start()
	{
		SkinMe( );
	}

	public override void SkinMe()
	{
		if (!UIManager.IsInitialised())
		{
			throw new System.Exception( "No UIManager when skinning button " + gameObject.name );
		}

		UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>( );

		UnityEngine.UI.ColorBlock buttonColours = button.colors;
        buttonColours.normalColor = UIManager.Instance.buttonNormalColour;
		buttonColours.highlightedColor = UIManager.Instance.buttonHighlightedColour;
		buttonColours.pressedColor = UIManager.Instance.buttonPressedColour;
		buttonColours.disabledColor = UIManager.Instance.buttonDisabledColour;
		buttonColours.fadeDuration = UIManager.Instance.buttonFadeDuration;

		button.colors = buttonColours;
	}

}

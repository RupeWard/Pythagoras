using UnityEngine;
using System.Collections;

public class SceneControllerProof : SceneController_Base 
{
	//	static private readonly bool DEBUG_LOCAL = false;

	#region inspector hooks

	public UnityEngine.UI.Button playButton;

	#endregion inspector hooks

	#region event handlers

	#endregion event handlers

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Proof;
	}

	override protected void PostStart()
	{
	}

	override protected void PostAwake()
	{
	}


	#endregion SceneController_Base

}


using UnityEngine;
using System.Collections;

public class SceneControllerDevSetUp : SceneController_Base 
{
#region inspector hooks

	public UnityEngine.UI.Text versionText;
	public float delay = 2f;

	public GameObject buttonPanel;

	#endregion inspector hooks

	private void MoveOn()
	{
		buttonPanel.gameObject.SetActive( true );
	}

#region event handlers

	public void HandleProofEngine1ModeButton()
	{
		SceneControllerProof.mode_ = SceneControllerProof.EMode.ProofEngine1;
		SceneManager.Instance.SwitchScene( SceneManager.EScene.Proof );
	}

	public void HandleProofEngine2ModeButton( )
	{
		SceneControllerProof.mode_ = SceneControllerProof.EMode.ProofEngine2;
		SceneManager.Instance.SwitchScene( SceneManager.EScene.Proof );
	}

	public void HandleInternalModeButton( )
	{
		SceneControllerProof.mode_ = SceneControllerProof.EMode.Internal;
		SceneManager.Instance.SwitchScene( SceneManager.EScene.Proof );
	}

	#endregion event handlers

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.DevSetUp;
	}

	override protected void PostStart()
	{
		versionText.text = RJWard.Core.Version.versionNumber.DebugDescribe ();

		buttonPanel.gameObject.SetActive( false );

		Invoke( "MoveOn", delay );
	}

	#endregion SceneController_Base

}

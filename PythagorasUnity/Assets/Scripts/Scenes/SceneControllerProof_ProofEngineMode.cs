using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

/* 
	Functions and fields only used when mode = ProofEngine1 or ProofEngine2
*/
public partial class SceneControllerProof : SceneController_Base
{
	#region Forward button

	public void HandleForwardButtonProofEngineMode( )
	{
#if UNITY_EDITOR
		ClearTestElements( );
#endif
		if (proofEngine_ == null)
		{
			switch (mode_)
			{
				case EMode.ProofEngine1:
					{
						CreateProofEngine1( );
						break;
					}
				case EMode.ProofEngine2:
					{
						CreateProofEngine2( );
						break;
					}
				default:
					{
						throw new System.Exception( "Should be in HandleForwardButtonProofEngineMode when mode = " + mode_ );
					}
			}
		}

		StepForward( );
	}

	private static readonly Dictionary< ProofEngine.EDirection, Quaternion > forwardButtonRotations = new Dictionary< ProofEngine.EDirection, Quaternion >( )
	{
		{ ProofEngine.EDirection.Forward, Quaternion.Euler( new Vector3( 0f, 0f, 180f)) },
		{ ProofEngine.EDirection.Reverse, Quaternion.Euler( new Vector3( 0f, 0f, 0f)) }
	};

    private void SetForwardButtonDirection( ProofEngine.EDirection dirn )
	{
		if (UIManager.DEBUG_UI)
		{
			Debug.LogWarning( "SetForwardButtonDirection to " + dirn );
		}
		forwardButton.transform.rotation = forwardButtonRotations[dirn];
	}

	#endregion Forward button

	#region Change direction button

	public void HandleChangeDirectionButton()
	{
		if (proofEngine_ != null)
		{
			proofEngine_.ChangeDirection( );
		}
		else
		{
			Debug.LogWarning( "Can't change direction as no proof engine" );
		}
	}

	#endregion Change direction button

	#region Loop button

	public void HandleLoopButton()
	{
		if (proofEngine_ != null)
		{
			proofEngine_.ToggleLoop( );
		}
		else
		{
			Debug.LogWarning( "Can't change loop as no proof engine" );
		}
	}

	private void SetLoopButtonSprite(bool loop)
	{
		Sprite s = (loop) ? (loopButtonSprite_Off) : (loopButtonSprite_On);
		loopButtonImage.sprite = s;
	}

	#endregion Loop button

	#region triangle settings

	private void HandleAngleChangedProofEngineMode( )
	{
		if (proofEngine_ != null)
		{
			CreateProofEngine1( );
			proofEngine_.Resume( );
		}
	}

	#endregion triangle settings

	#region speed panel

	private void HandleSpeedChangedProofEngineMode( )
	{
		if (proofEngine_ != null)
		{
			proofEngine_.SetSpeed( initialSpeed );
		}
	}

	#endregion speed panel

	// Following region is for when proofEngineMode == true
	#region proof engine sequence

	private ProofEngine proofEngine_ = null;
	/*
	private void HandleProofStageStarted( ProofStageBase psb )
	{
		ShowOrHideTriangleSettings( );
    }
	*/

	private void ShowOrHideTriangleSettings()
	{
		if (elements_.GetElementOfType< Element_Parallelogram >( parallelogramNames_[0] ) == null)
		{
			// Not yet made normal, so can change triangle
			EnableTriangleSettings( );
		}
		else
		{
			DisableTriangleSettings( );
		}
	}

	private void HandleProofStageFinished( ProofStageBase psb )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleProofStageFinished( " + psb.name + ")" );
		}


		if (false == fastForward_ && false == psb.dontPauseOnFinish )
		{
			proofEngine_.Pause( );
		}
		proofEngine_.ChangeToFollowingStage( psb );
		ShowOrHideTriangleSettings( );
    }

	private void StepForward( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "StepForward()" );
		}
		proofEngine_.TogglePause( );
	}

	#endregion proof engine sequence


}

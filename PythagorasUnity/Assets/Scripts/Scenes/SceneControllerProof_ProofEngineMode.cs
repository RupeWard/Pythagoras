﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

/* 
	Functions and fields only used in proof engine mode
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
			CreateProofEngine( );
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
		if (DEBUG_PROOF)
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

	#region triangle settings

	private void HandleAngleChangedProofEngineMode( )
	{
		if (proofEngine_ != null)
		{
			CreateProofEngine( );
			proofEngine_.Resume( );
		}
	}

	#endregion triangle settings

	// Following region is for when proofEngineMode == true
	#region proof engine sequence

	private ProofEngine proofEngine_ = null;
	private void CreateProofEngine( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateProofEngine()" );
		}

		if (proofEngine_ != null)
		{
			if (DEBUG_PROOF)
			{
				Debug.Log( "CreateProofEngine destroying proofEngine" );
			}
			GameObject.Destroy( proofEngine_.gameObject );
		}
		proofEngine_ = (new GameObject( "ProofEngine" )).AddComponent< ProofEngine >( );
		proofEngine_.onPauseAction += SetForwardButtonSprite;
		proofEngine_.onDirectionChangedAction += SetForwardButtonDirection;

		if (elements_.NumElements > 0)
		{
			if (DEBUG_PROOF)
			{
				Debug.Log( "CreateProofEngine destroying "+elements_.NumElements+" elements" );
			}
			elements_.DestroyAllElements( );
		}

		ProofStage_CreateRightTriangle createTriangleStage = new ProofStage_CreateRightTriangle(
			"Create Triangle",
			"Creating main triangle",
			geometryFactory_,
			mainField_,
			createTriangleDuration,
			HandleProofStageFinished,
			0f,
			new Vector2[]
				{
					new Vector2(-1f, 0f),
					new Vector2(1f, 0f)
				},
			initialAngle,
			mainTriangleColour,
			mainTriangleName_
			);

		ProofStage_CreateTriangleSide createSide1Stage = new ProofStage_CreateTriangleSide(
			"Create Side 1",
			"This is side 1",
			geometryFactory_,
			mainField_,
			showSideDuration_,
			HandleProofStageFinished,
			mainTriangleName_,
			1,
			true,
			-0.01f,
			0.01f,
			Color.black,
			triangleSideNames_[1]
			);

		createSide1Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		ProofStageBase.ConnectStages( createTriangleStage, createSide1Stage );

		ProofStage_ExtrudeLineToSquare createSquare1Stage = new ProofStage_ExtrudeLineToSquare(
			"Create Square 1",
			"Extruding side 1 to a square",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			triangleSideNames_[1],
			0f,
			90f,
			square0Colour,
			parallelogramNames_[0]
			);

		createSquare1Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse );

		ProofStageBase.ConnectStages( createSide1Stage, createSquare1Stage );

		ProofStage_CreateTriangleSide createSide2Stage = new ProofStage_CreateTriangleSide(
			"Create Side 2",
			"This is side 2",
			geometryFactory_,
			mainField_,
			showSideDuration_,
			HandleProofStageFinished,
			mainTriangleName_,
			2,
			true,
			-0.01f,
			0.01f,
			Color.black,
			triangleSideNames_[2]
			);

		createSide2Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		ProofStageBase.ConnectStages( createSquare1Stage, createSide2Stage );

		ProofStage_ExtrudeLineToSquare createSquare2Stage = new ProofStage_ExtrudeLineToSquare(
			"Create Square 2",
			"Extruding side 2 to a square",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			triangleSideNames_[2],
			0f,
			90f,
			square1Colour,
			parallelogramNames_[1]
			);

		createSquare2Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse );

		ProofStageBase.ConnectStages( createSide2Stage, createSquare2Stage );

		createTriangleStage.Init( ProofEngine.EDirection.Forward, elements_ );
		proofEngine_.Init( createTriangleStage );
		if (!proofEngine_.isPaused)
		{
			proofEngine_.Pause( );
		}

		changeDirectionButton.gameObject.SetActive( true );
	}

	private void HandleProofStageStarted( ProofStageBase psb )
	{
		if (elements_.GetElementOfType< Element_Parallelogram >( parallelogramNames_[0] ) == null)
		{
			// Not yet made first square, so can change triangle
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

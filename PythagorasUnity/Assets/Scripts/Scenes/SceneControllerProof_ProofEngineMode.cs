using UnityEngine;
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

		if (proofEngine_ != null)
		{
			StepForward( );
		}
		else
		{
			Debug.LogError( "No ProofEngine on ForwardButton press" );
		}
	}

	#endregion Forward button

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
			GameObject.Destroy( proofEngine_.gameObject );
		}
		proofEngine_ = (new GameObject( "ProofEngine" )).AddComponent<ProofEngine>( );

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

		createSide1Stage.SetDontPauseOnFinish( ProofStageBase.EDirection.Forward );

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

		createSquare1Stage.SetDontPauseOnFinish( ProofStageBase.EDirection.Reverse );

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

		createSide2Stage.SetDontPauseOnFinish( ProofStageBase.EDirection.Forward );

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

		createSquare2Stage.SetDontPauseOnFinish( ProofStageBase.EDirection.Reverse );

		ProofStageBase.ConnectStages( createSide2Stage, createSquare2Stage );

		createTriangleStage.Init( ProofStageBase.EDirection.Forward, elements_ );
		proofEngine_.Init( createTriangleStage );
		if (!proofEngine_.isPaused)
		{
			proofEngine_.Pause( );
		}
	}

	private void HandleProofStageStarted( ProofStageBase psb )
	{
		if (elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[0] ) == null)
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
		SetForwardButtonSprite( !proofEngine_.isPaused );
	}

	private void StepForward( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "StepForward()" );
		}
		proofEngine_.TogglePause( );
		SetForwardButtonSprite( !proofEngine_.isPaused );
	}

	#endregion proof engine sequence


}

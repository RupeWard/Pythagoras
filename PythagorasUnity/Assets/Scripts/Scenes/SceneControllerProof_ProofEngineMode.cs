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
			CreateProofEngine( );
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
		proofEngine_.onLoopChangedAction += SetLoopButtonSprite;

		proofEngine_.SetSpeed( initialSpeed );

		if (elements_.NumElements > 0)
		{
			if (DEBUG_PROOF)
			{
				Debug.Log( "CreateProofEngine destroying "+elements_.NumElements+" elements" );
			}
			elements_.DestroyAllElements( );
		}

		ProofStage_CreateRightTriangle createTriangle_Stage = new ProofStage_CreateRightTriangle(
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

		proofEngine_.RegisterStage( createTriangle_Stage );

		// TODO remove once triangles have their own sides
		ProofStage_CreateTriangleSide createSide1_Stage = new ProofStage_CreateTriangleSide(
			"Create Side 1",
			"This is side 1",
			geometryFactory_,
			mainField_,
			showSideDuration_,
			HandleProofStageFinished,
			mainTriangleName_,
			1,
			true,
			-GeometryHelpers.internalLayerSeparation,
			0.01f,
			Color.black,
			triangleSideNames_[1]
			);

		createSide1_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing( createSide1_Stage, createTriangle_Stage);

		ProofStage_ExtrudeLineToSquare createSquare1_Stage = new ProofStage_ExtrudeLineToSquare(
			"Create Square 1",
			"Extruding side 1 to a square",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			triangleSideNames_[1],
			-GeometryHelpers.externalLayerSeparation,
			90f,
			square0Colour,
			parallelogramNames_[0]
			);

		createSquare1_Stage.startReversedDestroyElementListDefinition
			= new ElementListDefinition(
				"CreateSquare1_Stage Reversed Start DestroyList",
				new Dictionary< string, System.Type >( )
				{ 
					{  shadowSquareNames_[0], typeof(Element_Parallelogram) }
				}
			);

		createSquare1_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse );

		proofEngine_.RegisterStageFollowing(createSquare1_Stage, createSide1_Stage);

		// TODO remove once triangles have their own sides
		ProofStage_CreateTriangleSide createSide2_Stage = new ProofStage_CreateTriangleSide(
			"Create Side 2",
			"This is side 2",
			geometryFactory_,
			mainField_,
			showSideDuration_,
			HandleProofStageFinished,
			mainTriangleName_,
			2,
			true,
			-GeometryHelpers.internalLayerSeparation,
			0.01f,
			Color.black,
			triangleSideNames_[2]
			);

		createSide2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing(createSide2_Stage, createSquare1_Stage);

		ProofStage_ExtrudeLineToSquare createSquare2_Stage = new ProofStage_ExtrudeLineToSquare(
			"Create Square 2",
			"Extruding side 2 to a square",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			triangleSideNames_[2],
			-GeometryHelpers.externalLayerSeparation,
			90f,
			square1Colour,
			parallelogramNames_[1]
			);

		createSquare2_Stage.startReversedDestroyElementListDefinition
			= new ElementListDefinition(
				"CreateSquare2_Stage Reversed Start DestroyList",
				new Dictionary< string, System.Type >( )
				{
					{  shadowSquareNames_[1], typeof(Element_Parallelogram) }
				}
			);

		createSquare2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse );

		proofEngine_.RegisterStageFollowing(createSquare2_Stage, createSide2_Stage );

		ProofStage_CloneElement createShadowSquare1_Stage = new ProofStage_CloneElement(
			"Create Shadow Square 1",
			"Creating shadow square 1",
			geometryFactory_,
			mainField_,
			0f,
			HandleProofStageFinished,
			parallelogramNames_[0],
			typeof( Element_Parallelogram ),
			GeometryHelpers.externalLayerSeparation,
			shadowColour,
			shadowSquareNames_[0]
			);

		createShadowSquare1_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing( createShadowSquare1_Stage, createSquare2_Stage );

		ProofStage_ShearParallelogram shearSquare1_Stage = new ProofStage_ShearParallelogram(
			"Shear Square 1",
			"Shearing square 1",
			geometryFactory_,
			mainField_,
			shearSquareDuration,
			HandleProofStageFinished,
			parallelogramNames_[0],
			shearAlpha,
			new AngleProvider_Parallelogram(
				parallelogramNames_[0],
				0,
				GeometryHelpers.EAngleModifier.Raw ),
			new AngleProvider_Polygon(
				mainTriangleName_,
				0,
				GeometryHelpers.EAngleModifier.Complementary
				)
			);

		proofEngine_.RegisterStageFollowing( shearSquare1_Stage , createShadowSquare1_Stage);
		
		ProofStage_CloneElement createShadowSquare2_Stage = new ProofStage_CloneElement(
			"Create Shadow Square 2",
			"Creating shadow square 2",
			geometryFactory_,
			mainField_,
			0f,
			HandleProofStageFinished,
			parallelogramNames_[1],
			typeof( Element_Parallelogram ),
			GeometryHelpers.externalLayerSeparation,
			shadowColour,
			shadowSquareNames_[1]
		);

		createShadowSquare2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing( createShadowSquare2_Stage, shearSquare1_Stage );

		ProofStage_ShearParallelogram shearSquare2_Stage = new ProofStage_ShearParallelogram(
			"Shear Square 2",
			"Shearing square 2",
			geometryFactory_,
			mainField_,
			shearSquareDuration,
			HandleProofStageFinished,
			parallelogramNames_[1],
			shearAlpha,
			new AngleProvider_Parallelogram(
				parallelogramNames_[1],
				0,
				GeometryHelpers.EAngleModifier.Raw ),
			new AngleProvider_Polygon(
				mainTriangleName_,
				0,
				GeometryHelpers.EAngleModifier.Supplementary
				)
			);

		proofEngine_.RegisterStageFollowing( shearSquare2_Stage , createShadowSquare2_Stage );

		ProofStage_RemoveElement removeShadowSquare1_Stage = new ProofStage_RemoveElement(
			"RemoveShadow Square 1",
			"Removing shadow square 1",
			geometryFactory_,
			mainField_,
			0.2f,
			HandleProofStageFinished,
			shadowSquareNames_[0],
			typeof(Element_Parallelogram) );

		removeShadowSquare1_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing(removeShadowSquare1_Stage , shearSquare2_Stage );

		ProofStage_RemoveElement removeShadowSquare2_Stage = new ProofStage_RemoveElement(
			"RemoveShadow Square 2",
			"Removing shadow square 2",
			geometryFactory_,
			mainField_,
			0.2f,
			HandleProofStageFinished,
			shadowSquareNames_[1],
			typeof( Element_Parallelogram ) );

		removeShadowSquare2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse);
		removeShadowSquare2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.RegisterStageFollowing( removeShadowSquare2_Stage, removeShadowSquare1_Stage );

		ProofStage_ShearParallelogram shearParallelogram0_Stage = new ProofStage_ShearParallelogram(
			"Shear Parallelogram 0",
			"Shearing parallelogram 0",
			geometryFactory_,
			mainField_,
			shearSquareDuration,
			HandleProofStageFinished,
			parallelogramNames_[0],
			1,
			shearAlpha,
			new AngleProvider_Parallelogram(
				parallelogramNames_[0],
				0,
				GeometryHelpers.EAngleModifier.Raw ),
			new AngleProvider_Constant(
				90f
			)
		);

		proofEngine_.RegisterStageFollowing( shearParallelogram0_Stage, removeShadowSquare2_Stage );
		
		ProofStage_ShearParallelogram shearParallelogram1_Stage = new ProofStage_ShearParallelogram(
			"Shear Parallelogram 1",
			"Shearing parallelogram 1",
			geometryFactory_,
			mainField_,
			shearSquareDuration,
			HandleProofStageFinished,
			parallelogramNames_[1],
			3,
			shearAlpha,
			new AngleProvider_Parallelogram(
				parallelogramNames_[1],
				0,
				GeometryHelpers.EAngleModifier.Raw ),
			new AngleProvider_Constant(
				90f
			)
		);

		proofEngine_.RegisterStageFollowing( shearParallelogram1_Stage, shearParallelogram0_Stage );

		proofEngine_.SkipStagesInReverse(
			new HashSet<ProofStageBase>( )
			{
				createShadowSquare1_Stage,
				createShadowSquare2_Stage,
				removeShadowSquare1_Stage,
				removeShadowSquare2_Stage
			}
		);

		createTriangle_Stage.Init( ProofEngine.EDirection.Forward, elements_ );
		proofEngine_.Start( createTriangle_Stage );
		if (!proofEngine_.isPaused)
		{
			proofEngine_.Pause( );
		}

		changeDirectionButton.gameObject.SetActive( true );
		loopButton.gameObject.SetActive( true );
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

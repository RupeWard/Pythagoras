﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

/* 
	Functions and fields only used when mode = ProofEngine1
*/
public partial class SceneControllerProof : SceneController_Base
{
	#region proof engine sequence

	private void CreateProofEngine1( )
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
			"CreateMainTriangle",
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

		proofEngine_.AddStageToEnd( createTriangle_Stage );

#if UNITY_EDITOR // temp pulse examples
		/*
		ProofStage_PulseDisplayElement pulse0 = new ProofStage_PulseDisplayElement(
			"Pulse MT",
			geometryFactory_, mainField_, 2f, HandleProofStageFinished,
			new ElementProvider_Name(mainTriangleName_),
			1.5f
			);
		proofEngine_.AddStageToEnd( pulse0 );

		ProofStage_PulseDisplayElement pulse1 = new ProofStage_PulseDisplayElement(
			"Pulse RA", 
			geometryFactory_, mainField_, 2f, HandleProofStageFinished,
			new AngleProvider_Polygon( mainTriangleName_, 1, GeometryHelpers.EAngleModifier.Raw),
			1.5f);
		proofEngine_.AddStageToEnd( pulse1 );
		*/
#endif

		//////////////////

		ProofStage_ExtrudeLineToSquare createSquare1_Stage = new ProofStage_ExtrudeLineToSquare(
			"CreateSquare1",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			new StraightLineProvider_Polygon( mainTriangleName_, 1 ),
			-GeometryHelpers.externalLayerSeparation,
			90f,
			squareColours[0],
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


		proofEngine_.AddStageToEnd(createSquare1_Stage);

#if UNITY_EDITOR // temp pulse examples
		/*
		ProofStage_PulseDisplayElement pulse2 = new ProofStage_PulseDisplayElement(
			"PulseSQ", 
			geometryFactory_, mainField_, 2f, HandleProofStageFinished,
			new ElementProvider_Name( parallelogramNames_[0]),
			1.5f );
		proofEngine_.AddStageToEnd( pulse2 );
		*/
#endif

		//////////////////////

		ProofStage_ExtrudeLineToSquare createSquare2_Stage = new ProofStage_ExtrudeLineToSquare(
			"CreateSquare2",
			geometryFactory_,
			mainField_,
			createSquareDuration,
			HandleProofStageFinished,
			new StraightLineProvider_Polygon(mainTriangleName_, 2),
			-GeometryHelpers.externalLayerSeparation,
			90f,
			squareColours[1],
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


		proofEngine_.AddStageToEnd( createSquare2_Stage);
		
		ProofStage_CreateLineThroughPointAtAngleToLine createMainTriangleNormal_Stage = new ProofStage_CreateLineThroughPointAtAngleToLine(
			"CreateMainTriangleNormal",
			geometryFactory_,
			mainField_,
			createNormalDuration,
			HandleProofStageFinished,
			new PointProvider_PolygonVertex( mainTriangleName_, 2 ),
			new StraightLineProvider_Polygon( mainTriangleName_, 0 ),
			90f,
			-3f * GeometryHelpers.externalLayerSeparation,
			0.02f,
			Color.gray,
			mainTriangleNormalName_,
            new LineExtender_Constant( new Vector2( 0f, 4f ) )
            );

		proofEngine_.AddStageToEnd( createMainTriangleNormal_Stage);

		//////////////////

		ProofStage_CloneElement createShadowSquare1_Stage = new ProofStage_CloneElement(
			"CreateShadowSquare1",
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

		proofEngine_.AddStageToEnd( createShadowSquare1_Stage);

		////////////

		ProofStage_CreatePolygonSide createShearSquareGuideline0_Stage = new ProofStage_CreatePolygonSide(
			"CreateShearSquareGuideline0",
			geometryFactory_,
			mainField_,
			shearSquareGuidelineCreationDuration_,
			HandleProofStageFinished,
			new StraightLineProvider_Polygon
			(
				parallelogramNames_[0],
				2
				),
			-3f * GeometryHelpers.externalLayerSeparation,
			0.02f,
			Color.gray,
			shearSquareGuidelineNames_[0],
			new LineExtender_LineIntersection(
				new StraightLineProvider_Name( mainTriangleNormalName_ )
				)
			);

		proofEngine_.AddStageToEnd( createShearSquareGuideline0_Stage);

		///////////////////////////////

		ProofStage_ShearParallelogram shearSquare1_Stage = new ProofStage_ShearParallelogram(
			"ShearSquare1",
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

		shearSquare1_Stage.startReversedDestroyElementListDefinition
			= new ElementListDefinition(
				"shearSquare1_Stage Reversed Start DestroyList",
				new Dictionary<string, System.Type>( )
				{
					{  congruentTriangleNames_[0], typeof(Element_Triangle) }
				}
			);
		
		proofEngine_.AddStageToEnd( shearSquare1_Stage );

		//////////////////////////////////////////////

		ProofStage_HideElements hideShearSquare1Ancillaries_Stage = new ProofStage_HideElements(
			"HideShearSquareAncillaries1",
			geometryFactory_,
			mainField_,
			removeShadowDuration,
			HandleProofStageFinished,
			new Dictionary<string, System.Type>()
			{
				{shadowSquareNames_[0], typeof( Element_Parallelogram )},
				{ shearSquareGuidelineNames_[0], typeof( Element_StraightLine) }
			}
		 );

		hideShearSquare1Ancillaries_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.AddStageToEnd( hideShearSquare1Ancillaries_Stage);

		///////////////////////////////////

		ProofStage_CreateTriangleFromSides createCongruentTriangle1_Stage = new ProofStage_CreateTriangleFromSides(
			"CreateCongruentTriangle1",
			geometryFactory_,
			mainField_,
			createCongruentTriangleDuration,
			HandleProofStageFinished,
			GeometryHelpers.externalLayerSeparation,
			new IStraightLineProvider[]
			{
				new StraightLineProvider_Polygon( shadowSquareNames_[0], 1),
				new StraightLineProvider_Polygon( parallelogramNames_[0], 1)
			},
			congruentTriangleColours[0],
			congruentTriangleNames_[0]
			);

		proofEngine_.AddStageToEnd( createCongruentTriangle1_Stage);

		//////////////////////////////////////////////

		ProofStage_HideElement hideCongruentTriangle1_Stage = new ProofStage_HideElement(
			"RemoveCongruentTriangle1",
			geometryFactory_,
			mainField_,
			removeShadowDuration,
			HandleProofStageFinished,
			congruentTriangleNames_[0],
			typeof( Element_Triangle) );

		hideShearSquare1Ancillaries_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.AddStageToEnd( hideCongruentTriangle1_Stage);

		//////////////////////////////////////////

		ProofStage_CloneElement createShadowSquare2_Stage = new ProofStage_CloneElement(
			"CreateShadowSquare2",
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

		proofEngine_.AddStageToEnd( createShadowSquare2_Stage);

		////////////

		ProofStage_CreatePolygonSide createShearSquareGuideline1_Stage = new ProofStage_CreatePolygonSide(
			"CreateShearSquareGuideline1",
			geometryFactory_,
			mainField_,
			shearSquareGuidelineCreationDuration_,
			HandleProofStageFinished,
			new StraightLineProvider_Polygon
			(
				parallelogramNames_[1],
				2
				),
			-3f * GeometryHelpers.externalLayerSeparation,
			0.02f,
			Color.gray,
			shearSquareGuidelineNames_[1],
			new LineExtender_LineIntersection(
				new StraightLineProvider_Name( mainTriangleNormalName_ )
				)
			);

		proofEngine_.AddStageToEnd( createShearSquareGuideline1_Stage);

		/////////////////////////

		ProofStage_ShearParallelogram shearSquare2_Stage = new ProofStage_ShearParallelogram(
			"ShearSquare2",
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

		shearSquare2_Stage.startReversedDestroyElementListDefinition
			= new ElementListDefinition(
				"shearSquare2_Stage Reversed Start DestroyList",
				new Dictionary<string, System.Type>( )
				{
					{  congruentTriangleNames_[1], typeof(Element_Triangle) }
				}
			);

		proofEngine_.AddStageToEnd( shearSquare2_Stage);

		//////////////////////////////

		ProofStage_HideElements hideShearSquareAncillaries2_Stage = new ProofStage_HideElements(
			"RemoveShadowSquare2",
			geometryFactory_,
			mainField_,
			0.2f,
			HandleProofStageFinished,
			new Dictionary<string, System.Type>()
			{
				{ shadowSquareNames_[1], typeof( Element_Parallelogram ) },
                { shearSquareGuidelineNames_[1], typeof( Element_StraightLine ) }
			}
		);

		hideShearSquareAncillaries2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Reverse);
		hideShearSquareAncillaries2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.AddStageToEnd( hideShearSquareAncillaries2_Stage);

		///////////////////////////////////

		ProofStage_CreateTriangleFromSides createCongruentTriangle2_Stage = new ProofStage_CreateTriangleFromSides(
			"CreateCongruentTriangle2",
			geometryFactory_,
			mainField_,
			createCongruentTriangleDuration,
			HandleProofStageFinished,
			GeometryHelpers.externalLayerSeparation,
			new IStraightLineProvider[]
			{
				new StraightLineProvider_Polygon( shadowSquareNames_[1], 3),
				new StraightLineProvider_Polygon( parallelogramNames_[1], 3)
			},
			congruentTriangleColours[1],
			congruentTriangleNames_[1]
			);

		proofEngine_.AddStageToEnd( createCongruentTriangle2_Stage);

		//////////////////////////////////////////////

		ProofStage_HideElement hideCongruentTriangle2_Stage = new ProofStage_HideElement(
			"RemoveCongruentTriangle2",
			geometryFactory_,
			mainField_,
			removeShadowDuration,
			HandleProofStageFinished,
			congruentTriangleNames_[1],
			typeof( Element_Triangle ) );

		hideShearSquareAncillaries2_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.AddStageToEnd( hideCongruentTriangle2_Stage);
		
		//////////////////////////////////////////////

		ProofStage_HideElement hideMainTriangleNormal_Stage = new ProofStage_HideElement(
			"RemoveMainTriangleNormal",
			geometryFactory_,
			mainField_,
			removeShadowDuration,
			HandleProofStageFinished,
			mainTriangleNormalName_,
			typeof( Element_StraightLine ) );

		hideMainTriangleNormal_Stage.SetDontPauseOnFinish( ProofEngine.EDirection.Forward );

		proofEngine_.AddStageToEnd( hideMainTriangleNormal_Stage);


		//////////////////////////////////

		ProofStage_ShearParallelogram shearParallelogram0_Stage = new ProofStage_ShearParallelogram(
			"ShearParallelogram0",
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

		proofEngine_.AddStageToEnd( shearParallelogram0_Stage);
		
		//////////////////////////////

		ProofStage_ShearParallelogram shearParallelogram1_Stage = new ProofStage_ShearParallelogram(
			"ShearParallelogram1",
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

		proofEngine_.AddStageToEnd( shearParallelogram1_Stage);

		/////////////////////

		proofEngine_.SkipStagesInReverse(
			new List<ProofStageBase>( )
			{
				createShadowSquare1_Stage,
				hideShearSquare1Ancillaries_Stage,
				createShadowSquare2_Stage,
				hideShearSquareAncillaries2_Stage,
				createCongruentTriangle1_Stage,
				hideCongruentTriangle1_Stage,
				createCongruentTriangle2_Stage,
				hideCongruentTriangle2_Stage,
				createMainTriangleNormal_Stage,
				hideMainTriangleNormal_Stage,
				createShearSquareGuideline0_Stage,
				createShearSquareGuideline1_Stage,
			}
		);

		createTriangle_Stage.Init( ProofEngine.EDirection.Forward, elements_ );
		proofEngine_.StartStage( createTriangle_Stage );
		if (!proofEngine_.isPaused)
		{
			proofEngine_.Pause( );
		}
		ShowOrHideTriangleSettings( );
        changeDirectionButton.gameObject.SetActive( true );
		loopButton.gameObject.SetActive( true );
	}

	#endregion proof engine sequence


}

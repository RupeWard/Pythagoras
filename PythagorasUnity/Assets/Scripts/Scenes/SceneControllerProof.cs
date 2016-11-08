using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

public class SceneControllerProof : SceneController_Base 
{
	static readonly private bool DEBUG_PROOF = true;

	public bool proofEngineMode = true;

	#region TEST

#if UNITY_EDITOR
	public bool testMode = true; // set to true to create some elements to play around with for testing
	private ElementList testElements = new ElementList( "Test Elements" );
	public void ClearTestElements( )
	{
		testElements.DestroyAllElements( );
	}
#endif

	#endregion TEST

	#region Public Settings

	public float createTriangleDuration = 3f;
	public Color mainTriangleColour = Color.blue;

	public float initialAngle = 30f;

	public float createSquareDuration = 3f;
	public Color square0Colour = Color.green;
	public Color square1Colour = Color.magenta;

	public float shearSquareDuration = 3f;
	public float postShearFadeDuration = 1f;

	public Color shadowColour = new Color( 100f / 255f, 100f / 255f, 100f / 255f );
	public float shadowSquareDepth = 0.1f;
	public float removeShadowDuration = 0.5f;

	public float shearAlpha = 125f / 255f;


	#endregion Public Settings


	#region inspector hooks

	public UnityEngine.UI.Button forwardButton;
	public UnityEngine.UI.Button fastForwardButton;

	public Sprite fastForwardButtonSprite_Go;
	public Sprite fastForwardButtonSprite_Stop;

	public Sprite forwardButtonSprite_Go;
	public Sprite forwardButtonSprite_Stop;


	#endregion inspector hooks

	#region prefabs

	public GameObject geometryFactoryPrefab;

	#endregion prefabs

	#region private data

	bool fastForward_ = false;
	private Field mainField_ = null;
	private GeometryFactory geometryFactory_;

	#endregion private data

	#region private elements

	private ElementList elements_ = new ElementList( "Elements" );

	private readonly string mainTriangleName_ = "Main Triangle";
	private readonly string[] parallelogramNames_ = { "Parallelogram 0", "Parallelogram 1" };
	private readonly string[] shadowSquareNames_ = { "Shadow Square 0", "Shadow Square 1" };
	private readonly string[] shadowParallelogramNames_ = { "Shadow Parallelogram 0", "Shadow Parallelogram 1" };
	private readonly string[] triangleSideNames_ = { "Hypotenuse", "Side 1", "Side 2" }
	;
	#endregion private elements

	#region MBFlow

	private void Update()
	{
		ProcessTriangleAngleChange( );
	}

	#endregion MBFlow

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Proof;
	}

	protected override void PostAwake( )
	{
		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
		geometryFactory_ = (Instantiate( geometryFactoryPrefab ) as GameObject).GetComponent<GeometryFactory>( );
		geometryFactory_.transform.SetParent( transform );

		DisableForwardButton( );
		DisableFastForwardButton( );
	}

	protected override void PostStart( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "SceneControllerProof.PostStart()" );
		}
#if UNITY_EDITOR
		if (testMode)
		{
			testElements.AddElement(
				"TestTri",
				geometryFactory_.AddTriangleToField(
					mainField_,
					"TestTri",
					0f,
					new Vector2[]
					{
						new Vector2(-1f, -1.5f),
						new Vector2(1f, -1.5f),
						new Vector2(1f, .5f)
					},
					Color.blue
					)
				);
			testElements.AddElement(
				"TestPar",
				geometryFactory_.AddParallelogramToField(
					mainField_,
					"TestPar",
					0f,
					new Vector2[]
					{
						new Vector2(-1f, 0.5f),
						new Vector2(1f, 0.5f)
					},
					1f,
					90f,
					Color.green
					)
				);
			testElements.AddElement(
				"TestRightTri",
				geometryFactory_.AddRightTriangleToField(
					mainField_,
					"TestRightTri",
					-0.1f,
					new Vector2[]
					{
						new Vector2(-1f, 0f),
						new Vector2(1f, 0f)
					},
					30f,
					Color.red
				)
			);
			testElements.AddElement(
				"TestLine",
				geometryFactory_.AddStraightLineToField(
					mainField_,
					"TestLine",
					-0.2f,
					new Vector2[]
					{
						new Vector2(-2f, -1f),
						new Vector2(1.5f, 2.5f)
					},
					0.1f,
					Color.red
				)
			);
		}
#endif

		EnableTriangleSettings( );

		if (false == proofEngineMode)
		{
			EnableForwardButton( CreateTriangle );
		}
		else
		{
			EnableForwardButton( StepForward );
		}
	}

	#endregion SceneController_Base

	#region Flow

	public void HandleReloadButton()
	{
		SceneManager.Instance.ReloadScene( SceneManager.EScene.Proof );
	}

	#endregion Flow

	#region ForwardButton

	private System.Action forwardButtonAction_;

	public void HandleForwardButton()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleForwardButton(), proofEngineMode = " + proofEngineMode );
		}
		if (forwardButtonAction_ == null)
		{
			Debug.LogError( "ForwardButtonPressed with no action" );
		}
		else
		{
			forwardButtonAction_( );
			if (fastForward_)
			{
				Debug.LogWarning( "Forward button pressed when fastForward is on" );
			}
			fastForward_ = false;
			EnableFastForwardButton( );
		}
	}

	private void DisableForwardButton()
	{
		forwardButtonAction_ = null;
		forwardButton.gameObject.SetActive( false );
	}

	private void EnableForwardButton(System.Action action)
	{
		forwardButtonAction_ = action;
		forwardButton.gameObject.SetActive( true );
	}

	#endregion ForwardButton

	#region FastForwardButton

	public void HandleFastForwardButton()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleFastForwardButton(), proofEngineMode = " + proofEngineMode +" FFwd = "+fastForward_);
		}
		fastForward_ = !fastForward_;
		SetFastForwardButtonSprite( );
		if (fastForward_)
		{
			DisableTriangleSettings( );
		}
		else
		{
			if (elements_.GetElementOfType< Element_Parallelogram >(parallelogramNames_[0]) == null)
			{
				// Not yet made first square, so can change triangle
				EnableTriangleSettings( );
			}
		}
		if (DEBUG_PROOF)
		{
			Debug.Log( "FastForward is now " + fastForward_ );
		}
	}

	private void DisableFastForwardButton( )
	{
		fastForwardButton.gameObject.SetActive( false );
	}

	private void EnableFastForwardButton( )
	{
		SetFastForwardButtonSprite( );
		fastForwardButton.gameObject.SetActive( true );
	}

	private void SetFastForwardButtonSprite()
	{
		Sprite s = (fastForward_) ? (fastForwardButtonSprite_Stop) : (fastForwardButtonSprite_Go);
		fastForwardButton.GetComponent<UnityEngine.UI.Image>( ).sprite = s;
	}

	private void SetForwardButtonSprite( )
	{
		Sprite s = (fastForward_) ? (fastForwardButtonSprite_Stop) : (fastForwardButtonSprite_Go);
		forwardButton.GetComponent<UnityEngine.UI.Image>( ).sprite = s;
	}

	#endregion FastForwardButton

	// Following region is for when proofEngineMode == true
	#region proof engine sequence

	private ProofEngine proofEngine_ = null;
	private void CreateProofEngine()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateProofEngine()" );
		}

#if UNITY_EDITOR
		ClearTestElements( );
#endif

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

		ProofStageBase.ConnectStages( createSide1Stage, createSquare1Stage );

		createTriangleStage.Init( ProofStageBase.EDirection.Forward, elements_ );
		proofEngine_.Init( createTriangleStage );
		proofEngine_.Resume( );
    }

	private float showSideDuration_ = 1f;

	private void HandleProofStageFinished( ProofStageBase psb)
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleProofStageFinished( "+psb.name+")" );
		}
		proofEngine_.Pause( );
		proofEngine_.ChangeToFollowingStage( psb );
	}

	private void StepForward()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "StepForward()" );
		}
		if (proofEngine_ == null)
		{
			CreateProofEngine( );
		}
		else
		{
			proofEngine_.TogglePause();
		}
	}

	#endregion proof engine sequence

	// Following region is for when proofEngineMode == false
	#region internal proof sequence 

	private void HandleEndOfSequence(System.Action nextSequence)
	{
		if (nextSequence == null)
		{
			throw new System.Exception( "null nextSequence!" );
		}
		if (fastForward_)
		{
			if (DEBUG_PROOF)
			{
				Debug.Log( "Fast-forwarding" );
			}
			nextSequence( );
		}
		else
		{
			DisableFastForwardButton( );
			EnableForwardButton( nextSequence );
		}
	}

	private void CreateTriangle()
	{
		DisableForwardButton( );
		StartCoroutine( CreateTriangleCR( ) );
	}

	private Element_Triangle CreateMainTriangle()
	{
		elements_.RemoveElementOfType<Element_Triangle>( mainTriangleName_, true );

		ElementBase e = elements_.AddElement(
			mainTriangleName_,
			geometryFactory_.AddRightTriangleToField(
				mainField_,
				mainTriangleName_,
				0f,
				new Vector2[]
				{
					new Vector2(-1f, 0f),
					new Vector2(1f, 0f)
				},
				initialAngle,
				mainTriangleColour
				)
			);
		return e as Element_Triangle;
	}

	private IEnumerator CreateTriangleCR()
	{
#if UNITY_EDITOR
		ClearTestElements( );
#endif
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateTriangleCR: START" );
		}


		Element_Triangle mainTriangle = CreateMainTriangle( );
		AssertMainTriangle( "CreateTriangleCR" );

		mainTriangle.SetAlpha( 0f );

		float elapsed = 0f;
		while (elapsed < createTriangleDuration)
		{
			elapsed += Time.deltaTime;
			mainTriangle.SetAlpha( Mathf.Lerp( 0f, 1f, elapsed / createTriangleDuration ) );
			yield return null;
		}
		mainTriangle.SetAlpha( 1f );
		yield return null;
		HandleEndOfSequence( CreateSquare0 );
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateTriangleCR: END" );
		}
	}

	private void CreateSquare0()
	{
		AssertMainTriangle( "CreateSquare0");
		DisableTriangleSettings( );
		DisableForwardButton( );
		StartCoroutine( CreateSquare0CR( ) );
	}

	private IEnumerator CreateSquare0CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare0CR: START" );
		}

		Element_Triangle mainTriangle = elements_.GetRequiredElementOfType<Element_Triangle>( mainTriangleName_ );

		Vector2[] baseline = elements_.GetRequiredElementOfType< Element_Triangle> (mainTriangleName_).GetSideExternal( 1 );

		Element_Parallelogram parallelogram0 =
			geometryFactory_.AddParallelogramToField(
				mainField_,
				parallelogramNames_[0],
				0f,
				baseline,
				0f,
				90f,
				square0Colour
				);
		elements_.AddElement( parallelogramNames_[0], parallelogram0 );

		float targetHeight = mainTriangle.GetSideLength( 1 );

		float elapsed = 0f;
		while (elapsed < createSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram0.SetHeight( Mathf.Lerp(0f, targetHeight, elapsed / createSquareDuration) );
			yield return null;
		}
		parallelogram0.SetHeight( targetHeight );
		yield return null;

		HandleEndOfSequence( CreateSquare1 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare0CR: END" );
		}
	}

	private void CreateSquare1( )
	{
		AssertMainTriangle( "CreateSquare1" );
		AssertParallelogram( "CreateSquare1", 0 );
        DisableForwardButton( );
		StartCoroutine( CreateSquare1CR( ) );
	}

	private IEnumerator CreateSquare1CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare1CR: START" );
		}

		Element_Triangle mainTriangle_ = elements_.GetRequiredElementOfType<Element_Triangle>( mainTriangleName_ );

		Vector2[] baseline = mainTriangle_.GetSideExternal( 2 );

		Element_Parallelogram parallelogram1 =
			geometryFactory_.AddParallelogramToField(
				mainField_,
				parallelogramNames_[1],
				0f,
				baseline,
				0f,
				90f,
				square1Colour
				);

		elements_.AddElement( parallelogramNames_[1], parallelogram1 );

		float targetHeight = mainTriangle_.GetSideLength( 2 );

		float elapsed = 0f;
		while (elapsed < createSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram1.SetHeight( Mathf.Lerp( 0f, targetHeight, elapsed / createSquareDuration ) );
			yield return null;
		}
		parallelogram1.SetHeight( targetHeight );
		yield return null;

		HandleEndOfSequence( ShearSquare0 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare1CR: END" );
		}
	}

	private void ShearSquare0( )
	{
		AssertParallelogram( "ShearSquare0", 0 );
		AssertShadowSquare( "ShearSquare0", 0, false );
		DisableForwardButton( );
		StartCoroutine( ShearSquare0CR( ) );
	}

	private IEnumerator ShearSquare0CR(  )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare0CR: START" );
		}

		Element_Parallelogram shadowSquare0 = elements_.GetElementOfType< Element_Parallelogram >(parallelogramNames_[0]).Clone<Element_Parallelogram>(shadowSquareNames_[0]);
		elements_.AddElement( shadowSquareNames_[0], shadowSquare0 );
		shadowSquare0.SetColour( shadowColour );
		shadowSquare0.SetDepth( shadowSquareDepth );

		float targetAngle = elements_.GetRequiredElementOfType< Element_Triangle >(mainTriangleName_).GetInternalAngleDegrees( 0 );

		Element_Parallelogram parallelogram0 = elements_.GetRequiredElementOfType<Element_Parallelogram>( parallelogramNames_[0] );
		parallelogram0.SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram0.SetAngle( Mathf.Lerp( 90f, targetAngle, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelogram0.SetAngle( targetAngle);
		yield return null;
		
		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			parallelogram0.SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction) );
			yield return null;
		}

		parallelogram0.SetAlpha( 1f );

		HandleEndOfSequence( ShearSquare1 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare0CR: END" );
		}
	}

	private void ShearSquare1( )
	{
		AssertParallelogram( "ShearSquare1", 1 );
		AssertShadowSquare( "ShearSquare1", 1, false );
		DisableForwardButton( );
		StartCoroutine( ShearSquare1CR( ) );
	}

	private IEnumerator ShearSquare1CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare1CR: START" );
		}

		yield return StartCoroutine( RemoveShadowSquareCR( 0 ) );

		Element_Parallelogram shadowSquare1 = elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[1] ).Clone<Element_Parallelogram>( shadowSquareNames_[1] );
		elements_.AddElement( shadowSquareNames_[1], shadowSquare1 );
		shadowSquare1.SetColour( shadowColour );
		shadowSquare1.SetDepth( shadowSquareDepth );

		float targetAngle = 180f - elements_.GetRequiredElementOfType<Element_Triangle>( mainTriangleName_ ).GetInternalAngleDegrees( 1 );

		Element_Parallelogram parallelogram1 = elements_.GetRequiredElementOfType<Element_Parallelogram>( parallelogramNames_[1] );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram1.SetAngle( Mathf.Lerp( 90f, targetAngle, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelogram1.SetAngle( targetAngle );
		yield return null;

		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			parallelogram1.SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction ) );
			yield return null;
		}

		parallelogram1.SetAlpha( 1f );

		HandleEndOfSequence( ShearParallelogram0 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare1CR: END" );
		}
	}

	private IEnumerator RemoveShadowSquareCR( int n )
	{
		if (n < 0 || n > 1)
		{
			throw new System.Exception( "n must be 0 or 1 in RemoveShadowSquare, not " + n );
		}

		Element_Parallelogram shadowSquare = elements_.GetRequiredElementOfType<Element_Parallelogram>( shadowSquareNames_[n] );
		yield return StartCoroutine( RemoveElementCR( shadowSquare, removeShadowDuration ) );
	}

	private IEnumerator RemoveShadowParallelogramCR( int n )
	{
		if (n < 0 || n > 1)
		{
			throw new System.Exception( "n must be 0 or 1 in RemoveShadowParallelogram, not " + n );
		}

		Element_Parallelogram shadowParallelogram  = elements_.GetRequiredElementOfType<Element_Parallelogram>( shadowParallelogramNames_[n] );
		yield return StartCoroutine( RemoveElementCR( shadowParallelogram, removeShadowDuration ) );
	}

	private IEnumerator RemoveElementCR( ElementBase eb, float duration )
	{
		float elapsed = 0f;
		while (elapsed < removeShadowDuration)
		{
			elapsed += Time.deltaTime;
			eb.SetAlpha( Mathf.Lerp( 1f, 0f, elapsed / removeShadowDuration ) );
			yield return null;
		}
		elements_.DestroyElement( eb );
	}

	private void ShearParallelogram0( )
	{
		AssertParallelogram( "ShearParallelogram0", 0 );
		AssertShadowParallelogram( "ShearParallelogram0", 0, false );
		DisableForwardButton( );
		StartCoroutine( ShearParallelogram0CR( ) );
	}

	private IEnumerator ShearParallelogram0CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearParallelogram0CR: START" );
		}

		yield return StartCoroutine(RemoveShadowSquareCR(1));

		Element_Parallelogram shadowParallelogram0 = elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[0] ).Clone<Element_Parallelogram>( shadowParallelogramNames_[0] );
		elements_.AddElement( shadowParallelogramNames_[0], shadowParallelogram0);

		shadowParallelogram0.SetColour( shadowColour );
		shadowParallelogram0.SetDepth( shadowSquareDepth );

		Element_Parallelogram parallelogram0 = elements_.GetRequiredElementOfType<Element_Parallelogram>( parallelogramNames_[0] );
		parallelogram0.ChangeBaseline( 1 );

		float startingAngle = parallelogram0.angle;
		parallelogram0.SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram0.SetAngle( Mathf.Lerp( startingAngle, 90f, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelogram0.SetAngle( 90f );
		yield return null;

		yield return StartCoroutine( RemoveShadowParallelogramCR( 0 ) );

		HandleEndOfSequence( ShearParallelogram1);

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearParallelogram0CR: END" );
		}
	}

	private void ShearParallelogram1( )
	{
		AssertParallelogram( "ShearParallelogram1", 1 );
		AssertShadowParallelogram( "ShearParallelogram1", 1, false );
		DisableForwardButton( );
		StartCoroutine( ShearParallelogram1CR( ) );
	}

	private IEnumerator ShearParallelogram1CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearParallelogram1CR: START" );
		}

		Element_Parallelogram shadowParallelogram1 = elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[1] ).Clone<Element_Parallelogram>( shadowParallelogramNames_[1] );
		elements_.AddElement( shadowParallelogramNames_[1], shadowParallelogram1 );

		shadowParallelogram1.SetColour( shadowColour );
		shadowParallelogram1.SetDepth( shadowSquareDepth );

		Element_Parallelogram parallelogram1 = elements_.GetRequiredElementOfType<Element_Parallelogram>( parallelogramNames_[1] );

		parallelogram1.ChangeBaseline( 3 );

		float startingAngle = parallelogram1.angle;
		parallelogram1.SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram1.SetAngle( Mathf.Lerp( startingAngle, 90f, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelogram1.SetAngle( 90f );
		yield return null;

		yield return StartCoroutine( RemoveShadowParallelogramCR( 1 ) );

		//		EnableForwardButton( CreateSquare1 );
		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearParallelogram1CR: END" );
		}
	}


	private void AssertMainTriangle(string locn)
	{
		if (elements_.GetElementOfType<Element_Triangle>(mainTriangleName_) == null)
		{
			elements_.DebugDescribe( );
			throw new System.Exception( "mainTriangle doesn't exist in " + locn );
		}
	}

	private void AssertParallelogram( string locn, int n )
	{
		if (elements_.GetElementOfType< Element_Parallelogram >(parallelogramNames_[n]) == null)
		{
			elements_.DebugDescribe( );
			throw new System.Exception( "parallelograms[ "+n+" ] doesn't exist in " + locn );
		}
	}

	private void AssertShadowSquare( string locn, int n, bool exists )
	{
		Element_Parallelogram ep = elements_.GetElementOfType<Element_Parallelogram>( shadowSquareNames_[n] );
		if (exists)
		{
			if (ep == null)
			{
				elements_.DebugDescribe( );
				throw new System.Exception( "shadowSquareNames[ " + n + " ] doesn't exist in " + locn );
			}
		}
		else
		{
			if (ep != null)
			{
				elements_.DebugDescribe( );
				throw new System.Exception( "shadowSquareNames[ " + n + " ] already exists in " + locn );
			}
		}
	}


	private void AssertShadowParallelogram( string locn, int n, bool exists )
	{
		Element_Parallelogram ep = elements_.GetElementOfType<Element_Parallelogram>( shadowParallelogramNames_[n] );
        if (exists) 
		{
			if (ep == null)
			{
				elements_.DebugDescribe( );
				throw new System.Exception( "shadowParallelograms[ " + n + " ] doesn't exist in " + locn );
			}
		}
		else
		{
			if (ep != null)
			{
				elements_.DebugDescribe( );
				throw new System.Exception( "shadowParallelograms[ " + n + " ] already exists in " + locn );
			}
		}
	}

	#endregion internal proof sequence

	#region triangleSettings

	private Vector2 minMaxTriangleAngle = new Vector2( 5f, 85f );

	public float triangleAngleChangeSpeed = 1f;

	private float currentTriangleAngleChangeSpeed = 0f;

	// Inspector hooks
	public GameObject triangleSettingsPanel;
	public UnityEngine.UI.InputField triangleAngleInputField;
	public UnityEngine.UI.Button triangleAngleUpButton;
	public UnityEngine.UI.Button triangleAngleDownButton;

	private void EnableTriangleSettings()
	{
		triangleSettingsPanel.SetActive( true );
		SetTriangleAngleText( );
	}

	private void SetTriangleAngleText()
    {
		triangleAngleInputField.text = initialAngle.ToString( "0.0");
	}

	private void DisableTriangleSettings( )
	{
		triangleSettingsPanel.SetActive( false );
	}

	private void ChangeInitialAngle(float f)
	{
		initialAngle = f;
		if (elements_.GetElementOfType<Element_Triangle>(mainTriangleName_) != null && currentTriangleAngleChangeSpeed == 0f )
		{
			CreateMainTriangle( ); // TODO maybe implement adjustment without re-creation?
		}
		SetTriangleAngleText( );
		triangleAngleUpButton.gameObject.SetActive( false == Mathf.Approximately( initialAngle, minMaxTriangleAngle.y ) );
		triangleAngleDownButton.gameObject.SetActive( false == Mathf.Approximately( initialAngle, minMaxTriangleAngle.x ) );
	}

	public void OnTriangleAngleInputFieldEndEdit()
	{
		float f;
		if (float.TryParse( triangleAngleInputField.text, out f))
		{
			if (f >= minMaxTriangleAngle.x && f <= minMaxTriangleAngle.y)
			{
				initialAngle = f;
				if (elements_.GetElementOfType< Element_Triangle >(mainTriangleName_) != null)
				{
					CreateMainTriangle( ); // TODO maybe implement adjustment without re-creation?
				}
			}
			else
			{
				// TODO Display warning
				Debug.LogWarning( "Angle out of range at " + f );
				SetTriangleAngleText( );
			}
		}
		else
		{
			// TODO Display warning
			Debug.LogWarning( "Can't parse angle from '" + triangleAngleInputField.text+"'" );
			SetTriangleAngleText( );
		}
	}

	public void HandleTriangleAngleDownButtonDown( )
	{
		currentTriangleAngleChangeSpeed = -1f * triangleAngleChangeSpeed;
	}

	public void HandleTriangleAngleUpButtonDown()
	{
		currentTriangleAngleChangeSpeed = triangleAngleChangeSpeed;
	}

	public void HandleTriangleAngleButtonUp( )
	{
		currentTriangleAngleChangeSpeed = 0f;
		CreateMainTriangle( );
	}

	private void ProcessTriangleAngleChange()
	{
		if (triangleSettingsPanel.gameObject.activeSelf)
		{
			if (currentTriangleAngleChangeSpeed != 0f)
			{
				float newAngle = initialAngle + currentTriangleAngleChangeSpeed * Time.deltaTime;
				newAngle = Mathf.Clamp( newAngle, minMaxTriangleAngle.x, minMaxTriangleAngle.y );
				if (Mathf.Approximately(newAngle, initialAngle))
				{
					currentTriangleAngleChangeSpeed = 0f;
				}
		//		else
				{
					ChangeInitialAngle( newAngle );
				}
			}
		}
	}
	#endregion triangleSettings

}


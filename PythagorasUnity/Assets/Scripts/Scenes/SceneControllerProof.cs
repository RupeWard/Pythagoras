using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

public partial class SceneControllerProof : SceneController_Base 
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
	public float initialSpeed = 1f;

	public float createSquareDuration = 3f;
	public Color square0Colour = Color.green;
	public Color square1Colour = Color.magenta;

	public float shearSquareDuration = 3f;
	public float postShearFadeDuration = 1f;
	public float createCongruentTriangleDuration = 3f;

	public Color shadowColour = new Color( 100f / 255f, 100f / 255f, 100f / 255f );
	public float shadowSquareDepth = 0.1f;
	public float removeShadowDuration = 1f;

	public float shearAlpha = 125f / 255f;

	public bool loop_ = false;

	#endregion Public Settings

	#region inspector hooks

	public UnityEngine.UI.Button forwardButton;
	public UnityEngine.UI.Button changeDirectionButton;
	public UnityEngine.UI.Button fastForwardButton;
	public UnityEngine.UI.Button loopButton;

	public Sprite fastForwardButtonSprite_Go;
	public Sprite fastForwardButtonSprite_Stop;

	public Sprite forwardButtonSprite_Go;
	public Sprite forwardButtonSprite_Stop;

	public Sprite loopButtonSprite_On;
	public Sprite loopButtonSprite_Off;

	#endregion inspector hooks

	#region private hooks

	public UnityEngine.UI.Image forwardButtonImage;
	public UnityEngine.UI.Image fastForwardButtonImage;
	public UnityEngine.UI.Image loopButtonImage;

	#endregion private hooks

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
	private readonly string[] triangleSideNames_ = { "Hypotenuse", "Side 1", "Side 2" };
	private readonly string[] congruentTriangleNames = new string[] { "congruentTriangle1", "congruentTriangle2" };

	#endregion private elements

	#region MBFlow

	private void Update()
	{
		ProcessTriangleAngleChange( );
		ProcessSpeedChange( );
	}

	#endregion MBFlow

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Proof;
	}

	protected override void PostAwake( )
	{
		forwardButtonImage = forwardButton.GetComponent< UnityEngine.UI.Image >( );
		fastForwardButtonImage = fastForwardButton.GetComponent< UnityEngine.UI.Image >( );

		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
		geometryFactory_ = (Instantiate( geometryFactoryPrefab ) as GameObject).GetComponent< GeometryFactory >( );
		geometryFactory_.transform.SetParent( transform );

		forwardButton.gameObject.SetActive( false );
		changeDirectionButton.gameObject.SetActive( false );
		fastForwardButton.gameObject.SetActive( false );
		loopButton.gameObject.SetActive( false );
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
			/*
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
				*/
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
			/*
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
			testElements.AddElement(
				"TestCircle",
				geometryFactory_.AddCircleToField(
					mainField_,
					"TestCircle",
					-0.3f,
					new Vector2( 0.5f, 0.5f ),
					1f,
					Color.yellow )
                );
				*/
			testElements.AddElement(
				"TestSector",
				geometryFactory_.AddSectorToField(
					mainField_,
					"TestSector",
					-0.4f,
					new Vector2( 0f,2f),
					0.5f,
					45f,
					10f,
					Color.gray
					)
				);
		}
#endif

		EnableTriangleSettings( );
		EnableSpeedPanel( );

		if (false == proofEngineMode)
		{
			EnableForwardButton( CreateTriangle );
		}
		else
		{
			forwardButton.gameObject.SetActive( true );
			fastForwardButton.gameObject.SetActive( true );
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

	public void HandleForwardButton()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleForwardButton(), proofEngineMode = " + proofEngineMode );
		}
		if (proofEngineMode)
		{
			HandleForwardButtonProofEngineMode( );
		}
		else
		{
			HandleForwardButtonInternalMode( );
		}
	}

	private void SetForwardButtonSprite( bool isPaused)
	{
		Sprite s = (isPaused) ? (forwardButtonSprite_Go) : (forwardButtonSprite_Stop);
		forwardButtonImage.sprite = s;
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
		fastForwardButtonImage.sprite = s;
	}

	#endregion FastForwardButton

	#region triangle settings

//	private static bool DEBUG_ANGLE = true;

	private Vector2 minMaxTriangleAngle = new Vector2( 5f, 85f );

	public float triangleAngleChangeInitialSpeed = 1f;
	public float triangleAngleChangeAcceleration = 1.02f;

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
		if (f < minMaxTriangleAngle.x || f > minMaxTriangleAngle.y)
		{
			throw new System.ArgumentException( "Angle " + f + " out of range " + minMaxSpeed );
		}

		if (false == Mathf.Approximately(f, initialAngle))
		{
			initialAngle = f;

			SetTriangleAngleText( );

			triangleAngleUpButton.gameObject.SetActive( initialAngle < minMaxTriangleAngle.y );
			triangleAngleDownButton.gameObject.SetActive( initialAngle > minMaxTriangleAngle.x );
		}
		/*
		else
		{
			if (DEBUG_ANGLE)
			{
				Debug.LogWarning( "Angle not changed enough" );
			}
		}*/
	}

	public void OnTriangleAngleInputFieldEndEdit()
	{
		float f;
		if (float.TryParse( triangleAngleInputField.text, out f))
		{
			if (f >= minMaxTriangleAngle.x && f <= minMaxTriangleAngle.y)
			{
				ChangeInitialAngle( f );
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
		currentTriangleAngleChangeSpeed = -1f * triangleAngleChangeInitialSpeed;
	}

	public void HandleTriangleAngleUpButtonDown()
	{
		currentTriangleAngleChangeSpeed = triangleAngleChangeInitialSpeed;
	}

	public void HandleTriangleAngleButtonUp( )
	{
		currentTriangleAngleChangeSpeed = 0f;
		HandleAngleChanged( );
	}

	private void HandleAngleChanged()
	{
		if (proofEngineMode)
		{
			HandleAngleChangedProofEngineMode( );
		}
		else
		{
			HandleAngleChangedInternalMode( );
		}
	}

	private void ProcessTriangleAngleChange()
	{
		if (triangleSettingsPanel.gameObject.activeSelf)
		{
			if (currentTriangleAngleChangeSpeed != 0f)
			{
				float newAngle = initialAngle + currentTriangleAngleChangeSpeed * Time.deltaTime;

				if (newAngle < minMaxTriangleAngle.x || newAngle > minMaxTriangleAngle.y)
				{
					newAngle = Mathf.Clamp( newAngle, minMaxTriangleAngle.x, minMaxTriangleAngle.y );
					currentTriangleAngleChangeSpeed = 0f;
				}
				else
				{
					currentTriangleAngleChangeSpeed *= triangleAngleChangeAcceleration;
				}

				ChangeInitialAngle( newAngle );
			}
		}
	}
	#endregion triangles ettings

	#region speed panel

//	static private bool DEBUG_SPEED = true;

	private Vector2 minMaxSpeed = new Vector2( 0f, 10f );

	public float speedChangeInitialSpeed = 0.2f;
	public float speedChangeAcceleration = 1.02f;

	private float currentSpeedChangeSpeed = 0f;

	// Inspector hooks
	public GameObject speedPanel;
	public UnityEngine.UI.InputField speedInputField;
	public UnityEngine.UI.Button speedUpButton;
	public UnityEngine.UI.Button speedDownButton;

	private void EnableSpeedPanel( )
	{
		speedPanel.SetActive( true );
		SetSpeedText( );
	}

	private void SetSpeedText( )
	{
		speedInputField.text = initialSpeed.ToString( "0.0" );
	}

	private void DisableSpeedPanel( )
	{
		speedPanel.SetActive( false );
	}

	private void ChangeInitialSpeed( float f )
	{
		if (f < minMaxSpeed.x || f > minMaxSpeed.y)
		{
			throw new System.ArgumentException( "Speed " + f + " out of range " + minMaxSpeed );
		}
		if (f != initialSpeed)
		{
			initialSpeed = f;
			SetSpeedText( );
			speedUpButton.gameObject.SetActive( false == Mathf.Approximately( initialSpeed, minMaxSpeed.y ) );
			speedDownButton.gameObject.SetActive( false == Mathf.Approximately( initialSpeed, minMaxSpeed.x ) );

			HandleSpeedChanged( );
		}
	}

	public void OnSpeedInputFieldEndEdit( )
	{
		float f;
		if (float.TryParse( speedInputField.text, out f ))
		{
			if (f >= minMaxSpeed.x && f <= minMaxSpeed.y)
			{
				ChangeInitialSpeed( f );
			}
			else
			{
				// TODO Display warning
				Debug.LogWarning( "Speed out of range at " + f );
				SetSpeedText( );
			}
		}
		else
		{
			// TODO Display warning
			Debug.LogWarning( "Can't parse speed from '" + speedInputField.text + "'" );
			SetSpeedText( );
		}
	}

	public void HandleSpeedDownButtonDown( )
	{
		currentSpeedChangeSpeed = -1f * speedChangeInitialSpeed;
	}

	public void HandleSpeedUpButtonDown( )
	{
		currentSpeedChangeSpeed = speedChangeInitialSpeed;
	}

	public void HandleSpeedButtonUp( )
	{
		currentSpeedChangeSpeed = 0f;
		HandleSpeedChanged( );
	}

	private void HandleSpeedChanged( )
	{
		if (proofEngineMode)
		{
			HandleSpeedChangedProofEngineMode( );
		}
		else
		{
			//HandleSpeedChangedInternalMode( ); // this mode uses initial speed directly so nothing to do
		}
	}

	private void ProcessSpeedChange( )
	{
		if (speedPanel.gameObject.activeSelf)
		{
			if (currentSpeedChangeSpeed != 0f)
			{
				float newSpeed = initialSpeed + currentSpeedChangeSpeed * Time.deltaTime;
				if (newSpeed < minMaxSpeed.x || newSpeed > minMaxSpeed.y)
				{
					newSpeed = Mathf.Clamp( newSpeed, minMaxSpeed.x, minMaxSpeed.y );
					currentSpeedChangeSpeed = 0f;
				}
				else
				{
					currentSpeedChangeSpeed *= speedChangeAcceleration;
				}
				ChangeInitialSpeed( newSpeed);
			}
		}
	}
	#endregion speed panel


}


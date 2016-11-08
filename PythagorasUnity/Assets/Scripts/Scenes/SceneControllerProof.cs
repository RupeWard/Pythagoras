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

	private float showSideDuration_ = 1f;

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

	#region private hooks

	public UnityEngine.UI.Image forwardButtonImage;
	public UnityEngine.UI.Image fastForwardButtonImage;
	
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
		forwardButtonImage = forwardButton.GetComponent< UnityEngine.UI.Image >( );
		fastForwardButtonImage = fastForwardButton.GetComponent< UnityEngine.UI.Image >( );

		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
		geometryFactory_ = (Instantiate( geometryFactoryPrefab ) as GameObject).GetComponent< GeometryFactory >( );
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
			CreateProofEngine( );
			forwardButton.gameObject.SetActive( true );
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

	private System.Action forwardButtonAction_; // only used in internal mode

	public void HandleForwardButton()
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "HandleForwardButton(), proofEngineMode = " + proofEngineMode );
		}
		if (proofEngineMode)
		{
			StepForward( );
		}
		else
		{
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
		fastForwardButtonImage.sprite = s;
	}

	private void SetForwardButtonSprite( bool isRunning )
	{
		Sprite s = (isRunning) ? (fastForwardButtonSprite_Stop) : (fastForwardButtonSprite_Go);
		forwardButtonImage.sprite = s;
	}

	#endregion FastForwardButton

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
		if (elements_.GetElementOfType< Element_Triangle >(mainTriangleName_) != null && currentTriangleAngleChangeSpeed == 0f )
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


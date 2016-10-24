using UnityEngine;
using System.Collections;

public class SceneControllerProof : SceneController_Base 
{
	static readonly private bool DEBUG_PROOF = true;

	#region inspector hooks

	public UnityEngine.UI.Button forwardButton;
	public UnityEngine.UI.Button fastForwardButton;

	public GeometryManager geometryManager;

	public Sprite fastForwardButtonSprite_Go;
	public Sprite fastForwardButtonSprite_Stop;

	#endregion inspector hooks

	#region private data

	bool fastForward_ = false;

	#endregion private data

	#region private elements

	private Triangle mainTriangle_ = null;
	private Parallelogram[] parallelograms = new Parallelogram[2];
	private Parallelogram[] shadowParallelograms = new Parallelogram[2];

	#endregion private elements

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Proof;
	}

	protected override void PostAwake( )
	{
		DisableForwardButton( );
		DisableFastForwardButton( );
	}

	protected override void PostStart( )
	{
		EnableForwardButton( CreateTriangle );
	}

	#endregion SceneController_Base

	#region ForwardButton

	private System.Action forwardButtonAction_;

	public void HandleForwardButton()
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
		fastForward_ = !fastForward_;
		SetFastForwardButtonSprite( );
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

	#endregion FastForwardButton

	#region proof

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

	public float createTriangleDuration = 3f;
	public Color mainTriangleColour = Color.blue;

	public float initialAngle = 30f;
	
	private void CreateTriangle()
	{
		DisableForwardButton( );
		StartCoroutine( CreateTriangleCR( ) );
	}

	private IEnumerator CreateTriangleCR()
	{
#if UNITY_EDITOR
		geometryManager.ClearTestElements( );
#endif
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateTriangleCR: START" );
		}
		Color colour = mainTriangleColour;

		mainTriangle_ = geometryManager.AddRightTriangleToMainField(
					"MainTriangle",
					0f,
					new Vector2[]
					{
						new Vector2(-1f, 0f),
						new Vector2(1f, 0f)
					},
					initialAngle,
					colour
			);
		mainTriangle_.SetColour( colour, 0f );

		float elapsed = 0f;
		while (elapsed < createTriangleDuration)
		{
			elapsed += Time.deltaTime;
			mainTriangle_.SetColour( colour, Mathf.Lerp( 0f, 1f, elapsed / createTriangleDuration ) );
			yield return null;
		}
		mainTriangle_.SetColour( colour, 1f );
		yield return null;
		HandleEndOfSequence( CreateSquare0 );
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateTriangleCR: END" );
		}
	}

	public float createSquareDuration = 3f;
	public Color square0Colour = Color.green;
	public Color square1Colour = Color.magenta;
	 
	private void CreateSquare0()
	{
		AssertMainTriangle( "CreateSquare0");
		DisableForwardButton( );
		StartCoroutine( CreateSquare0CR( ) );
	}

	private IEnumerator CreateSquare0CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare0CR: START" );
		}

		Vector2[] baseline = mainTriangle_.GetSideExternal( 1 );
		parallelograms[0] =
			geometryManager.AddParallelogramToMainField(
				"Par0",
				0f,
				baseline,
				0f,
				90f,
				square0Colour
				);

		float targetHeight = mainTriangle_.GetSideLength( 1 );

		float elapsed = 0f;
		while (elapsed < createSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[0].SetHeight( Mathf.Lerp(0f, targetHeight, elapsed / createSquareDuration) );
			yield return null;
		}
		parallelograms[0].SetHeight( targetHeight );
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

		Vector2[] baseline = mainTriangle_.GetSideExternal( 2 );
		parallelograms[1] =
			geometryManager.AddParallelogramToMainField(
				"Par1",
				0f,
				baseline,
				0f,
				90f,
				square1Colour
				);

		float targetHeight = mainTriangle_.GetSideLength( 2 );

		float elapsed = 0f;
		while (elapsed < createSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[1].SetHeight( Mathf.Lerp( 0f, targetHeight, elapsed / createSquareDuration ) );
			yield return null;
		}
		parallelograms[1].SetHeight( targetHeight );
		yield return null;

		HandleEndOfSequence( ShearSquare0 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare1CR: END" );
		}
	}

	public float shearSquareDuration = 3f;
	public float postShearFadeDuration = 1f;

	public Color shadowColour = new Color( 100f / 255f, 100f / 255f, 100f / 255f );
	public float shadowSquareDepth = 0.1f;

	public float shearAlpha = 125f/255f;

	private void ShearSquare0( )
	{
		AssertParallelogram( "ShearSquare0", 0 );
		DisableForwardButton( );
		StartCoroutine( ShearSquare0CR( ) );
	}

	private IEnumerator ShearSquare0CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare0CR: START" );
		}

		shadowParallelograms[0] = parallelograms[0].Clone<Parallelogram>("ShadowSquare0" );
		shadowParallelograms[0].SetColour( shadowColour );
		shadowParallelograms[0].SetDepth( shadowSquareDepth );

		float targetAngle = mainTriangle_.GetInternalAngleDegrees( 0 );

		parallelograms[0].SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[0].SetAngle( Mathf.Lerp( 90f, targetAngle, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelograms[0].SetAngle( targetAngle);
		yield return null;
		
		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			parallelograms[0].SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction) );
			shadowParallelograms[0].SetAlpha( Mathf.Lerp( 1f, 0f, fraction ) );
			yield return null;
		}

		parallelograms[0].SetAlpha( 1f );
		shadowParallelograms[0].SetAlpha( 0f );

		HandleEndOfSequence( ShearSquare1 );

		yield return null;
		GameObject.Destroy( shadowParallelograms[0].gameObject );
		shadowParallelograms[0] = null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare0CR: END" );
		}
	}

	private void ShearSquare1( )
	{
		AssertParallelogram( "ShearSquare1", 1 );
		DisableForwardButton( );
		StartCoroutine( ShearSquare1CR( ) );
	}

	private IEnumerator ShearSquare1CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare1CR: START" );
		}

		shadowParallelograms[1] = parallelograms[1].Clone<Parallelogram>( "ShadowSquare1" );
		shadowParallelograms[1].SetColour( shadowColour );
		shadowParallelograms[1].SetDepth( shadowSquareDepth );

		float targetAngle = 180f - mainTriangle_.GetInternalAngleDegrees( 1 );

		parallelograms[1].SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[1].SetAngle( Mathf.Lerp( 90f, targetAngle, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelograms[1].SetAngle( targetAngle );
		yield return null;

		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			parallelograms[1].SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction ) );
			shadowParallelograms[1].SetAlpha( Mathf.Lerp( 1f, 0f, fraction ) );
			yield return null;
		}

		parallelograms[1].SetAlpha( 1f );
		shadowParallelograms[1].SetAlpha( 0f );

		HandleEndOfSequence( ShearParallelogram0 );

		yield return null;
		GameObject.Destroy( shadowParallelograms[1].gameObject );
		shadowParallelograms[1] = null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare1CR: END" );
		}
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

		shadowParallelograms[0] = parallelograms[0].Clone<Parallelogram>( "ShadowParallelogram0" );
		shadowParallelograms[0].SetColour( shadowColour );
		shadowParallelograms[0].SetDepth( shadowSquareDepth );

		parallelograms[0].ChangeBaseline( 1 );

		float startingAngle = parallelograms[0].angle;
		parallelograms[0].SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[0].SetAngle( Mathf.Lerp( startingAngle, 90f, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelograms[0].SetAngle( 90f );
		yield return null;

		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			// parallelograms[0].SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction ) );
			shadowParallelograms[0].SetAlpha( Mathf.Lerp( 1f, 0f, fraction ) );
			yield return null;
		}

//		parallelograms[0].SetAlpha( 1f );
		shadowParallelograms[0].SetAlpha( 0f );

		HandleEndOfSequence( ShearParallelogram1);

		yield return null;
		GameObject.Destroy( shadowParallelograms[0].gameObject );
		shadowParallelograms[0] = null;

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

		shadowParallelograms[1] = parallelograms[1].Clone<Parallelogram>( "ShadowParallelogram1" );
		shadowParallelograms[1].SetColour( shadowColour );
		shadowParallelograms[1].SetDepth( shadowSquareDepth );

		parallelograms[1].ChangeBaseline( 3 );

		float startingAngle = parallelograms[1].angle;
		parallelograms[1].SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelograms[1].SetAngle( Mathf.Lerp( startingAngle, 90f, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelograms[1].SetAngle( 90f );
		yield return null;

		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			// parallelograms[0].SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction ) );
			shadowParallelograms[1].SetAlpha( Mathf.Lerp( 1f, 0f, fraction ) );
			yield return null;
		}

		//		parallelograms[0].SetAlpha( 1f );
		shadowParallelograms[1].SetAlpha( 0f );

		//		EnableForwardButton( CreateSquare1 );

		yield return null;
		GameObject.Destroy( shadowParallelograms[1].gameObject );
		shadowParallelograms[1] = null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearParallelogram1CR: END" );
		}
	}


	private void AssertMainTriangle(string locn)
	{
		if (mainTriangle_ == null)
		{
			throw new System.Exception( "mainTriangle doesn't exist in " + locn );
		}
	}

	private void AssertParallelogram( string locn, int n )
	{
		if (parallelograms[n] == null)
		{
			throw new System.Exception( "parallelograms[ "+n+" ] doesn't exist in " + locn );
		}
	}

	private void AssertShadowParallelogram( string locn, int n, bool exists )
	{
		if (exists) 
		{
			if (shadowParallelograms[n] == null)
			{
				throw new System.Exception( "shadowParallelograms[ " + n + " ] doesn't exist in " + locn );
			}
		}
		else
		{
			if (shadowParallelograms[n] != null)
			{
				throw new System.Exception( "shadowParallelograms[ " + n + " ] already exists in " + locn );
			}
		}
	}

	#endregion proof

}


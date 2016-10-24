using UnityEngine;
using System.Collections;

public class SceneControllerProof : SceneController_Base 
{
	static readonly private bool DEBUG_PROOF = true;

	#region inspector hooks

	public UnityEngine.UI.Button forwardButton;

	public GeometryManager geometryManager;

	#endregion inspector hooks

	#region private elements

	#region SceneController_Base

	private Triangle mainTriangle_ = null;
	private Parallelogram[] parallelograms = new Parallelogram[2];

	#endregion SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Proof;
	}

	protected override void PostAwake( )
	{
		DisableForwardButton( );
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

	#region proof

	public float createTriangleDuration = 3f;
	public Color mainTriangleColour = Color.blue;

	
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
						new Vector2(-1f, -2f),
						new Vector2(1f, -2f)
					},
					30f,
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
		EnableForwardButton( CreateSquare0 );
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

		EnableForwardButton( CreateSquare1 );

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

		//		EnableForwardButton( CreateSquare1 );

		yield return null;

		if (DEBUG_PROOF)
		{
			Debug.Log( "CreateSquare1CR: END" );
		}
	}


	private void AssertMainTriangle(string locn)
	{
		if (mainTriangle_ == null)
		{
			throw new System.Exception( "Main Triangle doesn't exist in " + locn );
		}
	}

	private void AssertParallelogram( string locn, int n )
	{
		if (parallelograms[n] == null)
		{
			throw new System.Exception( "Parallelogram[ "+n+" ] doesn't exist in " + locn );
		}
	}

	#endregion proof

}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RJWard.Geometry;

/* 
	Functions and fields only used in Internal proof mode
*/
public partial class SceneControllerProof : SceneController_Base
{
	#region ForwardButton

	private System.Action forwardButtonAction_; // only used in internal mode

	public void HandleForwardButtonInternalMode( )
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

	private void DisableForwardButton( )
	{
		forwardButtonAction_ = null;
		forwardButton.gameObject.SetActive( false );
	}

	private void EnableForwardButton( System.Action action )
	{
		forwardButtonAction_ = action;
		forwardButton.gameObject.SetActive( true );
	}

	#endregion ForwardButton

	// Following region is for when proofEngineMode == false
	#region internal proof sequence 

	private void HandleEndOfSequence( System.Action nextSequence )
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

	private void CreateTriangle( )
	{
		DisableForwardButton( );
		StartCoroutine( CreateTriangleCR( ) );
	}

	private Element_Triangle CreateMainTriangle( )
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

	private IEnumerator CreateTriangleCR( )
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

	private void CreateSquare0( )
	{
		AssertMainTriangle( "CreateSquare0" );
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

		Vector2[] baseline = elements_.GetRequiredElementOfType<Element_Triangle>( mainTriangleName_ ).GetSideExternal( 1 );

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
			parallelogram0.SetHeight( Mathf.Lerp( 0f, targetHeight, elapsed / createSquareDuration ) );
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

	private IEnumerator ShearSquare0CR( )
	{
		if (DEBUG_PROOF)
		{
			Debug.Log( "ShearSquare0CR: START" );
		}

		Element_Parallelogram shadowSquare0 = elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[0] ).Clone<Element_Parallelogram>( shadowSquareNames_[0] );
		elements_.AddElement( shadowSquareNames_[0], shadowSquare0 );
		shadowSquare0.SetColour( shadowColour );
		shadowSquare0.SetDepth( shadowSquareDepth );

		float targetAngle = elements_.GetRequiredElementOfType<Element_Triangle>( mainTriangleName_ ).GetInternalAngleDegrees( 0 );

		Element_Parallelogram parallelogram0 = elements_.GetRequiredElementOfType<Element_Parallelogram>( parallelogramNames_[0] );
		parallelogram0.SetAlpha( shearAlpha );

		float elapsed = 0f;
		while (elapsed < shearSquareDuration)
		{
			elapsed += Time.deltaTime;
			parallelogram0.SetAngle( Mathf.Lerp( 90f, targetAngle, elapsed / shearSquareDuration ) );
			yield return null;
		}
		parallelogram0.SetAngle( targetAngle );
		yield return null;

		elapsed = 0f;
		while (elapsed <= postShearFadeDuration)
		{
			elapsed += Time.deltaTime;
			float fraction = elapsed / postShearFadeDuration;
			parallelogram0.SetAlpha( Mathf.Lerp( shearAlpha, 1f, fraction ) );
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

		Element_Parallelogram shadowParallelogram = elements_.GetRequiredElementOfType<Element_Parallelogram>( shadowParallelogramNames_[n] );
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

		yield return StartCoroutine( RemoveShadowSquareCR( 1 ) );

		Element_Parallelogram shadowParallelogram0 = elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[0] ).Clone<Element_Parallelogram>( shadowParallelogramNames_[0] );
		elements_.AddElement( shadowParallelogramNames_[0], shadowParallelogram0 );

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

		HandleEndOfSequence( ShearParallelogram1 );

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
	
	private void AssertMainTriangle( string locn )
	{
		if (elements_.GetElementOfType<Element_Triangle>( mainTriangleName_ ) == null)
		{
			elements_.DebugDescribe( );
			throw new System.Exception( "mainTriangle doesn't exist in " + locn );
		}
	}

	private void AssertParallelogram( string locn, int n )
	{
		if (elements_.GetElementOfType<Element_Parallelogram>( parallelogramNames_[n] ) == null)
		{
			elements_.DebugDescribe( );
			throw new System.Exception( "parallelograms[ " + n + " ] doesn't exist in " + locn );
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

}

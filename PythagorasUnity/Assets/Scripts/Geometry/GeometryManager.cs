using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Handles creation & placement of elements in fields
*/
public class GeometryManager : MonoBehaviour
{
#if UNITY_EDITOR
	public bool testMode = true; // set to true to create some elements to play around with for testing
	private List<Element> testElements = new List<Element>( );
	public void ClearTestElements()
	{
		for (int i = 0; i<testElements.Count; i++)
		{
			GameObject.Destroy( testElements[i].gameObject );
		}
		testElements.Clear( );
	}
#endif
	   
	#region prefabs

	public GameObject trianglePrefab;
	public GameObject parallelogramPrefab;
	public GameObject straightLinePrefab;

	#endregion prefabs

	#region private data

	private Field mainField_ = null; // currently only using the one Field

	#endregion private data

	#region Flow

	private void Awake()
	{
		mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
	}

	private void Start ()
	{
#if UNITY_EDITOR
		if (testMode)
		{
			testElements.Add( 
				AddTriangleToMainField(
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
			testElements.Add(
				AddParallelogramToMainField(
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
			testElements.Add(
				AddRightTriangleToMainField(
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
			testElements.Add(
				AddStraightLineToMainField(
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

	}

	#endregion Flow

	#region creating elements

	// Instantiate a Triangle in a Field and set it up
	private Triangle AddTriangleToField( string n, Field f, float d, Vector2[] v, Color c)
	{
		GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
		triangleGO.name = n;
		Triangle triangle = triangleGO.GetComponent<Triangle>( );
		triangle.Init( f, d, v, c );
		return triangle;
	}

	// Instantiate a Triangle in main Field and set it up
	public Triangle AddTriangleToMainField( string n, float d, Vector2[] v, Color c )
	{
		return AddTriangleToField( n, mainField_, d, v, c );
	}

	// Instantiate a right-angled Triangle (with base defined by v and height h) in a Field and set it up
	private Triangle AddRightTriangleToField( string n, Field f, float d, Vector2[] hypotenuse, float angle, Color c )
	{
		GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
		triangleGO.name = n;
		Triangle triangle = triangleGO.GetComponent<Triangle>( );
		triangle.InitRightAngled( f, d, hypotenuse, angle, c );
		return triangle;
	}

	// Instantiate a right-angled Triangle (with base defined by v and height h) in main Field and set it up
	public Triangle AddRightTriangleToMainField( string n, float d, Vector2[] hypotenuse, float angle, Color c )
	{
		return AddRightTriangleToField( n, mainField_, d, hypotenuse, angle, c );
	}

	// Instantiate a Parallelogram in a Field and set it up
	private Parallelogram AddParallelogramToField( string n, Field f, float d, Vector2[] bl, float h, float a, Color c )
	{
		GameObject parallelogramGO = GameObject.Instantiate<GameObject>( parallelogramPrefab ) as GameObject;
		parallelogramGO.name = n;
		Parallelogram parallelogram = parallelogramGO.GetComponent<Parallelogram>( );
		parallelogram.Init( f, d, bl, h, a, c );
		return parallelogram;
	}

	// Instantiate a Parallelogram in a Field and set it up
	public Parallelogram AddParallelogramToMainField( string n, float d, Vector2[] bl, float h, float a, Color c )
	{
		return AddParallelogramToField( n, mainField_, d, bl, h, a, c );
	}

	// Instantiate a StraightLine in a Field and set it up
	private StraightLine AddStraightLineToField( string n, Field f, float d, Vector2[] es, float w, Color c )
	{
		GameObject straightLineGO = GameObject.Instantiate<GameObject>( straightLinePrefab) as GameObject;
		straightLineGO.name = n;
		StraightLine straightLine = straightLineGO.GetComponent<StraightLine>( );
		straightLine.Init( f, d, es, w, c );
		return straightLine;
	}

	// Instantiate a Parallelogram in a Field and set it up
	public StraightLine AddStraightLineToMainField( string n, float d, Vector2[] es, float w, Color c )
	{
		return AddStraightLineToField( n, mainField_, d, es, w, c );
	}

	#endregion creating elements

}

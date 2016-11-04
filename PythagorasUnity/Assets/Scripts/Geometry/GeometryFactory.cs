using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Handles creation & placement of elements in fields
*/

namespace RJWard.Geometry
{
	public class GeometryFactory : MonoBehaviour
	{
#if UNITY_EDITOR
		// TODO remove, should only done by client/scene
		public bool testMode = true; // set to true to create some elements to play around with for testing
		private List<ElementBase> testElements = new List<ElementBase>( );
		public void ClearTestElements( )
		{
			for (int i = 0; i < testElements.Count; i++)
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

		private Field mainField_ = null; // TODO remove, should only be known to clients

		#endregion private data

		#region Flow

		private void Awake( )
		{
			mainField_ = new GameObject( "MainField" ).AddComponent<Field>( );
		}

		private void Start( )
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
		private Element_Triangle AddTriangleToField( string n, Field f, float d, Vector2[] v, Color c )
		{
			GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
			triangleGO.name = n;
			Element_Triangle triangle = triangleGO.GetComponent<Element_Triangle>( );
			triangle.Init( f, d, v, c );
			return triangle;
		}

		// Instantiate a Triangle in main Field and set it up
		public Element_Triangle AddTriangleToMainField( string n, float d, Vector2[] v, Color c )
		{
			return AddTriangleToField( n, mainField_, d, v, c );
		}

		// Instantiate a right-angled Triangle (with base defined by v and height h) in a Field and set it up
		private Element_Triangle AddRightTriangleToField( string n, Field f, float d, Vector2[] hypotenuse, float angle, Color c )
		{
			GameObject triangleGO = GameObject.Instantiate<GameObject>( trianglePrefab ) as GameObject;
			triangleGO.name = n;
			Element_Triangle triangle = triangleGO.GetComponent<Element_Triangle>( );
			triangle.InitRightAngled( f, d, hypotenuse, angle, c );
			return triangle;
		}

		// Instantiate a right-angled Triangle (with base defined by v and height h) in main Field and set it up
		public Element_Triangle AddRightTriangleToMainField( string n, float d, Vector2[] hypotenuse, float angle, Color c )
		{
			return AddRightTriangleToField( n, mainField_, d, hypotenuse, angle, c );
		}

		// Instantiate a Parallelogram in a Field and set it up
		private Element_Parallelogram AddParallelogramToField( string n, Field f, float d, Vector2[] bl, float h, float a, Color c )
		{
			GameObject parallelogramGO = GameObject.Instantiate<GameObject>( parallelogramPrefab ) as GameObject;
			parallelogramGO.name = n;
			Element_Parallelogram parallelogram = parallelogramGO.GetComponent<Element_Parallelogram>( );
			parallelogram.Init( f, d, bl, h, a, c );
			return parallelogram;
		}

		// Instantiate a Parallelogram in a Field and set it up
		public Element_Parallelogram AddParallelogramToMainField( string n, float d, Vector2[] bl, float h, float a, Color c )
		{
			return AddParallelogramToField( n, mainField_, d, bl, h, a, c );
		}

		// Instantiate a StraightLine in a Field and set it up
		private Element_StraightLine AddStraightLineToField( string n, Field f, float d, Vector2[] es, float w, Color c )
		{
			GameObject straightLineGO = GameObject.Instantiate<GameObject>( straightLinePrefab ) as GameObject;
			straightLineGO.name = n;
			Element_StraightLine straightLine = straightLineGO.GetComponent<Element_StraightLine>( );
			straightLine.Init( f, d, es, w, c );
			return straightLine;
		}

		// Instantiate a Parallelogram in a Field and set it up
		public Element_StraightLine AddStraightLineToMainField( string n, float d, Vector2[] es, float w, Color c )
		{
			return AddStraightLineToField( n, mainField_, d, es, w, c );
		}

		#endregion creating elements

	}

}

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
		#region prefabs

		public GameObject trianglePrefab;
		public GameObject parallelogramPrefab;
		public GameObject straightLinePrefab;

		#endregion prefabs

		#region private data

		#endregion private data

		#region Flow

		private void Awake( )
		{
		}

		private void Start( )
		{

		}

		#endregion Flow

		#region creating elements

		// Instantiate a Triangle in a Field and set it up
		public Element_Triangle AddTriangleToField( Field f, string n, float d, Vector2[] v, Color c )
		{
			GameObject triangleGO = GameObject.Instantiate< GameObject >( trianglePrefab ) as GameObject;
			triangleGO.name = n;
			Element_Triangle triangle = triangleGO.GetComponent< Element_Triangle >( );
			triangle.Init( f, d, v, c );
			return triangle;
		}

		// Instantiate a right-angled Triangle (with base defined by v and height h) in a Field and set it up
		public Element_Triangle AddRightTriangleToField( Field f, string n, float d, Vector2[] hypotenuse, float angle, Color c )
		{
			GameObject triangleGO = GameObject.Instantiate< GameObject >( trianglePrefab ) as GameObject;
			triangleGO.name = n;
			Element_Triangle triangle = triangleGO.GetComponent< Element_Triangle >( );
			triangle.InitRightAngled( f, d, hypotenuse, angle, c );
			return triangle;
		}

		// Instantiate a Parallelogram in a Field and set it up
		public Element_Parallelogram AddParallelogramToField( Field f, string n, float d, Vector2[] bl, float h, float a, Color c )
		{
			GameObject parallelogramGO = GameObject.Instantiate< GameObject >( parallelogramPrefab ) as GameObject;
			parallelogramGO.name = n;
			Element_Parallelogram parallelogram = parallelogramGO.GetComponent< Element_Parallelogram >( );
			parallelogram.Init( f, d, bl, h, a, c );
			return parallelogram;
		}

		// Instantiate a Square Parallelogram in a Field and set it up
		public Element_Parallelogram AddSquareParallelogramToField( Field f, string n, float d, Vector2[] bl, float a, Color c )
		{
			GameObject parallelogramGO = GameObject.Instantiate< GameObject >( parallelogramPrefab ) as GameObject;
			parallelogramGO.name = n;
			Element_Parallelogram parallelogram = parallelogramGO.GetComponent< Element_Parallelogram >( );
			parallelogram.InitSquare( f, d, bl, c );
			return parallelogram;
		}

		// Instantiate a StraightLine in a Field and set it up
		public Element_StraightLine AddStraightLineToField( Field f, string n, float d, Vector2[] es, float w, Color c )
		{
			GameObject straightLineGO = GameObject.Instantiate< GameObject >( straightLinePrefab ) as GameObject;
			straightLineGO.name = n;
			Element_StraightLine straightLine = straightLineGO.GetComponent< Element_StraightLine >( );
			straightLine.Init( f, d, es, w, c );
			return straightLine;
		}

		public Element_StraightLine CreateStraightLineFromTriangleSide( string n, Element_Triangle triangle, int sideNumber, float relativeDepth, float width, Color colour, bool internalSide)
		{
			return AddStraightLineToField(
				triangle.field,
				n,
				triangle.depth + relativeDepth,
				(internalSide)?( triangle.GetSideInternal(sideNumber)):( triangle.GetSideExternal(sideNumber)),
				width,
				colour
				);
		}

		public Element_StraightLine CreateInternalStraightLineFromTriangleSide( string n, Element_Triangle triangle, int sideNumber, float relativeDepth, float width, Color colour)
		{
			return CreateStraightLineFromTriangleSide( n, triangle, sideNumber, relativeDepth, width, colour, true );
        }

		public Element_StraightLine CreateExternalStraightLineFromTriangleSide( string n, Element_Triangle triangle, int sideNumber, float relativeDepth, float width, Color colour )
		{
			return CreateStraightLineFromTriangleSide( n, triangle, sideNumber, relativeDepth, width, colour, false );
		}
		
		#endregion creating elements
	}
}

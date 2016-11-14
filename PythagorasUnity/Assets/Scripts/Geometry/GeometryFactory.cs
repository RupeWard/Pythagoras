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
		public GameObject circlePrefab;
		public GameObject lineSegmentPrefab;
		public GameObject curvePrefab;
		public GameObject sectorPrefab;

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
			triangle.Init( this, f, d, v, c );
			return triangle;
		}

		// Instantiate a right-angled Triangle (with base defined by v and height h) in a Field and set it up
		public Element_Triangle AddRightTriangleToField( Field f, string n, float d, Vector2[] hypotenuse, float angle, Color c )
		{
			GameObject triangleGO = GameObject.Instantiate< GameObject >( trianglePrefab ) as GameObject;
			triangleGO.name = n;
			Element_Triangle triangle = triangleGO.GetComponent< Element_Triangle >( );
			triangle.InitRightAngled( this, f, d, hypotenuse, angle, c );
			return triangle;
		}

		// Instantiate a Parallelogram in a Field and set it up
		public Element_Parallelogram AddParallelogramToField( Field f, string n, float d, Vector2[] bl, float h, float a, Color c )
		{
			GameObject parallelogramGO = GameObject.Instantiate< GameObject >( parallelogramPrefab ) as GameObject;
			parallelogramGO.name = n;
			Element_Parallelogram parallelogram = parallelogramGO.GetComponent< Element_Parallelogram >( );
			parallelogram.Init( this, f, d, bl, h, a, c );
			return parallelogram;
		}

		// Instantiate a Square Parallelogram in a Field and set it up
		public Element_Parallelogram AddSquareParallelogramToField( Field f, string n, float d, Vector2[] bl, float a, Color c )
		{
			GameObject parallelogramGO = GameObject.Instantiate< GameObject >( parallelogramPrefab ) as GameObject;
			parallelogramGO.name = n;
			Element_Parallelogram parallelogram = parallelogramGO.GetComponent< Element_Parallelogram >( );
			parallelogram.InitSquare(this, f, d, bl, c );
			return parallelogram;
		}

		// Instantiate a StraightLine in a Field and set it up
		public Element_StraightLine AddStraightLineToField( Field f, string n, float d, Vector2[] es, float w, Color c )
		{
			GameObject straightLineGO = GameObject.Instantiate< GameObject >( straightLinePrefab ) as GameObject;
			straightLineGO.name = n;
			Element_StraightLine straightLine = straightLineGO.GetComponent< Element_StraightLine >( );
			straightLine.Init( this, f, d, es, w, c );
			return straightLine;
		}

		// Instantiate a Circle in a Field and set it up
		public Element_Circle AddCircleToField( Field f, string n, float d, Vector2 ce, float r, Color c )
		{
			GameObject circleGO = GameObject.Instantiate< GameObject >( circlePrefab ) as GameObject;
			circleGO.name = n;
			Element_Circle circle = circleGO.GetComponent< Element_Circle >( );
			circle.Init( this, f, d, ce, r, c );
			return circle;
		}

		// Instantiate a Sector in a Field and set it up
		public Element_Sector AddSectorToField( Field f, string n, float d, Vector2 ce, float r, float ae, float ad, Color c )
		{
			GameObject sectorGO = GameObject.Instantiate< GameObject >( sectorPrefab ) as GameObject;
			sectorGO.name = n;
			Element_Sector sector = sectorGO.GetComponent< Element_Sector >( );
			sector.Init( this, f, d, ce, r, ae, ad, c );
			return sector;
		}

		public Element_Sector AddSectorBetweenLines( string n, float relD, Element_StraightLine[] lines, float r, Color c )
		{
			if (lines.Length != 2)
			{
				throw new System.Exception( "GeometryFactory.AddSectorBetweenLines() needs 2 lines not " + lines.Length );
			}

			GameObject sectorGO = GameObject.Instantiate<GameObject>( sectorPrefab ) as GameObject;
			sectorGO.name = n;
			Element_Sector sector = sectorGO.GetComponent<Element_Sector>( );

			float depth = 0f;
			if (relD < 0f)
			{
				depth = Mathf.Min( lines[0].depth, lines[1].depth );
				depth += relD;
			}
			else if (relD > 0f)
			{
				depth = Mathf.Max( lines[0].depth, lines[1].depth );
				depth += relD;
			}
			else
			{
				depth = 0.5f * (lines[0].depth + lines[1].depth);
			}

			Field field = lines[0].field;
			if (lines[1].field != field)
			{
				Debug.LogWarning( "Making a sector between 2 lines in different fields" );
			}
			sector.Init( this, field, depth, lines, r, c );
			return sector;
		}

		// Instantiate a line segment (as a StraightLine) and set it up using its parent curve's properties 
		public Element_StraightLine AddLineSegmentToCurve( Element_Curve curve, string n, Vector2[] es )
		{
			GameObject segmentGO = GameObject.Instantiate< GameObject >( lineSegmentPrefab ) as GameObject;
			segmentGO.name = n;
			Element_StraightLine segment = segmentGO.GetComponent< Element_StraightLine >( );
			segment.Init( this, curve.field, curve.depth, es, curve.decorator1D, curve.cachedMaterial );
			segment.cachedTransform.SetParent( curve.cachedTransform );
			return segment;
		}

		// Instantiate a Curve in a Field and set it up
		public Element_Curve AddCurveToField( Field f, string n, float d, List< Vector2 > pts, bool closed, float w, Color c )
		{
			GameObject curveGO = GameObject.Instantiate< GameObject >( curvePrefab ) as GameObject;
			curveGO.name = n;
			Element_Curve curve = curveGO.GetComponent< Element_Curve >( );
			curve.Init( this, f, d, pts, closed, w, c );
			return curve;
		}

		// Create a straight line element from specifed side of a triangle
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
		

		public ElementBase CreateClone( string n, ElementBase srcElement, float relativeDepth, Color colour )
		{
			ElementBase result = srcElement.Clone( n );
			result.SetDepth( srcElement.depth + relativeDepth );
			result.SetColour( colour );
			return result;
		}
		
		#endregion creating elements
	}
}

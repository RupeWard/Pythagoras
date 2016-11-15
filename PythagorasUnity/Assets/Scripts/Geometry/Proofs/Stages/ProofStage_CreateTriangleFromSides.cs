using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/*
		Triangle vertices are assigned in the following order: 1) common point 2) 1st line otger point 3 ) 2nd line other point
	*/
	class ProofStage_CreateTriangleFromSides : ProofStageBase
	{
		#region private data

		private IStraightLineProvider[] lineProviders_ = null;

		private Element_Triangle triangle_ = null;
		private Color triangleColour_ = Color.magenta;

		private float depth_ = 0f;

		private string triangleName_ = "[UNNAMED RIGHT TRIANGLE]";

		#endregion private data

		#region setup

		public ProofStage_CreateTriangleFromSides(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac, 
			float depth,
			IStraightLineProvider[] lp, 
			Color c,
			string tn) 
			: base (n, descn, gf, f, durn, ac )
		{
			if (lp.Length != 2)
			{
				throw new System.ArgumentException( "ProofStage_CreateTriangleFromSides ctor needs 2 line providers, not " + lp.Length );
			}
			lineProviders_ = lp;
			triangleColour_ = c;
			triangleName_ = tn;
			depth_ = depth;
			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirements",
				new Dictionary<string, System.Type> ()
				{
					{ triangleName_, typeof(Element_Triangle) }
				}
            );
		}

		private void CreateTriangleIfNeeded( )
		{
			triangle_ = elements.GetElementOfType< Element_Triangle >( triangleName_ );
			if (triangle_ == null)
			{
				Element_StraightLine line1 = lineProviders_[0].GetLine( elements );
				Element_StraightLine line2 = lineProviders_[1].GetLine( elements );

				Vector2 intersection = Vector2.zero;
				if (Element_StraightLine.Intersection( line1, line2, ref intersection))
				{
					Vector2[] vs = new Vector2[3];
					vs[0] = intersection;
					vs[1] = line1.GetEndFurthestFrom( intersection );
					vs[2] = line2.GetEndFurthestFrom( intersection );

					triangle_ = geometryFactory.AddTriangleToField(
						field,
						triangleName_,
						depth_,
						vs,
						triangleColour_
					);
					elements.AddElement( triangleName_, triangle_ );
					triangle_.SetAlpha( 0f );
				}
				else
				{
					throw new System.Exception( "Lines provided by line providers are parallel!" );
				}

			}
			else
			{
				triangle_.gameObject.SetActive( true );
			}
		}

		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			CreateTriangleIfNeeded( );
		}

		protected override void HandleInit( )
		{
		}

		protected override void DoUpdateView( )
		{
			triangle_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch(direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						triangle_.SetAlpha( 1f );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (triangle_ != null)
						{
							triangle_.SetAlpha( 0f );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

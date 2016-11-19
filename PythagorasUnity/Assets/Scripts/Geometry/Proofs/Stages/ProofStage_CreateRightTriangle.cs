using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_CreateRightTriangle : ProofStageBase
	{
		static public bool DEBUG_RIGHTTRIANGLE = false;
		
		#region private data

		private Vector2[] hypotenuse_ = null;
		private float angle_ = 0f;
		private float depth_ = 0f;

		private Element_Triangle triangle_ = null;
		private Color triangleColour_ = Color.magenta;

		private string triangleName_ = "[UNNAMED RIGHT TRIANGLE]";

		#endregion private data

		#region setup

		public ProofStage_CreateRightTriangle(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac, 
			float depth,
			Vector2[] h, 
			float a, 
			Color c,
			string tn) 
			: base (n, descn, gf, f, durn, ac )
		{
			hypotenuse_ = h;
			angle_ = a;
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
				triangle_ = geometryFactory.AddRightTriangleToField(
					field,
					triangleName_,
					depth_,
					hypotenuse_,
					angle_,
					triangleColour_
					);
				elements.AddElement( triangleName_, triangle_ );
				triangle_.SetAlpha( 0f );
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
			if (DEBUG_RIGHTTRIANGLE)
			{
				Debug.Log( "CreateRightTriangle.HandleFirstUpdateAfterInit" );
			}
			CreateTriangleIfNeeded( );
		}

		protected override void HandleInit( )
		{
		}

		protected override void DoUpdateView( )
		{
			triangle_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
			Element_Sector ra = triangle_.GetAngleElement( 1 );
			if (ra != null)
			{
				triangle_.SetShowAngleElement( 1 );
				ra.SetColour( Color.cyan );
				ra.SetRadius( 0.1f );
			}
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
							elements.DestroyElement( ref triangle_ );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

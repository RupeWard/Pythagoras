using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_CreateTriangle : ProofStageBase
	{
		#region private data

		private Vector2[] hypotenuse_ = null;
		private float angle_ = 0f;

		private Element_Triangle triangle_ = null;
		private Color triangleColour_ = Color.magenta;

		#endregion private data

		#region setup

		public ProofStage_CreateTriangle(
			string n, string d, GeometryFactory gf, Field f, float dn, System.Action<ProofStageBase> ac, 
			Vector2[] h, 
			float a, 
			Color c) 
			: base (n, d, gf, f, dn, ac )
		{
			hypotenuse_ = h;
			angle_ = a;
			triangleColour_ = c;

			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirments",
				new Dictionary<string, System.Type> ()
				{
					{ "MainTriangle", typeof(Element_Triangle) }
				}
            );
		}

		#endregion setup

		#region ProofStageBase 

		protected override void HandleInit( )
		{
			triangle_ = geometryFactory.AddRightTriangleToField(
				field,
				"MainTriangle",
				0f,
				hypotenuse_,
				angle_,
				triangleColour_
				);
			AddElement( "MainTriangle", triangle_ );
			triangle_.SetAlpha( 0f );
		}

		protected override void DoUpdateView( )
		{
			triangle_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );

		}

		protected override void HandleFinished( )
		{
			triangle_.SetAlpha( 1f );
		}

		#endregion ProofStageBase 

	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_CreateRightTriangle : ProofStageBase
	{
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

		#endregion setup

		#region ProofStageBase 

		protected override void HandleInit( )
		{
			triangle_ = geometryFactory.AddRightTriangleToField(
				field,
				triangleName_,
				depth_,
				hypotenuse_,
				angle_,
				triangleColour_
				);
			AddElement( triangleName_, triangle_ );
			triangle_.SetAlpha( 0f );
		}

		protected override void DoUpdateView( )
		{
			triangle_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch(direction)
			{
				case EDirection.Forward:
					{
						triangle_.SetAlpha( 1f );
						break;
					}
				case EDirection.Reverse:
					{
						triangle_.SetAlpha( 0f );
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}
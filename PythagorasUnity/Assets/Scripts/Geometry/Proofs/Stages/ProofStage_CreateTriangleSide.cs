using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/*
		Create a straight line element corresponding to the side of a triangle.

		??? DEPRECATED ??? No longer being used since triangles have edges via polygon base class. If we do want to create a side from it we can just clone that.
	*/
	class ProofStage_CreateTriangleSide : ProofStageBase
	{
		#region private data

		// the line
		Element_StraightLine line_ = null;

		// line settings for creation
		private string lineName_ = "[UNNAMED LINE]";
		private Color lineColour_;
		private float lineWidth_ = 0.01f;
		private float relativeDepth_ = 0f;

		// source details
		private string triangleName_ = "[UNKNOWN TRIANGLE NAME]";
		private bool externalSide_ = true;
		private int sideNumber_ = 0;

		#endregion private data

		#region setup

		public ProofStage_CreateTriangleSide(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string triangleName,
			int sideNumber,
			bool externalSide,
			float relDepth,
			float lw,
			Color c,
			string ln) 
			: base (n, descn, gf, f, durn, ac )
		{
			lineColour_ = c;
			lineName_ = ln;
			triangleName_ = triangleName;
			sideNumber_ = sideNumber;
			externalSide_ = externalSide;
			relativeDepth_ = relDepth;
			lineWidth_ = lw;

			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ triangleName, typeof(Element_Triangle) }
				}
			);
			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ lineName_, typeof(Element_StraightLine) }
				}
			);
		}

		private void CreateLineIfNeeded( )
		{
			line_ = elements.GetElementOfType< Element_StraightLine >( lineName_ );
			if (line_ == null)
			{
				Element_Triangle triangle = elements.GetRequiredElementOfType<Element_Triangle>( triangleName_ );
				line_ = geometryFactory.CreateStraightLineFromTriangleSide(
					lineName_,
					triangle,
					sideNumber_,
					triangle.depth + relativeDepth_,
					lineWidth_,
					lineColour_,
					externalSide_
					);
				elements.AddElement( lineName_, line_ );

				switch (direction)
				{
					case ProofEngine.EDirection.Forward:
						{
							line_.SetAlpha( 0f );
							break;
						}
					case ProofEngine.EDirection.Reverse:
						{
							line_.SetAlpha( 1f );
							break;
						}
				}
			}
			else
			{
				line_.gameObject.SetActive( true );
			}
		}

		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			CreateLineIfNeeded( );
		}

		protected override void HandleInit( )
		{ 
		}

		protected override void DoUpdateView( )
		{
			line_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (line_ != null)
						{
							line_.SetAlpha( 0f );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

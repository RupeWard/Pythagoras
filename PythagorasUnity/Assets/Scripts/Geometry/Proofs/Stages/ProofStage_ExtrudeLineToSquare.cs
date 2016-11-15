using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_ExtrudeLineToSquare : ProofStageBase
	{
		#region private data

		// The parallelogram
		Element_Parallelogram parallelogram_ = null;

		// parallelogram settings for creation
		private string parallelogramName_ = "[UNNAMED PARALLELOGRAM]";
		private float parallelogramAngle_ = 90f;
		private Color parallelogramColour_;
		private float parallelogramHeight_ = 0f;

		private float relativeDepth_ = 0f;

		private IStraightLineProvider lineProvider_ = null;

		
		#endregion private data

		#region setup

		public ProofStage_ExtrudeLineToSquare(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IStraightLineProvider ilp,
			float relDepth,
			float a,
			Color c,
			string pn) 
			: base (n, descn, gf, f, durn, ac )
		{
			parallelogramColour_ = c;
			parallelogramName_ = pn;
			parallelogramAngle_ = a;
			relativeDepth_ = relDepth;

			lineProvider_ = ilp;

			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ parallelogramName_, typeof(Element_Parallelogram) }
				}
			);
		}

		private void CreateSquareIfNeeded( )
		{
			parallelogram_ = elements.GetElementOfType<Element_Parallelogram>( parallelogramName_ );
			if (parallelogram_ == null)
			{
				Element_StraightLine line = lineProvider_.GetLine( elements );

				if (line == null)
				{
					throw new System.Exception( "Line provider returned null!" );
				}
				else
				{
					parallelogramHeight_ = line.length;

					parallelogram_ =
						geometryFactory.AddParallelogramToField(
							line.field,
							parallelogramName_,
							line.depth + relativeDepth_,
							line.GetEnds( ),
							parallelogramHeight_,
							parallelogramAngle_,
							parallelogramColour_
							);
					elements.AddElement( parallelogramName_, parallelogram_ );

					switch (direction)
					{
						case ProofEngine.EDirection.Forward:
							{
								parallelogram_.SetHeight( 0f );
								break;
							}
						case ProofEngine.EDirection.Reverse:
							{
								parallelogram_.SetHeight( parallelogramHeight_ );
								break;
							}
					}

				}
			}
		}


		#endregion setup

		#region ProofStageBase 

		protected override void HandleInit( )
		{
		}

		protected override void DoUpdateView( )
		{
			CreateSquareIfNeeded( );
			parallelogram_.SetHeight(Mathf.Lerp( 0f, parallelogramHeight_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			// TODO anything?
		}

		#endregion ProofStageBase 

	}
}

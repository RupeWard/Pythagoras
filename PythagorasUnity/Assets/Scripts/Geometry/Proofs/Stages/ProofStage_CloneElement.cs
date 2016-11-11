using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_CloneElement : ProofStageBase
	{
		#region private data

		// The clone element
		ElementBase cloneElement_ = null;

		// element settings for creation
		private string cloneName_ = "[UNNAMED CLONE]";
		private Color cloneColour_;
		private float relativeDepth_ = 0.01f;

		// source name
		private string srcName_ = "[UNKNOWN ELEMENT]";
		private System.Type srcType_ =  null;

		#endregion private data

		#region setup

		public ProofStage_CloneElement(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string sn,
			System.Type st,
			float relDepth,
			Color c,
			string cn) 
			: base (n, descn, gf, f, durn, ac )
		{
			cloneColour_ = c;
			cloneName_ = cn;
			relativeDepth_ = relDepth;

			srcName_ = sn;
			srcType_ = st;

			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ srcName_, srcType_ }
				}
			);

			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ cloneName_, srcType_ }
				}
			);
		}

		#endregion setup

		#region ProofStageBase 

		private void CreateCloneIfNeeded()
		{
			if (cloneElement_ == null || elements.GetElementOfType( cloneName_, srcType_ ) == null)
			{
				if (cloneElement_ != null)
				{
					Debug.LogWarning( "CloneElement isn't null" );
				}
				ElementBase srcElement = elements.GetRequiredElement( srcName_ );

				cloneElement_ = geometryFactory.CreateClone(
					cloneName_,
					srcElement,
					relativeDepth_,
					cloneColour_ );

				elements.AddElement( cloneName_, cloneElement_ );
			}
		}

		protected override void HandleInit( )
		{
		}

		protected override void DoUpdateView( )
		{
			CreateCloneIfNeeded( );
			cloneElement_.SetAlpha(Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						cloneElement_.SetAlpha( 1f );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (cloneElement_ != null)
						{
							cloneElement_.SetAlpha( 0f );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

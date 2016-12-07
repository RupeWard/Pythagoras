using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_HideElement : ProofStageBase
	{
		#region private data

		// the element
		ElementBase element_ = null;

		private string elementName_ = "[UNKNOWN ELEMENT]";
		
		#endregion private data

		#region setup

		public ProofStage_HideElement(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string en,
			System.Type elementType
			) 
			: base (n, descn, gf, f, durn, ac )
		{
			elementName_ = en;
			
			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ elementName_, elementType}
				}
			);
		}

		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			if (element_ != null)
			{
				element_.gameObject.SetActive( true );
			}
		}

		protected override void HandleInit( )
		{
			element_ = elements.GetElement(elementName_);
		}

		protected override void DoUpdateView( )
		{
			if (element_ != null)
			{
				element_.SetAlpha( Mathf.Lerp( 1f, 0f, currentTimeFractional ) );
			}
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						if (element_ != null)
						{
							element_.SetAlpha( 0f );
							element_.gameObject.SetActive( false );
						}
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (element_ != null)
						{
							element_.SetAlpha( 1f );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

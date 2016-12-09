using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_HideElements : ProofStageBase
	{
		#region private data

		// the elements
		private List <ElementBase> elementsToHide_ = new List< ElementBase >();

		private Dictionary<string, System.Type> elementDefns_ = null;
		
		#endregion private data

		#region setup

		public ProofStage_HideElements(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			Dictionary<string, System.Type> defns
			) 
			: base (n, gf, f, durn, ac )
		{
			elementDefns_ = defns;
			
			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				elementDefns_
			);
		}

		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			for (int i = 0; i < elementsToHide_.Count; i++)
			{
				if (elementsToHide_[i] != null)
				{
					elementsToHide_[i].gameObject.SetActive( true );
				}
			}
		}

		protected override void HandleInit( )
		{
			foreach (KeyValuePair< string, System.Type> kvp in elementDefns_)
			{
				elementsToHide_.Add(elements.GetRequiredElementOfType( kvp.Key, kvp.Value));
			}
		}

		protected override void DoUpdateView( )
		{
			for (int i = 0; i < elementsToHide_.Count; i++)
			{
				if (elementsToHide_[i] != null)
				{
					elementsToHide_[i].SetAlpha( Mathf.Lerp( 1f, 0f, currentTimeFractional ) );
				}
			}
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						for (int i = 0; i < elementsToHide_.Count; i++)
						{
							if (elementsToHide_[i] != null)
							{
								elementsToHide_[i].SetAlpha( 0f );
								elementsToHide_[i].gameObject.SetActive( false );
							}
						}
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						for (int i = 0; i < elementsToHide_.Count; i++)
						{
							if (elementsToHide_[i] != null)
							{
								elementsToHide_[i].SetAlpha( 1f );
							}
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

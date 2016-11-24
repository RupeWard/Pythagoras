﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_PulseDisplayElement : ProofStageBase
	{
		#region private data

		// the element
		ElementBase element_ = null;
		Vector3 startingPosition_;
		Vector3 startingScale_;
		Vector3 endScale_;
		float maxRelativeScale_;
		Vector3 pulseCentre_;

		private string elementName_ = "[UNKNOWN ELEMENT]";
		private System.Type elementType_;
		#endregion private data

		#region setup

		public ProofStage_PulseDisplayElement(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string en,
			float relScale,
			System.Type et
			) 
			: base (n, descn, gf, f, durn, ac )
		{
			elementName_ = en;
			elementType_ = et;
			maxRelativeScale_ = relScale;

			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ elementName_, elementType_}
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
				startingPosition_ = element_.cachedTransform.position;
				startingScale_ = element_.cachedTransform.localScale;
				endScale_ = startingScale_ * maxRelativeScale_;
				pulseCentre_ = element_.DefaultPulseCentre( );
			}
		}

		protected override void HandleInit( )
		{
			element_ = elements.GetElementOfType(elementName_, elementType_);
		}

		protected override void DoUpdateView( )
		{
			if (element_ != null)
			{
				SetScale(Mathf.Sin( currentTimeFractional * Mathf.PI ));
			}
		}

		private void SetScale(float f)
		{
			if (element_ != null)
			{
				element_.cachedTransform.localScale = Vector3.Lerp( startingScale_, endScale_, f);
				element_.cachedTransform.position = startingPosition_ - f * pulseCentre_;
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
							element_.cachedTransform.localScale = startingScale_;
						}
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (element_ != null)
						{
							element_.cachedTransform.localScale = startingScale_;
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

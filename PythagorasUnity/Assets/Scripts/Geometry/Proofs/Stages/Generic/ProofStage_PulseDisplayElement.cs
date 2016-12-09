using UnityEngine;
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

		Vector2 scaleRange_ = Vector2.zero;
		float scalez = 0f;

		float maxRelativeScale_;
		Vector3 pulseCentre_;

		private IElementProvider elementProvider_ = null;

		#endregion private data

		#region setup
		
		public ProofStage_PulseDisplayElement(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string en,
			float relScale
			) 
			: base (n, gf, f, durn, ac )
		{
			elementProvider_ = new ElementProvider_Name( en );
			maxRelativeScale_ = relScale;
		}

		public ProofStage_PulseDisplayElement(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IElementProvider iep,
			float relScale
			)
		: base( n, gf, f, durn, ac )
		{
			elementProvider_ = iep;
			maxRelativeScale_ = relScale;
		}


		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			if (element_ != null)
			{
				element_.gameObject.SetActive( true );
				startingPosition_ = element_.cachedTransform.position;
				scalez = element_.cachedTransform.localScale.z;
				scaleRange_.x = element_.cachedTransform.localScale.x;
				if (scaleRange_.x != element_.cachedTransform.localScale.y)
				{
					Debug.LogError( "Scale not symmetrical for " + element_.name + ": " + element_.cachedTransform.localScale );
				}
				scaleRange_.y = scaleRange_.x * maxRelativeScale_;
				pulseCentre_ = element_.DefaultPulseCentre( );
			}
		}

		protected override void HandleInit( )
		{
			element_ = elementProvider_.GetElement< ElementBase >(elements);
		}

		protected override void DoUpdateView( )
		{
			if (element_ != null)
			{
				SetScale(Mathf.Sin( currentTimeFractional * Mathf.PI ));
			}
		}

		// F goes from 0 to 1
		private void SetScale(float f)
		{
			if (element_ != null)
			{
				float scale = Mathf.Lerp( scaleRange_.x, scaleRange_.y, f );
				SetScaleVector( scale);
				element_.cachedTransform.position = startingPosition_ - pulseCentre_ * ( scale - scaleRange_.x);
			}
		}

		private void SetScaleVector(float f)
		{
			element_.cachedTransform.localScale = new Vector3( f, f, scalez );
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						if (element_ != null)
						{
							SetScale( 0f );
						}
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (element_ != null)
						{
							SetScale( 0f );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

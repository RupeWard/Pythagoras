using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_ShearParallelogram : ProofStageBase
	{
		#region private data

		// The parallelogram to shear
		Element_Parallelogram parallelogram_ = null;

		// parallelogram settings for creation
		private string parallelogramName_ = "[UNKNOWN PARALLELOGRAM]";
		private IAngleProvider angleProvider_ = null;

		private float parallelogramTargetAngle_ = 90f;
		private float parallelogramStartAngle_ = 90f;

		#endregion private data

		#region setup

		public ProofStage_ShearParallelogram(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			IAngleProvider ap
			) 
			: base (n, descn, gf, f, durn, ac )
		{
			parallelogramName_ = pn;
			angleProvider_ = ap;

			startRequiredElementListDefinition = new ElementListDefinition(
				"StartRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ parallelogramName_, typeof(Element_Parallelogram) }
				}
			);
			endRequiredElementListDefinition = new ElementListDefinition(
				"EndRequirements",
				new Dictionary<string, System.Type>( )
				{
					{ parallelogramName_, typeof(Element_Parallelogram) }
				}
			);
		}

		#endregion setup

		#region ProofStageBase 

		protected override void HandleInit( )
		{
			if (parallelogram_ == null)
			{
				parallelogram_ = elements.GetRequiredElementOfType<Element_Parallelogram>( parallelogramName_ );
				parallelogramStartAngle_ = parallelogram_.angle;
				parallelogramTargetAngle_ = angleProvider_.GetAngle( elements );
			}
		}

		protected override void DoUpdateView( )
		{
			parallelogram_.SetAngle(Mathf.Lerp( parallelogramStartAngle_, parallelogramTargetAngle_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
                    {
						parallelogram_.SetAngle( parallelogramTargetAngle_ );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						parallelogram_.SetAngle( parallelogramStartAngle_);
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

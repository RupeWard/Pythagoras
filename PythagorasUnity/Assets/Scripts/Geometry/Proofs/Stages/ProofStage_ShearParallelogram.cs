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
		private int baselineNumber_ = 0;

		private IAngleProvider startAngleProvider_ = null;
		private IAngleProvider targetAngleProvider_ = null;

		private float parallelogramTargetAngle_ = 90f;
		private float parallelogramStartAngle_ = 90f;

		private float shearAlpha_ = 1f;

		#endregion private data

		#region setup

		public ProofStage_ShearParallelogram(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			int bn,
			float sa,
			IAngleProvider sap,
            IAngleProvider tap
			) 
			: base (n, descn, gf, f, durn, ac )
		{
			CtorSetup( pn, bn, sa, sap, tap );
		}

		public ProofStage_ShearParallelogram(
			string n, string descn, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap
			)
			: base( n, descn, gf, f, durn, ac )
		{
			CtorSetup( pn, 0, sa, sap, tap );
		}

		private void CtorSetup(
			string pn,
			int bn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap
			)
		{
			parallelogramName_ = pn;
			shearAlpha_ = sa;
			baselineNumber_ = bn;
			startAngleProvider_ = sap;
			targetAngleProvider_ = tap;

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
				if (direction == ProofEngine.EDirection.Forward)
				{
					if (baselineNumber_ != 0)
					{
						parallelogram_.ChangeBaseline( baselineNumber_ );
					}
				}
				parallelogramStartAngle_ = startAngleProvider_.GetAngle( elements );
				parallelogramTargetAngle_ = targetAngleProvider_.GetAngle( elements );
			}
			else
			{
				if (direction == ProofEngine.EDirection.Forward)
				{
					if (baselineNumber_ != 0)
					{
						parallelogram_.ChangeBaseline( baselineNumber_ );
					}
				}
			}
		}

		protected override void DoUpdateView( )
		{
			if (currentTimeFractional > 0f)
			{
				parallelogram_.SetAlpha( shearAlpha_ );
			}
			parallelogram_.SetAngleDegrees(Mathf.Lerp( parallelogramStartAngle_, parallelogramTargetAngle_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			parallelogram_.SetAlpha( 1f );
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
                    {
						parallelogram_.SetAngleDegrees( parallelogramTargetAngle_ );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						parallelogram_.SetAngleDegrees( parallelogramStartAngle_);
						if (baselineNumber_ != 0)
						{
							parallelogram_.ChangeBaseline( 4 - baselineNumber_ );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}

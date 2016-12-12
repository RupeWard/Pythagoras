﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_ShearParallelogram : ProofStageBase
	{
		static private readonly bool DEBUG_LOCAL = true;

		#region private data

		// The parallelogram to shear
		Element_Parallelogram parallelogram_ = null;

		// parallelogram settings for creation
		private string parallelogramName_ = "[UNKNOWN PARALLELOGRAM]";
		private int baselineNumber_ = -1;

		private IAngleProvider startAngleProvider_ = null;
		private IAngleProvider targetAngleProvider_ = null;
		private IStraightLineProvider baselineIdentifier_ = null;

		private float parallelogramTargetAngle_ = float.NaN;
		private float parallelogramStartAngle_ = float.NaN;

		private float shearAlpha_ = 1f;

		#endregion private data

		#region setup

		public ProofStage_ShearParallelogram(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			int bn,
			float sa,
			IAngleProvider sap,
            IAngleProvider tap
			) 
			: base (n, gf, f, durn, ac )
		{
			CtorSetup( pn, bn, sa, sap, tap );
		}

		public ProofStage_ShearParallelogram(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap
			)
			: base( n, gf, f, durn, ac )
		{
			CtorSetup( pn, 0, sa, sap, tap );
		}

		public ProofStage_ShearParallelogram(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap,
			IStraightLineProvider bli
			)
		: base( n, gf, f, durn, ac )
		{
			CtorSetup( pn, -1, sa, sap, tap );
			baselineIdentifier_ = bli;
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

		override protected void HandleFirstUpdateAfterInit( )
		{
			parallelogram_.SetAlpha( shearAlpha_ );
			parallelogram_.SetShowEdgeElement( 0 );
			Element_StraightLine edge0 = parallelogram_.GetEdgeElement( 0 ) as Element_StraightLine;
			if (edge0 != null)
			{
				edge0.SetColour( Color.black );
				edge0.SetWidth( 0.05f );
			}
		}

		protected override void HandleInit( )
		{
			if (parallelogram_ == null)
			{
				parallelogram_ = elements.GetRequiredElementOfType<Element_Parallelogram>( parallelogramName_ );
				if (baselineIdentifier_ != null)
				{
					Element_StraightLine baselineToMatch = baselineIdentifier_.GetLine( elements );
					Element_StraightLine baseline = null;

					int index = parallelogram_.FindEdgeClosestToEdge( baselineToMatch, ref baseline );
					if (index < 0)
					{
						throw new System.Exception( "Failed to match baseline" );
					}
					else
					{
						if (DEBUG_LOCAL)
						{
							Debug.Log( "Found line matching "+baselineToMatch.name+" which has index #" + index +" and name "+baseline.name);
						}
						baselineNumber_ = index;
					}
				}
				if (baselineNumber_ < 0)
				{
					throw new System.Exception( "BaselineNumber not set" );
				}
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
				parallelogram_.gameObject.SetActive( true );
			}
			if (DEBUG_LOCAL)
			{
				Debug.Log( "In '"+name+"' HandleInit, start="+parallelogramStartAngle_+", target="+parallelogramTargetAngle_ );
			}
		}

		protected override void DoUpdateView( )
		{
			parallelogram_.SetAngleDegrees(Mathf.Lerp( parallelogramStartAngle_, parallelogramTargetAngle_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "' HandleFinished");
			}
			parallelogram_.SetAlpha( 1f );
			parallelogram_.SetHideEdgeElement( 0);
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

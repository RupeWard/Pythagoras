using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_ShearSquare : ProofStageBase
	{
		static private bool DEBUG_LOCAL = true;

		#region private data

		// The parallelogram to shear
		// Element_Parallelogram parallelogram_ = null;

		// parallelogram settings for creation
		private string parallelogramName_ = "[UNKNOWN PARALLELOGRAM]";
		private int baselineNumber_ = 0;

		private IAngleProvider startAngleProvider_ = null;
		private IAngleProvider targetAngleProvider_ = null;

//		private float parallelogramTargetAngle_ = float.NaN;
//		private float parallelogramStartAngle_ = float.NaN;

		private float shearAlpha_ = 1f;

		ProofStage_ShearParallelogram shearStage_ = null;

		#endregion private data

		#region setup

		public ProofStage_ShearSquare(
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

		public ProofStage_ShearSquare(
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
			if (DEBUG_LOCAL)
			{
				Debug.Log( "Setting up ShearSquare stage '" + name + "'" );
			}

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

		override protected void HandleInit()
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleInit" );
			}
		}

		override protected void HandleFirstUpdateAfterInit( )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleFirstUpdateAfterInit" );
			}

			if (shearStage_ == null)
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "'" + name + "': Creating internal shearparallelogram stage" );
				}
				shearStage_ = new ProofStage_ShearParallelogram(
					name, description, geometryFactory, field, durationSeconds, HandleShearStageFinishedFromStage,
					parallelogramName_,
					baselineNumber_,
					shearAlpha_,
					startAngleProvider_,
					targetAngleProvider_ );
			}
			proofEngine_.RunStageAsCR( shearStage_, direction, elements, HandleShearStageFinishedFromEngine );
		}

		protected override void DoUpdateView( )
		{
			if (direction == ProofEngine.EDirection.Reverse && currentTimeSeconds_ <= 0f)
			{
				currentTimeSeconds_ = Mathf.Epsilon;
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "'"+name+"' Fixed time in reverse direction as it has hit zero "+Time.time );
				}
			}
			if (direction == ProofEngine.EDirection.Forward && currentTimeSeconds_ >= durationSeconds)
			{
				currentTimeSeconds_ = durationSeconds - Mathf.Epsilon;
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "'"+name+" Fixed time in forward direction as it has hit duration "+Time.time );
				}
			}

			//			parallelogram_.SetAngleDegrees( Mathf.Lerp( parallelogramStartAngle_, parallelogramTargetAngle_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleFinished "+Time.time );
			}

			if (shearStage_ != null)
			{
				shearStage_ = null;
			}

			/*
			parallelogram_.SetAlpha( 1f );
			parallelogram_.SetHideEdgeElement( 0 );
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						parallelogram_.SetAngleDegrees( parallelogramTargetAngle_ );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						parallelogram_.SetAngleDegrees( parallelogramStartAngle_ );
						if (baselineNumber_ != 0)
						{
							parallelogram_.ChangeBaseline( 4 - baselineNumber_ );
						}
						break;
					}
			}*/
		}

		#endregion ProofStageBase 

		#region Process

		public void HandleShearStageFinishedFromStage( ProofStageBase stage )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleShearStageFinishedFromStage" );
			}
		}

		public void HandleShearStageFinishedFromEngine( ProofStageBase stage )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleShearStageFinishedFromEngine" );
			}
			Finish( );
		}


		#endregion Process

		/*
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
		}

		protected override void DoUpdateView( )
		{
			parallelogram_.SetAngleDegrees( Mathf.Lerp( parallelogramStartAngle_, parallelogramTargetAngle_, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			parallelogram_.SetAlpha( 1f );
			parallelogram_.SetHideEdgeElement( 0 );
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						parallelogram_.SetAngleDegrees( parallelogramTargetAngle_ );
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						parallelogram_.SetAngleDegrees( parallelogramStartAngle_ );
						if (baselineNumber_ != 0)
						{
							parallelogram_.ChangeBaseline( 4 - baselineNumber_ );
						}
						break;
					}
			}
		}
		*/


	}
}

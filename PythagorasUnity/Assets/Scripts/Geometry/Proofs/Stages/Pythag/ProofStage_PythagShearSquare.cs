using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	class ProofStage_PythagShearSquare : ProofStageBase
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
		private IStraightLineProvider baselineIdentifier_ = null;

//		private float parallelogramTargetAngle_ = float.NaN;
//		private float parallelogramStartAngle_ = float.NaN;

		private float shearAlpha_ = 1f;

		ProofStage_ShearParallelogram shearStage_ = null;

		#endregion private data

		#region setup

		public ProofStage_PythagShearSquare(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			int bn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap,
			IStraightLineProvider bsi
			) 
			: base (n, gf, f, durn, ac )
		{
			CtorSetup( pn, bn, sa, sap, tap );
			baselineIdentifier_ = bsi;
		}

		public ProofStage_PythagShearSquare(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			string pn,
			float sa,
			IAngleProvider sap,
			IAngleProvider tap,
			IStraightLineProvider bsi
			)
		: base( n, gf, f, durn, ac )
		{
			CtorSetup( pn, -1, sa, sap, tap );
			baselineIdentifier_ = bsi;
		}


		/*
		public ProofStage_ShearSquare(
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
		*/

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

				if (baselineIdentifier_  != null)
				{
					shearStage_ = new ProofStage_ShearParallelogram(
						"ShearParallelogram_" + name, geometryFactory, field, durationSeconds, HandleShearStageFinishedFromStage,
						parallelogramName_,
						shearAlpha_,
						startAngleProvider_,
						targetAngleProvider_,
						baselineIdentifier_ );
				}
				else
				{
					shearStage_ = new ProofStage_ShearParallelogram(
						"ShearParallelogram_" + name, geometryFactory, field, durationSeconds, HandleShearStageFinishedFromStage,
						parallelogramName_,
						baselineNumber_,
						shearAlpha_,
						startAngleProvider_,
						targetAngleProvider_);
				}
			}
			proofEngine_.RunSubStageAsCR( shearStage_, this, direction, elements, HandleShearStageFinishedFromEngine );
		}

		protected override void DoUpdateView( )
		{
			if (direction == ProofEngine.EDirection.Reverse && currentTimeSeconds_ <= 0f)
			{
				currentTimeSeconds_ = Time.deltaTime;
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "'"+name+ "' Fixed time in reverse direction to " + currentTimeSeconds_ + " as it has hit zero at time " + Time.time );
				}
			}
			if (direction == ProofEngine.EDirection.Forward && currentTimeSeconds_ >= durationSeconds)
			{
				currentTimeSeconds_ = durationSeconds - Time.deltaTime;
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "'"+name+" Fixed time in forward direction to "+currentTimeSeconds_+" as it has hit duration at time "+Time.time );
				}
			}
		}

		protected override void HandleFinished( )
		{
			if (DEBUG_LOCAL)
			{
				Debug.Log( "'" + name + "': HandleFinished "+Time.time );
			}

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

	}
}

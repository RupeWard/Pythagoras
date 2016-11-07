using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class ProofEngine : MonoBehaviour
	{
		public static bool DEBUG_PROOFENGINE = true;

		#region private data

		private ProofStageBase currentStage_ = null;

		const float MAXSPEED = 100f;
		 
		private float speed_ = 1f;
		private bool isPaused_ = true;

		#endregion private data

		#region setters

		public bool SetSpeed(float f)
		{
			bool success = true;
			if (speed_ != f)
			{
				if (f > 0f && f <= MAXSPEED)
				{
					if (DEBUG_PROOFENGINE)
					{
						Debug.Log( "ProofEngine speed changed from " + speed_ + " to " + f );
					}
					speed_ = f;
				}
				else
				{
					success = false;
					Debug.LogError( "New speed out of range at " + f + ", stays at " + speed_ );
				}
			}
			return success;
		}

		public bool SetPaused( bool b)
		{
			bool success = true;
			if (isPaused_ != b)
			{
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "ProofEngine isPaused changed to " + b );
				}
				isPaused_ = b;
			}
			return success;
		}

		#endregion setters

		#region editor modding

#if UNITY_EDITOR
		// for in-editor modification

		public float modSpeed = 1f;
		public bool modIsPaused = true;

		private void CheckIfModded( )
		{
			if (modSpeed != speed_)
			{
				if (SetSpeed(modSpeed))
				{
					if (DEBUG_PROOFENGINE)
					{
						Debug.LogWarning( "Inspector speed change made" );
					}
				}
				else
				{
					Debug.LogWarning( "Inspector speed change rejected" );
					modSpeed = speed_;
				}
			}
			if (modIsPaused != isPaused_)
			{
				if (SetPaused( modIsPaused))
				{
					if (DEBUG_PROOFENGINE)
					{
						Debug.LogWarning( "Inspector pause change made" );
					}
				}
			}

		}

		private void SetModdingValues( )
		{
			modSpeed = speed_;
			modIsPaused = isPaused_;
		}

#endif
		#endregion editor modding

		#region MD Flow

		private void Awake()
		{
			if (DEBUG_PROOFENGINE)
			{
				Debug.Log( "ProofEngine.Awake()" );
			}
		}

		private void OnDestroy()
		{
			if (DEBUG_PROOFENGINE)
			{
				Debug.Log( "ProofEngine.OnDestroy()" );
			}
		}

		private void Start()
		{
#if UNITY_ENGINE
			SetModdingValues( );
#endif
		}

		private void Update()
		{
#if UNITY_ENGINE
			CheckIfModded( );
#endif
			if (false == isPaused_)
			{
				if (currentStage_ != null)
				{
					currentStage_.HandleSecondsElapsed( Time.deltaTime * speed_ );
				}
			}
		}
		#endregion MD Flow

		#region Process

		public void ChangeStage( ProofStageBase b)
		{
			currentStage_ = b;
		}

		#endregion Process

	}
}

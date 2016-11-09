using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/* 
		ProofEngine

		Handles updates of proof stages
	*/
	public class ProofEngine : MonoBehaviour
	{
		public static bool DEBUG_PROOFENGINE = true;

		public enum EDirection
		{
			Forward,
			Reverse
		}

		#region private data

		private ProofStageBase currentStage_ = null;

		const float MAXSPEED = 100f;
		 
		private float speed_ = 1f; // each proof stage specifies duration, engine applies this global modifier
		private bool loop_ = false;

		private bool isPaused_ = true;

		#endregion private data

		#region properties

		public bool isPaused
		{
			get { return isPaused_; }
		}

		public bool loop
		{
			get { return loop_;  }
		}

		#endregion properties

		#region callbacks

		public System.Action< bool > onPauseAction;
		public System.Action< EDirection > onDirectionChangedAction;
		public System.Action<bool> onLoopChangedAction;

		#endregion callbacks

		#region setters

		/* Set speed

			returns true if speed is the speed asked for (including if it was already that speed)
		*/
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

		/* Pause/Resume
		*/
		public void SetPaused( bool b)
		{
			if (isPaused_ != b)
			{
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "ProofEngine isPaused changed to " + b );
				}
				isPaused_ = b;
				if (onPauseAction != null)
				{
					onPauseAction( isPaused_ );
				}
			}
			else
			{
				if (DEBUG_PROOFENGINE)
				{
					Debug.LogWarning( "ProofEngine isPaused was already " + b );
				}
			}
		}

		public void Pause()
		{
			SetPaused( true );
		}

		public void Resume( )
		{
			SetPaused( false );
		}

		public void TogglePause()
		{
			SetPaused( !isPaused_ );
		}

		public void SetLoop(bool l)
		{
			if (loop_ != l)
			{
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "Changing ProofEngie.loop to " + l );
				}
				loop_ = l;
				if (onLoopChangedAction != null)
				{
					onLoopChangedAction( loop_ );
				}
			}
			else
			{
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "ProofEngie.loop isd already " + l );
				}
			}
		}

		public void ToggleLoop()
		{
			SetLoop( !loop_ );
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
				SetPaused( modIsPaused );
				if (DEBUG_PROOFENGINE)
				{
					Debug.LogWarning( "Inspector pause change made" );
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
			// If not paused, update the current stage
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

		public void Init( ProofStageBase b)
		{
			currentStage_ = b;
			if (onDirectionChangedAction != null)
			{
				onDirectionChangedAction( currentStage_.direction );
			}
		}

		/* Change stage according to previously completed stage

			Note: this is direction dependent. GetFollowingStage can be either the one before or the one after.
		*/
		public void ChangeToFollowingStage( ProofStageBase b )
		{
			if (b == null)
			{
				throw new System.ArgumentNullException( "Stage supplied is null" );
			}
			ProofStageBase followingStage = b.GetFollowingStage( );
			if (followingStage != null)
			{
				currentStage_ = followingStage;
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "ChangeToFollowingStage(" + b.name + " ) changes it to " + currentStage_.name );
				}
			}
			else
			{
				ChangeDirection( );
				if (DEBUG_PROOFENGINE)
				{
					Debug.Log( "ChangeToFollowingStage(" + b.name + " ) has no following stage, so changed direction instead" );
				}
				if (loop_)
				{
					Resume( );
				}
				else
				{
					Pause( );
				}
			}
		}

		public bool ChangeDirection()
		{
			bool result = false;
			if (currentStage_ != null)
			{
				if (currentStage_.ChangeDirection( ))
				{
					result = true;
					if (onDirectionChangedAction != null)
					{
						onDirectionChangedAction( currentStage_.direction );
					}
				}
			}
			return result;
		}

		#endregion Process

	}
}

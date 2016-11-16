using UnityEngine;
using System.Collections;
using RJWard.Core.UI.Extensions;

public class ProofTextPanel : MonoBehaviour
{
	static public bool DEBUG_PROOFTEXTPANEL = true;

	#region inspector hooks

	public UnityEngine.UI.Text proofText;
	public RectTransform timerImageRT;
	public UnityEngine.UI.Image timerImage;

	#endregion inspector hooks

	#region inspector data

	public Color stoppedColour = Color.red;
	public Color runningColour = Color.green;

	#endregion inspector data

	#region private data 

	private float timeTillClose_ = -1f;
	private float timerTotal = 0f;

	private System.Action onCloseAction;

	private Vector2 sizeDeltaRange_ = Vector2.zero;

	private bool timerRunning_ = true;

	#endregion private data

	#region Flow

	private void Awake()
	{
		sizeDeltaRange_.x = timerImageRT.sizeDelta.y;
		sizeDeltaRange_.y = sizeDeltaRange_.x - timerImageRT.GetHeight( );
		if (DEBUG_PROOFTEXTPANEL)
		{
			Debug.Log( "sdr = " + sizeDeltaRange_ );
		}
	}
	
	#endregion Flow

	#region interface

	public void Close()
	{
		if (onCloseAction != null)
		{
			onCloseAction( );
			onCloseAction = null;
		}
		gameObject.SetActive( false );
	}

	#endregion interface

	#region Process

	private void ResetTimer(float t)
	{
		timerTotal = t;
		timeTillClose_ = t;
		SetTimerImage( );
	}

	private void StopTimer()
	{
		timerRunning_ = false;
		timerImage.color = stoppedColour;
		SetTimerImage( );
	}

	private void StartTimer( )
	{
		timerImage.color = runningColour;
		timerRunning_ = true;
		SetTimerImage( );
	}

	private void Start()
	{
		ResetTimer( 10f );
		StartTimer( );
	}

	private void Update()
	{
		if (timerTotal > 0f )
		{
            if (timeTillClose_ >= 0f)
			{
				if (timerRunning_)
				{
					timeTillClose_ -= Time.deltaTime;
					if (timeTillClose_ < 0f)
					{
						Close( );
					}
					else
					{
						SetTimerImage( );
					}
				}
			}
        }
	}

	private void SetTimerImage()
	{
		if (timerTotal > 0 && timeTillClose_ > 0f)
		{
			timerImageRT.sizeDelta = new Vector2( timerImageRT.sizeDelta.x, Mathf.Lerp( sizeDeltaRange_.y, sizeDeltaRange_.x, timeTillClose_ / timerTotal ) );
		}
		else
		{
			timerImageRT.sizeDelta = new Vector2(timerImageRT.sizeDelta.x, sizeDeltaRange_.x);
		}
	}

	#endregion Process


	#region Handlers

	public void OnOkButtonClicked()
	{
		Close( );
	}

	public void OnTimerButtonClicked()
	{
		if (timerRunning_)
		{
			ResetTimer( timerTotal);
			StopTimer( );
		}
		else 
		{
			if (timerTotal > 0f)
			{
				StartTimer( );
			}
		}
	}

	#endregion Handlers

}

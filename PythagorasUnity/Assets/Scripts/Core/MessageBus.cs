using UnityEngine;
using System;
using System.Collections;

public partial class MessageBus : MonoBehaviour
{
	private static MessageBus _instance;

	public static MessageBus instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("MessageBus").AddComponent<MessageBus>();
			}			
			return _instance;
		}
	}

	public static bool exists
	{
		get { return _instance != null;  }
	}

}


/*
Application-specific file 

	public partial class MessageBus
	{
		public System.Action< Type > exampleAction;
		public void sendExampleAction(Type t)
		{
			if (exampleAction != null)
			{
				exampleAction( t );
			}
			else
			{
				Debug.LogWarning( "No exampleAction" );
			}
		}
	}
	public void clear()
	{
		exampleAction -= null;
	}

*/

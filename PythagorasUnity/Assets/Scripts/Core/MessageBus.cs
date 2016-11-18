using UnityEngine;
using System;
using System.Collections;

public partial class MessageBus : RJWard.Core.Singleton.SingletonApplicationLifetimeLazy< MessageBus >
{

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

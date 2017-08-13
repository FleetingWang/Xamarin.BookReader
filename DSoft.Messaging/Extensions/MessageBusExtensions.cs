using System;

namespace DSoft.Messaging.Extensions
{
	/// <summary>
	/// MessageBus object extensions
	/// </summary>
	public static class MessageBusExtensions
	{
		/// <summary>
		/// Posts the event.
		/// </summary>
		/// <param name="Sender">Sender.</param>
		/// <param name="EventId">Event Id</param>
		public static void PostEvent (this object Sender, String EventId)
		{
			Sender.PostEvent (EventId, null);
		}

		/// <summary>
		/// Posts the event.
		/// </summary>
		/// <param name="Sender">Sender.</param>
		/// <param name="EventId">Event Id</param>
		/// <param name="Data">Additonal data</param>
		public static void PostEvent (this object Sender, String EventId, object[] Data)
		{
			MessageBus.Default.Post (EventId, Sender, Data);
		}
	}
}


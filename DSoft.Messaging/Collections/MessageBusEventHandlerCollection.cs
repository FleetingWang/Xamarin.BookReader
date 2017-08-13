using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace DSoft.Messaging.Collections
{
	/// <summary>
	/// Collection of messagebuseventhandlers
	/// </summary>
	internal class MessageBusEventHandlerCollection : Collection<MessageBusEventHandler>
	{
		#region Methods

		/// <summary>
		/// Handlers for event.
		/// </summary>
		/// <param name="EventId">The event identifier.</param>
		/// <returns></returns>
		internal MessageBusEventHandler[] HandlersForEvent (String EventId)
		{
			var results = from item in this.Items
			              where !String.IsNullOrWhiteSpace (item.EventId)
			              where item.EventId.ToLower ().Equals (EventId.ToLower ())
			              where item.EventAction != null
			              select item;

			var array = results.ToArray ();
			return array;
		}

		/// <summary>
		/// Handlerses for event type
		/// </summary>
		/// <returns>The for event.</returns>
		/// <param name="EventType">Event type.</param>
		internal MessageBusEventHandler[] HandlersForEvent (Type EventType)
		{
			var results = from item in this.Items
			              where item is TypedMessageBusEventHandler
			              where item.EventAction != null
			              select item;

			var list = new List<MessageBusEventHandler> ();

			foreach (TypedMessageBusEventHandler item in results.ToArray())
			{
				if (item.EventType != null && item.EventType.Equals (EventType))
				{
					list.Add (item);
				}
			}

			return list.ToArray ();
		}

		/// <summary>
		/// Returns the event handlers for the specified Generic MessageBusEvent Type 
		/// </summary>
		/// <returns>The for event.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		internal MessageBusEventHandler[] HandlersForEvent<T> () where T : MessageBusEvent
		{
			return HandlersForEvent (typeof(T));
		}

		#endregion
	}
}


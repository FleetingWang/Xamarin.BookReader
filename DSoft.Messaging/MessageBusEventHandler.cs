using System;

namespace DSoft.Messaging
{
	/// <summary>
	/// Message bus event handler.
	/// </summary>
	public class MessageBusEventHandler
	{
		#region Properties

		/// <summary>
		/// Event Id
		/// </summary>
		public String EventId { get; set; }

		/// <summary>
		/// Action to perform on event
		/// </summary>
		public Action<object, MessageBusEvent> EventAction { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.MessageBusEventHandler"/> class.
		/// </summary>
		public MessageBusEventHandler ()
		{
			EventId = String.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.MessageBusEventHandler"/> class.
		/// </summary>
		/// <param name="EventId">Event identifier.</param>
		/// <param name="Action">Action.</param>
		public MessageBusEventHandler (String EventId, Action<object, MessageBusEvent> Action)
		{
			this.EventId = EventId;
			this.EventAction = Action;
		}

		#endregion
	}

	/// <summary>
	/// Typed message bus event handler.
	/// </summary>
	internal class TypedMessageBusEventHandler : MessageBusEventHandler
	{
		#region Properties

		/// <summary>
		/// Gets or sets the type of the event.
		/// </summary>
		/// <value>The type of the event.</value>
		internal Type EventType { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.TypedMessageBusEventHandler"/> class.
		/// </summary>
		internal TypedMessageBusEventHandler ()
		{
			
		}

		#endregion
	}
}


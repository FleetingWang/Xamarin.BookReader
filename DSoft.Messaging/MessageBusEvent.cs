using System;

namespace DSoft.Messaging
{
	/// <summary>
	/// Message bus event class
	/// </summary>
	public abstract class MessageBusEvent
	{
		#region Properties

		/// <summary>
		/// Gets or sets the event identifier.
		/// </summary>
		/// <value>The event identifier.</value>
		public abstract String EventId { get; }

		/// <summary>
		/// Sender of the event
		/// </summary>
		public object Sender { get; set; }

		/// <summary>
		/// Data to pass with the event
		/// </summary>
		public object[] Data { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.MessageBusEvent"/> class.
		/// </summary>
		public MessageBusEvent ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.MessageBusEvent"/> class.
		/// </summary>
		/// <param name="Sender">Sender.</param>
		/// <param name="Data">Data.</param>
		public MessageBusEvent (object Sender, object[] Data)
		{
			this.Sender = Sender;
			this.Data = Data;
		}

		#endregion
	}

	/// <summary>
	/// Standard MessageBusEvent class
	/// </summary>
	public sealed class CoreMessageBusEvent : MessageBusEvent
	{
		#region Fields

		private String mEventID;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the event identifier. Will generate a new Guid based Id if not set
		/// </summary>
		/// <value>The event identifier.</value>
		public override String EventId { 
			get
			{
				if (String.IsNullOrWhiteSpace (mEventID))
				{
					mEventID = Guid.NewGuid ().ToString ();
				}

				return mEventID;
			}

		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.CoreMessageBusEvent"/> class.
		/// </summary>
		public CoreMessageBusEvent () : base ()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.CoreMessageBusEvent"/> class.
		/// </summary>
		/// <param name="EventID">Event Identifier.</param>
		public CoreMessageBusEvent (String EventID) 
			: this ()
		{
			mEventID = EventID;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DSoft.Messaging.CoreMessageBusEvent"/> class.
		/// </summary>
		/// <param name="Sender">Sender.</param>
		/// <param name="EventID">Event I.</param>
		/// <param name="Data">Data.</param>
		public CoreMessageBusEvent (object Sender, String EventID, object[] Data) 
			: base (Sender, Data)
		{
			mEventID = EventID;
		}

		#endregion
	}
}


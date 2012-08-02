using System;
using System.Collections.Generic;
using System.Linq;
using ATTi.Core.Contracts;
using ATTi.TMail.Model;
using System.Text.RegularExpressions;

namespace ATTi.TMail.Common
{
	public static class ModelExtensions
	{
		/// <summary>
		/// Adds the note to a mail tracking instance.
		/// </summary>
		/// <param name="app">The tracking instance.</param>
		/// <param name="env">The note.</param>
		/// <exception cref="InvalidOperationException">thrown if the note belongs to another tracking instance</exception>
		public static void AddNote(this MailTracking tracking, TrackingNote note)
		{
			Require.IsNotNull("tracking", tracking);
			Require.IsNotNull("note", note);

			if (note.Tracking != null && note.Tracking.ID != tracking.ID)
				throw new InvalidOperationException("Note already belongs to another tracking instance.");

			// If tracking.Notes is already a list just use it and add the new item...
			List<TrackingNote> notes = tracking.Notes as List<TrackingNote>;
			// otherwise create a new list to work with.
			if (notes == null)
			{
				notes = new List<TrackingNote>();
			}

			// Ensure the note reflects that it belongs to the tracking instance...
			note.Tracking = tracking;
			notes.Add(note);

			// Always reassign in case the concrete class has a special 
			// private store.
			tracking.Notes = notes;
		}

		static readonly Regex __emailRegex = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			
		public static void EnsureRecipientListIsValid(this Recipient[] list)
		{
			int fieldCount = 0;
			int i = 0;
			foreach (var r in list)
			{
				// Take the fieldcount from the first recipient...
				if (i++ == 0 && r.MergeData != null)
					fieldCount = r.MergeData.Length;

				if (String.IsNullOrEmpty(r.Email))
					throw new InvariantContractException(String.Concat("Recipient #", i, " must have an email address"));
				if (!__emailRegex.IsMatch(r.Email))
					throw new InvariantContractException(String.Concat("Recipient #", i, " appears to contain an invalid email address: ", r.Email));

				if (fieldCount > 0 && (r.MergeData == null || r.MergeData.Length != fieldCount))
					throw new InvariantContractException(String.Concat("Recipient #", i, " appears to contain an invalid number of data values"));
			}			
		}

	}
}

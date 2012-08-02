using ATTi.TMail.Model;
using System;
using System.Collections.Generic;

namespace ATTi.TMail.Common
{
	/// <summary>
	/// TMailService interface.
	/// </summary>
	public interface ITMailService: IDisposable
	{
		void ValidateApplicationAndEnvironment(string app, string env);				
		
		/// <summary>
		/// Determines whether a templates exists.
		/// </summary>
		/// <param name="app">The app.</param>
		/// <param name="env">The env.</param>
		/// <param name="template">The template.</param>
		/// <returns></returns>
		bool TemplateExists(string app, string env, string template);
		
		void CreateMailing(Guid id, string app, string env, string template, Recipient[] recipients);
		MailTracking GetMailingStatus(string app, string env, Guid mailingID);		
		
	}

}

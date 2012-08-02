namespace ATTi.TMailAgent.WindowsService
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using ATTi.TMail.Service.Implementation;
	using ATTi.Core;

	partial class TMailAgentService
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;
		private TMailService _agent = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				Util.Dispose(ref _agent);
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = String.Concat("TMail Agent v", Assembly.GetExecutingAssembly().GetName().Version);
			this._agent = null;
		}

		#endregion
	}
}

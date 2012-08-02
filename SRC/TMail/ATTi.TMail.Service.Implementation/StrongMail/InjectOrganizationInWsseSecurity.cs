using System;
using System.Collections;
using System.Xml;
using Microsoft.Web.Services2;

namespace ATTi.TMail.Service.Implementation.StrongMail
{
	internal class InjectOrganizationInWsseSecurity : SoapOutputFilter
	{
		private readonly string _ns = "http://www.strongmail.com/services/2009/03/02/schema";

		public InjectOrganizationInWsseSecurity(string org)
		{
			this.OrganizationName = org;
		}

		public string OrganizationName { get; set; }
		public string SubOrganizationID { get; set; }

		public override void ProcessMessage(SoapEnvelope envelope)
		{
			var orgElm = envelope.CreateElement("OrganizationToken", _ns);

			orgElm.AppendChild(envelope.CreateElement("organizationName", _ns))
				.AppendChild(envelope.CreateTextNode(this.OrganizationName));

			if (!String.IsNullOrEmpty(SubOrganizationID))
			{
				var subElm = orgElm.AppendChild(envelope.CreateElement("subOrganizationId", _ns));

				subElm.AppendChild(envelope.CreateElement("id", _ns))
					.AppendChild(envelope.CreateTextNode(this.SubOrganizationID));
			}

			AppendToWsseSecurity(envelope, orgElm);
		}

		private static void AppendToWsseSecurity(SoapEnvelope envelope, XmlElement elm)
		{
			IEnumerator it = envelope.Header.GetEnumerator();
			while (it.MoveNext())
			{
				XmlElement el = (XmlElement)it.Current;

				if (el.Name.Equals("wsse:Security"))
				{
					el.AppendChild(elm);

					// Remove Timestamp; CXF doesn't like it...
					XmlNodeList timestamp = el.GetElementsByTagName("wsu:Timestamp");
					el.RemoveChild(timestamp.Item(0));
					break;
				}
			}
		}
	}
}

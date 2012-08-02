using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Services.Protocols;
using ATTi.TMail.Service.Implementation.Configuration;
using ATTi.TMail.Service.Implementation.StrongMail;

namespace ATTi.TMail.StrongMail
{
	public static class MailingServiceExtensions
	{
		static readonly int CDefaultPageSizeForListRequest = 50;

		public static void ChangeOrganization(this MailingService svc, string orgName, string subOrgId)
		{
			InjectOrganizationInWsseSecurity filter = svc.Pipeline.OutputFilters[0] as InjectOrganizationInWsseSecurity;
			if (filter == null)
				throw new InvalidOperationException("MailingService must have an InjectOrganizationInWsseSecurity in position 0 of the pipeline output filters");
			filter.OrganizationName = orgName;
			filter.SubOrganizationID = subOrgId;
		}

		public static string GetCurrentOrganization(this MailingService svc)
		{
			InjectOrganizationInWsseSecurity filter = svc.Pipeline.OutputFilters[0] as InjectOrganizationInWsseSecurity;
			if (filter == null)
				throw new InvalidOperationException("MailingService must have an InjectOrganizationInWsseSecurity in position 0 of the pipeline output filters");
			return String.Concat(filter.OrganizationName, (String.IsNullOrEmpty(filter.SubOrganizationID)) ? String.Empty : String.Concat(":", filter.SubOrganizationID));
		}

		public static T[] GetObjectsAs<T>(this MailingService svc, ObjectId[] idlist)
			where T : BaseObject
		{
			List<T> result = new List<T>();
			var batchResp = svc.get(idlist);
			int i = 0;
			foreach (bool ret in batchResp.success)
			{
				if (ret)
				{
					result.Add((T)batchResp.getResponse[i].baseObject);
				}
				i++;
			}
			return result.ToArray();
		}

		public static IEnumerable<Organization> TakeChildrenOf(this IEnumerable<Organization> orgs, Organization parent)
		{
			return from o in orgs
						 where o.parentId != null && o.parentId.id == parent.objectId.id
						 orderby o.name
						 select o;
		}

		public static Organization TakeParentOf(IEnumerable<Organization> orgs, Organization child)
		{
			if (child.parentId == null) return default(Organization);

			return (from o in orgs
							where o.objectId.id == child.parentId.id
							select o).SingleOrDefault();
		}

		public static IEnumerable<Organization> AllOrganizations(this MailingService svc)
		{
			var list = new List<ObjectId>();
			var page = 0;

			while (true)
			{
				var req = new ListRequest();
				var filter = new OrganizationFilter();

				filter.recordsPerPage = CDefaultPageSizeForListRequest;
				filter.recordsPerPageSpecified = true;
				filter.pageNumber = page;
				filter.pageNumberSpecified = true;

				req.filter = filter;

				var records = svc.list(req);
				list.AddRange(records.objectId);

				// As long as we get a full page of data
				// request the next page...
				if (records.objectId.Length < CDefaultPageSizeForListRequest)
					break;

				page++;
			}

			return svc.GetObjectsAs<Organization>(list.ToArray());
		}

		public static Exception TranslateException(this MailingService svc, Exception e)
		{			
			if (typeof(SoapHeaderException).IsInstanceOfType(e))
			{
				if (e.Message.StartsWith("Unable to retrieve user from database; nested exception is com.sm.organization.business.exception.EmployeeNotFoundException:"))
				{
					return new SecurityException(String.Concat("Access not allowed: user = '", StrongMailConfigurationSection.Instance.UserName,
						"', organization = '", svc.GetCurrentOrganization(), "'."),
						e);
				}
				else
				{

				}
			}
			return e;
		}
	}
}

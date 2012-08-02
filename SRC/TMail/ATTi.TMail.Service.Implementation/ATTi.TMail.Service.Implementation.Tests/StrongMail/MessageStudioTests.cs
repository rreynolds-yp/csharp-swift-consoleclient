using System;
using System.Linq;
using ATTi.TMail.Service.Implementation.Configuration;
using ATTi.TMail.StrongMail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace ATTi.TMail.Service.Implementation.Tests.StrongMail
{
	/// <summary>
	/// MessageStudioTests
	/// </summary>
	[TestClass]
	public class MessageStudioTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void ListOrganizations()
		{
			try
			{
				var config = StrongMailConfigurationSection.Instance;
				using (var studio = config.CreateMailingService())
				{
					try
					{
						var levels = 0;
						var orgs = studio.AllOrganizations();
						Assert.IsNotNull(orgs);
						Assert.IsTrue(orgs.Count() > 0);

						var parent = orgs.SingleOrDefault(o => o.name == "ATTI");
						Assert.IsNotNull(parent);

						RecursiveForEach(studio, levels, parent, orgs, (s, i, o) =>
							{
								string indent = (i > 0) ? new String('\t', i) : String.Empty;
								Console.WriteLine(String.Concat(i, o.name, " { id=", o.objectId.id,
									", parentid=", (o.parentId != null) ? o.parentId.id : "null",
									", description='", o.description, "' }"));
							});
					}
					catch (Exception e)
					{
						throw studio.TranslateException(e);
					}
				}
			}
			catch (Exception ex)
			{ /* this catch is here to inspect the exception during debugging */
				throw ex;
			}
		}

		private void RecursiveForEach(MailingService svc, int depth, Organization o, IEnumerable<Organization> orgs, Action<MailingService, int, Organization> action)
		{
			Assert.IsNotNull(o);

			action(svc, depth, o);
			foreach (var child in orgs.TakeChildrenOf(o))
			{
				RecursiveForEach(svc, depth + 1, child, orgs, action);
			}
		}

		[TestMethod]
		public void GetOrganizationByName()
		{
			var args = new
			{
				Root = "ATTI",
				App = "tmail",
				Env = "test"
			};

			var config = StrongMailConfigurationSection.Instance;
			using (var studio = config.CreateMailingService())
			{
				try
				{
					var orgs = studio.AllOrganizations();
					Assert.IsNotNull(orgs);
					Assert.IsTrue(orgs.Count() > 0);

					var root = orgs.SingleOrDefault(o => o.name == args.Root);
					Assert.IsNotNull(root);
					Assert.AreEqual(args.Root, root.name);

					var app = orgs.TakeChildrenOf(root).SingleOrDefault(o => o.name == args.App);
					Assert.IsNotNull(app);
					Assert.AreEqual(args.App, app.name);

					var env = orgs.TakeChildrenOf(app).SingleOrDefault(o => o.name == args.Env);
					Assert.IsNotNull(env);
					Assert.AreEqual(args.Env, env.name);
				}
				catch (Exception e)
				{
					throw studio.TranslateException(e);
				}
			}
		}

		[TestMethod]
		public void ListMailings()
		{
			var config = StrongMailConfigurationSection.Instance;
			using (var studio = config.CreateMailingService())
			{
				try
				{
					var orgs = studio.AllOrganizations();
					Assert.IsNotNull(orgs);
					Assert.IsTrue(orgs.Count() > 0);

					var parent = orgs.SingleOrDefault(o => o.name == "ATTI");
					Assert.IsNotNull(parent);

					RecursiveForEach(studio, 0, parent, orgs, (s, i, o) =>
					{
						string indent = (i > 0) ? new String('\t', i) : String.Empty;
						Console.WriteLine(String.Concat(i, o.name, " { id=", o.objectId.id, ", parentid=", (o.parentId != null) ? o.parentId.id : "null", ", description='", o.description, "' }"));

						// Change the security context to the environment...
						s.ChangeOrganization(o.name, o.objectId.id);

						var mailingReq = new ListRequest();
						var mailingFilter = new MailingFilter();

						//mailingFilter.typeCondition = new ArrayStringFilterCondition();
						//mailingFilter.typeCondition.@operator = FilterArrayOperator.IN;
						//mailingFilter.typeCondition.value = new string[] { MailingType.RECURRING.ToString(), MailingType.TRANSACTIONAL.ToString() };

						//mailingFilter.orderBy = new MailingOrderBy[] { MailingOrderBy.NAME };
						//mailingFilter.statusCondition = new ArrayStringFilterCondition();
						//mailingFilter.statusCondition.@operator = FilterArrayOperator.IN;
						//mailingFilter.statusCondition.value = new string[] { MailingStatus.ACTIVE.ToString() };

						mailingReq.filter = mailingFilter;

						var mlist = studio.list(mailingReq);
						var mailings = studio.GetObjectsAs<Mailing>(mlist.objectId);
						foreach (var m in mailings)
						{
							Assert.AreEqual(o.objectId.id, m.organizationId.id);

							Console.WriteLine(String.Concat("\t\tMailing: ", m.name, " { id = ", m.objectId.id,
								", orgId = ", (m.organizationId != null) ? m.organizationId.id : "",
								", parentId = ", (m.parentId != null) ? m.parentId.id : "",
								" }"));
						}
					});
				}
				catch (SoapHeaderException e)
				{
					throw studio.TranslateException(e);
				}
			}
		}
	}
}

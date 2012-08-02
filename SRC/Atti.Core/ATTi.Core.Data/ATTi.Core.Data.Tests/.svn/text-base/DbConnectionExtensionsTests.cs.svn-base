using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using ATTi.Core.Parallel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATTi.Core.Data.Tests
{
	[TestClass]
	public class DbConnectionExtensionsTests
	{
		public DbConnectionExtensionsTests() { }
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void CreateCommand_with_CommandText_CommandType_Timeout()
		{
			var spec = new { CmdText = "SELECT Top 25 System.ItemPathDisplay FROM SYSTEMINDEX", CmdType = CommandType.Text, CmdTimeout = 1000 };

			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				Assert.IsNotNull(cn, "There should be a connection in the ConnectionStrings configuration section with the name 'windows-search'");

				cn.Open();
				DbCommand cmd = scope.Add(cn.CreateCommand(spec.CmdText, spec.CmdType, spec.CmdTimeout));

				Assert.AreEqual(spec.CmdText, cmd.CommandText);
				Assert.AreEqual(spec.CmdType, cmd.CommandType);
				Assert.AreEqual(spec.CmdTimeout, cmd.CommandTimeout);
			}
		}
		[TestMethod]
		public void CreateCommand_with_CommandText_CommandType()
		{
			var spec = new { CmdText = "SELECT Top 25 System.ItemPathDisplay FROM SYSTEMINDEX", CmdType = CommandType.Text };

			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				Assert.IsNotNull(cn, "There should be a connection in the ConnectionStrings configuration section with the name 'windows-search'");

				cn.Open();
				DbCommand cmd = scope.Add(cn.CreateCommand(spec.CmdText, spec.CmdType));

				Assert.AreEqual(spec.CmdText, cmd.CommandText);
				Assert.AreEqual(spec.CmdType, cmd.CommandType);
			}
		}
		[TestMethod]
		public void CreateCommand_with_CommandText()
		{
			var spec = new { CmdText = "SELECT Top 25 System.ItemPathDisplay FROM SYSTEMINDEX" };

			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				Assert.IsNotNull(cn, "There should be a connection in the ConnectionStrings configuration section with the name 'windows-search'");

				cn.Open();
				DbCommand cmd = scope.Add(cn.CreateCommand(spec.CmdText));

				Assert.AreEqual(spec.CmdText, cmd.CommandText);
			}
		}

		[TestMethod]
		public void ImmediateExecuteNonQuery_with_CommandText_CommandType_Timeout()
		{
			var spec = new { CmdText = "SELECT Top 25 System.ItemPathDisplay FROM SYSTEMINDEX", CmdType = CommandType.Text, CmdTimeout = 1000 };

			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				Assert.IsNotNull(cn, "There should be a connection in the ConnectionStrings configuration section with the name 'windows-search'");

				cn.Open();
				int result = cn.ImmediateExecuteNonQuery(spec.CmdText, spec.CmdType, spec.CmdTimeout);

				// TODO: Revise this test so it gets a predictable result (other than the default 0) upon success.
				Assert.AreEqual(0, result);
			}
		}

		[TestMethod]
		public void ImmediateExecuteSingle_with_CommandText_CommandType_Timeout()
		{
			var spec = new { CmdText = "SELECT Top 1 System.ItemPathDisplay FROM SYSTEMINDEX", CmdType = CommandType.Text, CmdTimeout = 1000 };

			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				Assert.IsNotNull(cn, "There should be a connection in the ConnectionStrings configuration section with the name 'windows-search'");

				cn.Open();
				string result = cn.ImmediateExecuteSingle<string>(spec.CmdText, spec.CmdType, spec.CmdTimeout, (d) => { return d.GetString(0); });

				// TODO: Revise this test so it gets a predictable result (other than the default 0) upon success.
				Assert.IsFalse(String.IsNullOrEmpty(result));
			}
		}

		[TestMethod]
		public void ImmediateExecuteAndTransform()
		{
			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				cn.Open();

				var items = cn.ImmediateExecuteAndTransform<string>(scope, "SELECT Top 25 System.ItemPathDisplay FROM SYSTEMINDEX", (d) => { return d.GetString(0); });
				foreach (string item in items)
				{
					Assert.IsFalse(String.IsNullOrEmpty(item));
				}
			}
		}

		class IndexItemDTO
		{
			public string ItemPathDisplay { get; set; }
			public string ItemType { get; set; }
		}
		[TestMethod]
		public void ImmediateExecuteAndTransform_Transform()
		{
			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				cn.Open();

				var items = cn.ImmediateExecuteAndTransform<IndexItemDTO>(scope
					, "SELECT Top 25 System.ItemPathDisplay, System.ItemType FROM SYSTEMINDEX"
					, (d) =>
					{
						return new IndexItemDTO
						{
							ItemPathDisplay = d.GetString(0),
							ItemType = d.GetString(1)
						};
					}
					);
				foreach (IndexItemDTO item in items)
				{
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemPathDisplay));
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemType));
				}
			}
		}

		[TestMethod]
		public void ImmediateExecuteEnumerable()
		{
			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				cn.Open();

				var items = from reader in cn.ImmediateExecuteEnumerable(scope, @"SELECT TOP 25 System.ItemPathDisplay, System.ItemType FROM SYSTEMINDEX")
										select new
										{
											ItemPathDisplay = reader.GetString(0),
											ItemType = reader.GetString(1)
										};

				foreach (var item in items)
				{
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemPathDisplay));
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemType));
				}
			}
		}

		[TestMethod]
		public void AsyncExecuteAndTransform()
		{
			using (CleanupScope scope = new CleanupScope())
			{
				DbConnection cn = scope.Add(DbExtensions.CreateConnection("windows-search"));
				cn.Open();

				IFuture<IEnumerable<IndexItemDTO>> futr = cn.AsyncExecuteAndTransform<IndexItemDTO>(scope
					, @"SELECT Top 250 System.ItemPathDisplay, System.ItemType FROM SYSTEMINDEX WHERE System.ItemType = '.lnk'"
					, (d) =>
					{
						return new IndexItemDTO
						{
							ItemPathDisplay = d.GetString(0),
							ItemType = d.GetString(1)
						};
					}
					, null
					);

				Stopwatch clock = new Stopwatch();
				clock.Start();

				IEnumerable<IndexItemDTO> items = futr.Value;
				string elapsed = clock.Elapsed.ToString();
				foreach (var item in items)
				{
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemPathDisplay));
					Assert.IsFalse(String.IsNullOrEmpty(item.ItemType));
				}
			}
		}
	}
}

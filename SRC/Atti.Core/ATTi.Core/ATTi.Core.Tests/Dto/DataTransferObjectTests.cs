using System;
using System.Xml.Linq;
using ATTi.Core.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ATTi.Core.Tests.Dto
{
	[TestClass]
	public class DataTransferObjectTests
	{
		[Dto]
		public interface ITestPerson
		{
			string FirstName { get; set; }
			string LastName { get; set; }
		}

		[Dto]
		public interface ITestPerson2 : ITestPerson
		{
			string MiddleName { get; set; }
			DateTime DateOfBirth { get; set; }
			void DoNothing();
		}

		[TestMethod]
		public void LoadsAnInstance()
		{
			var data = new
			{
				FirstName = "John",
				LastName = "Henry",
			};

			ITestPerson copy = DataTransfer.Load<ITestPerson>(p =>
			{
				p.FirstName = data.FirstName;
				p.LastName = data.LastName;
			});

			Assert.IsNotNull(copy);
			Assert.AreEqual(data.FirstName, copy.FirstName);
			Assert.AreEqual(data.LastName, copy.LastName);
		}

		[TestMethod]
		public void CopyConstructDtoFromXml()
		{
			var data = new
			{
				FirstName = "John",
				LastName = "Henry",
				MiddleName = "not-given",
				DateOfBirth = new DateTime(1997, 6, 12)
			};

			XElement item =
				new XElement("TestPerson",
					new XElement("FirstName", data.FirstName),
					new XElement("MiddleName", data.MiddleName),
					new XElement("LastName", data.LastName),
					new XElement("DateOfBirth", data.DateOfBirth)
					);

			ITestPerson2 copy = DataTransfer.FromXml<ITestPerson2>(item);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.FirstName, copy.FirstName);
			Assert.AreEqual(data.LastName, copy.LastName);
			Assert.AreEqual(data.MiddleName, copy.MiddleName);
			Assert.AreEqual(data.DateOfBirth, copy.DateOfBirth);
		}

		[TestMethod]
		public void CopyConstructDtoFromXmlWithCapture()
		{
			var data = new
			{
				FirstName = "John",
				LastName = "Henry",
				MiddleName = "not-given",
				DateOfBirth = new DateTime(1997, 6, 12)
			};

			XElement item =
				new XElement("TestPerson",
					new XElement("FirstName", data.FirstName),
					new XElement("MiddleName", data.MiddleName),
					new XElement("LastName", data.LastName),
					new XElement("DateOfBirth", data.DateOfBirth)
					);

			ITestPerson copy = DataTransfer.FromXmlWithCaptureExtraData<ITestPerson>(item);
			Assert.IsNotNull(copy);
			Assert.AreEqual(data.FirstName, copy.FirstName);
			Assert.AreEqual(data.LastName, copy.LastName);
			Assert.IsTrue(copy.HasExtraData("MiddleName"));
			Assert.AreEqual(data.MiddleName, copy.ExpectExtraData<string>("MiddleName"));
			Assert.IsTrue(copy.HasExtraData("DateOfBirth"));
			Assert.AreEqual(data.DateOfBirth, copy.ExpectExtraData<DateTime>("DateOfBirth"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void CopyConstructDtoFromXmlWithCapture_AttemptGetNonExistentExtraData()
		{
			var data = new
			{
				FirstName = "John",
				LastName = "Henry",
				MiddleName = "not-given",
				DateOfBirth = new DateTime(1997, 6, 12)
			};

			XElement item =
				new XElement("TestPerson",
					new XElement("FirstName", data.FirstName),
					new XElement("MiddleName", data.MiddleName),
					new XElement("LastName", data.LastName),
					new XElement("DateOfBirth", data.DateOfBirth)
					);

			ITestPerson copy = DataTransfer.FromXmlWithCaptureExtraData<ITestPerson>(item);

			Assert.IsNotNull(copy);
			Assert.AreEqual(data.FirstName, copy.FirstName);
			Assert.AreEqual(data.LastName, copy.LastName);
			Assert.IsTrue(copy.HasExtraData("MiddleName"));
			Assert.AreEqual(data.MiddleName, copy.ExpectExtraData<string>("MiddleName"));
			Assert.IsTrue(copy.HasExtraData("DateOfBirth"));
			Assert.AreEqual(data.DateOfBirth, copy.ExpectExtraData<DateTime>("DateOfBirth"));
			Assert.IsFalse(copy.HasExtraData("NonExistentData"));
			Assert.AreEqual(data.DateOfBirth, copy.ExpectExtraData<int>("NonExistentData"));
		}

		[TestMethod]
		public void CopyConstructDtoFromJson()
		{
			var data = new
			{
				FirstName = "John",
				LastName = "Henry",
				MiddleName = "not-given",
				DateOfBirth = new DateTime(1997, 6, 12)
			};

			JObject item =
				new JObject(new JProperty("FirstName", data.FirstName),
					new JProperty("MiddleName", data.MiddleName),
					new JProperty("LastName", data.LastName),
					new JProperty("DateOfBirth", data.DateOfBirth)
					);


			ITestPerson2 copy = DataTransfer.FromJson<ITestPerson2>(item);

			Assert.IsNotNull(copy);
			Assert.AreEqual(data.FirstName, copy.FirstName);
			Assert.AreEqual(data.LastName, copy.LastName);
			Assert.AreEqual(data.MiddleName, copy.MiddleName);
			Assert.AreEqual(data.DateOfBirth, copy.DateOfBirth);
		}
	}
}

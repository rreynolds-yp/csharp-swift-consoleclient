using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMailRestService;
using TMailRestService.Controllers;

namespace TMailRestService.Tests.Controllers
{
	[TestClass]
	public class HomeControllerTest
	{
		[TestMethod]
		public void Index()
		{
			// Arrange
			HomeController controller = new HomeController();

			// Act
			var result = controller.Index();

			Assert.IsNotNull(result);
			Assert.IsTrue(typeof(JsonResult).IsInstanceOfType(result));

		}


	}

}

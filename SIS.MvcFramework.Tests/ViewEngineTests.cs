namespace SIS.MvcFramework.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class ViewEngineTests
    {
        [Theory]
        [InlineData("OnlyHtmlView")]
        [InlineData("LoopsView")]
        [InlineData("ViewModelView")]
        public void GetHtmlTest(string testName)
        {
            var viewModel = new TestViewModel()
            {
                Name = "Niki",
                Year = 2020,
                Numbers = new List<int> {1, 10, 100, 1000, 10_000}
            };

            var viewContent = File.ReadAllText($"./ViewTests/{testName}.html");
            var expectedResultContent = File.ReadAllText($"./ViewTests/{testName}Expected.html");

            IViewEngine viewEngine = new ViewEngine();
            var actualResult = viewEngine.GetHtml(viewContent, viewModel);

            Assert.Equal(expectedResultContent, actualResult);
        }
    }
}

namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.Text;

    public class ErrorView : IView
    {
        private readonly IEnumerable<string> _errors;

        public ErrorView(IEnumerable<string> errors)
        {
            this._errors = errors;
        }

        public string GetHtml(object model)
        {
            var html = new StringBuilder();

            html.AppendLine("<h1>View compilation errors:</h1>");
            html.AppendLine("<ul>");

            foreach (var error in this._errors)
            {
                html.Append($"<li>{error}</li>");
            }

            html.AppendLine("</ul>");

            return html.ToString();
        }
    }
}

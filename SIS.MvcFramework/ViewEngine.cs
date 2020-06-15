namespace SIS.MvcFramework
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class ViewEngine : IViewEngine
    {
        public string GetHtml(string templateHtml, object model, string user)
        {
            var methodCode = this.PrepareCSharpCode(templateHtml);
            var typeName = model?.GetType().FullName ?? "object";

            if (model?.GetType().IsGenericType == true)
            {
                typeName = model.GetType().Name
                    .Replace("`1", string.Empty) + "<" + model.GetType().GenericTypeArguments.First().Name +">";
            }

            var x = DateTime.UtcNow.Year;

            var code = @$"using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SIS.MvcFramework;

namespace AppViewNamespace
{{
    public class AppViewCode : IView
    {{
        public string GetHtml(object model, string user)
        {{
            var Model = model as {typeName};
            var User = user;
            var html = new StringBuilder();
            
            {methodCode}

            return html.ToString();
        }}
    }}
}}";
            var view = this.GetInstanceFromCode(code, model);
            return view?.GetHtml(model, user).Trim() ?? string.Empty;
        }

        private string PrepareCSharpCode(string templateHtml)
        {
            var regex = new Regex(@"[^\<\s\""]+", RegexOptions.Compiled);
            
            var supportedOperators = new[] { "if", "for", "foreach", "else" };

            var cSharpCode = new StringBuilder();
            var reader = new StringReader(templateHtml);

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (   line.TrimStart().StartsWith("{") 
                    || line.TrimStart().StartsWith("}"))
                {
                    cSharpCode.AppendLine(line);
                }
                else if(supportedOperators.Any(x => line.TrimStart().StartsWith( "@" + x)))
                {
                    var indexOfAt = line.IndexOf("@", StringComparison.InvariantCulture);
                    line = line.Remove(indexOfAt, 1);

                    cSharpCode.AppendLine(line);
                }
                else
                {
                    var currentCSharpLine = new StringBuilder("html.AppendLine(@\"");

                    while (line.Contains("@"))
                    {
                        var indexOfAt = line.IndexOf("@", StringComparison.InvariantCulture);
                        var before = line.Substring(0, indexOfAt);

                        currentCSharpLine.Append(before.Replace("\"", "\"\"") + "\" + ");

                        var cSharpCodeAndEndOfLine = line.Substring(indexOfAt + 1);
                        var cSharpExpression = regex.Match(cSharpCodeAndEndOfLine);
                        
                        currentCSharpLine.Append(cSharpExpression.Value + " + @\"");

                        line = cSharpCodeAndEndOfLine.Substring(cSharpExpression.Length);
                    }

                    currentCSharpLine.AppendLine(line.Replace("\"", "\"\"") + "\");");
                    cSharpCode.AppendLine(currentCSharpLine.ToString());
                }
            }

            return cSharpCode.ToString();
        }

        /// <summary>
        /// Create Instance of Type IView from string
        /// </summary>
        /// <param name="code">Valid C# code</param>
        /// <param name="model">Plain C# Class</param>
        /// <returns>Instance from type IView</returns>
        private IView GetInstanceFromCode(string code, object model)
        {
            var compilationContext = CSharpCompilation.Create("AppViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            if (model != null)
            {
                compilationContext =
                    compilationContext.AddReferences(
                        MetadataReference.CreateFromFile(model.GetType().Assembly.Location));
            }

            var libraries = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();

            foreach (var library in libraries)
            {
                compilationContext = compilationContext.AddReferences(
                    MetadataReference.CreateFromFile(Assembly.Load(library).Location));
            }

            compilationContext = compilationContext
                .AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(code));

            Assembly assembly = null;

            using (var memoryStream = new MemoryStream())
            {
                var compilationResult = compilationContext.Emit(memoryStream);

                if (!compilationResult.Success)
                {
                    return new ErrorView(compilationResult.Diagnostics
                        .Where(x => x.Severity == DiagnosticSeverity.Error)
                        .Select(x => x.GetMessage()));
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                var assemblyByteArray = memoryStream.ToArray();
                assembly = Assembly.Load(assemblyByteArray);
            }

            var type = assembly.GetType("AppViewNamespace.AppViewCode");
            var instance = Activator.CreateInstance(type) as IView;

            return instance;
        }
    }
}

using System;
using System.Linq;
using System.Web.Razor;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace ACTransit.Framework.Web.Razor
{
    public class Engine<T> : IDisposable where T:TemplateBase
    {
        public delegate void UpdateTemplateDelegate();
        public UpdateTemplateDelegate UpdateTemplate;

        private T _currentTemplate;
        private RazorTemplateEngine _engine;
        public Engine()
        {
            _engine = SetupRazorEngine();
        }

        public string CurrentTemplateText { get; private set; }

        public string LoadTemplate(string template)
        {
            return LoadTemplate(template, new[] { "System.dll" });
        }
        //Load the template. first step after initialization!
        public string LoadTemplate(string template, string[] referencedAssemblies)
        {
            CurrentTemplateText = template;
            CurrentTemplate = null;
            string generatedCode;

            // Generate code for the template
            GeneratorResults razorResult;            

            using (TextReader rdr = new StringReader(template))
            {
                razorResult = _engine.GenerateCode(rdr);
            }

            using (CSharpCodeProvider codeProvider = new CSharpCodeProvider())
            {
                // Generate the code and put it in the text box:
                using (StringWriter sw = new StringWriter())
                {
                    codeProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
                    generatedCode = sw.GetStringBuilder().ToString();
                }

                // Compile the generated code into an assembly            
                var cp = new CompilerParameters(new[] { GetType().Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\") });
                foreach (string reference in referencedAssemblies)
                    cp.ReferencedAssemblies.Add(reference.Replace("file:///", "").Replace("/", "\\"));


                cp.GenerateInMemory = true;


                //codeProvider.CompileAssemblyFromSource(cp,)

                CompilerResults results = codeProvider.CompileAssemblyFromDom(
                    cp,
                    razorResult.GeneratedCode);

                if (results.Errors.HasErrors)
                {
                    CompilerError err = results.Errors
                                               .OfType<CompilerError>()
                                               .Where(ce => !ce.IsWarning)
                                               .First();
                    throw new Exception(String.Format("Error Compiling Template: ({0}, {1}) {2}",
                                                  err.Line, err.Column, err.ErrorText));
                }
                else
                {
                    // Load the assembly
                    //Assembly asm = Assembly.LoadFrom(outputAssemblyName);
                    Assembly asm = results.CompiledAssembly;
                    if (asm == null)
                    {
                        throw new Exception("Error loading template assembly");
                    }
                    else
                    {
                        // Get the template type
                        Type typ = asm.GetType("RazorOutput.Template");
                        if (typ == null)
                        {
                            throw new Exception(string.Format("Could not find type RazorOutput.Template in assembly {0}", asm.FullName));
                        }
                        else
                        {
                            T newTemplate = Activator.CreateInstance(typ) as T;
                            if (newTemplate == null)
                            {
                                throw new Exception("Could not construct RazorOutput.Template or it does not inherit from TemplateBase");
                            }
                            else
                            {
                                CurrentTemplate = newTemplate;
                            }
                        }
                    }
                }
            }

            return generatedCode;
        }
        public string Execute()
        {
            string result;
            if (CurrentTemplate == null)
            {
                throw new Exception("No Template Loaded!");
            }
            else
            {
                CurrentTemplate.Execute();                
                result = CurrentTemplate.Buffer.ToString();
                CurrentTemplate.Buffer.Clear();
            }
            return result;
        }

        public string Execute(params object[] param)
        {
            string result;
            if (CurrentTemplate == null)
            {
                throw new Exception("No Template Loaded!");
            }
            else
            {
                CurrentTemplate.Execute(param);
                result = CurrentTemplate.Buffer.ToString();
                CurrentTemplate.Buffer.Clear();
            }
            return result;
        }
        public T CurrentTemplate
        {
            get
            {
                return _currentTemplate;
            }
            set
            {
                _currentTemplate = value;
                if (UpdateTemplate != null)
                    UpdateTemplate();
            }
        }

        private RazorTemplateEngine SetupRazorEngine()
        {
            // Set up the hosting environment

            // a. Use the C# language (you could detect this based on the file extension if you want to)
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());

            // b. Set the base class
            host.DefaultBaseClass = typeof(T).FullName;

            // c. Set the output namespace and type name
            host.DefaultNamespace = "RazorOutput";
            host.DefaultClassName = "Template";

            // d. Add default imports
            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.IO");
            
            // Create the template engine using this host
            return new RazorTemplateEngine(host);
        }


        public void Dispose()
        {            
        }
    }
}
        
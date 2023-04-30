using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Scripting.Hosting;
using static Community.CsharpSqlite.Sqlite3;
using System.Dynamic;
using Microsoft.Scripting;

namespace Tango.Workbench.Python
{
    /// <summary>
    /// Runs a Python script stored in another file
    /// </summary>
    [Job("Python")]
    public sealed partial class PythonJob : Job
    {
        public override ValueTask Run()
        {
            if(this.Filename is null) throw new WorkbenchException($"no script specified");

            var filename = MakeScriptFilename(this.Filename);

            var engine = IronPython.Hosting.Python.CreateEngine();
            UpdateSearchPaths(engine, filename);

            var scope = engine.CreateScope();
            scope.SetVariable("job", MakeEnvironment());

            var script = engine.CreateScriptSourceFromFile(filename);

            try
            {
                script.Execute(scope);
            }
            catch(SyntaxErrorException e)
            {
                Log.Error($"python script {filename} failed on line {e.Line} : {e.Message}");
                throw;
            }

            return default;
        }

        private dynamic MakeEnvironment()
        {
            dynamic environment = new ExpandoObject();
            environment.context = this.Context;

            environment.log = new PythonLogger(this.Log);
            environment.verboselog = new PythonLogger(this.VerboseLog);

            environment.settings = MakeSettings();

            return environment;
        }

        private dynamic MakeSettings()
        {
            dynamic settings = new ExpandoObject();
            var dictionary = (IDictionary<string, object?>)settings;

            foreach(var setting in this.Settings)
            {
                if(setting.Name is not null) dictionary[setting.Name] = setting.Value;
            }            

            return settings;
        }

        /// <summary>
        /// Updates the scripting host search path to have additional locations
        /// </summary>
        /// <param name="scriptEngine"></param>
        /// <param name="scriptFilename"></param>
        private void UpdateSearchPaths(ScriptEngine scriptEngine, string scriptFilename)
        {
            var searchPaths = scriptEngine.GetSearchPaths().ToList();
            searchPaths.Add(Path.GetDirectoryName(scriptFilename));
            searchPaths.AddRange(this.SearchPaths);
            scriptEngine.SetSearchPaths(searchPaths);
        }

        /// <summary>
        /// Works out where the Python file to run lives
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string MakeScriptFilename(string filename)
        {
            if(Path.IsPathRooted(filename)) return filename;

            return Path.GetFullPath(Path.Combine(this.Context.ScriptDirectory, filename));
        }

        /// <summary>
        ///  The location of the Python script to run
        /// </summary>
        public string? Filename{get; set;}

        /// <summary>
        /// Additional search paths for the Python environment
        /// </summary>
        public List<string> SearchPaths{get;} = new();

        /// <summary>
        /// Additional settings for the Python script
        /// </summary>
        public List<Setting> Settings{get; set;} = new();
    }
}

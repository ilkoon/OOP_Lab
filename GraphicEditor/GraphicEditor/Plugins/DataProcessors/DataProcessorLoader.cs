using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphicEditor.Plugins;

namespace GraphicEditor.Plugins.DataProcessors
{
    /// <summary>
    /// Loads and manages data processor plugins
    /// </summary>
    public class DataProcessorLoader
    {
        private readonly string _pluginsDirectory;
        private readonly List<IDataProcessorPlugin> _loadedProcessors;

        public IReadOnlyList<IDataProcessorPlugin> LoadedProcessors => _loadedProcessors.AsReadOnly();

        /// <summary>
        /// Event fired when a processor is loaded
        /// </summary>
        public event EventHandler<DataProcessorLoadEventArgs> ProcessorLoaded;

        /// <summary>
        /// Event fired when processor load fails
        /// </summary>
        public event EventHandler<DataProcessorLoadErrorEventArgs> ProcessorLoadError;

        public DataProcessorLoader(string pluginsDirectory = "Plugins")
        {
            _pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsDirectory);
            _loadedProcessors = new List<IDataProcessorPlugin>();
        }

        /// <summary>
        /// Loads all data processor plugins from the plugins directory
        /// </summary>
        public void LoadAllProcessors()
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
                return;
            }

            foreach (string dllPath in Directory.GetFiles(_pluginsDirectory, "*.dll"))
            {
                LoadProcessorsFromAssembly(dllPath);
            }
        }

        /// <summary>
        /// Loads processors from a specific assembly
        /// </summary>
        private void LoadProcessorsFromAssembly(string assemblyPath)
        {
            try
            {
                // Check for signature file (required for security)
                string signaturePath = Path.ChangeExtension(assemblyPath, ".sig");
                if (!File.Exists(signaturePath))
                {
                    OnProcessorLoadError(new DataProcessorLoadErrorEventArgs(assemblyPath, "Missing signature file (.sig)"));
                    return;
                }

                // Verify signature
                var signature = PluginSigningManager.LoadSignature(signaturePath);
                if (signature == null || !PluginSigningManager.VerifyPlugin(assemblyPath, signature))
                {
                    OnProcessorLoadError(new DataProcessorLoadErrorEventArgs(assemblyPath, "Signature verification failed"));
                    return;
                }

                // Load assembly
                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                // Find and create data processor plugins
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IDataProcessorPlugin).IsAssignableFrom(type) &&
                        !type.IsInterface &&
                        !type.IsAbstract)
                    {
                        try
                        {
                            IDataProcessorPlugin processor = (IDataProcessorPlugin)Activator.CreateInstance(type);
                            _loadedProcessors.Add(processor);
                            OnProcessorLoaded(new DataProcessorLoadEventArgs(processor.ProcessorName, processor.Version));
                        }
                        catch (Exception ex)
                        {
                            OnProcessorLoadError(new DataProcessorLoadErrorEventArgs(assemblyPath, $"Failed to create processor: {ex.Message}"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnProcessorLoadError(new DataProcessorLoadErrorEventArgs(assemblyPath, ex.Message));
            }
        }

        /// <summary>
        /// Gets enabled processors sorted by execution order
        /// </summary>
        public List<IDataProcessorPlugin> GetEnabledProcessorsInOrder()
        {
            return _loadedProcessors
                .Where(p => p.IsEnabled)
                .OrderBy(p => p.ExecutionOrder)
                .ToList();
        }

        /// <summary>
        /// Applies all enabled processors to data BEFORE saving
        /// </summary>
        public string ProcessBeforeSave(string data)
        {
            string result = data;
            foreach (var processor in GetEnabledProcessorsInOrder())
            {
                result = processor.ProcessBeforeSave(result);
            }
            return result;
        }

        /// <summary>
        /// Applies all enabled processors to data AFTER loading (in reverse order)
        /// </summary>
        public string ProcessAfterLoad(string data)
        {
            string result = data;
            var processors = GetEnabledProcessorsInOrder();
            for (int i = processors.Count - 1; i >= 0; i--)
            {
                result = processors[i].ProcessAfterLoad(result);
            }
            return result;
        }   

        protected virtual void OnProcessorLoaded(DataProcessorLoadEventArgs e)
        {
            ProcessorLoaded?.Invoke(this, e);
        }

        protected virtual void OnProcessorLoadError(DataProcessorLoadErrorEventArgs e)
        {
            ProcessorLoadError?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Event args for successful processor load
    /// </summary>
    public class DataProcessorLoadEventArgs : EventArgs
    {
        public string ProcessorName { get; }
        public string Version { get; }

        public DataProcessorLoadEventArgs(string name, string version)
        {
            ProcessorName = name;
            Version = version;
        }
    }

    /// <summary>
    /// Event args for processor load error
    /// </summary>
    public class DataProcessorLoadErrorEventArgs : EventArgs
    {
        public string FilePath { get; }
        public string ErrorMessage { get; }

        public DataProcessorLoadErrorEventArgs(string filePath, string errorMessage)
        {
            FilePath = filePath;
            ErrorMessage = errorMessage;
        }
    }
}
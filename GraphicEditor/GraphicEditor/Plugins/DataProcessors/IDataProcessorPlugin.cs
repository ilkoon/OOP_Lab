using System;
using System.Windows.Forms;
using GraphicEditor.Plugins;

namespace GraphicEditor.Plugins.DataProcessors
{
    /// <summary>
    /// Interface for data processor plugins that transform data before save and after load
    /// </summary>
    public interface IDataProcessorPlugin : IPluginMetadata
    {
        /// <summary>
        /// Name displayed in settings menu
        /// </summary>
        string ProcessorName { get; }

        /// <summary>
        /// Process data BEFORE saving to file
        /// </summary>
        /// <param name="xmlData">XML string to process</param>
        /// <returns>Processed XML string</returns>
        string ProcessBeforeSave(string xmlData);

        /// <summary>
        /// Process data AFTER loading from file
        /// </summary>
        /// <param name="xmlData">XML string loaded from file</param>
        /// <returns>Processed XML string</returns>
        string ProcessAfterLoad(string xmlData);

        /// <summary>
        /// Whether this processor is currently enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets settings control for this processor (for bonus 10 points)
        /// </summary>
        Control GetSettingsControl();

        /// <summary>
        /// Applies settings from the control
        /// </summary>
        void ApplySettings();

        /// <summary>
        /// Order of execution (lower = executed first)
        /// </summary>
        int ExecutionOrder { get; set; }
    }

    /// <summary>
    /// Event arguments for data processing
    /// </summary>
    public class DataProcessingEventArgs : EventArgs
    {
        public string OriginalData { get; set; }
        public string ProcessedData { get; set; }
        public string ProcessorName { get; set; }

        public DataProcessingEventArgs(string original, string processed, string processorName)
        {
            OriginalData = original;
            ProcessedData = processed;
            ProcessorName = processorName;
        }
    }
}
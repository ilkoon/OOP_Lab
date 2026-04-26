// GraphicEditor/Plugins/PluginLoader.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace GraphicEditor.Plugins
{
    /// <summary>
    /// Plugin signature data structure
    /// </summary>
    public class PluginSignature
    {
        public string PluginId { get; set; }
        public string PluginHash { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string SignerName { get; set; }
        public DateTime SignedAt { get; set; }
        public string Signature { get; set; }
    }

    /// <summary>
    /// Handles plugin signing and verification
    /// </summary>
    public static class PluginSigningManager
    {
        // Public key for verifying plugin signatures
        // This key is hardcoded - only plugins signed with matching private key will load
        private const string PublicKey = @"<RSAKeyValue><Modulus>yYRPg2tEWQRJ9rC0AI3WIr08o1/bVCoxtxqwKx47aL0QpXO++KXW/1VI+G2I92mheVWIr8cahQvYQ/ikbHkWzs3rjeiasRL0oD/nxtPEuOpPG3hqyWECsA65ZU7lLeuHUTji6a08vDsuglHdF0dPhR7bSf4NpeB2gIpSUi2H/Z6SE6K5NWSholxI+1BWzospxcADRHM9UmDXPV4A2hXjoeMd/33aDTL97RNhPCkTIoY+nRcbgpIqTb9yl7MxplW9BNq0MY3+TW9Iac+Xc/dflCcT3uNPdVvLcnIXPFuhBFCZ7sRAvZObejaOnBGim6zjvdUJxr4XfiBZSTThTw8hfQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";



        /// <summary>
        /// Verifies plugin signature
        /// </summary>
        public static bool VerifyPlugin(string pluginPath, PluginSignature signature)
        {
            try
            {
                // Check expiration
                if (signature.ExpirationDate < DateTime.Now)
                {
                    System.Diagnostics.Debug.WriteLine($"Plugin {signature.PluginId} expired on {signature.ExpirationDate}");
                    return false;
                }

                // Compute current hash of plugin file
                string currentHash = ComputeFileHash(pluginPath);

                // Verify hash hasn't changed
                if (currentHash != signature.PluginHash)
                {
                    System.Diagnostics.Debug.WriteLine($"Plugin {signature.PluginId} hash mismatch");
                    return false;
                }

                // Verify digital signature
                string dataToVerify = $"{signature.PluginId}|{signature.PluginHash}|{signature.ExpirationDate:O}|{signature.SignerName}|{signature.SignedAt:O}";

                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(PublicKey);
                    byte[] signatureBytes = Convert.FromBase64String(signature.Signature);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(dataToVerify);

                    return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Signature verification failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Computes SHA256 hash of a file
        /// </summary>
        public static string ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Loads signature from signature file
        /// </summary>
        public static PluginSignature LoadSignature(string signaturePath)
        {
            try
            {
                string json = File.ReadAllText(signaturePath);
                return JsonConvert.DeserializeObject<PluginSignature>(json);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Event arguments for plugin loaded event
    /// </summary>
    public class PluginLoadEventArgs : EventArgs
    {
        public string PluginName { get; }
        public string Version { get; }
        public string Type { get; }

        public PluginLoadEventArgs(string name, string version, string type)
        {
            PluginName = name;
            Version = version;
            Type = type;
        }
    }

    /// <summary>
    /// Event arguments for plugin load error
    /// </summary>
    public class PluginLoadErrorEventArgs : EventArgs
    {
        public string FilePath { get; }
        public string ErrorMessage { get; }

        public PluginLoadErrorEventArgs(string filePath, string errorMessage)
        {
            FilePath = filePath;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Manages dynamic loading of shape plugins
    /// </summary>
    public class PluginLoader
    {
        private readonly string _pluginsDirectory;
        private readonly List<IShape2DPlugin> _loaded2DPlugins;
        private readonly List<IShape3DPlugin> _loaded3DPlugins;

        public IReadOnlyList<IShape2DPlugin> Loaded2DPlugins => _loaded2DPlugins.AsReadOnly();
        public IReadOnlyList<IShape3DPlugin> Loaded3DPlugins => _loaded3DPlugins.AsReadOnly();

        public event EventHandler<PluginLoadEventArgs> PluginLoaded;
        public event EventHandler<PluginLoadErrorEventArgs> PluginLoadError;

        public PluginLoader(string pluginsDirectory = "Plugins")
        {
            _pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsDirectory);
            _loaded2DPlugins = new List<IShape2DPlugin>();
            _loaded3DPlugins = new List<IShape3DPlugin>();
        }

        /// <summary>
        /// Loads all valid plugins from the plugins directory
        /// </summary>
        public void LoadAllPlugins()
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
                return;
            }

            foreach (string dllPath in Directory.GetFiles(_pluginsDirectory, "*.dll"))
            {
                LoadPluginFromAssembly(dllPath);
            }
        }

        /// <summary>
        /// Loads a specific plugin from a file path
        /// </summary>
        public bool LoadPlugin(string pluginPath)
        {
            return LoadPluginFromAssembly(pluginPath);
        }

        private bool LoadPluginFromAssembly(string assemblyPath)
        {
            try
            {
                // Check for signature file
                string signaturePath = Path.ChangeExtension(assemblyPath, ".sig");
                if (!File.Exists(signaturePath))
                {
                    OnPluginLoadError(new PluginLoadErrorEventArgs(assemblyPath, "Missing signature file (.sig)"));
                    return false;
                }

                // Load and verify signature
                var signature = PluginSigningManager.LoadSignature(signaturePath);
                if (signature == null)
                {
                    OnPluginLoadError(new PluginLoadErrorEventArgs(assemblyPath, "Invalid signature file"));
                    return false;
                }

                if (!PluginSigningManager.VerifyPlugin(assemblyPath, signature))
                {
                    OnPluginLoadError(new PluginLoadErrorEventArgs(assemblyPath, "Signature verification failed"));
                    return false;
                }

                // Load the assembly
                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                // Load 2D shape plugins
                foreach (Type type in assembly.GetTypes().Where(t => typeof(IShape2DPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
                {
                    IShape2DPlugin plugin = (IShape2DPlugin)Activator.CreateInstance(type);
                    _loaded2DPlugins.Add(plugin);
                    OnPluginLoaded(new PluginLoadEventArgs(plugin.PluginName, plugin.Version, "2D"));
                }

                // Load 3D shape plugins
                foreach (Type type in assembly.GetTypes().Where(t => typeof(IShape3DPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
                {
                    IShape3DPlugin plugin = (IShape3DPlugin)Activator.CreateInstance(type);
                    _loaded3DPlugins.Add(plugin);
                    OnPluginLoaded(new PluginLoadEventArgs(plugin.PluginName, plugin.Version, "3D"));
                }

                return true;
            }
            catch (Exception ex)
            {
                OnPluginLoadError(new PluginLoadErrorEventArgs(assemblyPath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Reloads all plugins from the plugins directory
        /// </summary>
        public void ReloadAllPlugins()
        {
            _loaded2DPlugins.Clear();
            _loaded3DPlugins.Clear();
            LoadAllPlugins();
        }

        protected virtual void OnPluginLoaded(PluginLoadEventArgs e)
        {
            PluginLoaded?.Invoke(this, e);
        }

        protected virtual void OnPluginLoadError(PluginLoadErrorEventArgs e)
        {
            PluginLoadError?.Invoke(this, e);
        }
    }
}
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace PluginSigner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Plugin Signer Tool ===");
            Console.WriteLine();

            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  PluginSigner.exe <plugin.dll> [expiration_days]");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine("  PluginSigner.exe PentagonPlugin.dll");
                Console.WriteLine("  PluginSigner.exe PentagonPlugin.dll 365");
                Console.WriteLine();
                Console.WriteLine("If no key exists, a new key pair will be generated.");
                return;
            }

            string pluginPath = args[0];
            int expirationDays = args.Length > 1 ? int.Parse(args[1]) : 365;

            if (!File.Exists(pluginPath))
            {
                Console.WriteLine($"Error: Plugin not found - {pluginPath}");
                return;
            }

            string keyPath = "signing_key.xml";
            string publicKeyPath = "public_key.xml";

            // Generate key pair if it doesn't exist
            if (!File.Exists(keyPath))
            {
                Console.WriteLine("No signing key found. Generating new key pair...");
                GenerateKeyPair(keyPath, publicKeyPath);
                Console.WriteLine($"  Private key: {keyPath}");
                Console.WriteLine($"  Public key:  {publicKeyPath}");
                Console.WriteLine();
                Console.WriteLine("IMPORTANT: Copy the public_key.xml to your main application!");
                Console.WriteLine("The public key must be embedded in PluginSigningManager for verification.");
                Console.WriteLine();
            }

            // Load private key
            Console.WriteLine("Loading private key...");
            string privateKeyXml = File.ReadAllText(keyPath);

            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKeyXml);
                SignPlugin(rsa, pluginPath, expirationDays);
            }
        }

        static void GenerateKeyPair(string privateKeyPath, string publicKeyPath)
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string privateKey = rsa.ToXmlString(true);
                string publicKey = rsa.ToXmlString(false);

                File.WriteAllText(privateKeyPath, privateKey);
                File.WriteAllText(publicKeyPath, publicKey);
            }
        }

        static void SignPlugin(RSA rsa, string pluginPath, int expirationDays)
        {
            Console.WriteLine($"Signing plugin: {Path.GetFileName(pluginPath)}");
            Console.WriteLine($"Expiration: {expirationDays} days from now");

            // Compute plugin hash
            string pluginHash = ComputeFileHash(pluginPath);
            Console.WriteLine($"Plugin hash: {pluginHash.Substring(0, 32)}...");

            string pluginId = Path.GetFileNameWithoutExtension(pluginPath);
            DateTime expirationDate = DateTime.Now.AddDays(expirationDays);
            DateTime signedAt = DateTime.Now;
            string signerName = Environment.UserName;

            // Create data to sign
            string dataToSign = $"{pluginId}|{pluginHash}|{expirationDate:O}|{signerName}|{signedAt:O}";

            // Sign the data
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
            byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string signature = Convert.ToBase64String(signatureBytes);

            // Create signature object
            var signatureObject = new
            {
                PluginId = pluginId,
                PluginHash = pluginHash,
                ExpirationDate = expirationDate,
                SignerName = signerName,
                SignedAt = signedAt,
                Signature = signature
            };

            // Save signature to file
            string signaturePath = Path.ChangeExtension(pluginPath, ".sig");
            string json = JsonConvert.SerializeObject(signatureObject, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(signaturePath, json);

            Console.WriteLine();
            Console.WriteLine("✓ Plugin signed successfully!");
            Console.WriteLine($"  Signature file: {signaturePath}");
            Console.WriteLine($"  Expires: {expirationDate:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
            Console.WriteLine("To use this plugin:");
            Console.WriteLine("  1. Copy the DLL and .sig file to the Plugins folder");
            Console.WriteLine("  2. Make sure the public key matches the one in PluginSigningManager");
            Console.WriteLine("  3. Restart the application or use Plugin Manager to load");
        }

        static string ComputeFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
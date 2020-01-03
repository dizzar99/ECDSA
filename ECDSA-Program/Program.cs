using System;
using System.IO;
using System.Numerics;
using BLL.Interface;
using CommandLine;
using DependencyResolver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECDSA_Program
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Options options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opt =>
                {
                    options = opt;
                });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{options.ConfigFile}")
                .Build();

            var curve = SetupCurve(config);
            var serviceProvider = ResolverConfig.CreateServiceProvider();
            var ecdsa = serviceProvider.GetService<IDigitalSignature>();

            Console.Clear();
            if (options.PrintPoints)
            {
                PrintPoints(curve);
            }

            var dh = new DiffieHellman(curve);
            var alice = dh.GenerateKeyPair();
            var bob = dh.GenerateKeyPair();

            Console.WriteLine();
            Console.WriteLine($"G = {curve.G}{Environment.NewLine}");
            Console.WriteLine($"Alice's public and private keys:{Environment.NewLine}{alice.privateKey}{Environment.NewLine}{alice.publicKey}{Environment.NewLine}");
            Console.WriteLine($"Bob's public and private keys:{Environment.NewLine}{bob.privateKey}{Environment.NewLine}{bob.publicKey}{Environment.NewLine}");
            Console.WriteLine($"{Environment.NewLine}Alice's key: {dh.GetSharedkey(alice.privateKey, bob.publicKey)} {Environment.NewLine}Bob's key: {dh.GetSharedkey(bob.privateKey, alice.publicKey)}");

            string message = "Hello world!";
            var keys = ecdsa.GenerateKeyPair();
            Console.WriteLine($"{Environment.NewLine}ECDSA");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"Public key: {keys.publicKey}");
            Console.WriteLine($"Private key: {keys.privateKey}");

            var signature = ecdsa.Sign(message, keys.privateKey);
            Console.WriteLine($"{Environment.NewLine}Signature:{Environment.NewLine}r = {signature.r}, s = {signature.s}");
            //signature.r++;
            string verification = ecdsa.Verify(message, signature, keys.publicKey) ? "Signature matches!" : "Signature does not match!";
            Console.WriteLine(verification);
        }

        private static void PrintPoints(EllipticCurve curve)
        {
            Console.WriteLine($"Curve: {curve}");
            Console.WriteLine($"Curve's order: {curve.N}{Environment.NewLine}");
            Console.WriteLine("Points of curve:");
            foreach (var point in curve.GenerateAllPoints())
            {
                Console.WriteLine(point);
            }
        }

        private static EllipticCurve SetupCurve(IConfigurationRoot config)
        {
            var curveSection = config.GetSection("EllipticCurve");
            if (curveSection is null)
            {
                return new EllipticCurve();
            }

            System.Globalization.NumberStyles ns;
            if (config["NumberStyle"] == "dec")
            {
                ns = System.Globalization.NumberStyles.Integer;
            }
            else
            {
                ns = System.Globalization.NumberStyles.HexNumber;
            }

            var a = BigInteger.Parse(curveSection["A"], ns);
            var b = BigInteger.Parse(curveSection["B"], ns);
            var p = BigInteger.Parse(curveSection["P"], ns);
            var n = BigInteger.Parse(curveSection["N"], ns);
            var g = new BigIntegerPoint(BigInteger.Parse(curveSection.GetSection("G")["X"], ns), BigInteger.Parse(curveSection.GetSection("G")["Y"], ns));

            return new EllipticCurve(a, b, p, g, n);
        }

        public class Options
        {
            [Option('p', "points", Required = false, HelpText = "Prints all points of a curve.")]
            public bool PrintPoints { get; set; }

            [Option('c', "config", Required = false, Default = "appsettings.json", HelpText = "Setup configuration file.")]
            public string ConfigFile { get; set; }
        }
    }
}
using BLL.Interface;
using DependencyResolver;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ECDSA_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ResolverConfig.CreateServiceProvider();
            var ecdsa = serviceProvider.GetService<IDigitalSignature>();

            EllipticCurve curve = new EllipticCurve(1, 1, 43, new BigIntegerPoint(21, 34), 34);
            var dh = new DiffieHellman(curve);
            var alice = dh.GenerateKeyPair();
            var bob = dh.GenerateKeyPair();
            foreach(var point in curve.GenerateAllPoints())
            {
                Console.WriteLine(point);
            }

            Console.WriteLine();
            Console.WriteLine($"G = {curve.G}");
            Console.WriteLine($"Alice's public and private keys:{Environment.NewLine}{alice.privateKey}{Environment.NewLine}{alice.publicKey}");
            Console.WriteLine($"Bob's public and private keys:{Environment.NewLine}{bob.privateKey}{Environment.NewLine}{bob.publicKey}");
            Console.WriteLine($"{Environment.NewLine}Alice's key: {dh.GetSharedkey(alice.privateKey, bob.publicKey)} {Environment.NewLine}Bob's key: {dh.GetSharedkey(bob.privateKey, alice.publicKey)}");

            Console.WriteLine();
            string message = "Hello world!";
            var keys = ecdsa.GenerateKeyPair();
            Console.WriteLine($"Public key: {keys.publicKey}");
            Console.WriteLine($"Private key: {keys.privateKey}");

            var signature = ecdsa.Sign(message, keys.privateKey);
            Console.WriteLine($"Signature:{Environment.NewLine}r = {signature.r}, s = {signature.s}");
            string verification = ecdsa.Verify(message, signature, keys.publicKey) ? "Signature matches" : "Not matches";
            Console.WriteLine(verification);
        }
    }
}
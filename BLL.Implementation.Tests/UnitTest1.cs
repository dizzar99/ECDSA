using BLL.Interface;
using NUnit.Framework;
using System.Numerics;

namespace BLL.Implementation.Tests
{
    public class Tests
    {
        EllipticCurve curve;
        int count;

        [SetUp]
        public void Setup()
        {
            //this.curve = new Interface.EllipticCurve(1, 5, 43, new BigIntegerPoint(11, 33), 37);
            this.curve = new EllipticCurve();
            this.count = 1000;
        }

        [Test]
        public void EllipticCurveDSA_Sign_Verify()
        {
            EllipticCurveDSA ecdsa = new EllipticCurveDSA(this.curve);
            string message = "Hello world";
            for (int i = 0; i < this.count; i++)
            {
                var keys = ecdsa.GenerateKeyPair();
                Assert.IsTrue(ecdsa.Verify(message, ecdsa.Sign(message, keys.privateKey), keys.publicKey));
            }
        }

        [Test]
        public void DiffieHellman()
        {
            var dh = new DiffieHellman(this.curve);
            for (int i = 0; i < this.count; i++)
            {
                var alice = dh.GenerateKeyPair();
                var bob = dh.GenerateKeyPair();

                Assert.AreEqual(dh.GetSharedkey(alice.privateKey, bob.publicKey), dh.GetSharedkey(bob.privateKey, alice.publicKey));
            }
        }
    }
}
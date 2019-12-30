using BLL.Interface;
using BLL.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyResolver
{
    public static class ResolverConfig
    {
        public static IServiceProvider CreateServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<EllipticCurve>()
                .AddSingleton<DiffieHellman>()
                .AddSingleton<IDigitalSignature, EllipticCurveDSA>()
                .BuildServiceProvider();
        }
    }
}

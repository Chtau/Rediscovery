using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationBase
{
    public static class GrpcHeadersExtensions
    {
        public static void AddAuthorizationHeader(this Metadata metadata, string bearerToken)
        {
            if (!string.IsNullOrWhiteSpace(bearerToken))
                metadata.Add("Authorization", $"Bearer {bearerToken}");
        }
    }
}

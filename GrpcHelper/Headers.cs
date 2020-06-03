using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcHelper
{
    public static class Headers
    {
        public static void AddAuthorizationHeader(this Metadata metadata, string bearerToken)
        {
            if (!string.IsNullOrWhiteSpace(bearerToken))
                metadata.Add("Authorization", $"Bearer {bearerToken}");
        }
    }
}

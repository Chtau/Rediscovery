using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Resources;

namespace CommunicationResourceProvider.ProtoServices
{
    public class ResourceExchangeService : ResourceExchange.ResourceExchangeBase
    {
        private readonly ILogger<ResourceExchangeService> _logger;
        private readonly IResourcesRepository _resourcesRepository;

        public ResourceExchangeService(ILoggerFactory loggerFactory, IResourcesRepository resourcesRepository)
        {
            _logger = loggerFactory.CreateLogger<ResourceExchangeService>();
            _resourcesRepository = resourcesRepository;
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task ActiveDevices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            return base.ActiveDevices(request, responseStream, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task Devices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            return base.Devices(request, responseStream, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceChangeRequest> DeleteDevice(DeviceChangeRequest request, ServerCallContext context)
        {
            return base.DeleteDevice(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task Features(Empty request, IServerStreamWriter<FeatureDefinitionExtended> responseStream, ServerCallContext context)
        {
            return base.Features(request, responseStream, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetails> FeatureDetail(FeatureDetailRequest request, ServerCallContext context)
        {
            return base.FeatureDetail(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailProfileDeleteRequest> FeatureDetailProfileDelete(FeatureDetailProfileDeleteRequest request, ServerCallContext context)
        {
            return base.FeatureDetailProfileDelete(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailProfileSaveRequest> FeatureDetailProfileSave(FeatureDetailProfileSaveRequest request, ServerCallContext context)
        {
            return base.FeatureDetailProfileSave(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<FeatureDetailSettingSaveRequest> FeatureDetailSettingSave(FeatureDetailSettingSaveRequest request, ServerCallContext context)
        {
            return base.FeatureDetailSettingSave(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task PendingDevices(Empty request, IServerStreamWriter<DeviceInfoList> responseStream, ServerCallContext context)
        {
            return base.PendingDevices(request, responseStream, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceChangeRequest> ResolvePendingDevice(DevicePendingRequest request, ServerCallContext context)
        {
            return base.ResolvePendingDevice(request, context);
        }

        [Authorize(Roles = AuthorizationRoles.ResourceConsumer)]
        public override Task<DeviceInfo> UpdateDevice(DeviceInfo request, ServerCallContext context)
        {
            return base.UpdateDevice(request, context);
        }
    }
}

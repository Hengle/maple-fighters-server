﻿using System;
using CommonCommunicationInterfaces;
using CommonTools.Log;
using Components.Common.Interfaces;
using JsonConfig;
using PeerLogic.Common.Components.Interfaces;
using ServerApplication.Common.ApplicationBase;
using ServerCommunication.Common;

namespace UserProfile.Server.Common
{
    public class UserProfileService : ServiceBase<UserProfileOperations, UserProfileEvents>, IUserProfileServiceAPI
    {
        private readonly int serverId;
        private readonly IPeerContainer peerContainer;
        private readonly IUserIdToPeerIdConverter userIdToPeerIdConverter;

        public UserProfileService()
        {
            var idGenerator = ServerComponents.GetComponent<IRandomNumberGenerator>().AssertNotNull();
            serverId = idGenerator.GenerateRandomNumber();

            peerContainer = ServerComponents.GetComponent<IPeerContainer>().AssertNotNull();
            userIdToPeerIdConverter = ServerComponents.AddComponent(new UserIdToPeerIdConverter());
        }

        protected override void OnAuthenticated()
        {
            base.OnAuthenticated();

            LogUtils.Log(MessageBuilder.Trace("Authenticated with UserProfile service."));

            SubscribeToUserPropertiesChanges();
            RegisterToUserProfileService();
        }

        protected override void OnDisconnected(DisconnectReason disconnectReason, string details)
        {
            base.OnDisconnected(disconnectReason, details);

            UnsubscribeFromUserPropertiesChanges();
            UnregisterFromUserProfileService();
        }

        private void SubscribeToUserPropertiesChanges()
        {
            Action<UserProfilePropertiesChangedEventParameters> action = OnUserProfilePropertiesChanged;
            OutboundServerPeerLogic?.SetEventHandler((byte)UserProfileEvents.UserProfilePropertiesChanged, action);
        }

        private void UnsubscribeFromUserPropertiesChanges()
        {
            OutboundServerPeerLogic?.RemoveEventHandler((byte)UserProfileEvents.UserProfilePropertiesChanged);
        }

        private void OnUserProfilePropertiesChanged(UserProfilePropertiesChangedEventParameters parameters)
        {
            var userId = parameters.UserId;
            var peerId = userIdToPeerIdConverter.Get(userId);
            if (peerId == null)
            {
                return;
            }

            var peerWrapper = peerContainer.GetPeerWrapper(peerId.Value);
            if (peerWrapper == null)
            {
                LogUtils.Log(MessageBuilder.Trace($"Unable to find a peer wrapper of user with id {userId}"));
                return;
            }

            var eventInvoker = peerWrapper.PeerLogic.Components.GetComponent<IUserProfilePropertiesChangesEventInvoker>().AssertNotNull();
            eventInvoker.Invoke(parameters);
        }

        private void RegisterToUserProfileService()
        {
            var parameters = new RegisterToUserProfileServiceRequestParameters(serverId);
            OutboundServerPeerLogic?.SendOperation((byte)UserProfileOperations.Register, parameters);
        }

        private void UnregisterFromUserProfileService()
        {
            var parameters = new UnregisterFromUserProfileServiceRequestParameters(serverId);
            OutboundServerPeerLogic?.SendOperation((byte)UserProfileOperations.Unregister, parameters);
        }

        public void ChangeUserProfileProperties(ChangeUserProfilePropertiesRequestParameters parameters)
        {
            OutboundServerPeerLogic?.SendOperation((byte)UserProfileOperations.ChangeUserProfileProperties, parameters);
        }

        public void SubscribeToUserProfile(int userId)
        {
            var parameters = new SubscribeToUserProfileRequestParameters(userId, serverId);
            OutboundServerPeerLogic?.SendOperation((byte)UserProfileOperations.Subscribe, parameters);
        }

        public void UnsubscribeFromUserProfile(int userId)
        {
            var parameters = new UnsubscribeFromUserProfileRequestParameters(userId, serverId);
            OutboundServerPeerLogic?.SendOperation((byte)UserProfileOperations.Unsubscribe, parameters);
        }

        protected override PeerConnectionInformation GetPeerConnectionInformation()
        {
            LogUtils.Assert(Config.Global.UserProfileService, MessageBuilder.Trace("Could not find a configuration for the UserProfile service."));

            var ip = (string)Config.Global.UserProfileService.IP;
            var port = (int)Config.Global.UserProfileService.Port;
            return new PeerConnectionInformation(ip, port);
        }

        protected override string GetSecretKey()
        {
            LogUtils.Assert(Config.Global.UserProfileService, MessageBuilder.Trace("Could not find a configuration for the UserProfile service."));

            var secretKey = (string)Config.Global.UserProfileService.SecretKey;
            return secretKey;
        }
    }
}
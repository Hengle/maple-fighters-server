﻿using CommonCommunicationInterfaces;
using CommonTools.Log;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationHelper;
using UserProfile.Server.Common;
using UserProfile.Service.Application.Components.Interfaces;
using UserProfile.Service.Application.PeerLogic.Components;

namespace UserProfile.Service.Application.PeerLogic.Operations
{
    internal class ChangeUserProfilePropertiesOperationHandler : IOperationRequestHandler<ChangeUserProfilePropertiesRequestParameters, EmptyParameters>
    {
        private readonly IDatabaseUserProfileExistence databaseUserProfileExistence;
        private readonly IDatabaseUserProfilePropertiesUpdater databaseUserProfilePropertiesUpdater;
        private readonly IUserProfilePropertiesChangesNotifier userProfilePropertiesChangesNotifier;
        private readonly IDatabaseUserProfileCreator databaseUserProfileCreator;
        private readonly IUsersContainer usersContainer;

        public ChangeUserProfilePropertiesOperationHandler(IUsersContainer usersContainer, IUserProfilePropertiesChangesNotifier userProfilePropertiesChangesNotifier)
        {
            this.usersContainer = usersContainer;
            this.userProfilePropertiesChangesNotifier = userProfilePropertiesChangesNotifier;

            databaseUserProfileExistence = ServerComponents.GetComponent<IDatabaseUserProfileExistence>().AssertNotNull();
            databaseUserProfilePropertiesUpdater = ServerComponents.GetComponent<IDatabaseUserProfilePropertiesUpdater>().AssertNotNull();
            databaseUserProfileCreator = ServerComponents.GetComponent<IDatabaseUserProfileCreator>().AssertNotNull();
        }

        public EmptyParameters? Handle(MessageData<ChangeUserProfilePropertiesRequestParameters> messageData, ref MessageSendOptions sendOptions)
        {
            var userId = messageData.Parameters.UserId;
            var localId = messageData.Parameters.LocalId;
            var serverType = messageData.Parameters.ServerType;
            var connectionStatus = messageData.Parameters.ConnectionStatus;

            if (databaseUserProfileExistence.Exists(userId))
            {
                databaseUserProfilePropertiesUpdater.Update(userId, localId, serverType, connectionStatus);
            }
            else
            {
                databaseUserProfileCreator.Create(userId, localId, serverType, connectionStatus);
            }

            if (connectionStatus == ConnectionStatus.Connected)
            {
                usersContainer.Add(userId);
            }
            else
            {
                usersContainer.Remove(userId);
            }

            var parameters = new UserProfilePropertiesChangedEventParameters(userId, serverType);
            userProfilePropertiesChangesNotifier.Notify(parameters);
            return null;
        }
    }
}
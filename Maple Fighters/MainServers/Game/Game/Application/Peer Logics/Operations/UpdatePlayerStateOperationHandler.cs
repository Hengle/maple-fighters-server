﻿using CommonCommunicationInterfaces;
using CommonTools.Log;
using Game.Application.GameObjects.Components.Interfaces;
using ServerCommunicationHelper;
using Game.Common;
using InterestManagement.Components.Interfaces;

namespace Game.Application.PeerLogic.Operations
{
    internal class UpdatePlayerStateOperationHandler : IOperationRequestHandler<UpdatePlayerStateRequestParameters, EmptyParameters>
    {
        private readonly ISceneObject sceneObject;
        private readonly IInterestAreaNotifier interestAreaNotifier;
        
        public UpdatePlayerStateOperationHandler(ISceneObject sceneObject)
        {
            this.sceneObject = sceneObject;

            interestAreaNotifier = sceneObject.Components.GetComponent<IInterestAreaNotifier>().AssertNotNull();
        }

        public EmptyParameters? Handle(MessageData<UpdatePlayerStateRequestParameters> messageData, ref MessageSendOptions sendOptions)
        {
            var playerState = messageData.Parameters.PlayerState;

            var playerPositionController = sceneObject.Components.GetComponent<IPlayerPositionController>();
            if (playerPositionController != null)
            {
                playerPositionController.PlayerState = playerState;
            }

            var parameters = new PlayerStateChangedEventParameters(playerState, sceneObject.Id);
            var messageSendOptions = MessageSendOptions.DefaultReliable((byte)GameDataChannels.Animations);
            interestAreaNotifier.NotifySubscribers((byte)GameEvents.PlayerStateChanged, parameters, messageSendOptions);
            return null;
        }
    }
}
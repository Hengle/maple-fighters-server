﻿using Chat.Application.PeerLogic.Components;
using Chat.Application.PeerLogic.Operations;
using Chat.Common;
using CommonTools.Log;
using ServerCommunicationInterfaces;
using Shared.ServerApplication.Common.PeerLogic;

namespace Chat.Application.PeerLogics
{
    internal class AuthenticatedPeerLogic : PeerLogicBase<ChatOperations, ChatEvents>
    {
        public override void Initialize(IClientPeerWrapper<IClientPeer> peer)
        {
            base.Initialize(peer);

            Entity.Container.AddComponent(new ChatMessageEventSender());

            AddHandlerForChatMessageOperation();
        }

        private void AddHandlerForChatMessageOperation()
        {
            var eventSenderWrapper = Entity.Container.GetComponent<ChatMessageEventSender>().AssertNotNull();
            OperationRequestHandlerRegister.SetHandler(ChatOperations.ChatMessage, new ChatMessageOperationHandler(eventSenderWrapper));
        }
    }
}
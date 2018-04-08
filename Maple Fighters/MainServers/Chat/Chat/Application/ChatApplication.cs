﻿using Authorization.Client.Common;
using Authorization.Server.Common;
using Chat.Application.PeerLogics;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationInterfaces;
using UserProfile.Server.Common;

namespace Chat.Application
{
    public class ChatApplication : ApplicationBase
    {
        public ChatApplication(IFiberProvider fiberProvider, IServerConnector serverConnector) 
            : base(fiberProvider, serverConnector)
        {
            // Left blank intentionally
        }

        public override void Startup()
        {
            base.Startup();

            AddCommonComponents();
            AddComponents();
        }

        public override void OnConnected(IClientPeer clientPeer)
        {
            base.OnConnected(clientPeer);

            WrapClientPeer(clientPeer, new UnauthorizedClientPeerLogic<AuthorizedClientPeerLogic>());
        }

        private void AddComponents()
        {
            ServerComponents.AddComponent(new AuthorizationService());
            ServerComponents.AddComponent(new UserProfileService());
        }
    }
}
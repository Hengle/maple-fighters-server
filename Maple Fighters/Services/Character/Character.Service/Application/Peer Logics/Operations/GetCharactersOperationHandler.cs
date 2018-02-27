﻿using System.Linq;
using Character.Client.Common;
using CharacterService.Application.Components;
using CommonCommunicationInterfaces;
using CommonTools.Log;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationHelper;

namespace CharacterService.Application.PeerLogics.Operations
{
    internal class GetCharactersOperationHandler : IOperationRequestHandler<EmptyParameters, GetCharactersResponseParameters>
    {
        private readonly int userId;
        private readonly IDatabaseCharactersGetter databaseCharactersGetter;

        public GetCharactersOperationHandler(int userId)
        {
            this.userId = userId;

            databaseCharactersGetter = Server.Components.GetComponent<IDatabaseCharactersGetter>().AssertNotNull();
        }

        public GetCharactersResponseParameters? Handle(MessageData<EmptyParameters> messageData, ref MessageSendOptions sendOptions)
        {
            var characters = databaseCharactersGetter.GetCharacters(userId).ToArray();
            return new GetCharactersResponseParameters(characters);
        }
    }
}
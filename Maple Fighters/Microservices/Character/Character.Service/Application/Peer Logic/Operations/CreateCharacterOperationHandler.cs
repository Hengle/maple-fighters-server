﻿using Character.Server.Common;
using CharacterService.Application.Components.Interfaces;
using CommonCommunicationInterfaces;
using CommonTools.Log;
using Game.Common;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationHelper;

namespace CharacterService.Application.PeerLogics.Operations
{
    internal class CreateCharacterOperationHandler : IOperationRequestHandler<CreateCharacterRequestParametersEx, CreateCharacterResponseParameters>
    {
        private readonly IDatabaseCharacterCreator databaseCharacterCreator;
        private readonly IDatabaseCharacterExistence databaseCharacterExistence;
        private readonly IDatabaseCharacterNameVerifier databaseCharacterNameVerifier;

        public CreateCharacterOperationHandler()
        {
            databaseCharacterCreator = ServerComponents.GetComponent<IDatabaseCharacterCreator>().AssertNotNull();
            databaseCharacterExistence = ServerComponents.GetComponent<IDatabaseCharacterExistence>().AssertNotNull();
            databaseCharacterNameVerifier = ServerComponents.GetComponent<IDatabaseCharacterNameVerifier>().AssertNotNull();
        }

        public CreateCharacterResponseParameters? Handle(MessageData<CreateCharacterRequestParametersEx> messageData, ref MessageSendOptions sendOptions)
        {
            var userId = messageData.Parameters.UserId;
            var characterClass = messageData.Parameters.CharacterClass;
            var name = messageData.Parameters.Name;
            var characterIndex = messageData.Parameters.Index;

            if (databaseCharacterExistence.Exists(userId, characterIndex))
            {
                return new CreateCharacterResponseParameters(CharacterCreationStatus.Failed);
            }

            if (databaseCharacterNameVerifier.Verify(name))
            {
                return new CreateCharacterResponseParameters(CharacterCreationStatus.NameUsed);
            }

            databaseCharacterCreator.Create(userId, name, characterClass, characterIndex);
            return new CreateCharacterResponseParameters(CharacterCreationStatus.Succeed);
        }
    }
}
﻿using System;
using System.Threading.Tasks;
using Character.Client.Common;
using Character.Server.Common;
using CommonCommunicationInterfaces;
using CommonTools.Coroutines;
using CommonTools.Log;
using ServerApplication.Common.ApplicationBase;
using ServerCommunicationHelper;
using Shared.Game.Common;

namespace Game.Application.PeerLogic.Operations
{
    internal class CharacterValidationOperationHandler : IAsyncOperationRequestHandler<ValidateCharacterRequestParameters, ValidateCharacterResponseParameters>
    {
        private readonly int userId;
        private readonly Action<CharacterFromDatabaseParameters?> onCharacterSelected;
        private readonly ICharacterServiceAPI charactersServiceApi;

        public CharacterValidationOperationHandler(int userId, Action<CharacterFromDatabaseParameters?> onCharacterSelected)
        {
            this.userId = userId;
            this.onCharacterSelected = onCharacterSelected;

            charactersServiceApi = Server.Components.GetComponent<ICharacterServiceAPI>().AssertNotNull();
        }

        public Task<ValidateCharacterResponseParameters?> Handle(IYield yield, MessageData<ValidateCharacterRequestParameters> messageData, ref MessageSendOptions sendOptions)
        {
            var characterIndex = messageData.Parameters.CharacterIndex;
            var parameters = new GetCharacterRequestParameters(userId, characterIndex);
            return GetCharacter(yield, parameters);
        }

        private async Task<ValidateCharacterResponseParameters?> GetCharacter(IYield yield, GetCharacterRequestParameters parameters)
        {
            var character = await charactersServiceApi.GetCharacter(yield, parameters);
            onCharacterSelected?.Invoke(character.Character);

            var status = character.Character.HasCharacter ? CharacterValidationStatus.Ok : CharacterValidationStatus.Wrong;
            return new ValidateCharacterResponseParameters(status);
        }
    }
}
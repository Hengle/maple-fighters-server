﻿using Character.Client.Common;
using ComponentModel.Common;

namespace CharacterService.Application.Components
{
    internal interface IDatabaseCharacterCreator : IExposableComponent
    {
        void Create(int userId, string name, CharacterClasses characterClass, CharacterIndex characterIndex);
    }
}
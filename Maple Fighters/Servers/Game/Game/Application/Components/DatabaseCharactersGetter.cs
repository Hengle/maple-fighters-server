﻿using System.Collections.Generic;
using System.Linq;
using CommonTools.Log;
using Database.Common.Components;
using Database.Common.TablesDefinition;
using ServerApplication.Common.ApplicationBase;
using ServerApplication.Common.ComponentModel;
using ServiceStack.OrmLite;
using Shared.Game.Common;

namespace Game.Application.Components
{
    internal class DatabaseCharactersGetter : Component<IServerEntity>
    {
        private const int MAXIMUM_CHARACTERS = 3;
        private DatabaseConnectionProvider databaseConnectionProvider;

        protected override void OnAwake()
        {
            base.OnAwake();

            databaseConnectionProvider = Entity.Container.GetComponent<DatabaseConnectionProvider>().AssertNotNull();
        }

        public IEnumerable<Character> GetCharacters(int userId)
        {
            using (var db = databaseConnectionProvider.GetDbConnection())
            {
                // Should only be 3 characters.
                var charactersFromDatabase = db.Select<CharactersTableDefinition>().Where(x => x.UserId == userId);
                var charactersDatabase = charactersFromDatabase.Select(GetCharacter).ToList();

                // If there is no characters, it means that there is an empty character. 
                var length = MAXIMUM_CHARACTERS - charactersDatabase.Count;
                for (var i = 0; i < length; i++)
                {
                    charactersDatabase.Add(new Character { HasCharacter = false, Index = CharacterIndex.Zero });
                }

                // Make an order of characters which will be sent to a client.
                var characters = new List<Character>(MAXIMUM_CHARACTERS)
                {
                    new Character { HasCharacter = false, Index = CharacterIndex.First },
                    new Character { HasCharacter = false, Index = CharacterIndex.Second },
                    new Character { HasCharacter = false, Index = CharacterIndex.Third }
                };

                foreach (var character in charactersDatabase)
                {
                    if (character.HasCharacter)
                    {
                        characters[(int)character.Index] = new Character(character.CharacterType, character.Name, character.Index);
                    }
                }
                return characters;
            }
        }

        public Character? GetCharacter(int userId, int characterIndex)
        {
            using (var db = databaseConnectionProvider.GetDbConnection())
            {
                var characterTable = db.Single<CharactersTableDefinition>(x => x.UserId == userId && x.CharacterIndex == characterIndex);
                return GetCharacter(characterTable);
            }
        }

        private Character GetCharacter(CharactersTableDefinition charactersTableDefinition)
        {
            if (charactersTableDefinition.CharacterType == CharacterClasses.Arrow.ToString())
            {
                return new Character(CharacterClasses.Arrow, charactersTableDefinition.Name, (CharacterIndex)charactersTableDefinition.CharacterIndex);
            }

            if (charactersTableDefinition.CharacterType == CharacterClasses.Knight.ToString())
            {
                return new Character(CharacterClasses.Knight, charactersTableDefinition.Name, (CharacterIndex)charactersTableDefinition.CharacterIndex);
            }

            if (charactersTableDefinition.CharacterType == CharacterClasses.Wizard.ToString())
            {
                return new Character(CharacterClasses.Wizard, charactersTableDefinition.Name, (CharacterIndex)charactersTableDefinition.CharacterIndex);
            }

            LogUtils.Log(MessageBuilder.Trace($"Can not get character type of user id #{charactersTableDefinition.UserId}"));
            return new Character { HasCharacter = false, Index = (CharacterIndex)charactersTableDefinition.CharacterIndex };
        }
    }
}
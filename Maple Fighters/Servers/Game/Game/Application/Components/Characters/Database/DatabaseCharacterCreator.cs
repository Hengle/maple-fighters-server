﻿using CommonTools.Log;
using ComponentModel.Common;
using Database.Common.Components;
using Database.Common.TablesDefinition;
using ServiceStack.OrmLite;
using Shared.Game.Common;

namespace Game.Application.Components
{
    internal class DatabaseCharacterCreator : Component, IDatabaseCharacterCreator
    {
        private IDatabaseConnectionProvider databaseConnectionProvider;

        protected override void OnAwake()
        {
            base.OnAwake();

            databaseConnectionProvider = Entity.GetComponent<IDatabaseConnectionProvider>().AssertNotNull();
        }

        public void Create(int userId, string name, CharacterClasses characterClass, CharacterIndex characterIndex)
        {
            using (var db = databaseConnectionProvider.GetDbConnection())
            {
                var user = new CharactersTableDefinition
                {
                    UserId = userId,
                    Name = name,
                    CharacterType = characterClass.ToString(),
                    CharacterIndex = (int)characterIndex
                };
                db.Insert(user);
            }
        }
    }
}
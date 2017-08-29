﻿using ServerApplication.Common.ComponentModel;
using Shared.Game.Common;

namespace Game.Entities
{
    internal class Entity : IEntity
    {
        public int Id { get; }
        public int PresenceSceneId { get; set; }

        public EntityType Type { get; }

        public IComponentsContainer Components { get; } = new ComponentsContainer();

        public Entity(int id, EntityType type)
        {
            Id = id;
            Type = type;
        }
    }
}
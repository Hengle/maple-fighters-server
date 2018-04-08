﻿using System.IO;
using CommonCommunicationInterfaces;

namespace Game.Common
{
    public struct ValidateCharacterResponseParameters : IParameters
    {
        public CharacterValidationStatus Status;

        public ValidateCharacterResponseParameters(CharacterValidationStatus status = CharacterValidationStatus.Wrong)
        {
            Status = status;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Status);
        }

        public void Deserialize(BinaryReader reader)
        {
            Status = (CharacterValidationStatus)reader.ReadByte();
        }
    }
}
﻿using System.IO;
using CommonCommunicationInterfaces;

namespace Shared.Game.Common
{
    public struct TestRequestParameters : IParameters
    {
        public int Number;

        public TestRequestParameters(int number)
        {
            Number = number;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Number);
        }

        public void Deserialize(BinaryReader reader)
        {
            Number = reader.ReadInt32();
        }
    }
}
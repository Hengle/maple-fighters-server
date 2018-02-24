﻿namespace Shared.Game.Common
{
    public enum GameOperations : byte
    {
        Authorize,
        EnterScene,
        ChangeScene,
        PositionChanged,
        PlayerStateChanged,
        ValidateCharacter
    }
}
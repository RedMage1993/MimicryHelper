using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MimicryHelper
{
    public enum MimicryRole { Tank = 1, Healer = 4, Dps = 2 };

    public interface MimicryMaster
    {
        const uint AethericMimicryActionID = 18322;

        void MimicRole(MimicryRole mimicryRole);
    }

    public sealed unsafe class Gogo : MimicryMaster
    {
        private List<PlayerCharacter> PlayerCharactersMatchingRole(MimicryRole mimicryRole)
        {
            var playerCharactersMatchingRole = new List<PlayerCharacter>();

            foreach (GameObject gameObject in Services.Objects)
            {
                if ((gameObject as PlayerCharacter)?.ClassJob.GameData?.Role == (byte)mimicryRole)
                {
                    playerCharactersMatchingRole.Add((gameObject as PlayerCharacter)!);
                }
            }

            return playerCharactersMatchingRole;
        }

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private PlayerCharacter? ClosestToLocalPlayer(List<PlayerCharacter> playerCharacters)
        {
            PlayerCharacter? closestPlayer = null;

            Vector3? myPosition = Services.ClientState.LocalPlayer?.Position;

            if (myPosition == null)
            {
                return null;
            }

            double smallestDistanceSoFar = -1;

            foreach (PlayerCharacter playerCharacter in playerCharacters)
            {
                var distance = GetDistance(playerCharacter.Position.X, playerCharacter.Position.Y, myPosition.Value.X, myPosition.Value.Y);

                if (distance < smallestDistanceSoFar || smallestDistanceSoFar < 0)
                {
                    smallestDistanceSoFar = distance;
                    closestPlayer = playerCharacter;
                }
            }

            return closestPlayer;
        }

        public void MimicRole(MimicryRole mimicryRole)
        {
            var closestPlayer = ClosestToLocalPlayer(PlayerCharactersMatchingRole(mimicryRole));

            if (closestPlayer == null)
            {
                Services.Chat.Print("Could not find a character to mimic.");
                return;
            }

            Services.Chat.Print($"Attempting to mimic {closestPlayer.Name}...");
            ActionManager.Instance()->UseAction(ActionType.Spell, MimicryMaster.AethericMimicryActionID, closestPlayer.ObjectId);
        }
    }
}

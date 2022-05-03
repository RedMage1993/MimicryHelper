using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MimicryHelper
{
    public enum MimicryRole { Tank = 1, MeleeDps = 2, RangedDps = 3, Healer = 4 };

    public interface IMimicryMaster
    {
        const uint AethericMimicryActionID = 18322;

        void MimicRole(List<MimicryRole> mimicryRoleList);
    }

    public sealed unsafe class Gogo : IMimicryMaster
    {
        private static List<PlayerCharacter> PlayerCharactersMatchingRole(MimicryRole mimicryRole)
        {
            var playerCharactersMatchingRole = new List<PlayerCharacter>();

            foreach (GameObject gameObject in Services.Objects)
            {
                PlayerCharacter? playerCharacter = gameObject as PlayerCharacter;

                if (playerCharacter == null || playerCharacter == Services.ClientState.LocalPlayer) {
                    continue;
                }

                if (playerCharacter.ClassJob.GameData?.Role == (byte)mimicryRole)
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

        private static PlayerCharacter? ClosestToLocalPlayer(List<PlayerCharacter> playerCharacters)
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

#if DEBUG
            if (closestPlayer != null)
            {
                Services.Chat.Print($"Found {closestPlayer.Name} at ({closestPlayer.Position.X}, {closestPlayer.Position.Y}) while you're at ({myPosition.Value.X}, {myPosition.Value.Y}), making distance = {smallestDistanceSoFar}...");
            }
#endif

            return closestPlayer;
        }

        public void MimicRole(List<MimicryRole> mimicryRoleList)
        {
            var playerCharactersMatchingRole = new List<PlayerCharacter>();
            foreach (MimicryRole mimicryRole in mimicryRoleList)
            {
                playerCharactersMatchingRole.AddRange(PlayerCharactersMatchingRole(mimicryRole));
            }

            var closestPlayer = ClosestToLocalPlayer(playerCharactersMatchingRole);

            if (closestPlayer == null)
            {
                Services.Chat.Print("Could not find a character to mimic.");
                return;
            }

            Services.Chat.Print($"Attempting to mimic {closestPlayer.Name}...");
            ActionManager.Instance()->UseAction(ActionType.Spell, IMimicryMaster.AethericMimicryActionID, closestPlayer.ObjectId);
        }
    }
}

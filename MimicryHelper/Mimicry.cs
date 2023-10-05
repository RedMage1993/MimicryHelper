using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MimicryHelper
{
    public interface IMimicryMaster
    {
        const uint AethericMimicryActionID = 18322;

        void MimicRole(IMimicryRole mimicryRole);
    }

    public sealed unsafe class Gogo : IMimicryMaster
    {
        private static readonly string[] CompassDirections = { "eastern", "northeastern", "northern", "northwestern", "western", "southwestern", "southern", "southeastern" };
        private static readonly double CompassDirectionPieceDegrees = 360.0 / CompassDirections.Length;

        private static List<PlayerCharacter> PlayerCharactersMatchingRole(MimicryRoleKind mimicryRoleKind)
        {
            var playerCharactersMatchingRole = new List<PlayerCharacter>();

            foreach (GameObject gameObject in Services.Objects)
            {
                PlayerCharacter? playerCharacter = gameObject as PlayerCharacter;

                if (playerCharacter == null || playerCharacter == Services.ClientState.LocalPlayer) {
                    continue;
                }

                if (playerCharacter?.ClassJob.GameData?.Role == (byte) mimicryRoleKind)
                {
                    playerCharactersMatchingRole.Add((gameObject as PlayerCharacter)!);
                }
            }

            return playerCharactersMatchingRole;
        }

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
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
                var distance = GetDistance(playerCharacter.Position.X, playerCharacter.Position.Z, myPosition.Value.X, myPosition.Value.Z);

                if (distance < smallestDistanceSoFar || smallestDistanceSoFar < 0)
                {
                    smallestDistanceSoFar = distance;
                    closestPlayer = playerCharacter;
                }
            }

#if DEBUG
            if (closestPlayer != null)
            {
                Services.Chat.Print($"Found {closestPlayer.Name} at ({closestPlayer.Position.X}, {closestPlayer.Position.Z}) while you're at ({myPosition.Value.X}, {myPosition.Value.Z}), making distance = {smallestDistanceSoFar}...");
            }
#endif

            return closestPlayer;
        }

        private static double PositiveModulo(double left, double right)
        {
            var remainder = left % right;                           
            return remainder >= 0 ? remainder : remainder + right;
        }

        private static double GetRelativeDirectionDegrees(double x1, double y1, double x2, double y2)
        {
            double direction = Math.Atan2(-(y2 - y1), x2 - x1) * 180.0 / Math.PI;

#if DEBUG
            Services.Chat.Print($"direction = {PositiveModulo(direction, 360.0)}");
#endif

            return PositiveModulo(direction, 360);
        }

        private static String GetRelativeCompassDirection(PlayerCharacter character)
        {

            Vector3? myPosition = Services.ClientState.LocalPlayer?.Position;

            if (myPosition == null)
            {
                return "unknown";
            }

            double relativeDirectionDegrees = GetRelativeDirectionDegrees(myPosition.Value.X, myPosition.Value.Z, character.Position.X, character.Position.Z);

            return CompassDirections[((int) Math.Round(relativeDirectionDegrees / CompassDirectionPieceDegrees)) % CompassDirections.Length];
        }

        public void MimicRole(IMimicryRole mimicryRole)
        {
            var playerCharactersMatchingRole = new List<PlayerCharacter>();
            foreach (MimicryRoleKind mimicryRoleKind in mimicryRole.RoleKinds)
            {
                playerCharactersMatchingRole.AddRange(PlayerCharactersMatchingRole(mimicryRoleKind));
            }

            var closestPlayer = ClosestToLocalPlayer(playerCharactersMatchingRole);

            if (closestPlayer == null)
            {
                Services.Chat.Print("Could not find a character to mimic.");
                return;
            }

            Services.Chat.Print($"Attempting to mimic {closestPlayer.Name} in the {GetRelativeCompassDirection(closestPlayer)} direction...");
            ActionManager.Instance()->UseAction(ActionType.Action, IMimicryMaster.AethericMimicryActionID, closestPlayer.ObjectId);
        }
    }
}

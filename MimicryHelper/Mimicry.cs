using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

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
        public void MimicRole(MimicryRole mimicryRole)
        {
            foreach (GameObject gameObject in Services.Objects)
            {
                if ((gameObject as PlayerCharacter)?.ClassJob.GameData?.Role == (byte) mimicryRole)
                {
                    Services.Chat.Print($"Attempting to mimic {gameObject.Name}...");
                    ActionManager.Instance()->UseAction(ActionType.Spell, MimicryMaster.AethericMimicryActionID, gameObject.ObjectId);
                    break;
                }
            }
        }
    }
}

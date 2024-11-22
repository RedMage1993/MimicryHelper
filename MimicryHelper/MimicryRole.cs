using System.Collections.Generic;

namespace MimicryHelper
{
    public enum MimicryRoleKind { Tank = 1, MeleeDps = 2, RangedDps = 3, Healer = 4 };

    public interface IMimicryRole
    {
        List<MimicryRoleKind> RoleKinds { get; }
    }

    public sealed class TankMimicryRole : IMimicryRole
    {
        public List<MimicryRoleKind> RoleKinds { get { return [MimicryRoleKind.Tank]; } }
    }

    public sealed class DpsMimicryRole : IMimicryRole
    {
        public List<MimicryRoleKind> RoleKinds { get { return [MimicryRoleKind.MeleeDps, MimicryRoleKind.RangedDps]; } }
    }

    public sealed class HealerMimicryRole : IMimicryRole
    {
        public List<MimicryRoleKind> RoleKinds { get { return [MimicryRoleKind.Healer]; } }
    }
}

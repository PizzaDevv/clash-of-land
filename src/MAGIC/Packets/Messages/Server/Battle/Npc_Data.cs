using ClashLand.Extensions.List;
using ClashLand.Files;
using ClashLand.Logic;
using ClashLand.Logic.Enums;

namespace ClashLand.Packets.Messages.Server.Battle
{
    internal class Npc_Data : Message
    {
        internal int Npc_ID = 0;
        internal Level Avatar;

        public Npc_Data(Device _Device) : base(_Device)
        {
            this.Identifier = 24133;
        }

        internal override void Encode()
        {
            using (Objects Home = new Objects(Avatar = this.Device.Player, NPC.Levels[Npc_ID]))
            {
                this.Data.AddInt(0);
                this.Data.AddRange(Home.ToBytes);
                this.Data.AddRange(this.Device.Player.Avatar.ToBytes);

                this.Data.AddInt(this.Npc_ID);
                this.Data.AddByte(0);
            }
        }

        internal override void Process()
        {
            this.Device.State = State.IN_NPC_BATTLE;
        }
    }
}
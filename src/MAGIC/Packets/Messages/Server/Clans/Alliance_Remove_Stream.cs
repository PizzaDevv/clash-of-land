using ClashLand.Extensions.List;
using ClashLand.Logic;

namespace ClashLand.Packets.Messages.Server.Clans
{
    internal class Alliance_Remove_Stream : Message
    {
        internal long Message_ID = 0;
        
        internal Alliance_Remove_Stream(Device _Device) : base(_Device)
        {
            this.Identifier = 24318;
        }

        internal override void Encode()
        {
            this.Data.AddLong(this.Message_ID);
        }
    }
}

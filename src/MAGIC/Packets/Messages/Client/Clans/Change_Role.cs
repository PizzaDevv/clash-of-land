using ClashLand.Core;
using ClashLand.Core.Networking;
using ClashLand.Extensions;
using ClashLand.Extensions.Binary;
using ClashLand.Logic;
using ClashLand.Logic.Enums;
using ClashLand.Logic.Structure.Slots.Items;
using ClashLand.Packets.Commands.Server;
using ClashLand.Packets.Messages.Server;
using ClashLand.Packets.Messages.Server.Clans;

namespace ClashLand.Packets.Messages.Client.Clans
{
    internal class Change_Role : Message
    {
        internal long UserID;
        internal Role Role;

        public Change_Role(Device device) : base(device)
        { 
        }

        internal override void Decode()
        {
            this.UserID = this.Reader.ReadInt64();
            this.Role = (Role) this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            Level Player = Players.Get(this.UserID, false);
            Clan Alliance = Resources.Clans.Get(this.Device.Player.Avatar.ClanId, false);
            if (Alliance != null && Player != null)
            {
                Role JudgeRole = Alliance.Members[this.Device.Player.Avatar.UserId].Role;
                Role DefendantRole = Alliance.Members[Player.Avatar.UserId].Role;
                if (JudgeRole == Role.Leader || JudgeRole == Role.Co_Leader)
                {
                    if (this.Role == Role.Leader)
                    {
                        if (JudgeRole == Role.Leader)
                        {

                            Alliance.Members[Player.Avatar.UserId].Role = Role.Leader;
                            Alliance.Members[this.Device.Player.Avatar.UserId].Role = Role.Co_Leader;

                            Alliance.Chats.Add(
                                new Entry
                                {
                                    Stream_Type = Alliance_Stream.EVENT,
                                    Sender_ID = Player.Avatar.UserId,
                                    Sender_Name = Player.Avatar.Name,
                                    Sender_Level = Player.Avatar.Level,
                                    Sender_League = Player.Avatar.League,
                                    Sender_Role = Role.Leader,
                                    Event_ID = Events.PROMOTE_MEMBER,
                                    Event_Player_Name = this.Device.Player.Avatar.Name,
                                    Event_Player_ID = this.Device.Player.Avatar.UserId

                                });
                            Alliance.Chats.Add(
                                new Entry
                                {
                                    Stream_Type = Alliance_Stream.EVENT,
                                    Sender_ID = this.Device.Player.Avatar.UserId,
                                    Sender_Name = this.Device.Player.Avatar.Name,
                                    Sender_Level = this.Device.Player.Avatar.Level,
                                    Sender_League = this.Device.Player.Avatar.League,
                                    Sender_Role = Role.Co_Leader,
                                    Event_ID = Events.DEPROMOTE_MEMBER,
                                    Event_Player_Name = this.Device.Player.Avatar.Name,
                                    Event_Player_ID = this.Device.Player.Avatar.UserId,

                                });

                            if (Player.Device != null)
                                new Server_Commands(Player.Device)
                                {
                                    Command = new Role_Update(Player.Device)
                                        {
                                            Clan = Alliance,
                                            Role = (int) Role.Leader
                                        }
                                        .Handle()
                                }.Send();

                            new Alliance_Change_Role_Ok(this.Device)
                                {
                                    UserID = Player.Avatar.UserId,
                                    Role = Role.Leader
                                }
                                .Send();

                            new Alliance_Change_Role_Ok(this.Device)
                            {
                                UserID = this.Device.Player.Avatar.UserId,
                                Role = Role.Co_Leader
                            }.Send();

                            new Server_Commands(this.Device)
                            {
                                Command = new Role_Update(this.Device)
                                    {
                                        Clan = Alliance,
                                        Role = (int) Role.Co_Leader
                                    }
                                    .Handle()
                            }.Send();
                        }
                    }
                    else
                    {
                        Alliance.Members[Player.Avatar.UserId].Role = this.Role;
                        if (Player.Device != null)
                            new Server_Commands(Player.Device)
                            {
                                Command = new Role_Update(Player.Device)
                                {
                                    Clan = Alliance,
                                    Role = (int)this.Role
                                }.Handle()
                            }.Send();

                        if (this.Device != null)
                            new Alliance_Change_Role_Ok(this.Device)
                            {
                                UserID = Player.Avatar.UserId,
                                Role = this.Role
                            }.Send();

                        Alliance.Chats.Add(
                            new Entry
                            {
                                Stream_Type = Alliance_Stream.EVENT,
                                Sender_ID = Device.Player.Avatar.UserId,
                                Sender_Name = Device.Player.Avatar.Name,
                                Sender_Level = Device.Player.Avatar.Level,
                                Sender_League = Player.Avatar.League,
                                Sender_Role = JudgeRole,
                                Event_ID = this.Role > DefendantRole ? Events.PROMOTE_MEMBER : Events.DEPROMOTE_MEMBER,
                                Event_Player_Name = this.Device.Player.Avatar.Name,
                                Event_Player_ID = this.Device.Player.Avatar.UserId
                            });
                    }
                }
            }
        }
    }
}

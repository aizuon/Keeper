namespace Keeper.Common
{
    public enum Opcode : byte
    {
        S2CConnectionHint,
        C2SKeyExchange,
        S2CKeyExchangeSuccess,
        LoginReq,
        LoginAck,
        RegisterReq,
        RegisterAck
    }
}

namespace Keeper.Common
{
    public enum Opcode : byte
    {
        C2SKeyExchange,
        S2CKeyExchangeSuccess,
        LoginReq,
        LoginAck,
        RegisterReq,
        RegisterAck
    }
}

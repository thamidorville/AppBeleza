namespace BelezaApp.API.Helpers;

using HashidsNet;

public static class HashIdHelper
{
    private static readonly Hashids _hashids = new Hashids("BelezaAppSalt2026", 8);

    public static string Encode(int id)
    {
        return _hashids.Encode(id);
    }

    public static int Decode(string hash)
    {
        var ids = _hashids.Decode(hash);
        return ids.Length > 0 ? ids[0] : 0;
    }
}

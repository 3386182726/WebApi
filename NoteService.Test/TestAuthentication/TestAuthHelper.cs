using System.Security.Claims;

namespace NoteService.Test.TestAuthentication;

public static class TestAuthHelper
{
    public static string SerializeClaims(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims);

        using MemoryStream memoryStream = new();
        using BinaryWriter writer = new(memoryStream);

        identity.WriteTo(writer);
        writer.Flush();

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static Claim[] DeserializeClaims(string serializedClaim)
    {
        using MemoryStream memoryStream = new(Convert.FromBase64String(serializedClaim));
        using BinaryReader reader = new(memoryStream);

        var identity = new ClaimsIdentity(reader);
        return identity.Claims.ToArray();
    }
}

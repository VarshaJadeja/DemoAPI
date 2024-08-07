namespace Demo.Services.Services;

public interface IEncryptionService
{
    public string Encode(string encodeMe);
    public string Decode(string decodeMe);
}
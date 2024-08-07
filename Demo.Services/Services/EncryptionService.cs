namespace Demo.Services.Services;

public class EncryptionService : IEncryptionService
{
    public string Encode(string inputString)
    {
        byte[] encoded = System.Text.Encoding.UTF8.GetBytes(inputString);
        return Convert.ToBase64String(encoded);
    }

    public string Decode(string inputString)
    {
        // Check if the inputString is a valid Base64 string
        if (!IsBase64String(inputString))
        {
            return null;
        }

        byte[] encoded = Convert.FromBase64String(inputString);

        return System.Text.Encoding.UTF8.GetString(encoded);
    }
    private bool IsBase64String(string inputString)
    {
        // Validate length (Base64 encoded strings should be a multiple of 4)
        if (inputString.Length % 4 != 0)
        {
            return false;
        }

        // Check if the string contains only valid Base64 characters
        return System.Text.RegularExpressions.Regex.IsMatch(inputString, @"^[a-zA-Z0-9+/]*={0,2}$");
    }
}

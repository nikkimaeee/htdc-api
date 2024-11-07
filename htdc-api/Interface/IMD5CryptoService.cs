namespace htdc_api.Interface;

public interface IMD5CryptoService
{
    string Encrypt(string text);
    string Decrypt(string cipher);
}
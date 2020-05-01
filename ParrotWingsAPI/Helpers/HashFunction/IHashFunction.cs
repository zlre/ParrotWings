namespace ParrotWingsAPI
{
    public interface IHashFunction
    {
        string Hash(byte[] salt, string password);
    }
}

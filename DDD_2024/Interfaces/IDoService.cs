namespace DDD_2024.Interfaces
{
    public interface IDoService
    {
        string GetStatusName(string? doStatus);
        string GetProjectID(string date);
        string GetDOID(string date);
    }
}

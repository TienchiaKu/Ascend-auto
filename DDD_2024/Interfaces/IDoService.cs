namespace DDD_2024.Interfaces
{
    public interface IDoService
    {
        int NewProjectEmpSEQ { get; }

        string GetStatusName(string? doStatus);
        string GetProjectID(string date);
        string GetDOID(string date);
        bool chk_DoTransDin(string projectID);
    }
}

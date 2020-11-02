using System.Threading.Tasks;

namespace Orleans.Grains
{
    public interface ISessionControlGrain : IGrainWithStringKey
    {
        Task Login(string userId);
        Task Logout(string userId);
        Task<int> GetActiveUserCount();
    }
}

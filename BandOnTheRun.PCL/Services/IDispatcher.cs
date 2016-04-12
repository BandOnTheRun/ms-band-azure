using System;
using System.Threading.Tasks;

namespace BandOnTheRun.PCL.Services
{
    public interface IDispatcher
    {
        Task RunAsync(Action action);
    }
}

using System;

namespace MSBandAzure.Services
{
    public interface INavigationService
    {
        void Navigate(string target, object param = null);
    }
}

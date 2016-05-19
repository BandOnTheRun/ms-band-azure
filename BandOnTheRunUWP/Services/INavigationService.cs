using System;

namespace MSBandAzure.Services
{
    public interface INavigationService
    {
        void Navigate(Type target, object param = null);
    }
}

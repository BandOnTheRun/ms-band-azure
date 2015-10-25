using Microsoft.Band;
using Microsoft.Band.Personalization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSBandAzure.Services.Fakes
{
    public class FakeBandPersonalizationManager : IBandPersonalizationManager
    {
        public Task<BandImage> GetMeTileImageAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BandImage> GetMeTileImageAsync(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public Task<BandTheme> GetThemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BandTheme> GetThemeAsync(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public Task SetMeTileImageAsync(BandImage image)
        {
            throw new NotImplementedException();
        }

        public Task SetMeTileImageAsync(BandImage image, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }

        public Task SetThemeAsync(BandTheme theme)
        {
            throw new NotImplementedException();
        }

        public Task SetThemeAsync(BandTheme theme, CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}

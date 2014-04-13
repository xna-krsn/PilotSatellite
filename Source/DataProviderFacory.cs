using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataProvider
{
    public class DataProviderFactory
    {
        public static NasaDataProvider CreateNasaDataProvider(SolarSystemDataManager dataManager)
        {
            return new NasaDataProvider(dataManager.GetNasaKeys());
        }

        public static MoviesDataProvider CreateMoviesDataProvider(SolarSystemDataManager dataManager)
        {
            return new MoviesDataProvider(dataManager.GetNasaKeys().Keys.ToList());
        }
    }
}

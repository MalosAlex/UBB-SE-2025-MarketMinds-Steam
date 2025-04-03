using SteamProfile.Data;
using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace SteamProfile.Repositories
{
    public class PointsOffersRepository
    {
        public List<PointsOffer> Offers = [];
        public PointsOffersRepository()
        {
            Offers.Add(new PointsOffer(2, 5));
            Offers.Add(new PointsOffer(8, 25));
            Offers.Add(new PointsOffer(15, 50));
            Offers.Add(new PointsOffer(20, 100));
            Offers.Add(new PointsOffer(50, 500));

        }

    }
}

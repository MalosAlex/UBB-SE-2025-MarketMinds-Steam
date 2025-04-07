using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class PointsOffersRepository : IPointsOffersRepository
    {
        public List<PointsOffer> Offers { get; } = [];

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
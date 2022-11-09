using Bogus;

namespace webapi.Controllers
{
    public class NewsController
    {
        public record NewsOverviewDto(int Id, string Headline, string ImageUrl);
        public record NewsDetailDto (int Id, string Headline, string Content);

        public List<NewsOverviewDto> GetAllNews()
        {
            string[] images = new string[]
            {
                "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA13UCsv.img?w=612&h=304&q=90&m=6&f=jpg&u=t",
                "https://www.bing.com/th?id=ORMS.c64be9536fb2ebb5673dfc61d8142abe&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
                "https://www.bing.com/th?id=ORMS.805cf20c3f313d9d74bf2cfc96fc7e00&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
                "https://www.bing.com/th?id=ORMS.430a52f4ed5a6e63b0a376680541e024&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0"
            };

            int rowNr = 1000;
            Randomizer.Seed = new Random(1529);
            var news = new Faker<NewsOverviewDto>("de").CustomInstantiator(f =>
            {
                return new NewsOverviewDto(
                    Id: rowNr++,
                    Headline: f.Person.LastName,
                    ImageUrl: f.Random.ListItem(images));
            })
            .Generate(10)
            .ToList();
            return news;
        }

        public NewsDetailDto GetNewsDetail(int id)
        {
            Randomizer.Seed = new Random(1529);
            var f = new Faker("de");

            return new NewsDetailDto(
                Id: id,
                Headline: f.Commerce.ProductMaterial(),
                Content: f.Commerce.ProductDescription());
        }
    }
}

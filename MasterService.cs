using Microsoft.EntityFrameworkCore;
using SSSSProject.Data;
using SSSSProject.Models;

namespace Service
{
    public interface IMasterService
    {
        Task<List<Keywords>> GetProductDetail();
    }
    public class MasterService : IMasterService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SSSSProjectContext _sSSSProjectContext;
        private readonly IConfiguration configuration;

        public MasterService(IHttpContextAccessor httpContextAccessor, SSSSProjectContext _sSSSProjectContext, IConfiguration configuration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._sSSSProjectContext = _sSSSProjectContext;
            this.configuration = configuration;
        }

        public async Task<List<Keywords>> GetProductDetail()
        {
            var productDetail = await _sSSSProjectContext.ProductDetail.Select(s => new ProductDetail
            {
                ProductId = s.ProductId,
                ProductColor = s.ProductColor,
                ProductName = s.ProductName,
                ProductSex = s.ProductSex,
                ProductType = s.ProductType,
            }).ToListAsync();
            var keywords = productDetail
    .GroupBy(g => new { g.ProductType, g.ProductSex, g.ProductColor })
    .Select(s => new Keywords
    {
        KeywordsName = s.Select(s => s.ProductName).Distinct().ToList(),
        KeywordsType = new List<string> { s.Key.ProductType }.Distinct().ToList(),
        KeywordsSex = new List<string> { s.Key.ProductSex }.Distinct().ToList(),
        KeywordsColor = new List<string> { s.Key.ProductColor }.Distinct().ToList()
    })
    .GroupBy(k => true)
    .Select(g => new Keywords
    {
        KeywordsName = g.SelectMany(k => k.KeywordsName).Distinct().OrderBy(n => n).ToList(),
        KeywordsType = g.SelectMany(k => k.KeywordsType).Distinct().OrderBy(n => n).ToList(),
        KeywordsSex = g.SelectMany(k => k.KeywordsSex).Distinct().OrderBy(n => n).ToList(),
        KeywordsColor = g.SelectMany(k => k.KeywordsColor).Distinct().OrderBy(n => n).ToList()
    })
    .ToList();
            string input = "i want a black sneaker for male ";

            //var keywords = new List<Keywords>
            //{
            //    // your keywords data here
            //};

            // Split the input into keywords
            var inputKeywords = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x != "i" && x != "want" && x != "a" && x != "an" && x != "with" && x != "or")
                .ToList();

            // Filter the keywords data based on the input keywords
            var result = keywords
                .Select(k => new
                {
                    Types = k.KeywordsType.Intersect(inputKeywords),
                    Colors = k.KeywordsColor.Intersect(inputKeywords),
                    Names = k.KeywordsName.Intersect(inputKeywords),
                    Sexs = k.KeywordsSex.Intersect(inputKeywords)

                })
                .FirstOrDefault(r => r.Types.Any() && r.Colors.Any());

            // Extract the relevant information from the result
            if (result != null)
            {
                var comparedtypes = result.Types.ToList();
                var comparedcolors = result.Colors.ToList();
                var comparedsexs = result.Sexs.ToList();
                var comparedNames = result.Names.ToList();
                // Do something with the extracted information
            }


            return keywords.ToList();
        }


        //public Task<ProductDetail> GetProductDetail()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
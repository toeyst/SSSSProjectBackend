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

            return keywords.ToList();
        }


        //public Task<ProductDetail> GetProductDetail()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
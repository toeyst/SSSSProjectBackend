using Microsoft.EntityFrameworkCore;
using SSSSProject.Data;
using SSSSProject.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;

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
            string input = "i want a black  or red sneaker for male ";

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
                string concatenatedStringColor = "";
                foreach (string str in comparedcolors)
                {
                    concatenatedStringColor += str + ";"; // use a semicolon as a separator
                }

                // Remove the trailing separator
                if (!string.IsNullOrEmpty(concatenatedStringColor))
                {
                    concatenatedStringColor = concatenatedStringColor.Substring(0, concatenatedStringColor.Length - 1);
                    string[] colorArray = concatenatedStringColor.Split(';');

                    List<ProductDetail> selectedProducts = (from p in productDetail
                                                            where colorArray.Contains(p.ProductColor)
                                                            select new ProductDetail
                                                            {
                                                                ProductColor = p.ProductColor,
                                                                ProductName = p.ProductName,
                                                                ProductSex = p.ProductSex,
                                                                ProductType = p.ProductType
                                                            }).ToList();
                }
                // Do something with the extracted information
                //string color_query = "";
                //if (comparedcolors.Count > 0)
                //{
                //    string colors = string.Join(",", comparedcolors);
                //    color_query = $"ProductColor in ({colors})";
                //}

                //string query = "SELECT * FROM ProductDetail WHERE ProductType=@Type AND ProductColor IN @Colors";
                //// Create a connection to the database
                //string connectionString = "Data Source=LAPTOP-B6ILTE6S;Initial Catalog=SSSSProject;Application Name = SSSSProject;trusted_connection=true;encrypt=false";
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    // Open the connection
                //    connection.Open();

                //    // Create a command object with the SQL query and the connection


                //    using (SqlCommand command = new SqlCommand(query, connection))
                //    {
                //        // Execute the query and get the results
                //        command.Parameters.AddWithValue("@Type", comparedtypes);
                //        command.Parameters.AddWithValue("@Colors", comparedcolors.ToArray());
                //        using (SqlDataReader reader = command.ExecuteReader())
                //        {
                //            // Print the results to the console
                //            while (reader.Read())
                //            {
                //                Console.WriteLine($"ID: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Type: {reader.GetString(2)}, Color: {reader.GetString(3)}, Sex: {reader.GetString(4)}, Price: {reader.GetDecimal(5)}");
                //            }
                //        }
                //    }
                //}
                //var lin = _sSSSProjectContext.ProductDetail
                //    .Where(p => comparedtypes.Contains(p.ProductType) && comparedcolors.Contains(p.ProductColor))
                //    .ToList();

                //string query = $"SELECT * FROM ProductDetail WHERE ProductType='{comparedtypes}' and ProductColor in ({color_query})";
            }





            return keywords.ToList();
        }


        //public Task<ProductDetail> GetProductDetail()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
namespace PrometheusDemo.Models
{
    // ProductRequestModel.cs
    public class ProductRequestModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // ProductResponseModel.cs
    public class ProductResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class GetProductRequestModel
    {
        public GetProductRequestModel()
        {
            // Set default values in the constructor
            BSESuccessFlag = true;
            InvestWellSuccessFlag = true;
            ErrorFlag = true;
            DbSource = "sql";
        }

        public bool BSESuccessFlag { get; set; }
        public bool InvestWellSuccessFlag { get; set; }
        public bool ErrorFlag {  get; set; }
        public string DbSource { get; set; }
    }

}

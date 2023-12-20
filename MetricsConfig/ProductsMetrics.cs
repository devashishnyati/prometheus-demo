using Prometheus;

namespace PrometheusDemo.MetricsConfig
{
    public class ProductsMetrics
    {
        public static readonly Counter GetBSEStarMFProductsError = Metrics.CreateCounter("get_bse_star_mf_products_error", "Errors in BSE Star MF API");
        public static readonly Counter GetInvestwellProductsError = Metrics.CreateCounter("get_investwellproducts_error", "Errors in Investwell API");
        public static readonly Counter GetProductsDBError = Metrics.CreateCounter("get_products_db_error", "Errors in getting produts from DB", labelNames: new[] { "db_source" });
        public static readonly Counter GetBSEStarMFProductsSuccess = Metrics.CreateCounter("get_bse_star_mf_products_success", "Success in BSE Star MF API");
        public static readonly Counter GetInvestwellProductsSuccess = Metrics.CreateCounter("get_investwellproducts_success", "Success in Investwell API");
        public static readonly Counter GetProductsDBSuccess = Metrics.CreateCounter("get_products_db_success", "Success in getting produts from DB", labelNames: new[] { "db_source" });

        public static readonly Histogram GetBSEStarMFProductsDuration = Metrics.CreateHistogram("get_bse_star_mf_products_duration", "Histogram of bse star mf poducts durations.");
        public static readonly Histogram GetInvestwellProductsDuration = Metrics.CreateHistogram("get_investwell_products_duration", "Histogram of bse star mf poducts durations.");

    }
}

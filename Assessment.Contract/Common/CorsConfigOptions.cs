namespace Assessment.Contract.Common
{
    public class CorsConfigOptions
    {
        public string[] Origins { get; set; }
        public string[] Headers { get; set; }
        public string[] ExposedHeaders { get; set; }
        public string[] Methods { get; set; }
    }
}

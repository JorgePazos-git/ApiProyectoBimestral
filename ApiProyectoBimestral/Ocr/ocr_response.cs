namespace ApiProyectoBimestral.Ocr
{
    public class ocr_response
    {
        public string language { get; set; }
        public decimal textAngle { get; set; }
        public string orientation { get; set; }
        public region[] regions { get; set; }
        public string modelVersion { get; set; }
    }
}

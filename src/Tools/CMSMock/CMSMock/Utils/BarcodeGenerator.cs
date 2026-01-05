namespace CMSMock.Utils
{
    public class BarcodeGenerator
    {
        public static string GenerateBarcode()
        {
            int length = 18;

            var random = new Random();
            var number = string.Empty;

            for (int i = 0; i < length; i++)
            {
                number += random.Next(0, 10).ToString();
            }

            return number;
        }
    }
}

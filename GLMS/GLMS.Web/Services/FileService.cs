namespace GLMS.Web.Services
{
    public class FileService
    {
        public bool IsValidPdf(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.ContentType != "application/pdf")
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".pdf")
                return false;

            return true;
        }
    }
}
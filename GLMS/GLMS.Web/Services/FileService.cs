namespace GLMS.Web.Services
{
    public class FileService
    {
        private readonly string _uploadFolder;

        public FileService(IWebHostEnvironment env)
        {
            _uploadFolder = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(_uploadFolder);
        }

        public async Task<string> SaveAgreementAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file provided.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".pdf")
                throw new InvalidOperationException("Only PDF files are permitted.");

            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var destination = Path.Combine(_uploadFolder, uniqueName);

            using var stream = new FileStream(destination, FileMode.Create);
            await file.CopyToAsync(stream);

            return uniqueName;
        }

        public static bool IsValidPdf(IFormFile? file)
        {
            if (file == null || file.Length == 0) return false;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension == ".pdf";
        }
    }
}

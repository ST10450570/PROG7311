namespace GLMS.Api.Services
{
    public class FileService
    {
        public bool IsValidPdf(Stream? stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (stream.Length < 4) return false;
            var header = new byte[4];
            stream.Read(header, 0, 4);
            stream.Position = 0;
            return header[0] == '%' && header[1] == 'P' && header[2] == 'D' && header[3] == 'F';
        }

        public bool ValidateFileSize(long fileSizeInBytes, long maxSizeInBytes)
        {
            if (fileSizeInBytes < 0) return false;
            return fileSizeInBytes <= maxSizeInBytes;
        }
    }
}
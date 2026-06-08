using GLMS.Web.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GLMS.Tests
{
    public class FileValidationTests
    {
        private static IFormFile MakeFormFile(string fileName, string contentType = "application/octet-stream")
        {
            var content = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF magic bytes
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Fact]
        public void IsValidPdf_ValidPdfFile_ReturnsTrue()
        {
            var file = MakeFormFile("agreement.pdf", "application/pdf");
            Assert.True(FileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_ExeFile_ReturnsFalse()
        {
            var file = MakeFormFile("malware.exe", "application/octet-stream");
            Assert.False(FileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_DocxFile_ReturnsFalse()
        {
            var file = MakeFormFile("contract.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            Assert.False(FileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_NullFile_ReturnsFalse()
        {
            Assert.False(FileService.IsValidPdf(null));
        }

        [Fact]
        public void IsValidPdf_EmptyFile_ReturnsFalse()
        {
            var stream = new MemoryStream(Array.Empty<byte>());
            var file = new FormFile(stream, 0, 0, "file", "empty.pdf");
            Assert.False(FileService.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_PdfExtensionUpperCase_ReturnsTrue()
        {
            var file = MakeFormFile("AGREEMENT.PDF", "application/pdf");
            Assert.True(FileService.IsValidPdf(file));
        }
    }
}

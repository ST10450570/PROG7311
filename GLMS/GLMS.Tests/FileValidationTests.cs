using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GLMS.Api.Services;
using Xunit;

namespace GLMS.Tests;

public class FileValidationTests
{
    private readonly FileService _fileService;

    public FileValidationTests()
    {
        _fileService = new FileService();
    }

    [Fact]
    public void IsValidPdf_ValidPdfHeader_ReturnsTrue()
    {
        // Arrange
        var pdfContent = "%PDF-1.4\n%âãÏÓ\nThis is a minimal PDF file content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(pdfContent));

        // Act
        var result = _fileService.IsValidPdf(stream);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPdf_InvalidHeader_ReturnsFalse()
    {
        // Arrange
        var textContent = "This is just a text file, not a PDF";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(textContent));

        // Act
        var result = _fileService.IsValidPdf(stream);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidPdf_EmptyStream_ReturnsFalse()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        var result = _fileService.IsValidPdf(stream);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidPdf_NullStream_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _fileService.IsValidPdf(null!));
    }

    [Fact]
    public void ValidateFileSize_WithinLimit_ReturnsTrue()
    {
        // Arrange
        long fileSizeInBytes = 5 * 1024 * 1024; // 5 MB
        long maxSizeInBytes = 10 * 1024 * 1024; // 10 MB

        // Act
        var result = _fileService.ValidateFileSize(fileSizeInBytes, maxSizeInBytes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateFileSize_ExceedsLimit_ReturnsFalse()
    {
        // Arrange
        long fileSizeInBytes = 15 * 1024 * 1024; // 15 MB
        long maxSizeInBytes = 10 * 1024 * 1024; // 10 MB

        // Act
        var result = _fileService.ValidateFileSize(fileSizeInBytes, maxSizeInBytes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateFileSize_ExactlyAtLimit_ReturnsTrue()
    {
        // Arrange
        long maxSizeInBytes = 10 * 1024 * 1024; // 10 MB

        // Act
        var result = _fileService.ValidateFileSize(maxSizeInBytes, maxSizeInBytes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateFileSize_NegativeSize_ReturnsFalse()
    {
        // Arrange
        long fileSizeInBytes = -1;
        long maxSizeInBytes = 10 * 1024 * 1024;

        // Act
        var result = _fileService.ValidateFileSize(fileSizeInBytes, maxSizeInBytes);

        // Assert
        Assert.False(result);
    }
}
using FileViewer.Grpc;
using Grpc.Core;
using GrpcFileInfo = FileViewer.Grpc.FileInfo;

namespace FileViewer.GrpcService.Services;

public class FileServiceImpl : FileService.FileServiceBase
{
    private readonly IConfiguration _configuration;

    public FileServiceImpl(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override Task<FileListResponse> GetFiles(
        EmptyRequest request,
        ServerCallContext context)
    {
        var folderPath =
            _configuration["FileSettings:DirectoryPath"];

        var response =
            new FileListResponse();

        if (Directory.Exists(folderPath))
        {
            var files =
                Directory.GetFiles(folderPath);

            foreach (var file in files)
            {
                response.Files.Add(
                  new FileViewer.Grpc.FileInfo()
                  {
                      FileName = Path.GetFileName(file)
                  }
                );
            }
        }

        return Task.FromResult(response);
    }

    public override async Task<FileContentResponse>
    GetFileContent(
        FileRequest request,
        ServerCallContext context)
    {
        var folderPath =
            _configuration["FileSettings:DirectoryPath"];

        Console.WriteLine($"Folder: {folderPath}");
        Console.WriteLine($"Requested File: {request.FileName}");

        var filePath =
            Path.Combine(folderPath, request.FileName);

        Console.WriteLine($"Full Path: {filePath}");
        Console.WriteLine($"Exists: {File.Exists(filePath)}");

        string content = "";

        if (File.Exists(filePath))
        {
            content = await File.ReadAllTextAsync(filePath);

            Console.WriteLine($"Content Read: {content}");
        }
        else
        {
            content = "Unable to locate the selected file.";
        }

        Console.WriteLine($"Returning: {content}");

        return new FileContentResponse
        {
            FileName = request.FileName,
            Content = content
        };
    }
using CommandLine;
using System.IO;

namespace Estranged.Workshop.Options
{
    [Verb("upload", HelpText = "Upload workshop items.")]
    internal sealed class UploadOptions
    {
        [Option('d', "directory", HelpText = "The directory to upload to the Steam workshop.", Required = true)]
        public DirectoryInfo UploadDirectory { get; set; }
    }
}


namespace ReaderWriterCsv
{
    public static class CurrentDirectory
    {
        public static string Get(int depth)
        {
            var startDirectory = new DirectoryInfo(Directory.GetCurrentDirectory())
                .Parent;

            var i = 0;
            string currentDirectory = "";
            do
            {
                if (startDirectory != null)
                {
                    i++;
                    currentDirectory = startDirectory.FullName;
                    startDirectory = startDirectory.Parent;
                }
            }
            while (i < depth);

            return currentDirectory;
        }
    }
}

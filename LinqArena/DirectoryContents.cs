namespace LinqArena
{
    internal class DirectoryContents
    {
        public static void ShowFilesInLoop(string s)
        {
            var dir = new DirectoryInfo(s);
            FileInfo[] files = dir.GetFiles();
            Array.Sort(files, new FileInfoComparer());
            for (var i = 0; i < 5; i++)
            {
                var file = files[i];
                Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");
            }
        }

        public static void ShowFilesQs(string s)
        {
            var files =
                from f in new DirectoryInfo(s).GetFiles()
                orderby f.Length descending
                select f;

            foreach (var file in files.Take(5))
            {
                Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");
            }
        }

        public static void ShowFilesMs(string s)
        {
            var files =
                new DirectoryInfo(s).GetFiles()
                    .OrderByDescending(f => f.Length)
                    .Take(5);

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Name,-20} : {file.Length,10:N0}");
            }
        }

        public static void IntroLinq()
        {
            string path = @"c:\windows";
            ShowFilesInLoop(path);
            Console.WriteLine("***");

            ShowFilesQs(path);
            Console.WriteLine("***");

            ShowFilesMs(path);
            Console.WriteLine("***");
        }
    }
}

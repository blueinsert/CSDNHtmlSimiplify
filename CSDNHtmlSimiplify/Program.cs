using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CSDNHtmlSimiplify
{
    internal class Program
    {
        private static void SubstractNode4CSDN(HtmlDocument doc)
        {
            List<HtmlNode> toRemoveNode = new List<HtmlNode>();

            List<string> toRemoveNodePathList = new List<string>() {
                "//div[@id='toolbarBox']",
                "//div[@id='asideProfile']",
                "//div[@id='asideHotArticle']",
                "//div[@id='asideNewComments']",
                "//div[@id='asideArchive']",
                "//div[@class='aside-box kind_person d-flex flex-column']",
                "//div[@class='csdn-side-toolbar']",
                "//div[@class='blog-footer-bottom']",
                "//div[@class='first-recommend-box recommend-box']",
                "//div[@class='second-recommend-box recommend-box']",
                "//div[@class='recommend-box insert-baidu-box recommend-box-style']",
                "//div[@class='left-toolbox']",
                "//div[@id='treeSkill']",
                "//div[@id='asideSearchArticle']",
                "//div[@id='asideWriteGuide']",
                "//div[@class='article-info-box']",
            };
            for (int i = 0; i < toRemoveNodePathList.Count; i++)
            {
                HtmlNode node = doc.DocumentNode.SelectSingleNode(toRemoveNodePathList[i]);
                if (node != null)
                {
                    toRemoveNode.Add(node);
                    Console.WriteLine($"Found node{i}");
                }
            }

            foreach (var node in toRemoveNode)
            {
                node.Remove();
            }
        }

        public enum Mode
        {
            CSDN,
            //TODO
        }

        const string USAGES = "XXX.exe xxx.html [-mode CSDN]";

        static bool ParseArgs(string[] args, out Mode mode, out string inputFileName)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++)
            {
                sb.Append(args[0]);
                sb.Append(" ");
            }
            Console.WriteLine($"args:\n {sb.ToString()}");

            mode = Mode.CSDN;
            inputFileName = "";

            if (args.Length < 1)
            {
                Console.WriteLine($"args.Length < 1");
                Console.WriteLine($"usages:\n {USAGES}");
                return false;
            }

            inputFileName = args[0];
            
            if(args.Length > 1)
            {
                if((args.Length - 1)%2 != 0)
                {
                    Console.WriteLine($"(args.Length - 2)%2 != 0");
                    Console.WriteLine($"usages:\n {USAGES}");
                    return false;
                }
                for(int i = 1; i < args.Length; i++)
                {
                    if (args[i] == "-mode")
                    {
                        if(i+1 >= args.Length)
                        {
                            Console.WriteLine($"i+1 >= args.Length");
                            Console.WriteLine($"usages:\n {USAGES}");
                            return false;
                        }
                        var modeStr = args[i + 1];
                        if(!Enum.TryParse<Mode>(modeStr,out mode))
                        {
                            Console.WriteLine($"usages:\n {USAGES}");
                            Console.WriteLine($"the mode is not supported!");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static void Main(string[] args)
        {
            Mode mode = Mode.CSDN;
            string inputFilePath = "./test.html";

            if (!ParseArgs(args, out mode, out inputFilePath)) return;

            FileInfo fileInfo = new FileInfo(inputFilePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine($"the file {inputFilePath} is not exist!");
                return;
            }

            var html = new HtmlDocument();
            FileStream stream = new FileStream(inputFilePath, FileMode.Open);
            html.Load(stream);

            if(mode == Mode.CSDN)
                SubstractNode4CSDN(html);

            var director = Path.GetDirectoryName(fileInfo.FullName);
            var outputFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName) + "_new.html";
            FileStream outputStream = new FileStream(Path.Combine(director, outputFileName), FileMode.Create);
            html.Save(outputStream);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace CSDNHtmlSimiplify
{
    internal class Program
    {
        public class ToRemoveNode
        {
            public string m_nodePath;
            public bool m_isMany;
        }

        public static readonly List<string> ToRemoveNodesCsdn = new List<string>() {
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
                "//div[@id='blogHuaweiyunAdvert']",
                "//div[@id='blogColumnPayAdvert']",
                "//div[@id='blogExtensionBox']",
        };

        public static readonly List<string> ToRemoveNodesZhiHu = new List<string>() {
                "//div[@class='ColumnPageHeader-Title']",
                "//div[@class='PostIndex-Contributions']",
                "//div[@role='complementary']",
                "//div[@class='ColumnPageHeader-Wrapper']",
                "//div[@class='Post-Author']",
                "//div[@class='Post-topicsAndReviewer']",
                "//div[@class='ContentItem-actions']",
                 "//div[@class='Sticky RichContent-actions is-bottom']",
                 "//div[@class='Post-Sub Post-NormalSub']",
        };

        public static readonly List<string> ToRemoveNodesCnblogs = new List<string>() {
               "//div[@id='sideBar']",
               "//div[@id='sidebar_categories']",
               "//div[@id='top_nav']",
               "//div[@id='navigator']",
               "//div[@id='under_post_card1']",
               "//div[@id='under_post_card2']",
                "//div[@id='comment_form']",
                "//div[@id='blog_post_info_block']",
                "//div[@id='cnblogs_ch']",
                "//div[@id='blog_c1']",
                 "//div[@class='under-post-card']",
                 "//div[@id='header']",
                 "//div[@id='footer']",
                 "//div[@class='postDesc']",
                 "//div[@id='MySignature']",
                 "//div[@class='imagebar forpc']",
        };

        private static void SubstractNode(HtmlDocument doc, Mode mode)
        {
            List<HtmlNode> toRemoveNode = new List<HtmlNode>();


            List<string> toRemoveNodePathList = null;
            switch (mode)
            {
                case Mode.CSDN:
                    toRemoveNodePathList = ToRemoveNodesCsdn;
                    break;
                case Mode.ZhiHu:
                    toRemoveNodePathList = ToRemoveNodesZhiHu;
                    break;
                case Mode.Cnblogs:
                    toRemoveNodePathList = ToRemoveNodesCnblogs;
                    break;
            }

            if (toRemoveNodePathList == null || toRemoveNodePathList.Count == 0)
                return;

            for (int i = 0; i < toRemoveNodePathList.Count; i++)
            {
                var nodes = doc.DocumentNode.SelectNodes(toRemoveNodePathList[i]);
                if (nodes != null)
                {
                    foreach(var node in nodes)
                    {
                        toRemoveNode.Add(node);
                    }
                    
                    Console.WriteLine($"Found node{i} count:{nodes.Count}");
                }else
                {
                    Console.WriteLine($"Found node{i} count:0");
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
            ZhiHu,
            Cnblogs,
            //TODO
        }

        const string USAGES = "XXX.exe xxx.html [-mode CSDN]";

        static bool ParseArgs(string[] args, out Mode mode, out string inputFileName)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i]);
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

            SubstractNode(html, mode);

            var director = Path.GetDirectoryName(fileInfo.FullName);
            var outputFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName) + "_new.html";
            FileStream outputStream = new FileStream(Path.Combine(director, outputFileName), FileMode.Create);
            html.Save(outputStream);

        }
    }
}

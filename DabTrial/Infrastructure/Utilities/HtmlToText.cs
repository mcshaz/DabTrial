﻿using HtmlAgilityPack;
using System;
using System.IO;
using System.Text.RegularExpressions;
//http://htmlagilitypack.codeplex.com/sourcecontrol/changeset/view/62772#52179

namespace DabTrial.Infrastructure.Utilities
{
    public static class HtmlToText
    {

        public static string Convert(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            using (StringWriter sw = new StringWriter())
            {
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        public static string ConvertHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            using (StringWriter sw = new StringWriter())
            {
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        public static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            string html;
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    string parentName = node.ParentNode.Name;
                    if ((parentName == "script") || (parentName == "style"))
                        break;

                    // get text
                    html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    html = Regex.Replace(html,@"^\s{2,}|\s{2,}$"," ");
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                        case "div":
                            // treat paragraphs & divs as crlf
                            outText.Write("\r\n");
                            break;
                        case "a":
                            if (node.Attributes.Contains("href"))
                            {
                                node.InnerHtml = node.InnerText.TrimEnd() + "&lt;" + node.Attributes["href"].Value + "&gt;";
                            }
                            
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }
    }
}
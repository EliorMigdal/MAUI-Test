﻿using OfflineReader.Model.HTMLParser.ArticleParsers;

namespace OfflineReader.Model.HTMLParser;

public class ArticleParserFactory
{
    public IArticleParser GenerateParser(string i_Website)
    {
        IArticleParser parser;

        switch (i_Website)
        {
            case "mako":
                parser = new MakoArticleParser();
                break;

            default:
                parser = null;
                break;
        }

        return parser;
    }
}
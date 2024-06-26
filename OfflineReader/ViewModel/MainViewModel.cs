﻿using System.Windows.Input;
using OfflineReader.Model.HTMLParser;

namespace OfflineReader.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ArticlesService ArticlesService { get; set; } = new();
    public ObservableCollection<Article> Articles { get; set; } = new();
    private HTMLSupplierService HTMLSupplier { get; set; } = new();
    private ArticleParserFactory ArticleParserFactory { get; set; } = new();
    private MainPageParserFactory MainPageParserFactory { get; set; } = new();
    public ICommand ArticleSelectedCommand { get; }
    private Article _selectedArticle;
    public Article SelectedArticle
    {
        get => _selectedArticle;

        set
        {
            if (_selectedArticle != value)
            {
                _selectedArticle = value;

                OnPropertyChanged();

                if (_selectedArticle != null)
                {
                    testCommand(_selectedArticle);
                }
            }
        }
    }

    public MainViewModel()
    {
        ArticleSelectedCommand = new Command<Article>(testCommand);
    }

    [RelayCommand]
    public async Task GetArticlesAsync()
    {
        string htmlCode = await HTMLSupplier.GetHTMLAsync("https://www.mako.co.il/");
        IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser("mako");
        List<Article> articles = mainPageParser.ParseHTML(htmlCode);

        foreach (Article article in articles)
        {
            Articles.Add(article);
        }
    }

    [RelayCommand]
    public async Task GoToReaderModeAsync(Article i_Article)
    {
        if (i_Article == null)
            return;

        await Shell.Current.GoToAsync(nameof(ReaderPage), true, new Dictionary<string, object>
        {
            { "Article", i_Article }
        });
    }

    private async void testCommand(Article i_Article)
    {
        if (i_Article is null)
            return;

        string htmlCode = await HTMLSupplier.GetHTMLAsync(i_Article.URL);
        SharedData.SharedArticle = i_Article;
        SharedData.HTML = htmlCode;
        IArticleParser articleParser = ArticleParserFactory.GenerateParser(i_Article.Website.ToLower());
        StackLayout ArticleLayout = articleParser.ParseHTML(htmlCode);

        await Shell.Current.GoToAsync(nameof(TestView), true);
    }
}
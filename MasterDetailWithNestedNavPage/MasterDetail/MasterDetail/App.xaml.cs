#region

using System;
using System.Threading;
using MasterDetail.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

#endregion

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace MasterDetail
{
	// XamlになれるとC#コードでも普通に画面書けることに気が付いた。
	// Intellisenseとコード補完最大限に効くので結構書きやすい。
	// 物凄く読みにくいけどね🍣

	public partial class App : Application
	{
		private readonly NavigationPage _innerNavPage;
		private readonly MasterDetailPage _masterDetailPage;
		private readonly NavigationPage _outerNavPage;

		public App()
		{
			InitializeComponent();

			// browse page
			var browsePage = new ItemsPage
			{
				Title = "Browse"
			};
			NavigationPage.SetHasNavigationBar(browsePage, false);

			// aboutPage
			var aboutPage = new AboutPage
			{
				Title = "About"
			};
			NavigationPage.SetHasNavigationBar(aboutPage, false);

			// InnerNavigationPage
			_innerNavPage = new NavigationPage(browsePage)
			{
				Title = browsePage.Title
			};
			_innerNavPage.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == NavigationPage.CurrentPageProperty.PropertyName)
					_innerNavPage.Title = _innerNavPage.CurrentPage.Title;
			};

			// OuterNavigationPage
			_outerNavPage = new NavigationPage(_innerNavPage);

			// MenuPage
			Page menuPage = null;
			menuPage = new ContentPage
			{
				Title = "Menu",
				Content = new StackLayout
				{
					Children =
					{
						new ContentView
						{
							Padding = 20,
							Content = new Label {Text = "Browse"},
							GestureRecognizers =
							{
								new TapGestureRecognizer
								{
									Command = new Command(() => { MessagingCenter.Send(menuPage, "Menu", "Browse"); })
								}
							}
						},
						new ContentView
						{
							Padding = 20,
							Content = new Label {Text = "About"},
							GestureRecognizers =
							{
								new TapGestureRecognizer
								{
									Command = new Command(() => { MessagingCenter.Send(menuPage, "Menu", "About"); })
								}
							}
						}
					}
				}
			};


			_masterDetailPage = new MasterDetailPage();
			_masterDetailPage.Master = menuPage;
			_masterDetailPage.Detail = _outerNavPage;

			MainPage = _masterDetailPage;

			MessagingCenter.Subscribe(this, "Navigation",
				new Action<ItemsPage, Page>((s, p) => { _outerNavPage.PushAsync(p); }));

			MessagingCenter.Subscribe(this, "Menu", new Action<Page, string>((p, s) =>
			{
				if (s == "Browse")
				{
					if (!(_innerNavPage.CurrentPage is ItemsPage))
						_innerNavPage.PopToRootAsync();
				}
				else if (s == "About")
				{
					if (_innerNavPage.CurrentPage is AboutPage)
						return;

					if (_innerNavPage.CurrentPage is ItemsPage)
					{
						_innerNavPage.PushAsync(aboutPage);
					}
					else
					{
						var currentPage = _innerNavPage.CurrentPage;
						_innerNavPage.PushAsync(aboutPage)
							.ContinueWith(task =>
							{
								SynchronizationContext.Current.Post(state => { _innerNavPage.Navigation.RemovePage(currentPage); }, null);
							});
					}
				}
				_masterDetailPage.IsPresented = false;
			}));
		}
	}
}
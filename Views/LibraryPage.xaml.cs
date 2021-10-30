using System;
using Arcade_Manager.ViewModels;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace Arcade_Manager.Views
{
    public sealed partial class LibraryPage : Page
    {
        public LibraryViewModel ViewModel { get; } = new LibraryViewModel();

        public LibraryPage()
        {
            InitializeComponent();
        }

        private async void Browse_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.GetRomsAsync();
        }

        private async void Done_Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog checkRomsDialog = new ContentDialog
            {
                Title = "Are you sure?",
                Content = "You are adding " + Rom_TreeView.SelectedNodes.Count + " to library.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await checkRomsDialog.ShowAsync();

            Collection<Rom> selected_roms = new Collection<Rom>();
            foreach(TreeViewNode node in Rom_TreeView.SelectedNodes)
            {
                foreach(Rom rom in ViewModel.DataSource)
                {
                    if (rom.ToString() == node.ToString())
                    {
                        selected_roms.Add(rom);
                    }
                }
            }
            await ViewModel.StageRoms(selected_roms);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace Arcade_Manager.ViewModels
{
    public class LibraryViewModel : ObservableObject
    {
        public ObservableCollection<Rom> DataSource { get; private set; } = new ObservableCollection<Rom>();

        public LibraryViewModel()
        {
            
        }

        /// <summary>
        /// Browse for a folder, perform 1-layer deep search for UCE files, and add to DataSource.
        /// </summary>
        /// <returns>Task</returns>
        public async Task GetRomsAsync()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".UCE");

            StorageFolder selected_folder = await folderPicker.PickSingleFolderAsync();

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;

            if (selected_folder != null)
            {
                // TODO: Save mruToken for later recall. 
                string mruToken = mru.Add(selected_folder, "library");

                DataSource.Clear();
                var itemsList = await selected_folder.GetItemsAsync();

                foreach(var item in itemsList)
                {
                    if (item is StorageFolder)
                    {
                        StorageFolder folder = item as StorageFolder;
                        var nested_folder_files = await folder.GetFilesAsync();
                        foreach (var file in nested_folder_files)
                        {
                            AddRom(file);
                        }
                    }
                    else
                    {
                        StorageFile file = item as StorageFile;
                        AddRom(file);
                    }
                }
            }
        }

        /// <summary>
        /// Add a rom to DataSource
        /// </summary>
        /// <param name="file">Reference to file to be added to DataSource</param>
        public void AddRom(StorageFile file)
        {
            Rom rom = new Rom();
            rom.Name = file.DisplayName;
            rom.File = file;
            DataSource.Add(rom);
        }

        /// <summary>
        /// Prepare the selected roms for processing.
        /// </summary>
        /// <param name="roms">Collection of roms to be processed</param>
        public async Task StageRoms(Collection<Rom> roms)
        {
            // Pick output folder
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".UCE");

            StorageFolder selected_folder = await folderPicker.PickSingleFolderAsync();

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;

            if (selected_folder != null)
            {
                // TODO: Save mruToken for later recall. 
                string mruToken = mru.Add(selected_folder, "output folder");

                foreach (var rom in roms)
                {
                    await rom.File.CopyAsync(selected_folder, rom.Name, NameCollisionOption.ReplaceExisting);
                }
            }
        }
    }

    public class Rom
    {
        public string Name { get; set; }
        public ObservableCollection<Rom> Children { get; set; } = new ObservableCollection<Rom>();
        public StorageFile File { get; set; }

        public Rom()
        {

        }

        public Rom(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

﻿using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Listicles
{
    /// <summary>
    /// Interaction logic for ListiclesView.xaml
    /// </summary>
    public partial class ListiclesView : UserControl
    {
        private ListiclesViewModel ViewModel;
        private int selectedGameIndex;
        public ListiclesView()
        {
            InitializeComponent();
        }
        public ListiclesView(ListiclesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
            lvListicles.ItemsSource = viewModel.Listicles;
            if (viewModel.Listicles.Count == 0)
            {
                gvcMain.Header = "You have no Listicles! Right-click a game in your Library and select 'Add to Listicle'";
            }
        }
        private void lvGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* if the user selects a game from another listicle, we want to make that the current listicle */
            Listicles.ListiclesItem previousListicle = ViewModel.CurrentListicle;
            Listicles.ListiclesItem currentListicle = ViewModel.Listicles.FirstOrDefault(x => x == ((ListView)sender).DataContext);
            if (currentListicle != null & currentListicle != previousListicle)
            {
                ViewModel.CurrentListicle = currentListicle;
                previousListicle.SelectedListicleHighlight = new FontFamily("Segoe UI Light");
                ViewModel.CurrentListicle.SelectedListicleHighlight = new FontFamily("Segoe UI Semibold");
            }
            
            /* when a game is removed, this event fires. we want to handle that by keeping the same index selected */
            //if (((ListView)sender).SelectedIndex < 0)
            //{
            //    ((ListView)sender).SelectedIndex = selectedGameIndex;
            //}
            /* save the currently selected game as the CurrentGame */
            ViewModel.CurrentGame = (Playnite.SDK.Models.Game)((ListView)sender).SelectedItem;
            //selectedGameIndex = ((ListView)sender).SelectedIndex;
            //Expander parent = (Expander)((ListView)sender).Parent;
            //parent.FontWeight = FontWeights.Bold;
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                ViewModel.CurrentListicle.SelectedListicleHighlight = new FontFamily("Segoe UI Light");
            }
            ViewModel.CurrentListicle = ViewModel.Listicles.First(x => x == ((Expander)sender).DataContext);
            ViewModel.CurrentListicle.SelectedListicleHighlight = new FontFamily("Segoe UI Semibold");
            if (ViewModel.CurrentListicle.SelectedGameIndex < 0) { ViewModel.CurrentListicle.SelectedGameIndex = 1; }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateListiclesData();
        }
        private void btnOpenLink_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                if (ViewModel.CurrentListicle.Link != String.Empty)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(ViewModel.CurrentListicle.Link);
                    }
                    catch (Exception ex) 
                    { 
                        
                    }
                }
            }
        }
        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.CurrentListicle.SelectedGameIndex;
                if (index > 0)
                { 
                    ViewModel.CurrentListicle.Games.Move(index, index - 1); 
                }
            }
        }
        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.CurrentListicle.SelectedGameIndex;
                if (index < ViewModel.CurrentListicle.Games.Count - 1 & index >= 0)
                {
                    ViewModel.CurrentListicle.Games.Move(index, index + 1);
                }
            }
        }
        private void btnMoveTop_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.CurrentListicle.SelectedGameIndex;
                if (index > 0)
                {
                    ViewModel.CurrentListicle.Games.Move(index, 0);
                }
            }
        }
        private void btnMoveBottom_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.CurrentListicle.SelectedGameIndex;
                if (index < ViewModel.CurrentListicle.Games.Count - 1 & index >= 0)
                {
                    ViewModel.CurrentListicle.Games.Move(index, ViewModel.CurrentListicle.Games.Count - 1);
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                if (ViewModel.CurrentListicle.SelectedGameIndex >= 0)
                {
                    ViewModel.CurrentListicle.Games.RemoveAt(ViewModel.CurrentListicle.SelectedGameIndex);
                    if (ViewModel.CurrentListicle.Games.Count == 0) { ViewModel.CurrentListicle.SelectedGameIndex = -1; }
                    //ViewModel.Listicles.First(x => x == ViewModel.CurrentListicle).Games.Remove(ViewModel.CurrentGame);
                }
            }
        }
        private void btnDeleteListicle_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                ViewModel.Listicles.Remove(ViewModel.CurrentListicle);
                ViewModel.CurrentListicle = null;
            }
        }
        private void btnAddStub_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                if (ViewModel.CurrentListicle.SelectedGameIndex >= 0)
                {
                    Playnite.SDK.Models.Game game = ViewModel.AddStub();
                    if (game != null)
                    {
                        // don't know how to select a game from the current ListView
                    }
                }
            }
        }
        private void btnShowGame_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                if (ViewModel.CurrentListicle.Games.Count > 0)
                {
                    int index = ViewModel.CurrentListicle.SelectedGameIndex;
                    if (index >= 0)
                    {
                        ViewModel.ShowGameInLibrary.Execute(ViewModel.CurrentListicle.Games[index]);
                    }
                }
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                ViewModel.EditListicle(ViewModel.CurrentListicle);
            }
        }
        private void btnReverse_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                List<Game> gameList = ViewModel.CurrentListicle.Games.ToList();
                gameList.Reverse();
                ViewModel.CurrentListicle.Games.Clear();
                gameList.ForEach(x => ViewModel.CurrentListicle.Games.Add(x));
            }
            //int lastIndex = ViewModel.CurrentListicle.Games.Count -1;
            //for (int i = 0; i <= lastIndex; i++)
            //{
            //    ViewModel.CurrentListicle.Games.Move(0, lastIndex);
            //}
            //int index = ViewModel.Listicles.IndexOf(ViewModel.CurrentListicle);
            //ViewModel.Listicles[index].Games = ViewModel.CurrentListicle.Games.Reverse().ToObservable();
            //ViewModel.CurrentListicle.Games = ViewModel.CurrentListicle.Games.Reverse().ToObservable();
            
            //ViewModel.CurrentListicle.Games = reversed.ToObservable();
        }
        private void btnMoveListicleUp_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.Listicles.IndexOf(ViewModel.CurrentListicle);
                if (index > 0)
                {
                    ViewModel.Listicles.Move(index, index - 1);
                }

            }
        }
        private void btnMoveListicleDown_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentListicle != null)
            {
                int index = ViewModel.Listicles.IndexOf(ViewModel.CurrentListicle);
                if (index < ViewModel.Listicles.Count - 1)
                {
                    ViewModel.Listicles.Move(index, index + 1);
                }
            }
        }

        private void lvListicles_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            /*  this little bit of copy/pasted code passed scrolling up to the scrollviewer
                to make it easier to scroll vertically */
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Solstice
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            myGrid();
        }

        private void myGrid()
        {
            var grid = new Grid();
            // 2 x 3 Grid
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var topLeft = new Label { Text = "Top Left" };
            var topMiddle = new Label { Text = "Top Middle" };
            var topRight = new Label { Text = "Top Right" };
            var bottomLeft = new Label { Text = "Bottom Left" };
            var bottomMiddle = new Label { Text = "Bottom Middle" };
            var bottomRight = new Label { Text = "Bottom Right" };

            grid.Children.Add(topLeft, 0, 0);
            grid.Children.Add(topMiddle, 1, 0);
            grid.Children.Add(topRight, 2, 0);
            grid.Children.Add(bottomLeft, 0, 1);
            grid.Children.Add(bottomMiddle, 1, 1);
            grid.Children.Add(bottomRight, 2, 1);
        }
    }
}

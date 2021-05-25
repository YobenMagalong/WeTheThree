using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using WeTheTree.Models;
using Plugin.Toast;
using System.IO;

namespace WeTheTree
{
    public partial class MainPage : ContentPage
    {
        string _dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "WeTheTree.db3");
        byte[] imageAsBytes = null;

        bool options_is_visible = false;
        bool editing = false;

        public List<Tree> Trees { get; set; }
        public List<Picture> Pictures { get; set; }
        public Frame target_frame;

        private Dictionary<string, bool> parameters = new Dictionary<string, bool>(){
            { "Tree Code", false },
            { "Initial Identification", false },
            { "Scientific Name", false },
            { "Location", false  },
            { "Landmarks of Location", false },
            { "Longitude", false },
            { "Latitude", false },
            { "Height", false },

            };

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            //Images
            btnadd.Source = ImageSource.FromResource("WeTheTree.Images.add.png");
            image.Source = ImageSource.FromResource("WeTheTree.Images.image.png");
            menu.Source = ImageSource.FromResource("WeTheTree.Images.menu.png");
            close.Source = ImageSource.FromResource("WeTheTree.Images.close.png");


            //Content
            var db = new SQLiteConnection(_dbPath);


            db.CreateTable<Tree>();
            db.CreateTable<Picture>();

            Trees = db.Table<Tree>().OrderBy(t => t.ID).ToList();

            if (Trees.Count() == 0)
                return;

            foreach (Tree tree in Trees)
            {
                Picture picture = db.Table<Picture>().Where(p => p.Tree_ID == tree.ID).FirstOrDefault();
                tree_item(tree, picture, content);
            }


        }

        void show_target(string _c_name, string _sc_name, int _tree_ID)
        {
            var db = new SQLiteConnection(_dbPath);
            byte[] img_bytes = db.Table<Picture>().Where(p => p.Tree_ID == _tree_ID).FirstOrDefault().Img_Blob;
            target_image.Source = ImageSource.FromStream(() => new MemoryStream(img_bytes));
            target_common_name.Text = _c_name;
            target_scientific_name.Text = _sc_name;
            image_overlay.IsVisible = true;
        }

        void tree_item(Tree _tree, Picture _picture, StackLayout _content)
        {
            Frame frame = new Frame();
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(0, 10, 0, 0);
            frame.HasShadow = true;
            frame.Padding = 0;
            frame.CornerRadius = 0;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                target_frame = frame;
                show_target(_tree.Initial_Identification, _tree.Scientific_Name, _tree.ID);
            };

            frame.GestureRecognizers.Add(tapGestureRecognizer);

            StackLayout sl_container = new StackLayout();

            Image tree = new Image();
            tree.Source = ImageSource.FromStream(() => new MemoryStream(_picture.Img_Blob));
            tree.HeightRequest = 500;
            tree.Aspect = Aspect.AspectFill;
            sl_container.Children.Add(tree);

            StackLayout lbl_container = new StackLayout();
            lbl_container.Orientation = StackOrientation.Horizontal;

            Label common_name = new Label();
            common_name.Text = _tree.Initial_Identification;
            common_name.TextColor = Color.Black;
            common_name.FontFamily = "Comfortaa";
            common_name.FontSize = 15;
            common_name.HorizontalOptions = LayoutOptions.StartAndExpand;
            common_name.Margin = new Thickness(20, 10, 0, 20);
            lbl_container.Children.Add(common_name);

            Label scientific_name = new Label();
            scientific_name.Text = _tree.Scientific_Name;
            scientific_name.TextColor = Color.Black;
            scientific_name.FontFamily = "Comfortaa Italic";
            scientific_name.FontSize = 15;
            scientific_name.HorizontalOptions = LayoutOptions.EndAndExpand;
            scientific_name.Margin = new Thickness(0, 10, 20, 20);
            lbl_container.Children.Add(scientific_name);

            sl_container.Children.Add(lbl_container);
            frame.Content = sl_container;

            _content.Children.Add(frame);
        }

        private void fab_Clicked(object sender, EventArgs e)
        {
            input_overlay.IsVisible = true;
            var db = new SQLiteConnection(_dbPath);

            Trees = db.Table<Tree>().OrderBy(t => t.ID).ToList();

            foreach (Tree tree in Trees)
            {
                Debug.WriteLine(tree);
            }

            Pictures = db.Table<Picture>().OrderBy(p => p.ID).ToList();

            foreach (Picture picture in Pictures)
            {
                Debug.WriteLine(picture);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (input_overlay.IsVisible)
            {
                input_overlay.IsVisible = false;
                return true;
            }

            if (options_is_visible == true)
            {
                close_Clicked(this, new EventArgs());
                return true;
            }

            if (details_Is_Lowered)
            {
                hide_target_parameters();
                return true;
            }

            if (image_overlay.IsVisible)
            {
                image_overlay.IsVisible = false;
                return true;
            }

            return true;
        }

        private void input_overlay_tapped(object sender, EventArgs e)
        {
            if (c_name.IsFocused)
            {
                c_name.Unfocus();
                return;
            }

            if (sc_name.IsFocused)
            {
                sc_name.Unfocus();
                return;
            }

            reset();
        }

        void reset()
        {
            input_overlay.IsVisible = false;

            image.Aspect = Aspect.AspectFit;
            image.Source = ImageSource.FromResource("WeTheTree.Images.image.png");
            imageAsBytes = null;

            c_name.Text = "";
            sc_name.Text = "";

        }

        void set_image_button()
        {
            image.Aspect = Aspect.AspectFill;
        }

        private void input_wrapper_tapped(object sender, EventArgs e)
        {
            return;
        }

        private void c_name_Focused(object sender, FocusEventArgs e)
        {
            input_wrapper.Margin = new Thickness(34, -170, 75, 0);
        }

        private void c_name_Unfocused(object sender, FocusEventArgs e)
        {
            input_wrapper.Margin = new Thickness(34, -80, 75, 0);
        }

        private void sc_name_Focused(object sender, FocusEventArgs e)
        {
            input_wrapper.Margin = new Thickness(34, -341, 75, 0);
        }

        private void sc_name_Unfocused(object sender, FocusEventArgs e)
        {
            input_wrapper.Margin = new Thickness(34, -80, 75, 0);
        }

        private async void image_Clicked(object sender, EventArgs e)
        {
            try
            {
                var file = await MediaPicker.PickPhotoAsync(
                   new MediaPickerOptions { Title = "Select Tree Picture" });

                if (file == null)
                    return;

                imageAsBytes = get_Image_Bytes(await file.OpenReadAsync());
                set_image_button();
                image.Source = ImageSource.FromStream(() => new MemoryStream(imageAsBytes));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void submit_Clicked(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(_dbPath);

            var t_pk = db.Table<Tree>().OrderByDescending(t => t.ID).FirstOrDefault();

            Tree tree = new Tree()
            {
                Initial_Identification = c_name.Text,
                Scientific_Name = sc_name.Text
            };

            Picture picture = new Picture()
            {
                Tree_ID = (t_pk == null ? 1 : t_pk.ID + 1),
                Img_Blob = imageAsBytes
            };

            db.Insert(tree);
            db.Insert(picture);

            tree_item(tree, picture, content);

            CrossToastPopUp.Current.ShowToastMessage(c_name.Text + " successfully saved");
            input_overlay_tapped(this, new EventArgs());
        }

        private byte[] get_Image_Bytes(Stream _stream)
        {
            byte[] image_bytes;
            using (var memoryStream = new MemoryStream())
            {
                _stream.CopyTo(memoryStream);
                image_bytes = memoryStream.ToArray();
            }
            return image_bytes;
        }

        private double previousScrollPosition = 0;
        private async void scrollview_Scrolled(object sender, ScrolledEventArgs e)
        {

            if (previousScrollPosition < e.ScrollY)
            {
                await add_button.TranslateTo(0, 123, 50);
                await header.TranslateTo(0, -e.ScrollY, 50);
                previousScrollPosition = e.ScrollY;
            }
            else
            {
                await add_button.TranslateTo(0, 0, 50);
                await header.TranslateTo(0, 0, 50);
                if (Convert.ToInt16(e.ScrollY) == 0)
                    previousScrollPosition = 0;

            }
        }
        public bool details_Is_Lowered = false;
        void target_parameter(KeyValuePair<string, bool> property, string value, StackLayout _parent)
        {
            Label parameter = new Label();
            parameter.Text = property.Key;
            parameter.TextColor = Color.White;
            parameter.FontFamily = "Comfortaa SemiBold";
            parameter.FontSize = 16;
            _parent.Children.Add(parameter);

            if (property.Value)
            {
                Editor input = new Editor();
                input.CharacterSpacing = 1;
                input.InputTransparent = true;
                input.HeightRequest = 100;
                input.MaxLength = 100;

                if (value != null)
                    input.Text = value;

                input.TextColor = Color.White;
                input.Placeholder = "ADD INFO";
                input.PlaceholderColor = Color.FromHex("#9b9b9b");
                input.FontFamily = "Comfortaa";
                input.FontSize = 16;
                _parent.Children.Add(input);

            }
            else
            {
                Entry input = new Entry();
                input.CharacterSpacing = 1;
                input.InputTransparent = true;
                input.MaxLength = 25;

                if (value != null)
                    input.Text = value;

                input.TextColor = Color.White;
                input.Placeholder = "ADD INFO";
                input.PlaceholderColor = Color.FromHex("#9b9b9b");
                input.FontFamily = "Comfortaa";
                input.FontSize = 16;
                _parent.Children.Add(input);

            }

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!details_Is_Lowered)
            {
                show_target_parameters();
                populate_target_parameters_container();
                return;
            }

            hide_target_parameters();
        }

        void populate_target_parameters_container()
        {
            if (target_parameters_container.Children.Count == 0)
            {
                var db = new SQLiteConnection(_dbPath);
                var trees = db.Table<Tree>().Where(t => t.Initial_Identification == target_common_name.Text).FirstOrDefault();

                List<string> values = new List<string>()
                {
                    trees.Tree_Code,
                    trees.Initial_Identification,
                    trees.Scientific_Name,
                    trees.Longitude,
                    trees.Latitude,
                    trees.Location,
                    trees.Landmarks_of_Location,
                    trees.Height,
                };

                target_parameters_container.Children.Clear();
                for (int i = 0; i < values.Count(); i++)
                {
                    target_parameter(parameters.ElementAt(i), values[i], target_parameters_container);
                }
            }
        }

        public async void show_target_parameters()
        {
            details_Is_Lowered = true;
            await target_init_parameters.FadeTo(0, 80);
            await parameter_container.TranslateTo(0, 0, 50);
            await parameter_background.FadeTo(.7, 80);
            await target_parameters_container.FadeTo(1, 80);
        }

        public async void hide_target_parameters()
        {
            details_Is_Lowered = false;
            await target_parameters_container.FadeTo(0, 50);
            await parameter_container.TranslateTo(0, 600, 50);
            await parameter_background.FadeTo(0, 80);
            await target_init_parameters.FadeTo(1, 90);

            disable_parameters();
            update_target_details();
            editing = false;
        }

        void update_target_details()
        {
            if (!editing)
                return;

            var db = new SQLiteConnection(_dbPath);
            Tree tree = db.Table<Tree>().Where(t => t.Initial_Identification == target_common_name.Text).FirstOrDefault();

            List<string> new_data = get_all_data();

            Tree new_tree = new Tree()
            {
                ID = tree.ID,
                Tree_Code = new_data[0],
                Initial_Identification = new_data[1],
                Scientific_Name = new_data[2],
                Longitude = new_data[3],
                Latitude = new_data[4],
                Location = new_data[5],
                Landmarks_of_Location = new_data[6],
                Height = new_data[7],

            };

            db.Update(new_tree);
            CrossToastPopUp.Current.ShowToastMessage(target_common_name.Text + " successfully updated");
        }

        List<string> get_all_data()
        {
            List<string> data = new List<string>();
            foreach (var input in target_parameters_container.Children)
            {
                if (input is Entry)
                {
                    data.Add(((Entry)input).Text);
                }
                else if (input is Editor)
                {
                    data.Add(((Editor)input).Text);
                }
            }

            return data;
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            hide_target_parameters();
        }

        private async void menu_Clicked(object sender, EventArgs e)
        {
            await options.TranslateTo(165, 0, 50);
            options_is_visible = true;
        }

        private async void close_Clicked(object sender, EventArgs e)
        {
            await options.TranslateTo(420, 0, 50);
            options_is_visible = false;
        }

        private void edit_Tapped(object sender, EventArgs e)
        {
            if (!details_Is_Lowered)
            {
                show_target_parameters();
            }

            populate_target_parameters_container();
            enable_parameters();
            close_Clicked(this, new EventArgs());
            editing = true;
        }

        void enable_parameters()
        {
            var input_parameters = target_parameters_container.Children;

            foreach (var input in input_parameters)
            {
                if (input.GetType() == typeof(Entry) || input.GetType() == typeof(Editor))
                {
                    input.InputTransparent = false;
                }
            }
        }

        void disable_parameters()
        {
            var input_parameters = target_parameters_container.Children;

            foreach (var input in input_parameters)
            {
                if (input.GetType() == typeof(Entry) || input.GetType() == typeof(Editor))
                {
                    input.InputTransparent = true;
                }
            }
        }

        private void delete_Tapped(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(_dbPath);
            Tree tree = db.Table<Tree>().Where(t => t.Initial_Identification == target_common_name.Text).FirstOrDefault();
            db.Delete(tree);
            db.Table<Picture>().Delete(p => p.Tree_ID == tree.ID);
            content.Children.Remove(target_frame);

            close_Clicked(this, new EventArgs());
            CrossToastPopUp.Current.ShowToastMessage(target_common_name.Text + " successfully deleted");
            image_overlay.IsVisible = false;
        }
    }
}


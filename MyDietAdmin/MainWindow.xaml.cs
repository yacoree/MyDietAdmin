using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
using System.IO;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Windows.Media.Media3D;

namespace MyDietAdmin
{
    public partial class MainWindow : Window
    {
        //private static readonly RestClient client = new RestClient("https://localhost:7214/api/1.0"); 
        private static readonly RestClient client = new RestClient("https://a27570-688f.c.d-f.pw/api/1.0");
        private List<DishComposition> compositions = new List<DishComposition>();
        public MainWindow()
        {
            InitializeComponent();
            GetIngredientData();
            GetDishData();
            GetUnitData();
        }

        private void GetIngredientData()
        {
            RestRequest request = new RestRequest("/Ingredient", Method.Get);
            var response = client.Execute(request);

            string stream = response.Content;

            dynamic d = JsonConvert.DeserializeObject(stream);
            Ingredients.ItemsSource = d.ingredients;
            DishCompositionIngredient.ItemsSource = d.ingredients;
        }
        private void GetUnitData()
        {
            RestRequest request = new RestRequest("/Unit", Method.Get);
            var response = client.Execute(request);

            string stream = response.Content;

            dynamic d = JsonConvert.DeserializeObject(stream);
            DishCompositionUnit.ItemsSource = d.units;
        }

        private void GetDishData()
        {
            RestRequest request = new RestRequest("/Dish", Method.Get);
            //IRestResponse response = client.Execute(request);
            var response = client.Execute(request);

            string stream = response.Content;

            dynamic d = JsonConvert.DeserializeObject(stream);
            Dishes.ItemsSource = d.dishes;

        }

        private void SaveIngredient_Click(object sender, RoutedEventArgs e)
        {
            RestRequest request = new RestRequest("/Ingredient", Method.Post) { RequestFormat = RestSharp.DataFormat.Json }
                .AddBody(
                    new
                    {
                        ingredient_name = IngredientName.Text,
                        calories = Convert.ToInt32(IngredientCalories.Text),
                        protein = Convert.ToDecimal(IngredientProtein.Text),
                        carbohydrates = Convert.ToDecimal(IngredientCarbohydrates.Text),
                        fats = Convert.ToDecimal(IngredientFats.Text),
                        image = IngredientImage.Text
                    });
            var response = client.Execute(request);

            GetIngredientData();
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            dynamic ingredient = DishCompositionIngredient.Items[DishCompositionIngredient.SelectedIndex];
            dynamic unit = DishCompositionUnit.Items[DishCompositionUnit.SelectedIndex];

            var entity = new DishComposition
            {
                ingredient_id = ingredient.id,
                unit_id = DishCompositionUnit.SelectedIndex == -1 ? null : unit.id,
                value = DishCompositionValue.Text == string.Empty ? Convert.ToInt16(0) : Convert.ToInt16(DishCompositionValue.Text),
                weight = Convert.ToInt32(DishCompositionWeight.Text),
            };
            compositions.Add(entity);
            DishComposition.Items.Add(entity);
        }

        private void SaveDish_Click(object sender, RoutedEventArgs e)
        {
            RestRequest request = new RestRequest("/Dish", Method.Post) { RequestFormat = RestSharp.DataFormat.Json }
                .AddBody(
                    new
                    {
                        dish_name = DishName.Text,
                        recipe = DishRecipe.Text,
                        calories = Convert.ToInt32(DishCalories.Text),
                        protein = Convert.ToDecimal(DishProtein.Text),
                        carbohydrates = Convert.ToDecimal(DishCarbohydrates.Text),
                        fats = Convert.ToDecimal(DishFats.Text),
                        weight = 0,
                        image = DishImage.Text,
                    });
            var response = client.Execute(request);

            var json = JsonConvert.SerializeObject(compositions);

            RestRequest request2 = new RestRequest($"/DishComposition/{response.Content}", Method.Post) { RequestFormat = RestSharp.DataFormat.Json }
                .AddBody(new
                {
                    ingredients = compositions
                });
            var response2 = client.Execute(request2);

            
            compositions = new List<DishComposition>();
            DishCompositionIngredient.Items.Clear();
            //compositions;
            GetDishData();
        }
    }
}

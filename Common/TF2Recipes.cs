using Terraria;
using Terraria.ModLoader;

namespace TF2.Common
{
    // This class contains thoughtful examples of item recipe creation.
    public class TF2Recipes : ModSystem
    {
        // A place to store the recipe group so we can easily use it later
        public RecipeGroup ExampleRecipeGroup; // This used to be static

        public override void Unload() => ExampleRecipeGroup = null;

        //public override void PostAddRecipes()
        //{
        //for (int i = 0; i < Recipe.numRecipes; i++)
        //{
        //Recipe recipe = Main.recipe[i];

        //if (recipe.TryGetIngredient(ModContent.ItemType<Items.Demoman.CharginTarge>(), out Item ingredient))
        //{
        //ingredient.material = false;
        //}
        //}
        //}
    }
}
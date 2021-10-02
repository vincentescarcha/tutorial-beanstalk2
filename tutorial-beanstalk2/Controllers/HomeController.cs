using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Models;
using Shop.Web.DataMapper;
using Shop.Web.Models;
using Shop.Web.Models.Category;
using Shop.Web.Models.Food;
using Shop.Web.Models.Home;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IFood _foodService;
        private readonly Mapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(IFood foodService, SignInManager<ApplicationUser> signInManager)
        {
            _foodService = foodService;
            _signInManager = signInManager;
            _mapper = new Mapper();
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // TODO: Remove for debugging only
            if (!User.Identity.IsAuthenticated)
            {
                await _signInManager.PasswordSignInAsync("testes@test.com", "Test123", false, false);
            }
            var preferedFoods = _foodService.GetPreferred(10);
            var model = _mapper.FoodsToHomeIndexModel(preferedFoods);
            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("/search")]
        public IActionResult Search(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery) || string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("Index");
            }

            var searchedFoods = _foodService.GetFilteredFoods(searchQuery);
            var model = _mapper.FoodsToHomeIndexModel(searchedFoods);

            return View(model);
        }
    }
}

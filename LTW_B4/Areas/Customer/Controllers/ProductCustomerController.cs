using LTW_B4.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LTW_B4.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    [Authorize(Policy = "DenyAccessToCustomer")]
    public class ProductCustomerController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductCustomerController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Action index
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Action chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Không cho phép chỉnh sửa sản phẩm
        [Authorize(Policy = "DenyAccessToCustomer")]
        public IActionResult Edit(int id)
        {
            return Forbid();
        }

        // Không cho phép xóa sản phẩm
        [Authorize(Policy = "DenyAccessToCustomer")]
        public IActionResult Delete(int id)
        {
            return Forbid();
        }
    }
}

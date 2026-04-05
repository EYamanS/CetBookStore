using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CetBookStore.Data;
using CetBookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CetBookStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;

        public CartController(ApplicationDbContext context)
        {
            this.context = context;
        }


        // GET: Cart
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await context.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = 1
                };
                context.CartItems.Add(cartItem);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem != null)
            {
                context.CartItems.Remove(cartItem);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Cart/Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await context.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            decimal total = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in cartItems)
            {
                total = total + item.Book.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Book.Price
                });
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = total,
                OrderItems = orderItems
            };

            context.Orders.Add(order);

            foreach (var item in cartItems)
            {
                context.CartItems.Remove(item);
            }

            await context.SaveChangesAsync();

            return RedirectToAction("MyOrders", "Order");
        }
    }
}

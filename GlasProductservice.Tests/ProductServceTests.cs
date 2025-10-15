using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlasProductservice.Services;
using GlasProductservice.Data;
using GlasProductservice.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GlasProductservice.Tests;

[TestClass]
public class ProductServiceTests
{
    private ProductDbContext _context;
    private ProductService _service;

    [TestInitialize]
    public void Setup()
    {
        //var options = new DbContextOptionsBuilder<ProductDbContext>()
        //    .UseInMemoryDatabase(databaseName: "ProductServiceTestsDB")
        //    .Options;

        var options = new DbContextOptionsBuilder<ProductDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // <—
        .Options;

        _context = new ProductDbContext(options);
        _service = new ProductService(_context);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product { Name = "Glass Panel", Price = 100, Stock = 10 };

        // Act
        var result = await _service.CreateAsync(product);
        var allProducts = await _service.GetAllAsync();

        // Assert
        Assert.AreEqual(1, allProducts.Count());
        Assert.AreEqual("Glass Panel", result.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnCorrectProduct()
    {
        // Arrange
        var product = new Product { Name = "SmartGlass", Price = 200, Stock = 5 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(product.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("SmartGlass", result.Name);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldModifyExistingProduct()
    {
        // Arrange
        var product = new Product { Name = "Old Glass", Price = 150, Stock = 3 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var updated = new Product { Name = "New Glass", Price = 180, Stock = 10 };

        // Act
        var result = await _service.UpdateAsync(product.Id, updated);
        var updatedProduct = await _service.GetByIdAsync(product.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("New Glass", updatedProduct.Name);
        Assert.AreEqual(180, updatedProduct.Price);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var product = new Product { Name = "Temp Product", Price = 50, Stock = 2 };
        _context.Products.Add(product);
        //await _service.CreateAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var deleted = await _service.DeleteAsync(product.Id);
        var allProducts = await _service.GetAllAsync();

        // Assert
        Assert.IsTrue(deleted);
        Assert.AreEqual(0, allProducts.Count());
    }
}

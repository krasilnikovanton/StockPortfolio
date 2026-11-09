using api.Controllers;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Test.Controller
{
    public class SctockControllerTest
    {
        private async Task<ApplicationDBContext> GetDatabseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDBContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Stocks.CountAsync() <= 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Stocks.Add(
                        new Models.Stock()
                        {
                            Symbol = i.ToString(),
                            CompanyName = i.ToString(),
                            Purchase = i,
                            LastDiv = i,
                            Industry = i.ToString(),
                            MarketCap = i,
                            Comments = new List<Models.Comment>(),
                            Portfolios = new List<Models.Portfolio>()
                        }
                    );
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }
        //private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public SctockControllerTest()
        {
            _stockRepo = A.Fake<IStockRepository>();
            //_context = A.Fake<ApplicationDBContext>();
        }

        [Fact]
        public async void SctockControllerTestGetAllReturnOK()
        {
            //Arrange
            var _context = await GetDatabseContext();
            var query = A.Fake<QueryObject>();
            var stocksList = A.Fake<List<Models.Stock>>();
            var stocks = A.CallTo(() => _stockRepo.GetAllAsync(query)).Returns(stocksList);
            var stockDto = A.Fake<List<StockDto>>();
            var controller = new StockController(_context, _stockRepo);

            //Act
            var result = controller.GetAll(query);
            //Assert
            result.Should().NotBeNull();
        }
        [Fact]
        public async void SctockControllerTestGetByIdReturnOK()
        {
            //Arrange
            var _context = await GetDatabseContext();
            int i = 1;
            var stock = A.Fake<Models.Stock>();
            A.CallTo(() => _stockRepo.GetByIdAsync(i)).Returns(stock);
            var controller = new StockController(_context, _stockRepo);

            //Act
            var result = controller.GetById(i);
            //Assert
            result.Should().NotBeNull();
        }
    }
}

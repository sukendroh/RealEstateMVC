using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateMVC.Controllers;
using RealEstateMVC.Data;
using RealEstateMVC.Models;
using Xunit;
using System.Linq;

namespace RealEstateMVC.Tests
{
    public class AgentControllerTests
    {
        private RealEstateContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new RealEstateContext(options);
        }

        [Fact]
        public void Index_Returns_AllAgents()
        {
            var ctx = CreateContext(nameof(Index_Returns_AllAgents));
            ctx.Agents.AddRange(
                new Agent { FirstName = "Alice", LastName = "Smith", Email = "a@x.com", Company = "Acme" },
                new Agent { FirstName = "Bob", LastName = "Jones", Email = "b@x.com", Company = "BetaCorp" }
            );
            ctx.SaveChanges();
            var controller = new AgentController(ctx);

            var result = controller.Index() as ViewResult;
            var model = Assert.IsType<System.Collections.Generic.List<Agent>>(result.Model);

            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_ValidAgent_SavesAndRedirects()
        {
            var ctx = CreateContext(nameof(Create_ValidAgent_SavesAndRedirects));
            var controller = new AgentController(ctx);
            var agent = new Agent { FirstName = "Cara", LastName = "Lane", Email = "c@x.com", Company = "Acme" };

            var result = controller.Create(agent);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(ctx.Agents);
        }

        [Fact]
        public void Create_InvalidModel_ReturnsView()
        {
            var ctx = CreateContext(nameof(Create_InvalidModel_ReturnsView));
            var controller = new AgentController(ctx);
            controller.ModelState.AddModelError("FirstName", "Required");
            var agent = new Agent { FirstName = "", LastName = "", Email = "bad" };

            var result = controller.Create(agent);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(agent, view.Model);
            Assert.Empty(ctx.Agents);
        }

        [Fact]
        public void Search_Returns_MatchingAgents()
        {
            var ctx = CreateContext(nameof(Search_Returns_MatchingAgents));
            ctx.Agents.AddRange(
                new Agent { FirstName = "Ann", LastName = "Smith", Email = "a@x.com", Company = "Acme" },
                new Agent { FirstName = "Ben", LastName = "Smythe", Email = "b@x.com", Company = "BetaCorp" },
                new Agent { FirstName = "Cara", LastName = "Jones", Email = "c@x.com", Company = "GammaGroup" }
            );
            ctx.SaveChanges();
            var controller = new AgentController(ctx);

            var result = controller.Search("Sm") as JsonResult;
            Assert.NotNull(result);

            var list = result.Value as System.Collections.IEnumerable;
            Assert.NotNull(list);

            var count = 0;
            foreach (var item in list)
            {
                count++;
            }

            Assert.Equal(2, count);
        }

        [Fact]
        public void Delete_RemovesAgent()
        {
            var ctx = CreateContext(nameof(Delete_RemovesAgent));
            var agent = new Agent { FirstName = "Del", LastName = "Test", Email = "d@x.com", Company = "Acme" };
            ctx.Agents.Add(agent);
            ctx.SaveChanges();
            var controller = new AgentController(ctx);

            var result = controller.Delete(agent.AgentID);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(ctx.Agents);
        }
    }
}

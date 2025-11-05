using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateMVC.Data;
using RealEstateMVC.Models;
using System.Linq;

namespace RealEstateMVC.Controllers
{
    public class AgentController : Controller
    {
        private readonly RealEstateContext db;

        public AgentController(RealEstateContext context)
        {
            db = context;
        }

        // GET: /Agent/Index
        public IActionResult Index()
        {
            var agents = db.Agents.ToList();
            return View(agents);
        }

        // GET: /Agent/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Agent/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Agent agent)
        {
            Console.WriteLine("POST /Agent/Create triggered");
            Console.WriteLine($"FirstName: {agent.FirstName}");
            Console.WriteLine($"LastName: {agent.LastName}");
            Console.WriteLine($"Email: {agent.Email}");
            Console.WriteLine($"Company: {agent.Company}");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Agents.Add(agent);
                    db.SaveChanges();
                    Console.WriteLine("Agent saved successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving agent: {ex.Message}");
                    ModelState.AddModelError("", "Error saving agent. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid.");
            }

            return View(agent);
        }

        // GET: /Agent/Edit/5
        public IActionResult Edit(int id)
        {
            var agent = db.Agents.Find(id);
            if (agent == null) return NotFound();
            return View(agent);
        }

        // POST: /Agent/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Agent agent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(agent).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Error updating agent. Please try again.");
                }
            }
            return View(agent);
        }

        // POST: /Agent/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var agent = db.Agents.Find(id);
                if (agent != null)
                {
                    db.Agents.Remove(agent);
                    db.SaveChanges();
                }
            }
            catch
            {
                // Handle delete errors
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Agent/Search?lastName=Smith
        [HttpGet]
        public JsonResult Search(string lastName)
        {
            var results = db.Agents
                            .Where(a => a.LastName.StartsWith(lastName))
                            .Select(a => new { a.AgentID, a.FirstName, a.LastName })
                            .ToList();
            return Json(results);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelsForms.Data;
using ModelsForms.Models;

namespace ModelsForms.Controllers
{
    public class VentigratersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentigratersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Ventigraters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ventigrater.ToListAsync());
        }

        // GET: Ventigraters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventigrater = await _context.Ventigrater
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ventigrater == null)
            {
                return NotFound();
            }

            return View(ventigrater);
        }

        // GET: Ventigraters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ventigraters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Title")] Ventigrater ventigrater)
        public async Task<IActionResult> Create([Bind("Id,Name,Title, Experience")] Ventigrater ventigrater)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ventigrater);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ventigrater);
        }

        // GET: Ventigraters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventigrater = await _context.Ventigrater.SingleOrDefaultAsync(m => m.Id == id);
            if (ventigrater == null)
            {
                return NotFound();
            }
            return View(ventigrater);
        }

        // POST: Ventigraters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title")] Ventigrater ventigrater)
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Title,Experience")] Ventigrater ventigrater)
        {
            if (id != ventigrater.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ventigrater);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentigraterExists(ventigrater.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(ventigrater);
        }

        // GET: Ventigraters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ventigrater = await _context.Ventigrater
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ventigrater == null)
            {
                return NotFound();
            }

            return View(ventigrater);
        }

        // POST: Ventigraters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ventigrater = await _context.Ventigrater.SingleOrDefaultAsync(m => m.Id == id);
            _context.Ventigrater.Remove(ventigrater);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool VentigraterExists(int id)
        {
            return _context.Ventigrater.Any(e => e.Id == id);
        }
    }
}

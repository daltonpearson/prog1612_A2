using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dpearson4_MVC_Music.Data;
using dpearson4_MVC_Music.Models;

namespace dpearson4_MVC_Music.Controllers
{
    public class MusiciansController : Controller
    {
        private readonly MusicContext _context;

        public MusiciansController(MusicContext context)
        {
            _context = context;
        }

        // GET: Musicians
        public async Task<IActionResult> Index()
        {
            var musicContext = _context.Musicians.Include(m => m.Instrument);
            return View(await musicContext.ToListAsync());
        }

        // GET: Musicians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians
                .Include(m => m.Instrument)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (musician == null)
            {
                return NotFound();
            }

            return View(musician);
        }

        // GET: Musicians/Create
        public IActionResult Create()
        {
            ViewData["InstrumentID"] = new SelectList(_context.Instruments.OrderBy(x => x.Name), "ID", "Name");
            return View();
        }

        // POST: Musicians/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,DOB,SIN,InstrumentID")] Musician musician)
        {
            if (MusicianSinExists(musician.SIN))
            {
                ModelState.AddModelError("", "Unable to add musician. SIN must be unique.");
                ViewData["InstrumentID"] = new SelectList(_context.Instruments.OrderBy(x => x.Name), "ID", "Name", musician.InstrumentID);
                return View(musician);
            }
            if (ModelState.IsValid)
            {   
                _context.Add(musician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstrumentID"] = new SelectList(_context.Instruments.OrderBy(x => x.Name), "ID", "Name", musician.InstrumentID);
            return View(musician);
        }

        // GET: Musicians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians.FindAsync(id);
            if (musician == null)
            {
                return NotFound();
            }
            ViewData["InstrumentID"] = new SelectList(_context.Instruments.OrderBy(x => x.Name), "ID", "Name", musician.InstrumentID);
            return View(musician);
        }

        // POST: Musicians/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,MiddleName,LastName,Phone,DOB,SIN,InstrumentID")] Musician musician)
        {

            if (MusicianSinExists(musician.SIN, musician.ID))
            {
                ModelState.AddModelError("", "Unable to save changes. SIN must be unique.");
                ViewData["InstrumentID"] = new SelectList(_context.Instruments, "ID", "Name", musician.InstrumentID);
                return View(musician);
            }

            if (id != musician.ID)
            {
                return NotFound();
            }

            Musician musicianToUpdate = await _context.Musicians.SingleOrDefaultAsync(s => s.ID == id);
            

            if (await TryUpdateModelAsync<Musician>(musicianToUpdate,"", m => m.FirstName, m=>m.MiddleName,m=> m.LastName,m=>m.Phone,m=>m.DOB,m=>m.SIN,m=>m.InstrumentID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists, " +
                "see your system administrator.");
                
                }
                
            }
            ViewData["InstrumentID"] = new SelectList(_context.Instruments.OrderBy(x => x.Name), "ID", "Name", musician.InstrumentID);
            return View(musician);
        }

        // GET: Musicians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musician = await _context.Musicians
                .Include(m => m.Instrument)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (musician == null)
            {
                return NotFound();
            }

            return View(musician);
        }

        // POST: Musicians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musician = await _context.Musicians.FindAsync(id);

            if (musician == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Musicians.Remove(musician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool MusicianExists(int id)
        {
            return _context.Musicians.Any(e => e.ID == id);
        }

        private bool MusicianSinExists(string sin, int? id = null)
        {
            return _context.Musicians.Any(e => e.SIN == sin && e.ID != id);
        }
    }
}

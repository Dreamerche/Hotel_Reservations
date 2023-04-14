using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Rsesrvations.Data;
using Hotel_Rsesrvations.Models;

namespace Hotel_Rsesrvations.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary> Отваряне на таблица със записи за стаи
        /// </summary>
        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            List<Room> rooms = await _context.Rooms
    .Include(r => r.Reservations)
        .ThenInclude(r => r.ReservationClients)
            .ThenInclude(rc => rc.Client)
    .ToListAsync();

            foreach (Room item in rooms)
            {
                var reservations = item.Reservations.OrderBy(r => r.checkInDate);
                foreach (Reservation r in reservations)
                {
                    if (r.checkInDate<=DateTime.Now && r.vacatingDate>=DateTime.Now)
                    {
                        item.isOccupied = true;
                    }
                }
            }
            await _context.UpdateRoomsAsync(rooms);

            return View(await _context.Rooms.ToListAsync());
        }

        /// <summary> Извеждане на детайли за дадена стая
        /// </summary>
        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Reservations) // eager load reservations
                    .ThenInclude(r => r.ReservationClients) // eager load reservation clients
                        .ThenInclude(rc => rc.Client) // eager load clients
                .FirstOrDefaultAsync(m => m.ID == id);

            if (room == null)
            {
                return NotFound();
            }
            var reservations = room.Reservations.OrderBy(r => r.checkInDate);

            room.Reservations = new HashSet<Reservation>(reservations);

            return View(room);
        }

        /// <summary> Създаване на стая
        /// </summary>
        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }


        /// <summary> Добавяне на стая
        /// </summary>
        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,capacity,roomType,isOccupied,priceForAdult,priceForChild,number")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        /// <summary> Отваряне на страница за редактиране на стая
        /// </summary>
        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        /// <summary> Редактиране на стая
        /// </summary>
        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,capacity,roomType,isOccupied,priceForAdult,priceForChild,number")] Room room)
        {
            if (id != room.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        /// <summary> Отваряне на страница за изтриване на дадена стая
        /// </summary>
        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.ID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        /// <summary> Изтриване на дадена стая
        /// </summary>
        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary> Метод за проверка дали стаята съществува
        /// </summary>
        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.ID == id);
        }

    }
}

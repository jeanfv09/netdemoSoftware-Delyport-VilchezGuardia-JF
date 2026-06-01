using Microsoft.AspNetCore.Mvc;
using SISDELYPORT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SISDELYPORT.Controllers
{
    public class SolicitudController : Controller
    {
        private static List<Solicitud> _solicitudes = new List<Solicitud>();
        private static int _contador = 1;

        private static List<string> _distritosCobertura = new List<string>
        {
            "San Isidro", "Miraflores", "Surco", "La Molina", "San Borja",
            "Cercado de Lima", "Santa Anita", "San Luis", "El Agustino", "Ate"
        };

        // HU-001: Registrar solicitud - GET
        public IActionResult Registrar()
        {
            ViewBag.Distritos = _distritosCobertura;
            return View();
        }

        // HU-001: Registrar solicitud - POST
        [HttpPost]
        public IActionResult Registrar(Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                solicitud.Id = _contador++;
                solicitud.CodigoSolicitud = "SOL-" + solicitud.Id.ToString("D3");
                solicitud.FechaRegistro = DateTime.Now;
                solicitud.Estado = "Pendiente";
                _solicitudes.Add(solicitud);
                return RedirectToAction("Resumen", new { id = solicitud.Id });
            }
            ViewBag.Distritos = _distritosCobertura;
            return View(solicitud);
        }

        // HU-001: Resumen de solicitud registrada
        public IActionResult Resumen(int id)
        {
            var solicitud = _solicitudes.FirstOrDefault(s => s.Id == id);
            if (solicitud == null) return NotFound();
            return View(solicitud);
        }

        // Placeholder para HU-002 y HU-003 (se completará después)
        public IActionResult Lista()
        {
            return View(_solicitudes);
        }
    }
}
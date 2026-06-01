using Microsoft.AspNetCore.Mvc;
using SISDELYPORT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SISDELYPORT.Controllers
{
    public class SolicitudController : Controller
    {
        private static List<Solicitud> _solicitudes = new List<Solicitud>
        {
            new Solicitud
            {
                Id = 1,
                CodigoSolicitud = "SOL-001",
                NombreCliente = "Bodega El Sol",
                DistritoDestino = "San Isidro",
                DescripcionPaquete = "Caja de electrodomésticos 20kg",
                FechaRequerida = DateTime.Now.AddDays(2),
                Estado = "Pendiente",
                FechaRegistro = DateTime.Now.AddDays(-1)
            },

            new Solicitud
            {
                Id = 2,
                CodigoSolicitud = "SOL-002",
                NombreCliente = "Minimarket Los Andes",
                DistritoDestino = "Miraflores",
                DescripcionPaquete = "Paquete de ropa importada 15kg",
                FechaRequerida = DateTime.Now.AddDays(3),
                Estado = "Validado",
                FechaRegistro = DateTime.Now.AddDays(-2)
            }
        };

        private static int _contador = 3;

        private static List<string> _distritosCobertura = new List<string>
        {
            "San Isidro",
            "Miraflores",
            "Surco",
            "La Molina",
            "San Borja",
            "Cercado de Lima",
            "Santa Anita",
            "San Luis",
            "El Agustino",
            "Ate"
        };

        // =========================
        // REGISTRAR
        // =========================

        public IActionResult Registrar()
        {
            ViewBag.Distritos = _distritosCobertura;
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                solicitud.Id = _contador++;

                solicitud.CodigoSolicitud =
                    "SOL-" + solicitud.Id.ToString("D3");

                solicitud.FechaRegistro = DateTime.Now;

                solicitud.Estado = "Pendiente";

                _solicitudes.Add(solicitud);

                return RedirectToAction(
                    "Resumen",
                    new { id = solicitud.Id }
                );
            }

            ViewBag.Distritos = _distritosCobertura;

            return View(solicitud);
        }

        public IActionResult Resumen(int id)
        {
            var solicitud =
                _solicitudes.FirstOrDefault(s => s.Id == id);

            if (solicitud == null)
                return NotFound();

            return View(solicitud);
        }

        // =========================
        // MODIFICAR
        // =========================

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var solicitud =
                _solicitudes.FirstOrDefault(s => s.Id == id);

            if (solicitud == null)
                return NotFound();

            ViewBag.Distritos = _distritosCobertura;

            ViewBag.Bloqueado =
                solicitud.Estado == "Validado"
                || solicitud.Estado == "Rechazado";

            return View(solicitud);
        }

        [HttpPost]
        public IActionResult Modificar(Solicitud solicitudEditada)
        {
            var solicitud =
                _solicitudes.FirstOrDefault(s => s.Id == solicitudEditada.Id);

            if (solicitud == null)
                return NotFound();

            // VALIDAR BLOQUEO
            if (solicitud.Estado == "Validado"
                || solicitud.Estado == "Rechazado")
            {
                TempData["Error"] =
                    "No se puede modificar una solicitud con estado: "
                    + solicitud.Estado;

                return RedirectToAction(
                    "Modificar",
                    new { id = solicitudEditada.Id }
                );
            }

            // ACTUALIZAR DATOS
            solicitud.NombreCliente =
                solicitudEditada.NombreCliente;

            solicitud.DistritoDestino =
                solicitudEditada.DistritoDestino;

            solicitud.DescripcionPaquete =
                solicitudEditada.DescripcionPaquete;

            solicitud.FechaRequerida =
                solicitudEditada.FechaRequerida;

            // HISTORIAL
            solicitud.HistorialCambios +=
                $"[{DateTime.Now:dd/MM/yyyy HH:mm}] " +
                $"Solicitud modificada. ";

            TempData["Exito"] =
                $"Solicitud {solicitud.CodigoSolicitud} modificada correctamente.";

            // REDIRECCIONAR A LISTA
            return RedirectToAction("Lista");
        }

        // =========================
        // LISTA
        // =========================

        public IActionResult Lista()
        {
            return View(_solicitudes);
        }

        // =========================
        // VALIDAR
        // =========================

        public IActionResult Validar(int id)
        {
            return RedirectToAction("Lista");
        }
    }
}
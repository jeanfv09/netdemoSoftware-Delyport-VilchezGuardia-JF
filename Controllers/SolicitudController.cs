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
            },
            new Solicitud
            {
                Id = 3,
                CodigoSolicitud = "SOL-003",
                NombreCliente = "Tienda Don Carlos",
                DistritoDestino = "Huacho",
                DescripcionPaquete = "Accesorios tecnológicos 5kg",
                FechaRequerida = DateTime.Now.AddDays(1),
                Estado = "Pendiente",
                FechaRegistro = DateTime.Now.AddDays(-1)
            },
            // Caso 2 DoD: campos incompletos para probar rechazo por datos
            new Solicitud
            {
                Id = 4,
                CodigoSolicitud = "SOL-004",
                NombreCliente = "",
                DistritoDestino = "Ate",
                DescripcionPaquete = "",
                FechaRequerida = DateTime.Now.AddDays(1),
                Estado = "Pendiente",
                FechaRegistro = DateTime.Now.AddDays(-1)
            }
        };

        private static int _contador = 5;

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

        // Método público estático para acceso externo si se necesita
        public static List<Solicitud> ObtenerSolicitudes() => _solicitudes;

        // =========================
        // HU-001: REGISTRAR
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
                solicitud.CodigoSolicitud = "SOL-" + solicitud.Id.ToString("D3");
                solicitud.FechaRegistro = DateTime.Now;
                solicitud.Estado = "Pendiente";
                _solicitudes.Add(solicitud);
                return RedirectToAction("Resumen", new { id = solicitud.Id });
            }
            ViewBag.Distritos = _distritosCobertura;
            return View(solicitud);
        }

        public IActionResult Resumen(int id)
        {
            var solicitud = _solicitudes.FirstOrDefault(s => s.Id == id);
            if (solicitud == null) return NotFound();
            return View(solicitud);
        }

        // =========================
        // HU-002: MODIFICAR
        // =========================

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var solicitud = _solicitudes.FirstOrDefault(s => s.Id == id);
            if (solicitud == null) return NotFound();

            ViewBag.Distritos = _distritosCobertura;
            ViewBag.Bloqueado = solicitud.Estado == "Validado"
                             || solicitud.Estado == "Rechazado";
            return View(solicitud);
        }

        [HttpPost]
        public IActionResult Modificar(Solicitud solicitudEditada)
        {
            var solicitud = _solicitudes.FirstOrDefault(s => s.Id == solicitudEditada.Id);
            if (solicitud == null) return NotFound();

            // Bloqueo: no se puede editar si ya fue validada o rechazada
            if (solicitud.Estado == "Validado" || solicitud.Estado == "Rechazado")
            {
                TempData["Error"] = "No se puede modificar una solicitud con estado: "
                                  + solicitud.Estado;
                return RedirectToAction("Modificar", new { id = solicitudEditada.Id });
            }

            // Actualizar datos
            solicitud.NombreCliente      = solicitudEditada.NombreCliente;
            solicitud.DistritoDestino    = solicitudEditada.DistritoDestino;
            solicitud.DescripcionPaquete = solicitudEditada.DescripcionPaquete;
            solicitud.FechaRequerida     = solicitudEditada.FechaRequerida;

            // Registrar historial de cambios
            solicitud.HistorialCambios +=
                $"[{DateTime.Now:dd/MM/yyyy HH:mm}] Modificado por Vilchez Guardia. ";

            TempData["Exito"] = $"Solicitud {solicitud.CodigoSolicitud} modificada correctamente.";
            return RedirectToAction("Lista");
        }

        // =========================
        // HU-003: LISTA Y VALIDAR
        // =========================

        public IActionResult Lista()
        {
            return View(_solicitudes);
        }

        public IActionResult Validar(int id)
        {
            var solicitud = _solicitudes.FirstOrDefault(s => s.Id == id);
            if (solicitud == null) return NotFound();

            // Regla 1: datos obligatorios completos
            if (string.IsNullOrWhiteSpace(solicitud.NombreCliente) ||
                string.IsNullOrWhiteSpace(solicitud.DistritoDestino) ||
                string.IsNullOrWhiteSpace(solicitud.DescripcionPaquete))
            {
                solicitud.Estado = "Rechazado";
                solicitud.MotivoRechazo = "Datos obligatorios incompletos.";
            }
            // Regla 2: distrito dentro de cobertura
            else if (!_distritosCobertura.Contains(solicitud.DistritoDestino))
            {
                solicitud.Estado = "Rechazado";
                solicitud.MotivoRechazo = "Distrito '" + solicitud.DistritoDestino
                                        + "' fuera de cobertura de DELYPORT.";
            }
            else
            {
                solicitud.Estado = "Validado";
                solicitud.MotivoRechazo = "";
            }

            TempData["Exito"] = solicitud.Estado == "Validado"
                ? "✅ Solicitud " + solicitud.CodigoSolicitud + " validada correctamente."
                : "❌ Solicitud " + solicitud.CodigoSolicitud
                  + " rechazada: " + solicitud.MotivoRechazo;

            return RedirectToAction("Lista");
        }
    }
}
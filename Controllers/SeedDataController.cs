using Microsoft.AspNetCore.Mvc;
using ExpandeBO_Backend.Models;
using ExpandeBO_Backend.Repositories;
using BCrypt.Net;

namespace ExpandeBO_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedDataController : ControllerBase
{
    private readonly ICiudadRepository _ciudadRepository;
    private readonly IPlanRepository _planRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ISubcategoriaRepository _subcategoriaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IPerfilComercialRepository _perfilComercialRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly ISuscripcionEmpresaRepository _suscripcionEmpresaRepository;

    public SeedDataController(
        ICiudadRepository ciudadRepository,
        IPlanRepository planRepository,
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepository,
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IPerfilComercialRepository perfilComercialRepository,
        IProductoRepository productoRepository,
        ISuscripcionEmpresaRepository suscripcionEmpresaRepository)
    {
        _ciudadRepository = ciudadRepository;
        _planRepository = planRepository;
        _categoriaRepository = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepository;
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _perfilComercialRepository = perfilComercialRepository;
        _productoRepository = productoRepository;
        _suscripcionEmpresaRepository = suscripcionEmpresaRepository;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            // 1. Seed Ciudades de Bolivia
            var ciudades = await SeedCiudades();
            
            // 2. Seed Planes
            var planes = await SeedPlanes();
            
            // 3. Seed Categorías
            var categorias = await SeedCategorias();
            
            // 4. Seed Subcategorías
            var subcategorias = await SeedSubcategorias(categorias);
            
            // 5. Seed Empresas y Usuarios
            var empresas = await SeedEmpresas(ciudades);
            
            // 6. Seed Perfiles Comerciales
            var perfiles = await SeedPerfilesComerciales(empresas, ciudades);
            
            // 7. Seed Productos
            var productos = await SeedProductos(perfiles, categorias, subcategorias);

            return Ok(new
            {
                message = "Base de datos poblada exitosamente",
                ciudades = ciudades.Count,
                planes = planes.Count,
                categorias = categorias.Count,
                subcategorias = subcategorias.Count,
                empresas = empresas.Count,
                perfiles = perfiles.Count,
                productos = productos.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al poblar la base de datos", error = ex.Message });
        }
    }

    private async Task<List<Ciudad>> SeedCiudades()
    {
        var ciudadesBolivia = new List<(string nombre, string codigo)>
        {
            ("La Paz", "LP"),
            ("El Alto", "EA"),
            ("Cochabamba", "CB"),
            ("Santa Cruz", "SC"),
            ("Oruro", "OR"),
            ("Potosí", "PT"),
            ("Sucre", "SU"),
            ("Tarija", "TJ"),
            ("Trinidad", "TR"),
            ("Cobija", "CO"),
            ("Riberalta", "RB"),
            ("Montero", "MO"),
            ("Warnes", "WA"),
            ("Yacuiba", "YA"),
            ("Villazón", "VZ"),
            ("Tupiza", "TU"),
            ("Camiri", "CA"),
            ("Quillacollo", "QU"),
            ("Sacaba", "SA"),
            ("Villa Tunari", "VT")
        };

        var ciudadesCreadas = new List<Ciudad>();

        foreach (var (nombre, codigo) in ciudadesBolivia)
        {
            var ciudadExistente = (await _ciudadRepository.GetAllAsync())
                .FirstOrDefault(c => c.Nombre == nombre);

            if (ciudadExistente == null)
            {
                var ciudad = new Ciudad
                {
                    Nombre = nombre,
                    Codigo = codigo,
                    Activa = true,
                    FechaCreacion = DateTime.UtcNow
                };
                ciudadesCreadas.Add(await _ciudadRepository.CreateAsync(ciudad));
            }
            else
            {
                ciudadesCreadas.Add(ciudadExistente);
            }
        }

        return ciudadesCreadas;
    }

    private async Task<List<Plan>> SeedPlanes()
    {
        var planes = new List<Plan>
        {
            new Plan
            {
                Nombre = "Plan Básico",
                Descripcion = "Plan ideal para empresas pequeñas que comienzan",
                Precio = 99.00m,
                DuracionDias = 30,
                MaxPerfilesComerciales = 1,
                MaxProductos = 50,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Plan
            {
                Nombre = "Plan Premium",
                Descripcion = "Plan para empresas en crecimiento",
                Precio = 199.00m,
                DuracionDias = 30,
                MaxPerfilesComerciales = 3,
                MaxProductos = 200,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Plan
            {
                Nombre = "Plan Empresarial",
                Descripcion = "Plan completo para grandes empresas",
                Precio = 399.00m,
                DuracionDias = 30,
                MaxPerfilesComerciales = 10,
                MaxProductos = 1000,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Plan
            {
                Nombre = "Plan Anual Básico",
                Descripcion = "Plan básico con descuento anual",
                Precio = 990.00m,
                DuracionDias = 365,
                MaxPerfilesComerciales = 1,
                MaxProductos = 50,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Plan
            {
                Nombre = "Plan Anual Premium",
                Descripcion = "Plan premium con descuento anual",
                Precio = 1990.00m,
                DuracionDias = 365,
                MaxPerfilesComerciales = 3,
                MaxProductos = 200,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        var planesCreados = new List<Plan>();

        foreach (var plan in planes)
        {
            var planExistente = (await _planRepository.GetAllAsync())
                .FirstOrDefault(p => p.Nombre == plan.Nombre);

            if (planExistente == null)
            {
                planesCreados.Add(await _planRepository.CreateAsync(plan));
            }
            else
            {
                planesCreados.Add(planExistente);
            }
        }

        return planesCreados;
    }

    private async Task<List<Categoria>> SeedCategorias()
    {
        var categorias = new List<Categoria>
        {
            new Categoria
            {
                Nombre = "Alimentos y Bebidas",
                Descripcion = "Productos alimenticios y bebidas",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Tecnología",
                Descripcion = "Dispositivos y accesorios tecnológicos",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Ropa y Accesorios",
                Descripcion = "Ropa, calzado y accesorios de moda",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Hogar y Jardín",
                Descripcion = "Artículos para el hogar y jardín",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Salud y Belleza",
                Descripcion = "Productos de cuidado personal y belleza",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Deportes y Recreación",
                Descripcion = "Artículos deportivos y recreativos",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Libros y Material Educativo",
                Descripcion = "Libros, material educativo y papelería",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Categoria
            {
                Nombre = "Juguetes y Juegos",
                Descripcion = "Juguetes y juegos para todas las edades",
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        var categoriasCreadas = new List<Categoria>();

        foreach (var categoria in categorias)
        {
            var categoriaExistente = (await _categoriaRepository.GetAllAsync())
                .FirstOrDefault(c => c.Nombre == categoria.Nombre);

            if (categoriaExistente == null)
            {
                categoriasCreadas.Add(await _categoriaRepository.CreateAsync(categoria));
            }
            else
            {
                categoriasCreadas.Add(categoriaExistente);
            }
        }

        return categoriasCreadas;
    }

    private async Task<List<Subcategoria>> SeedSubcategorias(List<Categoria> categorias)
    {
        var subcategorias = new List<(string categoriaNombre, string nombre, string descripcion)>
        {
            // Alimentos y Bebidas
            ("Alimentos y Bebidas", "Frutas y Verduras", "Frutas y verduras frescas"),
            ("Alimentos y Bebidas", "Carnes y Pescados", "Carnes, aves y pescados"),
            ("Alimentos y Bebidas", "Lácteos", "Leche, queso, yogurt y derivados"),
            ("Alimentos y Bebidas", "Panadería", "Pan, pasteles y productos de panadería"),
            ("Alimentos y Bebidas", "Bebidas", "Bebidas alcohólicas y no alcohólicas"),
            ("Alimentos y Bebidas", "Snacks", "Snacks y aperitivos"),
            
            // Tecnología
            ("Tecnología", "Smartphones", "Teléfonos inteligentes"),
            ("Tecnología", "Computadoras", "Laptops y computadoras de escritorio"),
            ("Tecnología", "Tablets", "Tablets y dispositivos táctiles"),
            ("Tecnología", "Accesorios", "Cargadores, cables y accesorios"),
            ("Tecnología", "Audio", "Auriculares, altavoces y audio"),
            
            // Ropa y Accesorios
            ("Ropa y Accesorios", "Ropa Masculina", "Ropa para hombres"),
            ("Ropa y Accesorios", "Ropa Femenina", "Ropa para mujeres"),
            ("Ropa y Accesorios", "Ropa Infantil", "Ropa para niños"),
            ("Ropa y Accesorios", "Calzado", "Zapatos y calzado"),
            ("Ropa y Accesorios", "Accesorios", "Bolsos, relojes y accesorios"),
            
            // Hogar y Jardín
            ("Hogar y Jardín", "Muebles", "Muebles para el hogar"),
            ("Hogar y Jardín", "Decoración", "Artículos de decoración"),
            ("Hogar y Jardín", "Cocina", "Utensilios y electrodomésticos de cocina"),
            ("Hogar y Jardín", "Jardín", "Herramientas y plantas para jardín"),
            
            // Salud y Belleza
            ("Salud y Belleza", "Cuidado Facial", "Productos para el cuidado facial"),
            ("Salud y Belleza", "Cuidado Corporal", "Productos para el cuidado del cuerpo"),
            ("Salud y Belleza", "Maquillaje", "Productos de maquillaje"),
            ("Salud y Belleza", "Perfumes", "Perfumes y fragancias"),
            
            // Deportes y Recreación
            ("Deportes y Recreación", "Fútbol", "Artículos de fútbol"),
            ("Deportes y Recreación", "Ciclismo", "Bicicletas y accesorios"),
            ("Deportes y Recreación", "Fitness", "Equipamiento de fitness"),
            ("Deportes y Recreación", "Camping", "Artículos para camping"),
            
            // Libros y Material Educativo
            ("Libros y Material Educativo", "Libros", "Libros de todas las categorías"),
            ("Libros y Material Educativo", "Material Escolar", "Cuadernos, lápices y material escolar"),
            ("Libros y Material Educativo", "Electrónica Educativa", "Tablets y dispositivos educativos"),
            
            // Juguetes y Juegos
            ("Juguetes y Juegos", "Juguetes para Bebés", "Juguetes para bebés y niños pequeños"),
            ("Juguetes y Juegos", "Juguetes Educativos", "Juguetes educativos y didácticos"),
            ("Juguetes y Juegos", "Juegos de Mesa", "Juegos de mesa y cartas"),
            ("Juguetes y Juegos", "Videojuegos", "Videojuegos y consolas")
        };

        var subcategoriasCreadas = new List<Subcategoria>();

        foreach (var (categoriaNombre, nombre, descripcion) in subcategorias)
        {
            var categoria = categorias.FirstOrDefault(c => c.Nombre == categoriaNombre);
            if (categoria == null) continue;

            var subcategoriaExistente = (await _subcategoriaRepository.GetAllAsync())
                .FirstOrDefault(s => s.Nombre == nombre && s.CategoriaId == categoria.Id);

            if (subcategoriaExistente == null)
            {
                var subcategoria = new Subcategoria
                {
                    CategoriaId = categoria.Id!,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Activa = true,
                    FechaCreacion = DateTime.UtcNow
                };
                subcategoriasCreadas.Add(await _subcategoriaRepository.CreateAsync(subcategoria));
            }
            else
            {
                subcategoriasCreadas.Add(subcategoriaExistente);
            }
        }

        return subcategoriasCreadas;
    }

    private async Task<List<Empresa>> SeedEmpresas(List<Ciudad> ciudades)
    {
        var empresasData = new List<(string razonSocial, string nit, string email, string password, string nombre, string apellido, string telefono, string direccion, string ciudadNombre)>
        {
            ("Supermercado La Paz S.A.", "123456789", "supermercadolapaz@example.com", "Password123", "Carlos", "Mamani", "71234567", "Av. 16 de Julio 1234", "La Paz"),
            ("Distribuidora Tech Bolivia", "987654321", "techbolivia@example.com", "Password123", "María", "García", "72234567", "Av. Mariscal Santa Cruz 567", "La Paz"),
            ("Ropa Fashion Bolivia", "112233445", "fashionbolivia@example.com", "Password123", "Juan", "Quispe", "73234567", "Calle Comercio 890", "El Alto"),
            ("Alimentos Frescos S.R.L.", "556677889", "alimentosfrescos@example.com", "Password123", "Ana", "Choque", "74234567", "Av. Blanco Galindo 123", "Cochabamba"),
            ("Electrohogar Bolivia", "998877665", "electrohogar@example.com", "Password123", "Pedro", "Vargas", "75234567", "Av. Banzer 456", "Santa Cruz"),
            ("Deportes Max S.A.", "443322110", "deportesmax@example.com", "Password123", "Laura", "Fernández", "76234567", "Calle España 789", "Oruro"),
            ("Librería Educativa", "778899001", "libreriaeducativa@example.com", "Password123", "Roberto", "Mendoza", "77234567", "Av. 25 de Mayo 234", "Potosí"),
            ("Juguetes del Sur", "221100334", "juguetesdelsur@example.com", "Password123", "Carmen", "Ramos", "78234567", "Calle Sucre 567", "Tarija")
        };

        var empresasCreadas = new List<Empresa>();

        foreach (var (razonSocial, nit, email, password, nombre, apellido, telefono, direccion, ciudadNombre) in empresasData)
        {
            var usuarioExistente = await _usuarioRepository.GetByEmailAsync(email);
            if (usuarioExistente != null)
            {
                var empresaExistente = await _empresaRepository.GetByUsuarioIdAsync(usuarioExistente.Id!);
                if (empresaExistente != null)
                {
                    empresasCreadas.Add(empresaExistente);
                    continue;
                }
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Email = email,
                PasswordHash = passwordHash,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Rol = "Empresa",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            usuario = await _usuarioRepository.CreateAsync(usuario);

            var empresa = new Empresa
            {
                UsuarioId = usuario.Id!,
                RazonSocial = razonSocial,
                NIT = nit,
                Direccion = direccion,
                Telefono = telefono,
                Email = email,
                Activa = true,
                FechaCreacion = DateTime.UtcNow
            };

            empresa = await _empresaRepository.CreateAsync(empresa);
            empresasCreadas.Add(empresa);
        }

        return empresasCreadas;
    }

    private async Task<List<PerfilComercial>> SeedPerfilesComerciales(List<Empresa> empresas, List<Ciudad> ciudades)
    {
        var perfilesData = new List<(string empresaRazonSocial, string nombre, string descripcion, string direccion, string ciudadNombre, string telefono, string email, double latitud, double longitud, string horarioApertura, string horarioCierre)>
        {
            ("Supermercado La Paz S.A.", "Sucursal Centro", "Supermercado en el centro de La Paz", "Av. 16 de Julio 1234", "La Paz", "71234567", "centro@supermercadolapaz.com", -16.5000, -68.1500, "08:00", "22:00"),
            ("Distribuidora Tech Bolivia", "Tienda Principal", "Tienda de tecnología en La Paz", "Av. Mariscal Santa Cruz 567", "La Paz", "72234567", "tienda@techbolivia.com", -16.5100, -68.1200, "09:00", "20:00"),
            ("Ropa Fashion Bolivia", "Boutique El Alto", "Boutique de moda en El Alto", "Calle Comercio 890", "El Alto", "73234567", "boutique@fashionbolivia.com", -16.5047, -68.1635, "10:00", "19:00"),
            ("Alimentos Frescos S.R.L.", "Mercado Central", "Puesto en el mercado central", "Av. Blanco Galindo 123", "Cochabamba", "74234567", "mercado@alimentosfrescos.com", -17.3935, -66.1570, "06:00", "18:00"),
            ("Electrohogar Bolivia", "Sucursal Santa Cruz", "Tienda de electrodomésticos", "Av. Banzer 456", "Santa Cruz", "75234567", "santacruz@electrohogar.com", -17.8146, -63.1561, "09:00", "21:00"),
            ("Deportes Max S.A.", "Tienda Deportiva", "Tienda de artículos deportivos", "Calle España 789", "Oruro", "76234567", "tienda@deportesmax.com", -17.9749, -67.1120, "08:00", "20:00"),
            ("Librería Educativa", "Librería Central", "Librería y material educativo", "Av. 25 de Mayo 234", "Potosí", "77234567", "central@libreriaeducativa.com", -19.5833, -65.7531, "09:00", "19:00"),
            ("Juguetes del Sur", "Tienda Tarija", "Tienda de juguetes y juegos", "Calle Sucre 567", "Tarija", "78234567", "tarija@juguetesdelsur.com", -21.5318, -64.7311, "10:00", "20:00")
        };

        var perfilesCreados = new List<PerfilComercial>();

        foreach (var (empresaRazonSocial, nombre, descripcion, direccion, ciudadNombre, telefono, email, latitud, longitud, horarioApertura, horarioCierre) in perfilesData)
        {
            var empresa = empresas.FirstOrDefault(e => e.RazonSocial == empresaRazonSocial);
            if (empresa == null) continue;

            var perfilExistente = (await _perfilComercialRepository.GetByEmpresaIdAsync(empresa.Id!))
                .FirstOrDefault(p => p.Nombre == nombre);

            if (perfilExistente == null)
            {
                var perfil = new PerfilComercial
                {
                    EmpresaId = empresa.Id!,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Direccion = direccion,
                    CiudadId = ciudades.FirstOrDefault(c => c.Nombre == ciudadNombre)?.Id ?? "",
                    Telefono = telefono,
                    Email = email,
                    Latitud = latitud,
                    Longitud = longitud,
                    HorarioApertura = horarioApertura,
                    HorarioCierre = horarioCierre,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                perfilesCreados.Add(await _perfilComercialRepository.CreateAsync(perfil));
            }
            else
            {
                perfilesCreados.Add(perfilExistente);
            }
        }

        return perfilesCreados;
    }

    private async Task<List<Producto>> SeedProductos(List<PerfilComercial> perfiles, List<Categoria> categorias, List<Subcategoria> subcategorias)
    {
        var productosData = new List<(string perfilNombre, string nombre, string descripcion, decimal precio, int stock, string categoriaNombre, string subcategoriaNombre, string imagenUrl)>
        {
            // Supermercado La Paz
            ("Sucursal Centro", "Manzanas Rojas", "Manzanas rojas frescas importadas", 8.50m, 100, "Alimentos y Bebidas", "Frutas y Verduras", ""),
            ("Sucursal Centro", "Leche Entera 1L", "Leche entera pasteurizada", 6.50m, 200, "Alimentos y Bebidas", "Lácteos", ""),
            ("Sucursal Centro", "Pan Integral", "Pan integral recién horneado", 4.00m, 150, "Alimentos y Bebidas", "Panadería", ""),
            ("Sucursal Centro", "Pollo Entero", "Pollo fresco entero", 12.00m, 50, "Alimentos y Bebidas", "Carnes y Pescados", ""),
            
            // Tech Bolivia
            ("Tienda Principal", "Smartphone Samsung Galaxy", "Smartphone Android con 128GB", 2500.00m, 25, "Tecnología", "Smartphones", ""),
            ("Tienda Principal", "Laptop HP 15", "Laptop HP 15 pulgadas, 8GB RAM", 3500.00m, 15, "Tecnología", "Computadoras", ""),
            ("Tienda Principal", "Tablet iPad", "Tablet Apple iPad 10.2 pulgadas", 2800.00m, 10, "Tecnología", "Tablets", ""),
            ("Tienda Principal", "Auriculares Bluetooth", "Auriculares inalámbricos con cancelación de ruido", 350.00m, 40, "Tecnología", "Audio", ""),
            
            // Fashion Bolivia
            ("Boutique El Alto", "Camisa Formal Hombre", "Camisa formal de algodón para hombre", 120.00m, 30, "Ropa y Accesorios", "Ropa Masculina", ""),
            ("Boutique El Alto", "Vestido Casual Mujer", "Vestido casual elegante para mujer", 180.00m, 25, "Ropa y Accesorios", "Ropa Femenina", ""),
            ("Boutique El Alto", "Zapatos Deportivos", "Zapatos deportivos unisex", 250.00m, 40, "Ropa y Accesorios", "Calzado", ""),
            
            // Alimentos Frescos
            ("Mercado Central", "Tomates Orgánicos", "Tomates orgánicos frescos", 5.00m, 80, "Alimentos y Bebidas", "Frutas y Verduras", ""),
            ("Mercado Central", "Queso Fresco", "Queso fresco artesanal", 15.00m, 60, "Alimentos y Bebidas", "Lácteos", ""),
            ("Mercado Central", "Huevos AA", "Huevos AA frescos, docena", 8.00m, 100, "Alimentos y Bebidas", "Carnes y Pescados", ""),
            
            // Electrohogar
            ("Sucursal Santa Cruz", "Refrigerador Samsung", "Refrigerador de 300L, eficiencia energética A+", 2800.00m, 8, "Hogar y Jardín", "Cocina", ""),
            ("Sucursal Santa Cruz", "Microondas LG", "Microondas de 20L con grill", 450.00m, 20, "Hogar y Jardín", "Cocina", ""),
            ("Sucursal Santa Cruz", "Sofá 3 Plazas", "Sofá cómodo de 3 plazas", 1200.00m, 12, "Hogar y Jardín", "Muebles", ""),
            
            // Deportes Max
            ("Tienda Deportiva", "Balón de Fútbol", "Balón de fútbol oficial tamaño 5", 85.00m, 50, "Deportes y Recreación", "Fútbol", ""),
            ("Tienda Deportiva", "Bicicleta Mountain Bike", "Bicicleta de montaña 21 velocidades", 1800.00m, 10, "Deportes y Recreación", "Ciclismo", ""),
            ("Tienda Deportiva", "Pesas Ajustables", "Juego de pesas ajustables 2x20kg", 450.00m, 15, "Deportes y Recreación", "Fitness", ""),
            
            // Librería Educativa
            ("Librería Central", "Libro: Historia de Bolivia", "Libro de historia de Bolivia", 45.00m, 30, "Libros y Material Educativo", "Libros", ""),
            ("Librería Central", "Cuaderno Universitario", "Cuaderno universitario rayado, 100 hojas", 12.00m, 200, "Libros y Material Educativo", "Material Escolar", ""),
            ("Librería Central", "Lápices de Colores", "Caja de lápices de colores 24 unidades", 25.00m, 80, "Libros y Material Educativo", "Material Escolar", ""),
            
            // Juguetes del Sur
            ("Tienda Tarija", "Muñeca Barbie", "Muñeca Barbie con accesorios", 120.00m, 25, "Juguetes y Juegos", "Juguetes para Bebés", ""),
            ("Tienda Tarija", "Lego Classic", "Set de construcción Lego Classic 500 piezas", 350.00m, 20, "Juguetes y Juegos", "Juguetes Educativos", ""),
            ("Tienda Tarija", "Monopoly", "Juego de mesa Monopoly clásico", 180.00m, 15, "Juguetes y Juegos", "Juegos de Mesa", "")
        };

        var productosCreados = new List<Producto>();

        foreach (var (perfilNombre, nombre, descripcion, precio, stock, categoriaNombre, subcategoriaNombre, imagenUrl) in productosData)
        {
            var perfil = perfiles.FirstOrDefault(p => p.Nombre == perfilNombre);
            if (perfil == null) continue;

            var categoria = categorias.FirstOrDefault(c => c.Nombre == categoriaNombre);
            if (categoria == null) continue;

            var subcategoria = subcategorias.FirstOrDefault(s => s.Nombre == subcategoriaNombre && s.CategoriaId == categoria.Id);
            if (subcategoria == null) continue;

            var productoExistente = (await _productoRepository.GetAllAsync())
                .FirstOrDefault(p => p.Nombre == nombre && p.PerfilComercialId == perfil.Id);

            if (productoExistente == null)
            {
                var producto = new Producto
                {
                    PerfilComercialId = perfil.Id!,
                    CategoriaId = categoria.Id!,
                    SubcategoriaId = subcategoria.Id!,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Precio = precio,
                    Stock = stock,
                    ImagenUrl = imagenUrl,
                    Disponible = true,
                    FechaCreacion = DateTime.UtcNow
                };
                productosCreados.Add(await _productoRepository.CreateAsync(producto));
            }
            else
            {
                productosCreados.Add(productoExistente);
            }
        }

        return productosCreados;
    }

    [HttpPost("seed-empresas-completas")]
    public async Task<IActionResult> SeedEmpresasCompletas()
    {
        try
        {
            // Obtener ciudades específicas
            var todasCiudades = await _ciudadRepository.GetAllAsync();
            var ciudadesFiltradas = todasCiudades.Where(c => 
                c.Nombre == "Santa Cruz" || 
                c.Nombre == "Cochabamba" || 
                c.Nombre == "La Paz" || 
                c.Nombre == "El Alto" || 
                c.Nombre == "Oruro" || 
                c.Nombre == "Potosí" || 
                c.Nombre == "Sucre" || 
                c.Nombre == "Trinidad" || 
                c.Nombre == "Tarija"
            ).ToList();

            if (ciudadesFiltradas.Count == 0)
            {
                return BadRequest(new { message = "No se encontraron las ciudades especificadas" });
            }

            // Obtener planes, categorías y subcategorías existentes
            var planes = await _planRepository.GetAllAsync();
            var categorias = await _categoriaRepository.GetAllAsync();
            var subcategorias = await _subcategoriaRepository.GetAllAsync();

            // Crear empresas, perfiles y productos
            var empresasCreadas = await SeedEmpresasConTodo(ciudadesFiltradas, planes, categorias, subcategorias);

            return Ok(new
            {
                message = "Empresas con planes, perfiles y productos creados exitosamente",
                empresas = empresasCreadas.Count,
                ciudades = ciudadesFiltradas.Select(c => c.Nombre).ToList()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al poblar las empresas", error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    private async Task<List<Empresa>> SeedEmpresasConTodo(
        List<Ciudad> ciudades, 
        List<Plan> planes, 
        List<Categoria> categorias, 
        List<Subcategoria> subcategorias)
    {
        var empresasCreadas = new List<Empresa>();
        var random = new Random();

        // Definir empresas por ciudad con marcas conocidas de Bolivia
        var empresasData = new List<dynamic>
        {
            // SANTA CRUZ
            new {
                Ciudad = "Santa Cruz",
                RazonSocial = "Supermercado Fidalga S.A.",
                NIT = "1020304050",
                Email = "fidalga@santacruz.bo",
                Password = "Password123",
                Nombre = "Roberto",
                Apellido = "Vargas",
                Telefono = "75300001",
                Direccion = "Av. Ejército Nacional 100",
                LogoUrl = "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=400",
                Plan = "Plan Premium",
                Perfil = new {
                    Nombre = "Fidalga Centro",
                    Descripcion = "Supermercado líder en Santa Cruz",
                    Direccion = "Av. Ejército Nacional 100",
                    Telefono = "75300001",
                    Email = "centro@fidalga.bo",
                    Latitud = -17.8146,
                    Longitud = -63.1561,
                    HorarioApertura = "08:00",
                    HorarioCierre = "22:00"
                },
                Productos = new[] {
                    new { Nombre = "Aceite IDEAL 1L", Descripcion = "Aceite vegetal IDEAL", Precio = 18.50m, Stock = 150, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1474979266404-7eaacbcd87c5?w=400" },
                    new { Nombre = "Yogurt PIL 1L", Descripcion = "Yogurt natural PIL", Precio = 12.00m, Stock = 200, Categoria = "Alimentos y Bebidas", Subcategoria = "Lácteos", Imagen = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400" },
                    new { Nombre = "Gaseosa COCA COLA 2L", Descripcion = "Coca Cola 2 litros", Precio = 15.00m, Stock = 180, Categoria = "Alimentos y Bebidas", Subcategoria = "Bebidas", Imagen = "https://images.unsplash.com/photo-1554866585-cd94860890b7?w=400" },
                    new { Nombre = "Arroz PIL 1kg", Descripcion = "Arroz Pil extra calidad", Precio = 8.50m, Stock = 300, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400" },
                    new { Nombre = "Azúcar SUCRE 1kg", Descripcion = "Azúcar blanca Sucre", Precio = 7.00m, Stock = 250, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=400" }
                }
            },
            new {
                Ciudad = "Santa Cruz",
                RazonSocial = "Atelier Antezana",
                NIT = "2020304050",
                Email = "atelier@antezana.bo",
                Password = "Password123",
                Nombre = "Sophie",
                Apellido = "Antezana",
                Telefono = "75300002",
                Direccion = "Av. Libertador 250",
                LogoUrl = "https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=400",
                Plan = "Plan Básico",
                Perfil = new {
                    Nombre = "Atelier Antezana",
                    Descripcion = "Restaurante francés auténtico en Santa Cruz",
                    Direccion = "Av. Libertador 250",
                    Telefono = "75300002",
                    Email = "info@atelierantezana.bo",
                    Latitud = -17.8200,
                    Longitud = -63.1500,
                    HorarioApertura = "12:00",
                    HorarioCierre = "22:00"
                },
                Productos = new[] {
                    new { Nombre = "Ratatouille", Descripcion = "Plato tradicional francés de verduras", Precio = 85.00m, Stock = 20, Categoria = "Alimentos y Bebidas", Subcategoria = "Frutas y Verduras", Imagen = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400" },
                    new { Nombre = "Coq au Vin", Descripcion = "Pollo al vino tinto francés", Precio = 120.00m, Stock = 15, Categoria = "Alimentos y Bebidas", Subcategoria = "Carnes y Pescados", Imagen = "https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=400" },
                    new { Nombre = "Croissant Francés", Descripcion = "Croissant artesanal tradicional", Precio = 15.00m, Stock = 50, Categoria = "Alimentos y Bebidas", Subcategoria = "Panadería", Imagen = "https://images.unsplash.com/photo-1555507036-ab1f4038808a?w=400" },
                    new { Nombre = "Bouillabaisse", Descripcion = "Sopa de pescado marsellesa", Precio = 95.00m, Stock = 18, Categoria = "Alimentos y Bebidas", Subcategoria = "Carnes y Pescados", Imagen = "https://images.unsplash.com/photo-1546069901-5c02f7b84461?w=400" },
                    new { Nombre = "Tarta Tatin", Descripcion = "Tarta de manzana invertida francesa", Precio = 45.00m, Stock = 25, Categoria = "Alimentos y Bebidas", Subcategoria = "Panadería", Imagen = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400" }
                }
            },
            // COCHABAMBA
            new {
                Ciudad = "Cochabamba",
                RazonSocial = "Supermercado Ketal S.A.",
                NIT = "3030405060",
                Email = "ketal@cochabamba.bo",
                Password = "Password123",
                Nombre = "María",
                Apellido = "García",
                Telefono = "74300001",
                Direccion = "Av. Heroínas 500",
                LogoUrl = "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=400",
                Plan = "Plan Premium",
                Perfil = new {
                    Nombre = "Ketal Heroínas",
                    Descripcion = "Supermercado Ketal en el centro",
                    Direccion = "Av. Heroínas 500",
                    Telefono = "74300001",
                    Email = "heroinas@ketal.bo",
                    Latitud = -17.3935,
                    Longitud = -66.1570,
                    HorarioApertura = "08:00",
                    HorarioCierre = "22:00"
                },
                Productos = new[] {
                    new { Nombre = "Leche PIL 1L", Descripcion = "Leche entera Pil", Precio = 6.50m, Stock = 200, Categoria = "Alimentos y Bebidas", Subcategoria = "Lácteos", Imagen = "https://images.unsplash.com/photo-1550583724-b2692b85b150?w=400" },
                    new { Nombre = "Queso La Suiza 500g", Descripcion = "Queso tipo suizo", Precio = 35.00m, Stock = 80, Categoria = "Alimentos y Bebidas", Subcategoria = "Lácteos", Imagen = "https://images.unsplash.com/photo-1618164436269-690a82b3c0b4?w=400" },
                    new { Nombre = "Huevos de Granja AA", Descripcion = "Huevos frescos AA docena", Precio = 12.00m, Stock = 150, Categoria = "Alimentos y Bebidas", Subcategoria = "Carnes y Pescados", Imagen = "https://images.unsplash.com/photo-1582722872445-44dc5f7e3c8f?w=400" },
                    new { Nombre = "Mantequilla PIL 250g", Descripcion = "Mantequilla Pil", Precio = 18.00m, Stock = 100, Categoria = "Alimentos y Bebidas", Subcategoria = "Lácteos", Imagen = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=400" },
                    new { Nombre = "Harina Blanca Flor 1kg", Descripcion = "Harina de trigo Blanca Flor", Precio = 9.50m, Stock = 180, Categoria = "Alimentos y Bebidas", Subcategoria = "Panadería", Imagen = "https://images.unsplash.com/photo-1567206563064-6f60f40a2b57?w=400" }
                }
            },
            // LA PAZ
            new {
                Ciudad = "La Paz",
                RazonSocial = "Tienda Samsung Bolivia",
                NIT = "4040506070",
                Email = "samsung@lapaz.bo",
                Password = "Password123",
                Nombre = "Carlos",
                Apellido = "Mamani",
                Telefono = "72300001",
                Direccion = "Av. 16 de Julio 1500",
                LogoUrl = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=400",
                Plan = "Plan Empresarial",
                Perfil = new {
                    Nombre = "Samsung Store La Paz",
                    Descripcion = "Tienda oficial Samsung",
                    Direccion = "Av. 16 de Julio 1500",
                    Telefono = "72300001",
                    Email = "lapaz@samsung.bo",
                    Latitud = -16.5000,
                    Longitud = -68.1500,
                    HorarioApertura = "09:00",
                    HorarioCierre = "21:00"
                },
                Productos = new[] {
                    new { Nombre = "Samsung Galaxy S23", Descripcion = "Smartphone Samsung Galaxy S23 256GB", Precio = 4500.00m, Stock = 30, Categoria = "Tecnología", Subcategoria = "Smartphones", Imagen = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400" },
                    new { Nombre = "Samsung Galaxy Watch 6", Descripcion = "Smartwatch Galaxy Watch 6", Precio = 1200.00m, Stock = 25, Categoria = "Tecnología", Subcategoria = "Accesorios", Imagen = "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=400" },
                    new { Nombre = "Samsung 65\" QLED", Descripcion = "TV Samsung 65 pulgadas QLED 4K", Precio = 8500.00m, Stock = 12, Categoria = "Tecnología", Subcategoria = "Audio", Imagen = "https://images.unsplash.com/photo-1593359677879-a4bb92f829d1?w=400" },
                    new { Nombre = "Samsung Galaxy Buds2", Descripcion = "Auriculares inalámbricos Galaxy Buds2", Precio = 450.00m, Stock = 40, Categoria = "Tecnología", Subcategoria = "Audio", Imagen = "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=400" },
                    new { Nombre = "Tablet Samsung Galaxy Tab", Descripcion = "Tablet Samsung Galaxy Tab S9", Precio = 3200.00m, Stock = 20, Categoria = "Tecnología", Subcategoria = "Tablets", Imagen = "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400" }
                }
            },
            // EL ALTO
            new {
                Ciudad = "El Alto",
                RazonSocial = "Boutique Moda Alta",
                NIT = "5050607080",
                Email = "modaalta@elalto.bo",
                Password = "Password123",
                Nombre = "Ana",
                Apellido = "Choque",
                Telefono = "73300001",
                Direccion = "Av. 6 de Marzo 800",
                LogoUrl = "https://images.unsplash.com/photo-1441984904996-e0b6ba687e04?w=400",
                Plan = "Plan Básico",
                Perfil = new {
                    Nombre = "Moda Alta Boutique",
                    Descripcion = "Ropa y accesorios de moda en El Alto",
                    Direccion = "Av. 6 de Marzo 800",
                    Telefono = "73300001",
                    Email = "info@modaalta.bo",
                    Latitud = -16.5047,
                    Longitud = -68.1635,
                    HorarioApertura = "09:00",
                    HorarioCierre = "20:00"
                },
                Productos = new[] {
                    new { Nombre = "Poncho Altiplánico", Descripcion = "Poncho tradicional boliviano", Precio = 180.00m, Stock = 35, Categoria = "Ropa y Accesorios", Subcategoria = "Ropa Femenina", Imagen = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400" },
                    new { Nombre = "Aguayo Artesanal", Descripcion = "Aguayo tradicional multiuso", Precio = 95.00m, Stock = 50, Categoria = "Ropa y Accesorios", Subcategoria = "Accesorios", Imagen = "https://images.unsplash.com/photo-1583394838336-acd977736f90?w=400" },
                    new { Nombre = "Pollera Tradicional", Descripcion = "Pollera tradicional boliviana", Precio = 250.00m, Stock = 25, Categoria = "Ropa y Accesorios", Subcategoria = "Ropa Femenina", Imagen = "https://images.unsplash.com/photo-1594633312681-425c7b97ccd1?w=400" },
                    new { Nombre = "Chompa de Alpaca", Descripcion = "Chompa de lana de alpaca", Precio = 320.00m, Stock = 30, Categoria = "Ropa y Accesorios", Subcategoria = "Ropa Masculina", Imagen = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400" },
                    new { Nombre = "Gorra Tipo Chola", Descripcion = "Gorra tradicional tipo chola", Precio = 75.00m, Stock = 60, Categoria = "Ropa y Accesorios", Subcategoria = "Accesorios", Imagen = "https://images.unsplash.com/photo-1588850561407-ed78c282e89b?w=400" }
                }
            },
            // ORURO
            new {
                Ciudad = "Oruro",
                RazonSocial = "Ferretería El Constructor",
                NIT = "6060708090",
                Email = "constructor@oruro.bo",
                Password = "Password123",
                Nombre = "Pedro",
                Apellido = "Fernández",
                Telefono = "76300001",
                Direccion = "Calle La Plata 300",
                LogoUrl = "https://images.unsplash.com/photo-1581092160562-40aa08e78837?w=400",
                Plan = "Plan Básico",
                Perfil = new {
                    Nombre = "El Constructor Oruro",
                    Descripcion = "Ferretería y materiales de construcción",
                    Direccion = "Calle La Plata 300",
                    Telefono = "76300001",
                    Email = "oruro@constructor.bo",
                    Latitud = -17.9749,
                    Longitud = -67.1120,
                    HorarioApertura = "08:00",
                    HorarioCierre = "19:00"
                },
                Productos = new[] {
                    new { Nombre = "Cemento SOBOCE 50kg", Descripcion = "Cemento Portland SOBOCE", Precio = 85.00m, Stock = 100, Categoria = "Hogar y Jardín", Subcategoria = "Jardín", Imagen = "https://images.unsplash.com/photo-1615800002346-6e421a95f8c1?w=400" },
                    new { Nombre = "Ladrillos Horno 1000u", Descripcion = "Ladrillos de horno por mil", Precio = 450.00m, Stock = 50, Categoria = "Hogar y Jardín", Subcategoria = "Jardín", Imagen = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400" },
                    new { Nombre = "Pintura Lanco 20L", Descripcion = "Pintura látex Lanco blanco", Precio = 280.00m, Stock = 40, Categoria = "Hogar y Jardín", Subcategoria = "Decoración", Imagen = "https://images.unsplash.com/photo-1563453392212-326f5e854473?w=400" },
                    new { Nombre = "Tornillos Variados 1kg", Descripcion = "Kit de tornillos variados", Precio = 45.00m, Stock = 80, Categoria = "Hogar y Jardín", Subcategoria = "Jardín", Imagen = "https://images.unsplash.com/photo-1601972602237-8c79241e468b?w=400" },
                    new { Nombre = "Llave Stillson 24\"", Descripcion = "Llave Stillson profesional", Precio = 125.00m, Stock = 25, Categoria = "Hogar y Jardín", Subcategoria = "Jardín", Imagen = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400" }
                }
            },
            // POTOSÍ
            new {
                Ciudad = "Potosí",
                RazonSocial = "Librería Cultural Potosí",
                NIT = "7070809010",
                Email = "cultural@potosi.bo",
                Password = "Password123",
                Nombre = "Roberto",
                Apellido = "Mendoza",
                Telefono = "77300001",
                Direccion = "Av. 10 de Noviembre 200",
                LogoUrl = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=400",
                Plan = "Plan Básico",
                Perfil = new {
                    Nombre = "Librería Cultural",
                    Descripcion = "Libros y material educativo",
                    Direccion = "Av. 10 de Noviembre 200",
                    Telefono = "77300001",
                    Email = "info@culturalpotosi.bo",
                    Latitud = -19.5833,
                    Longitud = -65.7531,
                    HorarioApertura = "09:00",
                    HorarioCierre = "19:00"
                },
                Productos = new[] {
                    new { Nombre = "Historia de Potosí", Descripcion = "Libro sobre la historia de Potosí", Precio = 65.00m, Stock = 40, Categoria = "Libros y Material Educativo", Subcategoria = "Libros", Imagen = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400" },
                    new { Nombre = "Cuaderno Universitario", Descripcion = "Cuaderno universitario 100 hojas", Precio = 12.00m, Stock = 200, Categoria = "Libros y Material Educativo", Subcategoria = "Material Escolar", Imagen = "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?w=400" },
                    new { Nombre = "Lápices Faber Castell", Descripcion = "Caja lápices Faber Castell 24 colores", Precio = 35.00m, Stock = 60, Categoria = "Libros y Material Educativo", Subcategoria = "Material Escolar", Imagen = "https://images.unsplash.com/photo-1583484963886-47b95bba26f4?w=400" },
                    new { Nombre = "Diccionario RAE 2024", Descripcion = "Diccionario Real Academia Española", Precio = 95.00m, Stock = 30, Categoria = "Libros y Material Educativo", Subcategoria = "Libros", Imagen = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400" },
                    new { Nombre = "Mochila Escolar", Descripcion = "Mochila escolar resistente", Precio = 85.00m, Stock = 50, Categoria = "Libros y Material Educativo", Subcategoria = "Material Escolar", Imagen = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400" }
                }
            },
            // SUCRE
            new {
                Ciudad = "Sucre",
                RazonSocial = "Farmacia Universal Sucre",
                NIT = "8080901011",
                Email = "universal@sucre.bo",
                Password = "Password123",
                Nombre = "Laura",
                Apellido = "Ramos",
                Telefono = "78300001",
                Direccion = "Calle Arenales 150",
                LogoUrl = "https://images.unsplash.com/photo-1587854692152-cbe660dbde88?w=400",
                Plan = "Plan Premium",
                Perfil = new {
                    Nombre = "Farmacia Universal",
                    Descripcion = "Farmacia y productos de salud",
                    Direccion = "Calle Arenales 150",
                    Telefono = "78300001",
                    Email = "sucre@farmaciauniversal.bo",
                    Latitud = -19.0500,
                    Longitud = -65.2500,
                    HorarioApertura = "08:00",
                    HorarioCierre = "21:00"
                },
                Productos = new[] {
                    new { Nombre = "Paracetamol 500mg", Descripcion = "Paracetamol genérico 500mg x20", Precio = 8.50m, Stock = 150, Categoria = "Salud y Belleza", Subcategoria = "Cuidado Corporal", Imagen = "https://images.unsplash.com/photo-1584308666744-24d5c474f2ae?w=400" },
                    new { Nombre = "Ibuprofeno 400mg", Descripcion = "Ibuprofeno 400mg x20 comprimidos", Precio = 10.00m, Stock = 120, Categoria = "Salud y Belleza", Subcategoria = "Cuidado Corporal", Imagen = "https://images.unsplash.com/photo-1587854692152-cbe660dbde88?w=400" },
                    new { Nombre = "Jabón Protex", Descripcion = "Jabón antibacterial Protex", Precio = 6.50m, Stock = 200, Categoria = "Salud y Belleza", Subcategoria = "Cuidado Corporal", Imagen = "https://images.unsplash.com/photo-1526045431048-f857369baa09?w=400" },
                    new { Nombre = "Protector Solar Nivea", Descripcion = "Protector solar Nivea FPS 50", Precio = 65.00m, Stock = 40, Categoria = "Salud y Belleza", Subcategoria = "Cuidado Facial", Imagen = "https://images.unsplash.com/photo-1556228578-0d85b1a4d571?w=400" },
                    new { Nombre = "Shampoo Sedal", Descripcion = "Shampoo Sedal Nutritivo 400ml", Precio = 18.00m, Stock = 80, Categoria = "Salud y Belleza", Subcategoria = "Cuidado Corporal", Imagen = "https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=400" }
                }
            },
            // TRINIDAD
            new {
                Ciudad = "Trinidad",
                RazonSocial = "Supermercado El Norte",
                NIT = "9090101112",
                Email = "elnorte@trinidad.bo",
                Password = "Password123",
                Nombre = "Carmen",
                Apellido = "Villarroel",
                Telefono = "79300001",
                Direccion = "Av. 6 de Agosto 400",
                LogoUrl = "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=400",
                Plan = "Plan Básico",
                Perfil = new {
                    Nombre = "Supermercado El Norte",
                    Descripcion = "Supermercado regional en Trinidad",
                    Direccion = "Av. 6 de Agosto 400",
                    Telefono = "79300001",
                    Email = "info@elnorte.bo",
                    Latitud = -14.8333,
                    Longitud = -64.9000,
                    HorarioApertura = "07:00",
                    HorarioCierre = "21:00"
                },
                Productos = new[] {
                    new { Nombre = "Pescado Surubí 1kg", Descripcion = "Pescado surubí fresco del río", Precio = 45.00m, Stock = 30, Categoria = "Alimentos y Bebidas", Subcategoria = "Carnes y Pescados", Imagen = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400" },
                    new { Nombre = "Plátano Verde", Descripcion = "Plátano verde para freír", Precio = 8.00m, Stock = 100, Categoria = "Alimentos y Bebidas", Subcategoria = "Frutas y Verduras", Imagen = "https://images.unsplash.com/photo-1603833665858-e61d17a86224?w=400" },
                    new { Nombre = "Yuca Fresca 1kg", Descripcion = "Yuca fresca del Beni", Precio = 6.50m, Stock = 120, Categoria = "Alimentos y Bebidas", Subcategoria = "Frutas y Verduras", Imagen = "https://images.unsplash.com/photo-1601972602237-8c79241e468b?w=400" },
                    new { Nombre = "Arroz Integral 1kg", Descripcion = "Arroz integral de buena calidad", Precio = 9.00m, Stock = 90, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400" },
                    new { Nombre = "Fécula de Yuca 500g", Descripcion = "Almidón de yuca para cocina", Precio = 12.00m, Stock = 60, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1567206563064-6f60f40a2b57?w=400" }
                }
            },
            // TARIJA
            new {
                Ciudad = "Tarija",
                RazonSocial = "Bodega Casa Real",
                NIT = "1010111213",
                Email = "casareal@tarija.bo",
                Password = "Password123",
                Nombre = "Juan",
                Apellido = "Pérez",
                Telefono = "80300001",
                Direccion = "Av. Las Américas 600",
                LogoUrl = "https://images.unsplash.com/photo-1556910096-6f5e72db6803?w=400",
                Plan = "Plan Premium",
                Perfil = new {
                    Nombre = "Bodega Casa Real",
                    Descripcion = "Vinos y productos tarijeños",
                    Direccion = "Av. Las Américas 600",
                    Telefono = "80300001",
                    Email = "info@casareal.bo",
                    Latitud = -21.5318,
                    Longitud = -64.7311,
                    HorarioApertura = "09:00",
                    HorarioCierre = "20:00"
                },
                Productos = new[] {
                    new { Nombre = "Vino Tinto Casa Real", Descripcion = "Vino tinto Casa Real reserva", Precio = 85.00m, Stock = 50, Categoria = "Alimentos y Bebidas", Subcategoria = "Bebidas", Imagen = "https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=400" },
                    new { Nombre = "Singani Casa Real", Descripcion = "Singani premium Casa Real 750ml", Precio = 120.00m, Stock = 40, Categoria = "Alimentos y Bebidas", Subcategoria = "Bebidas", Imagen = "https://images.unsplash.com/photo-1556910096-6f5e72db6803?w=400" },
                    new { Nombre = "Durazno en Almíbar", Descripcion = "Durazno en almíbar tarifeño", Precio = 15.00m, Stock = 80, Categoria = "Alimentos y Bebidas", Subcategoria = "Frutas y Verduras", Imagen = "https://images.unsplash.com/photo-1563565375-f3fdfdbefa83?w=400" },
                    new { Nombre = "Miel de Abeja", Descripcion = "Miel de abeja natural tarijeña", Precio = 35.00m, Stock = 45, Categoria = "Alimentos y Bebidas", Subcategoria = "Snacks", Imagen = "https://images.unsplash.com/photo-1587049352846-4a222e784d38?w=400" },
                    new { Nombre = "Uva Tarijeña 1kg", Descripcion = "Uva fresca de Tarija", Precio = 18.00m, Stock = 60, Categoria = "Alimentos y Bebidas", Subcategoria = "Frutas y Verduras", Imagen = "https://images.unsplash.com/photo-1603833665858-e61d17a86224?w=400" }
                }
            }
        };

        foreach (var empresaData in empresasData)
        {
            try
            {
                var ciudad = ciudades.FirstOrDefault(c => c.Nombre == empresaData.Ciudad);
                if (ciudad == null) continue;

                // Verificar si la empresa ya existe
                var usuarioExistente = await _usuarioRepository.GetByEmailAsync(empresaData.Email);
                Empresa empresa;

                if (usuarioExistente != null)
                {
                    empresa = await _empresaRepository.GetByUsuarioIdAsync(usuarioExistente.Id!);
                    if (empresa != null)
                    {
                        empresasCreadas.Add(empresa);
                        continue;
                    }
                }

                // Crear usuario
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(empresaData.Password);
                var usuario = new Usuario
                {
                    Email = empresaData.Email,
                    PasswordHash = passwordHash,
                    Nombre = empresaData.Nombre,
                    Apellido = empresaData.Apellido,
                    Telefono = empresaData.Telefono,
                    Rol = "Empresa",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                usuario = await _usuarioRepository.CreateAsync(usuario);

                // Crear empresa
                empresa = new Empresa
                {
                    UsuarioId = usuario.Id!,
                    RazonSocial = empresaData.RazonSocial,
                    NIT = empresaData.NIT,
                    Direccion = empresaData.Direccion,
                    Telefono = empresaData.Telefono,
                    Email = empresaData.Email,
                    Activa = true,
                    FechaCreacion = DateTime.UtcNow
                };
                empresa = await _empresaRepository.CreateAsync(empresa);

                // Asignar plan y crear suscripción
                var plan = planes.FirstOrDefault(p => p.Nombre == empresaData.Plan);
                if (plan != null)
                {
                    var suscripcion = new SuscripcionEmpresa
                    {
                        EmpresaId = empresa.Id!,
                        PlanId = plan.Id!,
                        FechaInicio = DateTime.UtcNow,
                        FechaFin = DateTime.UtcNow.AddDays(plan.DuracionDias),
                        Activa = true,
                        FechaCreacion = DateTime.UtcNow
                    };
                    await _suscripcionEmpresaRepository.CreateAsync(suscripcion);
                }

                // Crear perfil comercial
                var perfil = new PerfilComercial
                {
                    EmpresaId = empresa.Id!,
                    Nombre = empresaData.Perfil.Nombre,
                    Descripcion = empresaData.Perfil.Descripcion,
                    Direccion = empresaData.Perfil.Direccion,
                    CiudadId = ciudad.Id!,
                    Telefono = empresaData.Perfil.Telefono,
                    Email = empresaData.Perfil.Email,
                    Latitud = empresaData.Perfil.Latitud,
                    Longitud = empresaData.Perfil.Longitud,
                    HorarioApertura = empresaData.Perfil.HorarioApertura,
                    HorarioCierre = empresaData.Perfil.HorarioCierre,
                    ImagenUrl = empresaData.LogoUrl,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };
                perfil = await _perfilComercialRepository.CreateAsync(perfil);

                // Crear productos
                foreach (var productoData in empresaData.Productos)
                {
                    var categoria = categorias.FirstOrDefault(c => c.Nombre == productoData.Categoria);
                    if (categoria == null) continue;

                    var subcategoria = subcategorias.FirstOrDefault(s => 
                        s.Nombre == productoData.Subcategoria && s.CategoriaId == categoria.Id);
                    if (subcategoria == null) continue;

                    var producto = new Producto
                    {
                        PerfilComercialId = perfil.Id!,
                        CategoriaId = categoria.Id!,
                        SubcategoriaId = subcategoria.Id!,
                        Nombre = productoData.Nombre,
                        Descripcion = productoData.Descripcion,
                        Precio = productoData.Precio,
                        Stock = productoData.Stock,
                        ImagenUrl = productoData.Imagen,
                        Disponible = true,
                        FechaCreacion = DateTime.UtcNow
                    };
                    await _productoRepository.CreateAsync(producto);
                }

                empresasCreadas.Add(empresa);
            }
            catch (Exception ex)
            {
                // Continuar con la siguiente empresa si hay error
                Console.WriteLine($"Error creando empresa {empresaData.RazonSocial}: {ex.Message}");
            }
        }

        return empresasCreadas;
    }
}


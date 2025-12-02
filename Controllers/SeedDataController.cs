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

    public SeedDataController(
        ICiudadRepository ciudadRepository,
        IPlanRepository planRepository,
        ICategoriaRepository categoriaRepository,
        ISubcategoriaRepository subcategoriaRepository,
        IUsuarioRepository usuarioRepository,
        IEmpresaRepository empresaRepository,
        IPerfilComercialRepository perfilComercialRepository,
        IProductoRepository productoRepository)
    {
        _ciudadRepository = ciudadRepository;
        _planRepository = planRepository;
        _categoriaRepository = categoriaRepository;
        _subcategoriaRepository = subcategoriaRepository;
        _usuarioRepository = usuarioRepository;
        _empresaRepository = empresaRepository;
        _perfilComercialRepository = perfilComercialRepository;
        _productoRepository = productoRepository;
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
}


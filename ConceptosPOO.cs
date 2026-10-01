using System;

// ABSTRACCIÓN: Producto es abstracta, no se puede instanciar directamente
// Define QUÉ hace todo producto, sin decir CÓMO se calcula su precio final
public abstract class Producto
{
    // ENCAPSULAMIENTO: campos privados, solo la clase puede modificarlos.
    private int _stock;
    private int _precio;

    public string Nombre { get; }                   // solo lectura desde afuera
    public int Stock { get { return _stock; } }     // se puede leer, no asignar

    public int Precio
    {
        get { return _precio; }
        set
        {
            if (value < 0)
            {
                Console.WriteLine("Error: el precio no puede ser negativo");
                return;   // no se asigna el valor
            }
            _precio = value;
        }
    }

    public Producto(string nombre, int precio, int stock)
    {
        Nombre = nombre;
        Precio = precio;   // pasa por el set, así que también se valida
        _stock = stock;
    }

    // Método que valida antes de modificar el stock privado.
    public bool Retirar(int cantidad)
    {
        if (cantidad <= 0 || cantidad > _stock)
        {
            Console.WriteLine("Error: cantidad inválida para " + Nombre);
            return false;
        }
        _stock -= cantidad;
        return true;
    }

    // POLIMORFISMO: cada subclase define su propio comportamiento.
    public abstract int CalcularPrecioFinal();  // Método abstracto: no tiene implementación aquí
    public virtual bool PuedeVenderse(bool tieneReceta) { return true; }  // Virtual: las subclases pueden sobrescribirlo
}

// HERENCIA nivel 2: Medicamento hereda de Producto
public class Medicamento : Producto
{
    public string Laboratorio { get; set; }

    public Medicamento(string nombre, int precio, int stock, string laboratorio)
        : base(nombre, precio, stock)  // Llama al constructor de la clase base Producto
    {
        Laboratorio = laboratorio;
    }

    // Implementa el método abstracto: el precio final es el mismo precio.
    public override int CalcularPrecioFinal() { return Precio; }
}

// HERENCIA nivel 3: Psicotropico hereda de Medicamento, que hereda de Producto
public class Psicotropico : Medicamento
{
    public Psicotropico(string nombre, int precio, int stock, string laboratorio)
        : base(nombre, precio, stock, laboratorio) { }

    // Sobrescribe el método: exige receta para vender.
    public override bool PuedeVenderse(bool tieneReceta) { return tieneReceta; }
}

// HERENCIA nivel 2: CuidadoPersonal hereda de Producto
public class CuidadoPersonal : Producto
{
    public string Uso { get; set; }

    public CuidadoPersonal(string nombre, int precio, int stock, string uso)
        : base(nombre, precio, stock)
    {
        Uso = uso;
    }

    // Suma un 13% al precio (ejemplo de impuesto).
    public override int CalcularPrecioFinal() { return Precio + Precio * 13 / 100; }
}

// Clase que representa al proveedor de la farmacia (relación de asociación).
public class Proveedor
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    public Proveedor(string nombre, string telefono)
    {
        Nombre = nombre;
        Telefono = telefono;
    }
}

public class Farmacia
{
    // ENCAPSULAMIENTO: el arreglo es privado, solo se modifica con Agregar().
    private Producto[] _productos = new Producto[10];
    private int _cantidad = 0;   // cuántos productos hay actualmente en el arreglo

    public string Nombre { get; set; }
    public Proveedor Proveedor { get; set; }   // la farmacia "tiene un" proveedor (composición/asociación)

    public Farmacia(string nombre, Proveedor proveedor)
    {
        Nombre = nombre;
        Proveedor = proveedor;
    }

    // Agrega un producto si todavía hay espacio en el arreglo.
    public void Agregar(Producto p)
    {
        if (_cantidad >= _productos.Length)
        {
            Console.WriteLine("Error: el inventario está lleno");
            return;
        }
        _productos[_cantidad] = p;
        _cantidad++;
    }

    // POLIMORFISMO en acción: se llama al mismo método sobre Producto,
    // pero cada objeto responde según su clase real.
    public void MostrarInventario()
    {
        Console.WriteLine(" Inventario de " + Nombre );
        for (int i = 0; i < _cantidad; i++)
        {
            Producto p = _productos[i];   // referencia de tipo Producto, objeto de cualquier subclase
            Console.WriteLine(p.Nombre
                + " | stock: " + p.Stock
                + " | precio final: " + p.CalcularPrecioFinal()    // cambia según la clase real
                + " | venta sin receta: " + p.PuedeVenderse(false)); // Psicotropico devuelve false
        }
    }
}

public class Program
{
    public static void Main()
    {
        Proveedor proveedor = new Proveedor("Distribuidora Salud", "2222-3333");
        Farmacia farmacia = new Farmacia("Farmacia Central", proveedor);

        // Objetos de distintas clases de la jerarquía de Producto.
        Medicamento ibuprofeno = new Medicamento("Ibuprofeno", 1500, 20, "Bayer");
        Psicotropico alprazolam = new Psicotropico("Alprazolam", 3000, 5, "Pfizer");
        CuidadoPersonal protector = new CuidadoPersonal("Protector solar", 8000, 10, "Piel");

        // Todos se guardan en el mismo arreglo de Producto gracias a la herencia.
        farmacia.Agregar(ibuprofeno);
        farmacia.Agregar(alprazolam);
        farmacia.Agregar(protector);

        farmacia.MostrarInventario();

        Console.WriteLine();
        Console.WriteLine("--- Prueba de encapsulamiento ---");
        ibuprofeno.Precio = -100;              // rechazado: el set valida que no sea negativo
        ibuprofeno.Retirar(100);               // rechazado: es más que el stock disponible (20)

        if (ibuprofeno.Retirar(5))             // válido: el stock baja de 20 a 15
            Console.WriteLine("Venta realizada. Stock de Ibuprofeno: " + ibuprofeno.Stock);

    }
}